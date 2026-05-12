using CookBookBackend.Api.DTO.DishIngredient;
using CookBookBackend.Core.Enums;

namespace CookBookBackend.Api.DTO.Dish
{
    public class DishCreateResultDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? PhotoUrls { get; set; } = [];
        public decimal Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
        public decimal Carbohydrates { get; set; }
        public List<DishIngredientCreateDTO> Ingredients { get; set; } = [];
        public decimal ServingSize { get; set; }
        public DishCategory Category { get; set; }
        public DietaryFlags DietaryFlags { get; set; } = DietaryFlags.NONE;
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; } = null;
    }
}