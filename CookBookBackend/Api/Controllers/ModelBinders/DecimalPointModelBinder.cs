using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;

namespace CookBookBackend.Api.Controllers.ModelBinders
{
    public class DecimalPointModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            string value = valueProviderResult.FirstValue ?? "";
            if (string.IsNullOrWhiteSpace(value)) return Task.CompletedTask;

            value = value.Replace(",", ".");

            if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                bindingContext.Result = ModelBindingResult.Success(result);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Invalid decimal format.");
            }

            return Task.CompletedTask;
        }
    }

}
