using CookBookBackend.Core.ValueObjects;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Core.Services
{
    public class NutritionFactsService
    {
        public NutritionFacts CalculateFromIngredients(IEnumerable<DishIngredient> ingredients)
        {
            var totalCalories = 0m;
            var totalProteins = 0m;
            var totalFats = 0m;
            var totalCarbohydrates = 0m;

            foreach (var ingredient in ingredients)
            {
                var foodItem = ingredient.FoodItem;
                var fraction = (decimal)(ingredient.AmountGrams / 100.0m);

                totalCalories += foodItem.Calories * fraction;
                totalProteins += foodItem.Proteins * fraction;
                totalFats += foodItem.Fats * fraction;
                totalCarbohydrates += foodItem.Carbohydrates * fraction;
            }

            return new NutritionFacts(
                Math.Round(totalCalories, 2),
                Math.Round(totalProteins, 2),
                Math.Round(totalFats, 2),
                Math.Round(totalCarbohydrates, 2));
        }
    }
}
