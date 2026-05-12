namespace CookBookBackend.Api.DTO.DishIngredient
{
    public class DishIngredientDetailedDTO
    {
        public int FoodItemId { get; set; }
        public string? FoodItemName { get; set; }
        public decimal? AmountGrams { get; set; }
    }
}
