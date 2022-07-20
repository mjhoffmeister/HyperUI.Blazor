using System.Buffers;
using System.Text.Json;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for <see cref="JsonElement"/>.
/// </summary>
public static class JsonElementExtensions
{
    /// <summary>
    /// Converts the <see cref="JsonElement"/> into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">Object type.</typeparam>
    /// <param name="jsonElement">The <see cref="JsonElement"/> to convert.</param>
    /// <param name="options">Serialization options (optional.)</param>
    /// <returns>The converted <see cref="JsonElement"/>.</returns>
    public static T? GetObject<T>(
        this JsonElement jsonElement, JsonSerializerOptions? options = null)
    {
        // TODO: the following should be available in a future release of System.Text.Json 

        /*******************************************************************************/
        /* var operations = JsonSerializer.Deserialize<IEnumerable<Operation>>(        */
        /*     operationsElement);                                                     */
        /*******************************************************************************/

        var bufferWriter = new ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(bufferWriter)) jsonElement.WriteTo(writer);
        return JsonSerializer.Deserialize<T>(bufferWriter.WrittenSpan, options);
    }

    /// <summary>
    /// Converts the <see cref="JsonElement"/> into an object based on its kind. Currently, only
    /// primitive conversions are implemented.
    /// </summary>
    /// <param name="jsonElement">The <see cref="JsonElement"/> to convert.</param>
    /// <returns>The converted <see cref="JsonElement"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    public static object? GetValue(this JsonElement jsonElement) => jsonElement.ValueKind switch
    {
        JsonValueKind.False or JsonValueKind.True => jsonElement.GetBoolean(),
        JsonValueKind.Null => null,
        JsonValueKind.Number => jsonElement.GetInt32(),
        JsonValueKind.Object => jsonElement,
        JsonValueKind.Undefined => new ArgumentException("Cannot convert a JsonElement with" +
            "an undefined value kind."),
        JsonValueKind.String => jsonElement.GetString(),
        _ => throw new NotImplementedException($"Conversion from ${jsonElement.ValueKind} is " +
                "not yet implemented")
    };
}
