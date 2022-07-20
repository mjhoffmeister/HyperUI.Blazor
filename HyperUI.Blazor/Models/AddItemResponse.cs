using Hydra.NET;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Response for an added item.
/// </summary>
internal class AddItemResponse
{
    /// <summary>
    /// Context.
    /// </summary>
    [JsonPropertyName("@context")]
    public static Context Context => new(new Uri("http://www.w3.org/ns/hydra/context.jsonld"));

    /// <summary>
    /// Id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Operations.
    /// </summary>
    [JsonPropertyName("operation")]
    public IEnumerable<Operation>? Operations { get; set; }
}
