using AutoMapper;
using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Core.ValueObjects;
using CookBookBackend.Data;
using CookBookBackend.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CookBookBackend.Core.Services
{
    public class DishService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<DishService> _logger;
        private readonly PhotoService _photoService;
        private readonly string _photoFolderPath;
        private readonly NutritionFactsService _nutritionService;

        public DishService(IMapper mapper, AppDbContext context, IWebHostEnvironment env, ILogger<DishService> logger, PhotoService photoService, NutritionFactsService nutritionService)
        {
            _mapper = mapper;
            _context = context;
            _env = env;
            _logger = logger;
            _photoService = photoService;
            _photoFolderPath = Path.Combine(_env.WebRootPath, "images", "food_items");
            _nutritionService = nutritionService;
        }

        public async Task<DishCreateResultDTO> CreateDishAsync(DishCreateDTO dto)
        {
            var dish = _mapper.Map<Dish>(dto);

            if (dto.Photos != null && dto.Photos.Any())
            {
                dish.PhotoPaths = await _photoService.SavePhotosAsync(dto.Photos, _photoFolderPath);
            }


            foreach (var ingredientDto in dto.Ingredients)
            {
                dish.Ingredients.Add(new DishIngredient
                {
                    FoodItem = _context.FoodItems.Find(ingredientDto.FoodItemId) ?? throw new EntityNotFoundException("DishIngredient", ingredientDto.FoodItemId),
                    Dish = dish,
                    FoodItemId = ingredientDto.FoodItemId,
                    AmountGrams = ingredientDto.AmountGrams 
                });
            }

            NutritionFacts values = _nutritionService.CalculateFromIngredients(dish.Ingredients);
            if (dto.Calories == null) dish.Calories = values.Calories;
            if (dto.Proteins == null) dish.Proteins = values.Proteins;
            if (dto.Fats == null) dish.Fats = values.Fats;
            if (dto.Carbohydrates == null) dish.Carbohydrates = values.Carbohydrates;


            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();
            
            return _mapper.Map<DishCreateResultDTO>(dish);

        }

        public async Task DeleteDishAsync(int id)
        {
            var toDelete = await _context.Dishes.FirstOrDefaultAsync(d => d.Id == id);

            if (toDelete == null)
                throw new EntityNotFoundException("Dish", id);


            _ = DeletePhotosOfDishAsync(toDelete);
            _context.Dishes.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        private async Task DeletePhotosOfDishAsync(Dish dish)
        {
            foreach (var localPath in dish.PhotoPaths ?? [])
            {
                File.Delete(Path.Combine(_env.WebRootPath, localPath));
            }
        }


        public async Task<DishEditResultDTO> EditDishAsync(int id, DishEditDTO dto)
        {
            var toChange = await _context.Dishes
                                .Include(d => d.Ingredients)
                                    .ThenInclude(fi => fi.FoodItem)
                                .FirstOrDefaultAsync(d => d.Id == id);
            if (toChange == null)
            {
                throw new EntityNotFoundException("Dish", id);
            }


            _mapper.Map(dto, toChange);

            if (dto.Ingredients != null)
            {
                if (!dto.Ingredients.Any())
                    throw new ArgumentException("Dish must have at least one ingredient");
                toChange.Ingredients = _mapper.Map<List<DishIngredient>>(dto.Ingredients);
            }


            if (dto.Photos != null && dto.Photos.Any())
            {
                if (toChange.PhotoPaths == null)
                    toChange.PhotoPaths = new List<string>();
                toChange.PhotoPaths.AddRange(await _photoService.SavePhotosAsync(dto.Photos, _photoFolderPath));
            }
            if (dto.PhotoUrlsToDelete != null && dto.PhotoUrlsToDelete.Any())
            {
                foreach (var url in dto.PhotoUrlsToDelete)
                {
                    var pathToRemove = _photoService.ConvertUrlToRelativePath(url);
                    toChange.PhotoPaths?.Remove(pathToRemove);
                    _photoService.DeleteFileByUrl(url);
                }
            }

            await _context.SaveChangesAsync();

            var toReturn = _mapper.Map<DishEditResultDTO>(toChange);
            return toReturn;
        }

        public async Task<List<DishPreviewDTO>> GetFoodItemsAsync(string? toSearch, DishCategory? category, DietaryFlags? flags)
        {
            var query = _context.Dishes.AsQueryable();

            if (category != null)
            {
                query = query.Where(f => f.Category == category.Value);
            }
            if (flags != null)
            {
                query = flags.Value.HasFlag(DietaryFlags.VEGAN) ? query.Where(d => (d.DietaryFlags & DietaryFlags.VEGAN) != 0) : query;
                query = flags.Value.HasFlag(DietaryFlags.GLUTEN_FREE) ? query.Where(d => (d.DietaryFlags & DietaryFlags.GLUTEN_FREE) != 0) : query;
                query = flags.Value.HasFlag(DietaryFlags.SUGAR_FREE) ? query.Where(d => (d.DietaryFlags & DietaryFlags.SUGAR_FREE) != 0) : query;
            }

            if (!string.IsNullOrWhiteSpace(toSearch))
            {
                query = query.Where(f => f.Name.ToLower().Contains(toSearch.ToLower()));
            }



            var items = await query.
                Include(d => d.Ingredients)
                    .ThenInclude(fi => fi.FoodItem)
                .OrderBy(f => f.Name)
                .ToListAsync();

            return _mapper.Map<List<DishPreviewDTO>>(items);
        }

        public async Task<DishDetailedDTO> GetDishDetailedAsync(int id)
        {
            var toGet = await _context.Dishes.FindAsync(id);
            if (toGet == null)
                throw new EntityNotFoundException("Dish", id);



            return _mapper.Map<DishDetailedDTO>(toGet);
        }
    }
}
