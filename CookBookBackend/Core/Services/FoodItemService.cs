using AutoMapper;
using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Data;
using CookBookBackend.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace CookBookBackend.Core.Services
{
    public class FoodItemService
    {
        private readonly IMapper _mapper;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<FoodItemService> _logger;
        private readonly PhotoService _photoService;
        private readonly string _photoFolderPath;
        public FoodItemService(IMapper mapper, AppDbContext context, IWebHostEnvironment env, ILogger<FoodItemService> logger, PhotoService photoService)
        {
            _mapper = mapper;
            _context = context;
            _env = env;
            _logger = logger;
            _photoService = photoService;
            _photoFolderPath = Path.Combine(_env.WebRootPath, "images", "food_items");
        }

        public async Task<FoodItemCreateResultDTO> CreateFoodItemAsync(FoodItemCreateDTO dto)
        {
            var foodItem = _mapper.Map<FoodItem>(dto);

            if (dto.Photos != null && dto.Photos.Any())
            {
                foodItem.PhotoPaths = await _photoService.SavePhotosAsync(dto.Photos, _photoFolderPath);
            }


            _context.FoodItems.Add(foodItem);
            await _context.SaveChangesAsync();

            return _mapper.Map<FoodItemCreateResultDTO>(foodItem);
           
        }

        public async Task DeleteFoodItemAsync(int id) 
        {
            var toDelete = await _context.FoodItems
                .Include(f => f.DishesWithThisIngredient)
                .ThenInclude(di => di.Dish)
                .FirstOrDefaultAsync(fi => fi.Id == id);

            if (toDelete == null)
                throw new EntityNotFoundException("FoodItem", id);
            
            var usedInDishes = _context.DishIngredients
                .Select(di => di.Dish.Name)
                .ToList();

            if (usedInDishes.Any())
            {
                throw new FoodItemInUseException(id, toDelete.Name, usedInDishes);
            }


            _ = DeletePhotosOfFoodItemAsync(toDelete);
            _context.FoodItems.Remove(toDelete);
            await _context.SaveChangesAsync();
        }

        public async Task<List<FoodItemPreviewDTO>> GetFoodItemsAsync(string? toSearch, string? sortBy, bool sortDescending, FoodItemCategory? category, DietaryFlags? flags, ReadinessToEat? readinessToEat)
        {
            var query = _context.FoodItems.AsQueryable();

            if (category != null)
            {
                query = query.Where(f => f.Category == category.Value);
            }
            if (flags != null)
            {
                query = flags.Value.HasFlag(DietaryFlags.VEGAN) ? query.Where(f => (f.DietaryFlags & DietaryFlags.VEGAN) != 0) : query;
                query = flags.Value.HasFlag(DietaryFlags.GLUTEN_FREE) ? query.Where(f => (f.DietaryFlags & DietaryFlags.GLUTEN_FREE) != 0) : query;
                query = flags.Value.HasFlag(DietaryFlags.SUGAR_FREE) ? query.Where(f => (f.DietaryFlags & DietaryFlags.SUGAR_FREE) != 0) : query;
            }
            if (readinessToEat != null)
            {
                query = query.Where(f => f.ReadinessToEat == readinessToEat.Value);
            }

            if (!string.IsNullOrWhiteSpace(toSearch))
            {
                query = query.Where(f => f.Name.ToLower().Contains(toSearch.ToLower()));
            }

            
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                query = query.OrderBy($"{sortBy} {(sortDescending ? "descending" : "ascending")}"); 
            }
            else
            {
                query = query.OrderBy(f => f.Name);
            }

            var items = await query.ToListAsync();

            return _mapper.Map<List<FoodItemPreviewDTO>>(items);
        }

        public async Task<FoodItemEditResultDTO> EditFoodItemAsync(int id, FoodItemEditDTO dto)
        {
            var toChange = _context.FoodItems.Find(id);
            if (toChange == null)
            {
                throw new EntityNotFoundException("FoodItem", id);
            }


            _mapper.Map(dto, toChange);

            if (dto.Photos != null && dto.Photos.Any())
            {
                toChange.PhotoPaths.AddRange(await _photoService.SavePhotosAsync(dto.Photos, _photoFolderPath));
            }
            if (dto.PhotoUrlsToDelete != null && dto.PhotoUrlsToDelete.Any())
            {
                foreach (var url in dto.PhotoUrlsToDelete)
                {
                    var pathToRemove = _photoService.ConvertUrlToRelativePath(url);
                    toChange.PhotoPaths.Remove(pathToRemove);
                    _photoService.DeleteFileByUrl(url);
                }
            }

            await _context.SaveChangesAsync();

            return _mapper.Map<FoodItemEditResultDTO>(toChange);
        }

        private async Task DeletePhotosOfFoodItemAsync(FoodItem item)
        {
            foreach (var localPath in item.PhotoPaths ?? [])
            {
                File.Delete(Path.Combine(_env.WebRootPath, localPath));
            }
        }

        public async Task<FoodItemDetailedDTO> GetFoodItemDetailedAsync(int id)
        {
            var toGet = await _context.FoodItems.FindAsync(id);
            if (toGet == null)
                throw new EntityNotFoundException("FoodItem", id);



            return _mapper.Map<FoodItemDetailedDTO>(toGet);
        }
    }
}
