using CookBookBackend.Core.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json;

namespace CookBookBackend.Api.Controllers.ModelBinders
{
    public class DietaryFlagsModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            var value = valueProviderResult.FirstValue;

            if (string.IsNullOrEmpty(value))
            {
                bindingContext.Result = ModelBindingResult.Success(DietaryFlags.NONE);
                return Task.CompletedTask;
            }

            var trimmed = value.Trim();
            var flags = DietaryFlags.NONE;

            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                try
                {
                    using var doc = JsonDocument.Parse(trimmed);
                    if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var element in doc.RootElement.EnumerateArray())
                        {
                            if (element.ValueKind == JsonValueKind.String &&
                                Enum.TryParse<DietaryFlags>(element.GetString(), true, out var flag))
                            {
                                flags |= flag;
                            }
                        }
                    }
                }
                catch (JsonException)
                {
                }
            }
            else if (trimmed.Contains(',') && !trimmed.Contains('['))
            {
                var parts = trimmed.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var part in parts)
                {
                    if (Enum.TryParse<DietaryFlags>(part.Trim(), true, out var flag))
                    {
                        flags |= flag;
                    }
                }
            }
            else if (Enum.TryParse<DietaryFlags>(trimmed, true, out var singleFlag) ||
                        (int.TryParse(trimmed, out var intValue) && Enum.IsDefined(typeof(DietaryFlags), intValue)))
            {
                flags = singleFlag;
            }

            bindingContext.Result = ModelBindingResult.Success(flags);
            return Task.CompletedTask;
        }
    }
}
