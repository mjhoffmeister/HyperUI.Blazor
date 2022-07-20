using HyperUI.Core;
using Microsoft.OpenApi.Models;
using MudBlazor;
using System.Text.Json;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for the <see cref="OpenApiSchema"/> class.
/// </summary>
internal static class OpenApiSchemaExtensions
{
    public static Type GetComponentType(this OpenApiSchema schema)
    {
        if (schema.Type == OpenApiDataType.Boolean)
            return typeof(MudSwitch<bool?>);

        if (schema.Type == OpenApiDataType.String)
            return typeof(TextInput);

        throw new NotImplementedException();
    }

    /// <summary>
    /// Tries to get nested properties.
    /// </summary>
    /// <param name="propertySchema">Property schema.</param>
    /// <param name="property">Property object.</param>
    /// <returns>Nested properties, if they could be retrieved.</returns>
    public static IEnumerable<NestedProperty>? TryGetNestedProperties(
        this OpenApiSchema? propertySchema, object? property)
    {
        if (propertySchema == null || property is not JsonElement jsonObject)
            return null;

        List<NestedProperty> nestedProperties = new();

        Dictionary<string, JsonElement>? parentPropertyMap =
            property as Dictionary<string, JsonElement>;

        IEnumerable<KeyValuePair<string, OpenApiSchema>> allNestedSchemasButLast =
            propertySchema.Properties.SkipLast(1);

        foreach (KeyValuePair<string, OpenApiSchema> nestedSchema in allNestedSchemasButLast)
        {
            GetNestedProperty(nestedSchema);
        }

        nestedProperties.AddRange(allNestedSchemasButLast
            .Select(nestedSchema => GetNestedProperty(nestedSchema)));

        nestedProperties.Add(GetNestedProperty(propertySchema.Properties.Last(), true));

        return nestedProperties;

        NestedProperty GetNestedProperty(
            KeyValuePair<string, OpenApiSchema> schema, bool isLast = false)
        {
            object? value = null;

            if (jsonObject.TryGetProperty(
                schema.Key, out JsonElement jsonElement) == true)
            {
                value = jsonElement.GetValue();
            }

            OpenApiSchema schemaValue = schema.Value;

            return new(isLast, schemaValue.Type, schemaValue.Title, value);
        }
    }
}
