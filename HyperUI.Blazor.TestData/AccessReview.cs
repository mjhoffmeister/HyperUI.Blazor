using Hydra.NET;
using HyperUI.Core;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.TestData;

public class AccessReview : ApiObject
{
    /// <summary>
    /// JSON-LD context.
    /// </summary>
    [JsonPropertyName("@context")]
    public static Context Context => new(new Uri("http://www.w3.org/ns/hydra/context.jsonld"));

    /// <summary>
    /// Day.
    /// </summary>
    [JsonPropertyName("day")]
    public string? Day { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Time.
    /// </summary>
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    /// <summary>
    /// Gets the OpenAPI schema for the access review class.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "AccessReview",
        Type = "object",
        Required = new HashSet<string>()
        {
            "day",
            "description",
            "time"
        },
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["description"] = new OpenApiSchema()
            {
                Title = "Description",
                Type = OpenApiDataType.String,
            },
            ["day"] = new OpenApiSchema
            {
                Format = OpenApiStringFormat.DayOfWeek,
                Title = "Day",
                Type = OpenApiDataType.String,
            },
            ["time"] = new OpenApiSchema
            {
                Format = OpenApiStringFormat.Time,
                Title = "Time",
                Type = OpenApiDataType.String,
            }
        }
    };
}
