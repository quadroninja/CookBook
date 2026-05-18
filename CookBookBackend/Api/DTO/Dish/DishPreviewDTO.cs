namespace CookBookBackend.Api.DTO.Dish
{
    public class DishPreviewDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }

        public decimal Calories { get; set; }
        public decimal Proteins { get; set; }
        public decimal Fats { get; set; }
        public decimal Carbohydrates { get; set; }
    }
}
