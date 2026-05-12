using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace CookBookBackend.Api.Controllers.ModelBinders
{
    public class DecimalPointModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(decimal))
            {
                return new BinderTypeModelBinder(typeof(DecimalPointModelBinder));
            }
            return null;
        }

    }
}
