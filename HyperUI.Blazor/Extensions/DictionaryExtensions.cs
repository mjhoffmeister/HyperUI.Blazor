using Hydra.NET;
using System.Text.Json;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for <see cref="IDictionary{TKey, TValue}"/>, which serves as the data object
/// type.
/// </summary>
internal static class DictionaryExtensions
{
    /// <summary>
    /// Determines whether the dictionary representing an object can be deleted.
    /// </summary>
    /// <param name="context">The dictionary to evaluate.</param>
    /// <returns>
    /// <see cref="true"/> if the object can be deleted; <see cref="false"/>, otherwise.
    /// </returns>
    public static bool CanDelete(this Dictionary<string, object?> context) =>
        context.DoOperationsExist(o => o.Method == Method.Delete);

    /// <summary>
    /// Determines whether the dictionary representing an object can be edited.
    /// </summary>
    /// <param name="context">The dictionary to evaluate.</param>
    /// <returns>
    /// <see cref="true"/> if the object can be edited; <see cref="false"/>, otherwise.
    /// </returns>
    public static bool CanEdit(this Dictionary<string, object?> context) =>
        context.DoOperationsExist(o => o.Method == Method.Put);

    /// <summary>
    /// Converts <see cref="JsonElement"/>s into expected types, when possible.
    /// </summary>
    /// <param name="jsonObject">The JSON object to convert.</param>
    /// <returns>The converted data</returns>
    public static Dictionary<string, object?> ConvertJson(
        this Dictionary<string, JsonElement> jsonObject)
    {
        Dictionary<string, object?> @object = new();

        foreach ((string key, JsonElement jsonValue) in jsonObject)
        {
            object? value = key == "operation" ?
                jsonValue.GetObject<IEnumerable<Operation>>() :
                jsonValue.GetValue();

            @object.Add(key, value);
        }

        return @object;
    }

    /// <summary>
    /// Converts <see cref="JsonElement"/>s in a collection into expected types, when possible.
    /// </summary>
    /// <param name="jsonObjects">The collection of JSON objects to convert.</param>
    /// <returns>The converted data.</returns>
    public static List<Dictionary<string, object?>> ConvertJson(
        this IEnumerable<Dictionary<string, JsonElement>> jsonObjects)
    {
        List<Dictionary<string, object?>> objects = new();

        foreach (Dictionary<string, JsonElement> jsonObject in jsonObjects)
        {
            objects.Add(jsonObject.ConvertJson());
        }

        return objects;
    }

    /// <summary>
    /// Determines whether operations exist given a match function.
    /// </summary>
    /// <param name="context">The dictionary to evaluate.</param>
    /// <param name="matchFunction">Operation match function.</param>
    /// <returns>
    /// <see cref="true"/> if the operations exists; <see cref="false"/>, otherwise.
    /// </returns>
    public static bool DoOperationsExist(
        this Dictionary<string, object?> context, Func<Operation, bool> matchFunction)
    {
        // If the context object has a Hydra PUT operation, it can be edited
        if (context.TryGetValue("operation", out object? operationsObject))
        {
            // TODO: the following should be available in a future release of System.Text.Json 

            /*******************************************************************************/
            /* var operations = JsonSerializer.Deserialize<IEnumerable<Operation>>(        */
            /*     operationsElement);                                                     */
            /*******************************************************************************/

            if (operationsObject is IEnumerable<Operation> operations)
                return operations.Any(o => matchFunction(o));
        }

        return false;
    }

    /// <summary>
    /// Gets a value from the dictionary if its key exists; otherwise, null.
    /// </summary>
    /// <typeparam name="TKey">Key type.</typeparam>
    /// <typeparam name="TValue">Value type.</typeparam>
    /// <param name="dictionary"><see cref="IDictionary{TKey, TValue}"/>.</param>
    /// <param name="key">Key.</param>
    /// <returns>The value, if found.</returns>
    public static TValue? GetWritableValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key)
            where TKey : notnull
            where TValue : class?
    {
        if (dictionary.ContainsKey(key))
            return dictionary[key];

        return null;
    }

    /// <summary>
    /// Determines whether or not the dictionary has an actions key defined as DELETE or POST
    /// operations.
    /// </summary>
    /// <param name="dictionary">The dictionary to evaluate.</param>
    /// <returns>
    /// <see cref="true"/> if the context has actions; <see cref="false"/>, otherwise.
    /// </returns>
    public static bool HasActions(this Dictionary<string, object?> dictionary) =>
        dictionary.DoOperationsExist(o => o.Method is Method.Delete or Method.Post);
}
