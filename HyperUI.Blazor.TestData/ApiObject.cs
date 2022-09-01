using Hydra.NET;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.TestData;

/// <summary>
/// API object base class.
/// </summary>
public abstract class ApiObject
{
    /// <summary>
    /// JSON-LD id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    // <summary>
    /// Hydra operations.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("operation")]
    public IEnumerable<Operation>? Operations { get; set; }
}
