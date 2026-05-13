using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.DishIngredient;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;
using FluentValidation;
using FluentValidation.Validators;

namespace CookBookBackend.Api.DTO.Validators
{
    public class DishCreateValidator : AbstractValidator<DishCreateDTO>
    {


        private DietaryFlags allDietaryFlagsChecked;

        public DishCreateValidator() 
        {
            allDietaryFlagsChecked = (DietaryFlags)Enum.GetValues<DietaryFlags>()
                .Where(f => f != DietaryFlags.NONE)
                .Select(f => f)
                .Aggregate((DietaryFlags)0, (current, f) => current | f);


            RuleFor(dish => dish.Photos)
                .Must(photos => (photos?.Count ?? 0) <= 5)
                .When(dish => dish.Photos is not null)
                .WithMessage($"Maximum 5 photos allowed");
            RuleFor(dish => dish.Name)
                .NotEmpty()
                .WithMessage("Name should not be empty");
            RuleFor(dish => dish.Calories)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Calories should not negative");
            RuleFor(dish => dish.Proteins)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Proteins should not be negative");
            RuleFor(dish => dish.Fats)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Fats should not be negative");
            RuleFor(dish => dish.Carbohydrates)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Carbohydrates should not be negative");

            RuleFor(foodItem => foodItem.Category)
                .Must(category => category != DishCategory.NONE);

            RuleFor(foodItem => foodItem.DietaryFlags)
                .Must(dietaryFlags => IsValidDietaryFlags(dietaryFlags))
                .WithMessage("Invalid dietary flags");

            RuleFor(dish => dish.Ingredients)
                .NotNull().WithMessage("Ingredients list cannot be null")
                .Must(ingredients => ingredients?.Any() ?? false).WithMessage("A dish must contain at least one ingredient");

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
        }
        private bool IsValidDietaryFlags(DietaryFlags flags)
        {
            return (flags & ~allDietaryFlagsChecked) == 0;  // ~00...01111 = 11...10000, если хотя бы один бит будет из неопределенных в enum-е - в результате будет не ноль
        }
    }

}



