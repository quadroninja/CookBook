namespace CookBookBackend.Core.Exceptions
{
    public class FoodItemInUseException : Exception
    {
        public int FoodItemId { get; }
        public string FoodItemName { get; }
        public List<string> DishNames;

        public FoodItemInUseException(int foodItemId, string foodItemName, List<string> dishNames)
        : base($"Food item '{foodItemName}' (ID: {foodItemId}) cant be deleted because it is used in dishes: {string.Join(", ", dishNames)}")
        {
            FoodItemId = foodItemId;
            FoodItemName = foodItemName;
            DishNames = dishNames;
        }

    }
}
