using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBookBackend.Api.DTO.FoodItem
{
    public class FoodItemEditDTO
    {
        public List<IFormFile>? Photos { get; set; } = null;
        public List<string>? PhotoUrlsToDelete { get; set; } = null;
        public string? Name { get; set; } = null;
        public decimal? Calories { get; set; } = null;
        public decimal? Proteins { get; set; } = null;
        public decimal? Fats { get; set; } = null;
        public decimal? Carbohydrates { get; set; } = null;
        public string? Contents { get; set; } = null;
        public FoodItemCategory? Category { get; set; } = null;
        public ReadinessToEat? ReadinessToEat { get; set; } = null;
        [ModelBinder(BinderType = typeof(DietaryFlagsModelBinder))]
        public DietaryFlags? DietaryFlags { get; set; } = null;
    }
}