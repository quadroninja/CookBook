using Microsoft.EntityFrameworkCore;

namespace CookBookBackend.Data.Models
{
    [PrimaryKey(nameof(DishId), nameof(FoodItemId))]
    public class DishIngredient
    {
        public int DishId { get; set; }
        public Dish Dish { get; set; }

        public int FoodItemId { get; set; }
        public FoodItem FoodItem { get; set; }

        public decimal AmountGrams { get; set; }  // количество в граммах
    }

}
