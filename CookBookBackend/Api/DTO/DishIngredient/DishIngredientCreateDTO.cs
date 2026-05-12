namespace CookBookBackend.Api.DTO.DishIngredient
{
    public class DishIngredientCreateDTO
    {
        public int DishId {  get; set; }
        public int FoodItemId { get; set; }
        public decimal AmountGrams { get; set; }
    }
}
