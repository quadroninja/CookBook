using CookBookBackend.Data.Models;

namespace CookBookBackend.Core.Utils
{
    public static class DishNutritionCalculator
    {
        public static NutritionalValues CalculateFromIngredients(IEnumerable<DishIngredient> ingredients)
        {
            var totalCalories       = 0m;
            var totalProteins       = 0m;
            var totalFats           = 0m;
            var totalCarbohydrates  = 0m;

            foreach (var ingredient in ingredients)
            {
                var foodItem = ingredient.FoodItem;
                var fraction = (decimal)(ingredient.AmountGrams / 100.0m);

                totalCalories       += foodItem.Calories;
                totalProteins       += foodItem.Proteins;
                totalFats           += foodItem.Fats;
                totalCarbohydrates  += foodItem.Carbohydrates;
            }

            return new NutritionalValues(
                Math.Round(totalCalories, 2),
                Math.Round(totalProteins, 2),
                Math.Round(totalFats, 2),
                Math.Round(totalCarbohydrates, 2));
        }
    }
}
