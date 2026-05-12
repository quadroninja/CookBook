using CookBookBackend.Api.Converters;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json.Serialization;

namespace CookBookBackend.Core.Enums
{
    [JsonConverter(typeof(DietaryFlagsJsonConverter))]
    [Flags]
    public enum DietaryFlags
    {
        NONE        = 0,
        VEGAN       = 1 << 0,
        GLUTEN_FREE  = 1 << 1,
        SUGAR_FREE   = 1 << 2
    }
}
