using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CookBookBackend.Api.Controllers.ModelBinders
{
    public class NullableModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext context)
        {
            var value = context.ValueProvider.GetValue(context.ModelName).FirstValue;

            if (string.IsNullOrWhiteSpace(value))
            {
                context.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            if (decimal.TryParse(value, out var result))
            {
                context.Result = ModelBindingResult.Success(result);
            }

            return Task.CompletedTask;
        }
    }
}
