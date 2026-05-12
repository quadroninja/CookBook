using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CookBookBackend.Api.DTO.Validator
{
    public class FoodItemCreateValidator : AbstractValidator<FoodItemCreateDTO>
    {
        private DietaryFlags allDietaryFlagsChecked;

        public FoodItemCreateValidator()
        {
            allDietaryFlagsChecked = (DietaryFlags)Enum.GetValues<DietaryFlags>()
                .Where(f => f != DietaryFlags.NONE)
                .Select(f => f) 
                .Aggregate((DietaryFlags)0, (current, f) => current | f);

            RuleFor(foodItem => foodItem.Photos)
                .Must(photos => photos.Count <= 5)
                .When(foodItem => foodItem.Photos is not null)
                .WithMessage($"Maximum 5 photos allowed");
            RuleFor(foodItem => foodItem.Name)
                .NotEmpty()
                .WithMessage("Name should not be empty");
            RuleFor(foodItem => foodItem.Calories)
                .NotNull()
                .GreaterThanOrEqualTo(0m)
                .WithMessage("Calories should not be null or negative");
            RuleFor(foodItem => foodItem.Proteins)
                .NotNull()
                .GreaterThanOrEqualTo(0m)
                .LessThanOrEqualTo(100m)
                .WithMessage("Proteins should be in [0; 100]");
            RuleFor(foodItem => foodItem.Fats)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100m)
                .WithMessage("Fats should be in [0; 100]");
            RuleFor(foodItem => foodItem.Carbohydrates)
                .NotNull()
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(100m)
                .WithMessage("Carbohydrates should be in [0; 100]");
            
            RuleFor(foodItem => foodItem.Category)
                .NotEqual(FoodItemCategory.NONE)
                .WithMessage("FoodItemCategory should be specified");

            RuleFor(foodItem => foodItem.ReadinessToEat)
                .NotEqual(ReadinessToEat.NONE)
                .WithMessage("Food Item ReadinessToEat should be specified");

            RuleFor(foodItem => foodItem.DietaryFlags)
                .Must(dietaryFlags => IsValidDietaryFlags(dietaryFlags))
                .WithMessage("Invalid dietary flags");
        }

        private bool IsValidDietaryFlags(DietaryFlags flags)
        {
            return (flags & ~allDietaryFlagsChecked) == 0;  // ~00...01111 = 11...10000, если хотя бы один бит будет из неопределенных в enum-е - в результате будет не ноль
        }
    }
}