using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Core.Services;
using CookBookBackend.Data;
using FluentValidation;

namespace CookBookBackend.Api.Validators
{
    public class DishEditValidator : AbstractValidator<DishEditDTO>
    {
        private enum PhotoVerdict
        {
            NONE,
            CORRECT,
            TOO_MUCH_DELETED,
            TOO_MUCH_ADDED
        }

        private readonly AppDbContext _context; // нужно, чтобы проверить, сколько фото уже загружено на момент редактирования
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PhotoService _photoService;
        public DishEditValidator(AppDbContext context, IHttpContextAccessor httpContextAccessor, PhotoService photoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;

            
            RuleFor(dish => dish.Ingredients)
                .Must(ingredients => ingredients?.Any() ?? true).WithMessage("A dish must contain at least one ingredient"); // если null - значит не меняем

            RuleForEach(dish => dish.Ingredients) 
                .ChildRules(ingredient =>
                {
                    ingredient.RuleFor(i => i.FoodItemId)
                        .GreaterThanOrEqualTo(0)
                        .WithMessage("Food Item id is invalid (should be non-negative)");
                    ingredient.RuleFor(i => i.AmountGrams)
                        .GreaterThan(0)
                        .WithMessage("Amount of ingredient should be greater than 0");
                });

            RuleFor(dish => dish)
                .Must(dish => 
                    PhotosEditValidityCheck(dish, 
                                            Int32.TryParse(
                                                _httpContextAccessor.HttpContext?.GetRouteValue("id")?.ToString(), 
                                                out var idOfEdited) ? idOfEdited : -1)
                    == PhotoVerdict.CORRECT)
                .WithMessage("Total photos (existing + new) cannot exceed 5 and you cant delete too much");
        }
        private PhotoVerdict PhotosEditValidityCheck(DishEditDTO dto, int idOfEdited)
        {
            int existingPhotosCount = _context.Dishes?.Find(idOfEdited)?.PhotoPaths.Count ?? -1;
            if (existingPhotosCount == -1)
                throw new EntityNotFoundException("Dish", idOfEdited);

            if ((dto.PhotoUrlsToDelete?.Count ?? 0) > existingPhotosCount)
                return PhotoVerdict.TOO_MUCH_DELETED;

            if (!dto.Photos?.Any() ?? true)
                return PhotoVerdict.CORRECT;

            if (((dto.Photos?.Count ?? 0) - (_photoService.CountExistingPhotosByUrl(dto.PhotoUrlsToDelete)) + existingPhotosCount) > 5)
                return PhotoVerdict.TOO_MUCH_ADDED;

            return PhotoVerdict.CORRECT;
        }
    }
}
