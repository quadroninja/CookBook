using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Api.DTO.DishIngredient;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBookBackend.Api.DTO.Dish
{
    public class DishCreateDTO
    {
        public string Name { get; set; }
        public decimal? Calories { get; set; }
        public decimal? Proteins { get; set; }
        public decimal? Fats { get; set; }
        public decimal? Carbohydrates { get; set; }

        public List<DishIngredientCreateDTO> Ingredients { get; set; } = [];
        public decimal ServingSize { get; set; }
        public DishCategory Category { get; set; }

        [ModelBinder(typeof(DietaryFlagsModelBinder))]
        public DietaryFlags DietaryFlags { get; set; } = DietaryFlags.NONE;
        public List<IFormFile>? Photos { get; set; } 

    }
}