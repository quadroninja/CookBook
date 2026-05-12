using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Core.Exceptions;
using CookBookBackend.Core.Services;
using CookBookBackend.Data;
using FluentValidation;

namespace CookBookBackend.Api.Validators
{
    public class FoodItemEditValidator : AbstractValidator<FoodItemEditDTO>
    {
        private enum PhotoVerdict
        {
            NONE,
            CORRECT,
            TOO_MUCH_DELETED,
            TOO_MUCH_ADDED
        }

        private readonly AppDbContext _context; // нужно, чтобы проверить, сколько фото уже загружено на момент редактирования
        private readonly DietaryFlags allDietaryFlagsChecked;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PhotoService _photoService;
        public FoodItemEditValidator(AppDbContext context, IHttpContextAccessor httpContextAccessor, PhotoService photoService)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;

            allDietaryFlagsChecked = (DietaryFlags)Enum.GetValues<DietaryFlags>()
                .Where(f => f != DietaryFlags.NONE)
                .Select(f => f)
                .Aggregate((DietaryFlags)0, (current, f) => current | f);

            RuleFor(foodItem => foodItem)
                .Must(foodItem => 
                    PhotosEditValidityCheck(foodItem, 
                                            Int32.TryParse(
                                                _httpContextAccessor.HttpContext?.GetRouteValue("id")?.ToString(), 
                                                out var idOfEdited) ? idOfEdited : -1)
                    == PhotoVerdict.CORRECT)
                .WithMessage("Total photos (existing + new) cannot exceed 5 and you cant delete too much");
        }
        private PhotoVerdict PhotosEditValidityCheck(FoodItemEditDTO dto, int idOfEdited)
        {
            int existingPhotosCount = _context.FoodItems?.Find(idOfEdited)?.PhotoPaths.Count ?? -1;
            if (existingPhotosCount == -1)
                throw new EntityNotFoundException("FoodItem", idOfEdited);

            if ((dto.PhotoUrlsToDelete?.Count ?? 0) > existingPhotosCount)
                return PhotoVerdict.TOO_MUCH_DELETED;

            if (!dto.Photos?.Any() ?? true)
                return PhotoVerdict.CORRECT;

            if (((dto.Photos?.Count ?? 0) - (_photoService.CountExistingPhotosByUrl(dto.PhotoUrlsToDelete)) + existingPhotosCount) > 5)
                return PhotoVerdict.TOO_MUCH_ADDED;

            return PhotoVerdict.CORRECT;
        }



        private bool IsValidDietaryFlags(DietaryFlags flags)
        {
            return (flags & ~allDietaryFlagsChecked) == 0;  // ~00...01111 = 11...10000, если хотя бы один бит будет из неопределенных в enum-е - в результате будет не ноль
        }
    }
}
