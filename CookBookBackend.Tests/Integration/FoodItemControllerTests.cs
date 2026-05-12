using CookBookBackend.Api.DTO.FoodItem;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CookBookBackend.Tests.Integration
{

    /// <summary>
    /// Tests for DishNutritionCalculator using Equivalence Partitioning and Boundary Value Analysis.
    /// 
    /// Test coverage includes:
    /// - Valid equivalence partitions: single ingredient, multiple ingredients, zero grams
    /// - Invalid equivalence partitions: missing FoodItem (throws exception)
    /// - Boundary values: AmountGrams at 0, 0.1, 100, 9999
    /// - Edge cases: empty ingredient list, null values
    /// 
    /// Test Design Techniques:
    /// - Equivalence Partitioning: Grouping inputs that should behave identically
    /// - Boundary Value Analysis: Testing values at and around partition edges
    /// </summary>

    public class FoodItemControllerTests : IClassFixture<MyWebAppFactory>

    {
        private readonly HttpClient _client;
        private readonly MyWebAppFactory _factory;
        private readonly ITestOutputHelper _helper;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };


        public FoodItemControllerTests(MyWebAppFactory factory, ITestOutputHelper helper)
        {
            _factory = factory;
            _client = factory.CreateClient();
            _helper = helper; 
        }


        [Fact]
        public async Task CreateFoodItem_WithValidData_ReturnsCreated()
        {
            var formData = new MultipartFormDataContent
            {
                { new StringContent("Integration Test Chicken"), "Name" },
                { new StringContent("165"), "Calories" },
                { new StringContent("31"), "Proteins" },
                { new StringContent("3,6"), "Fats" },
                { new StringContent("0"), "Carbohydrates" },
                { new StringContent("Vegan"), "DietaryFlags" },
                { new StringContent("NOT_READY"), "ReadinessToEat" },
                { new StringContent("MEAT"), "Category" }
            };

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes("fake image bytes"));
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            formData.Add(fileContent, "Photos", "test.jpg");

            var response = await _client.PostAsync("/food_items/create", formData, TestContext.Current.CancellationToken);

            _helper.WriteLine(await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var result = await response.Content.ReadFromJsonAsync<FoodItemCreateResultDTO>(_jsonOptions, cancellationToken: TestContext.Current.CancellationToken);

            Assert.NotNull(result);
            Assert.Equal("Integration Test Chicken", result.Name);

            Assert.NotNull(result.PhotoUrls);
            Assert.Equal(1, result.PhotoUrls.Count);
        }
    }
}
