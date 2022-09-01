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
public class User : ApiObject
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
    /// Active indicator.
    /// </summary>
    [JsonPropertyName("isActive")]
    public bool? IsActive { get; set; }

    /// <summary>
    /// Security admin indicator.
    /// </summary>
    [JsonPropertyName("isSecurityAdmin")]
    public bool? IsSecurityAdmin { get; set; }

    /// <summary>
    /// User admin indicator.
    /// </summary>
    [JsonPropertyName("isUserAdmin")]
    public bool? IsUserAdmin { get; set; }

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
        Extensions = new Dictionary<string, IOpenApiExtension>()
        {
            [PropertyGroups.OpenApiExtensionName] = new OpenApiObject
            {
                ["Roles"] = PropertyGroup.CreateOpenApiSpecification(
                    "isSecurityAdmin", "isUserAdmin")
            }
        },
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
                Title = "Name",
                Type = OpenApiDataType.String,
            },
            ["emailAddress"] = new OpenApiSchema
            {
                Title = "Email address",
                Type = OpenApiDataType.String,
            },
            ["isSecurityAdmin"] = new OpenApiSchema
            {
                Format = OpenApiBooleanFormat.Choice,
                Title = "Security admin",
                Type = OpenApiDataType.Boolean
            },
            ["isUserAdmin"] = new OpenApiSchema
            {
                Format = OpenApiBooleanFormat.Choice,
                Title = "User admin",
                Type = OpenApiDataType.Boolean
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
            ["isActive"] = new OpenApiSchema
            {
                Title = "Active",
                Type = OpenApiDataType.Boolean
            }
        }
    };
}