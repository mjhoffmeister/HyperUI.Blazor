using Hydra.NET;
using HyperUI.Core;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.TestData;

/// <summary>
/// User model.
/// </summary>
/// <param name="FullName">Full name.</param>
/// <param name="EmailAddress">Email address.</param>
/// <param name="Type">Type.</param>
/// <param name="IsActive">Active indicator.</param>
public class User
{
    /// <summary>
    /// JSON-LD context.
    /// </summary>
    [JsonPropertyName("@context")]
    public static Context Context => new(new Uri("http://www.w3.org/ns/hydra/context.jsonld"));

    /// <summary>
    /// Email address.
    /// </summary>
    [JsonPropertyName("emailAddress")]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Full name.
    /// </summary>
    [JsonPropertyName("fullName")]
    public string? FullName { get; set; }

    /// <summary>
    /// JSON-LD id.
    /// </summary>
    [JsonPropertyName("@id")]
    public string? Id { get; set; }

    /// <summary>
    /// Active indicator.
    /// </summary>
    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Hydra operations.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("operation")]
    public IEnumerable<Operation>? Operations { get; set; }

    /// <summary>
    /// Roles
    /// </summary>
    [JsonPropertyName("roles")]
    public Roles? Roles { get; set; }

    /// <summary>
    /// Type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>
    /// Gets the OpenAPI schema for the user class.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "User",
        Type = "object",
        Required = new HashSet<string>() 
        { 
            "emailAddress",
            "fullName",
            "isActive",
            "roles",
            "type"
        },
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["fullName"] = new OpenApiSchema
            {
                ReadOnly = true,
                Title = "Name",
                Type = OpenApiDataType.String,
            },
            ["emailAddress"] = new OpenApiSchema
            {
                Title = "Email address",
                Type = OpenApiDataType.String,
            },
            ["type"] = new OpenApiSchema
            {
                Title = "Type",
                Type = OpenApiDataType.String,
                Enum = new List<IOpenApiAny>()
                {
                    new OpenApiString("Basic"),
                    new OpenApiString("Admin")
                }
            },
            ["roles"] = new OpenApiSchema
            {
                Reference = new OpenApiReference()
                {
                    Id = "Roles",
                    Type = ReferenceType.Schema
                }
            },
            ["isActive"] = new OpenApiSchema
            {
                Title = "Active",
                Type = OpenApiDataType.Boolean
            }
        }
    };
}