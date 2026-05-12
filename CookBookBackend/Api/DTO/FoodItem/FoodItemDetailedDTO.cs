using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Core.Enums;
using CookBookBackend.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace CookBookBackend.Api.DTO.FoodItem
{
    public class FoodItemDetailedDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
        public decimal Carbohydrates { get; set; }
        public string? Contents { get; set; } //переименовать?
        public FoodItemCategory Category { get; set; }
        public ReadinessToEat ReadinessToEat { get; set; }
        [ModelBinder(BinderType = typeof(DietaryFlagsModelBinder))]
        public DietaryFlags DietaryFlags { get; set; } = DietaryFlags.NONE;
        public List<string>? PhotoUrls { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset? UpdatedOn { get; set; } = null;
    }
}