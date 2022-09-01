using HyperUI.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MudBlazor;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for the <see cref="OpenApiSchema"/> class.
/// </summary>
internal static class OpenApiSchemaExtensions
{
    // Boolean property converter
    private static readonly BoolConverter<object?> _boolPropertyConverter = new()
    {
        GetFunc = value => value == true,
        SetFunc = boolObject => boolObject is bool value && value,
    };

    // String property converter for nullable objects
    private static readonly Converter<object?> _stringPropertyConverter = new()
    {
        SetFunc = stringObject => stringObject?.ToString(),
        GetFunc = text => (object?)text,
    };

    /// <summary>
    /// Gets configuration for dynamically-rendered components based on
    /// <see cref="OpenApiSchema"/>s.
    /// </summary>
    /// <param name="objectSchema">Object schema.</param>
    /// <param name="propertyKey">Property key.</param>
    /// <param name="value">Property value.</param>
    /// <param name="isReadOnlyOverride">Overrides <see cref="OpenApiSchema.ReadOnly"/>.</param>
    /// <param name="selectItemsBuilder">Builder for select items.</param>
    /// <param name="valueChangedCallback">Value changed callback</param>
    /// <returns>Component configuration.</returns>
    public static (IDictionary<string, object?>, Type) GetComponentConfiguration(
        this OpenApiSchema objectSchema,
        string propertyKey,
        object? value,
        bool isReadOnlyOverride,
        Func<IList<IOpenApiAny>, RenderFragment> selectItemsBuilder,
        EventCallback<object> valueChangedCallback)
    {
        // Property schema not found
        if (!objectSchema.Properties.TryGetValue(propertyKey, out OpenApiSchema? propertySchema))
            return NothingConfiguration(propertyKey, "Schema not found");

        bool isReadOnly = isReadOnlyOverride || propertySchema.ReadOnly;

        // Boolean schema type
        if (propertySchema.Type == OpenApiDataType.Boolean)
        {
            return BooleanConfiguration(
                propertySchema, propertyKey, value, isReadOnly, valueChangedCallback);
        }

        // String schema type
        if (propertySchema.Type == OpenApiDataType.String)
        {
            return StringConfiguration(
                propertySchema, value, isReadOnly, selectItemsBuilder, valueChangedCallback);
        }

        // Unsupported schema type
        return NothingConfiguration(propertyKey, "Unsupported schema type");
    }

    /// <summary>
    /// Configuration for components that render boolean properties.
    /// </summary>
    /// <param name="propertySchema">Property schema.</param>
    /// <param name="propertyKey">Property key.</param>
    /// <param name="value">Value.</param>
    /// <param name="isReadOnly">Read-only indicator.</param>
    /// <returns>Configuration.</returns>
    private static (IDictionary<string, object?>, Type) BooleanConfiguration(
        OpenApiSchema propertySchema,
        string propertyKey,
        object? value,
        bool isReadOnly,
        EventCallback<object> valueChangedCallback)
    {
        Type type = typeof(MudSwitch<object?>);

        Dictionary<string, object?> booleanParameters = new()
        {
            ["Checked"] = value,
            ["Disabled"] = isReadOnly,
            ["StopClickPropagation"] = !isReadOnly,
            ["CheckedChanged"] = valueChangedCallback
        };

        if (!isReadOnly)
            booleanParameters.Add("Converter", _boolPropertyConverter);

        if (propertySchema.Format == OpenApiBooleanFormat.Choice)
        {
            booleanParameters.Add("Dense", true);
            booleanParameters.Add("Label", propertySchema.Title ?? propertyKey);
            booleanParameters.Add("Style", "margin-bottom: 2px;");

            type = typeof(MudCheckBox<object?>);
        }

        return (booleanParameters, type);
    }

    /// <summary>
    /// Configuration for the <see cref="Nothing"/> component.
    /// </summary>
    /// <param name="propertyKey">Property key for which nothing will be displayed.</param>
    /// <param name="message">Message.</param>
    /// <returns>Configuration.</returns>
    private static (IDictionary<string, object?>, Type) NothingConfiguration(
        string propertyKey, string message)
    {
        return
        (
            new Dictionary<string, object?>()
            {
                ["PropertyKey"] = propertyKey,
                ["Message"] = message
            },
            typeof(Nothing)
        );
    }

    /// <summary>
    /// Configuration for components that render string properties.
    /// </summary>
    /// <param name="propertySchema">Property schema.</param>
    /// <param name="value">Value.</param>
    /// <param name="isReadOnly">Read-only indicator.</param>
    /// <param name="selectItemsBuilder">
    /// Function for building a <see cref="RenderFragment"/> of <see cref="MudSelectItem{T}"/>s.
    /// </param>
    /// <param name="valueChangedCallback">Value changed callback</param>
    /// <returns>Configuration</returns>
    private static (IDictionary<string, object?>, Type) StringConfiguration(
        OpenApiSchema propertySchema,
        object? value,
        bool isReadOnly,
        Func<IList<IOpenApiAny>, RenderFragment> selectItemsBuilder,
        EventCallback<object> valueChangedCallback)
    {
        Dictionary<string, object?> stringParameters = new()
        {
            ["Value"] = value
        };

        if (isReadOnly)
        {
            if (TimeOnly.TryParse((string?)value, out TimeOnly time))
                stringParameters["Value"] = time.ToString("h:mm tt");

            return (stringParameters, typeof(ReadOnlyText));
        }

        stringParameters.Add("Class", "mb-4");
        stringParameters.Add("Margin", Margin.Dense);
        stringParameters.Add("ValueChanged", valueChangedCallback);

        if (propertySchema.Enum.Any() || propertySchema.Format == OpenApiStringFormat.DayOfWeek)
        {
            stringParameters.Add("T", typeof(object));
            stringParameters.Add("ChildContent", BuildSelectItems());
            stringParameters.Add("AnchorOrigin", Origin.BottomCenter);

            return (stringParameters, typeof(MudSelect<object>));
        }

        stringParameters.Add("Converter", _stringPropertyConverter);

        if (propertySchema.Format == OpenApiStringFormat.Time)
            stringParameters.Add("InputType", InputType.Time);

        return (stringParameters, typeof(MudTextField<object?>));

        RenderFragment BuildSelectItems()
        {
            if (propertySchema.Format == OpenApiStringFormat.DayOfWeek)
                return DaysOfWeek.RenderSelectItems();

            return selectItemsBuilder(propertySchema.Enum);
        }
    }
}