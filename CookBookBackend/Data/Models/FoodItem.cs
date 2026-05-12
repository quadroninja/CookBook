using CookBookBackend.Api.Controllers;
using CookBookBackend.Core.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CookBookBackend.Data.Models
{
    public class FoodItem : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string>? PhotoPaths { get; set; } = new(); // хранятся пути к фото в папке wwwroot
        public decimal Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
        public decimal Carbohydrates { get; set; }
        public string? Contents { get; set; } //переименовать?
        public FoodItemCategory Category { get; set; }

        public List<DishIngredient> DishesWithThisIngredient { get; set; } = new();
        public ReadinessToEat ReadinessToEat { get; set; }
        public DietaryFlags? DietaryFlags { get; set; } = null; //FluentValidation обработает этот случай

    }
}
