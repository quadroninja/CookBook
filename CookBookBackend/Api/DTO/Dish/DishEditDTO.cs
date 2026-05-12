using CookBookBackend.Api.DTO.DishIngredient;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;

namespace CookBookBackend.Api.DTO.Dish
{
    public class DishEditDTO
    {

        public List<IFormFile>? Photos { get; set; } = null;
        public List<string>? PhotoUrlsToDelete { get; set; } = null;

        public string? Name { get; set; } = null;
        public decimal? Calories { get; set; } = null;
        public decimal? Proteins { get; set; } = null;
        public decimal? Fats { get; set; } = null;
        public decimal? Carbohydrates { get; set; } = null;

        public List<DishIngredientCreateDTO>? Ingredients { get; set; } = null;
        public decimal? ServingSize { get; set; } = null;
        public DishCategory? Category { get; set; } = null;

        public DietaryFlags? DietaryFlags { get; set; } = null;
    }
}