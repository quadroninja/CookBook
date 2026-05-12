namespace CookBookBackend.Api.Converters
{
    using CookBookBackend.Core.Enums;
    using System.Text.Json;
    using System.Text.Json.Serialization;

    public class DietaryFlagsJsonConverter : JsonConverter<DietaryFlags>
    {
        public override DietaryFlags Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            //для строк через запятую: "Vegan, GlutenFree"
            if (reader.TokenType == JsonTokenType.String)
            {
                var stringValue = reader.GetString();
                return ParseDietaryFlagsFromString(stringValue);
            }
            //для списков ["Vegan", "GlutenFree"]
            else if (reader.TokenType == JsonTokenType.StartArray)
            {
                var flags = DietaryFlags.NONE;
                while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                {
                    if (reader.TokenType == JsonTokenType.String)
                    {
                        var flagName = reader.GetString();
                        if (Enum.TryParse<DietaryFlags>(flagName, true, out var flag))
                        {
                            flags |= flag;
                        }
                    }
                }
                return flags;
            }

            throw new JsonException($"Unexpected token type for DietaryFlags: {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DietaryFlags value, JsonSerializerOptions options)
        {
            var flagNames = Enum.GetValues<DietaryFlags>()
                .Where(f => f != DietaryFlags.NONE && value.HasFlag(f))
                .Select(f => f.ToString());

            writer.WriteStringValue(string.Join(", ", flagNames));
        }

        private DietaryFlags ParseDietaryFlagsFromString(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return DietaryFlags.NONE;

            var trimmed = input.Trim();
            if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
            {
                return ParseDietaryFlagsFromJsonArray(trimmed);
            }


            var flags = DietaryFlags.NONE;
            var parts = input.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (Enum.TryParse<DietaryFlags>(part.Trim(), true, out var flag))
                {
                    flags |= flag;
                }
            }

            return flags;
        }

        private DietaryFlags ParseDietaryFlagsFromJsonArray(string? input)
        {
            var flags = DietaryFlags.NONE;

            var content = input.TrimStart('[').TrimEnd(']');

            if (string.IsNullOrWhiteSpace(content))
                return DietaryFlags.NONE;

            var flagNames = content.Split(',')
                .Select(part => part.Trim().Trim('"').Trim('\''))
                .Where(part => !string.IsNullOrEmpty(part));

            foreach (var flagName in flagNames)
            {
                if (Enum.TryParse<DietaryFlags>(flagName, true, out var flag))
                {
                    flags |= flag;
                }
            }

            return flags;

        }
    }
}
