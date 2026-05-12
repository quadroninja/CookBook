using CookBookBackend.Core.Enums;

namespace CookBookBackend.Data.Models
{
    public class Dish : BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string>? PhotoPaths { get; set; } = []; // хранятся пути к фото в папке wwwroot

        public decimal Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
        public decimal Carbohydrates { get; set; }

        public List<DishIngredient> Ingredients { get; set; } = [];//DishIngredient - связующая таблица 
        public decimal ServingSize { get; set; }
        public DishCategory Category { get; set; }

        public DietaryFlags DietaryFlags { get; set; }

    }
}
