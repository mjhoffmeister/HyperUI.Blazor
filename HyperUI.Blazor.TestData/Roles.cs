using HyperUI.Core;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.TestData;

/// <summary>
/// User roles.
/// </summary>
public class Roles
{
    /// <summary>
    /// Default constructor for JSON deserialization.
    /// </summary>
    public Roles()
    {

    }

    /// <summary>
    /// Creates a new <see cref="Roles"/>.
    /// </summary>
    /// <param name="isSecurityAdmin">Security admin indicator.</param>
    /// <param name="isUserAdmin">User admin indicator.</param>
    public Roles(bool? isSecurityAdmin, bool? isUserAdmin)
    {
        IsSecurityAdmin = isSecurityAdmin;
        IsUserAdmin = isUserAdmin;
    }

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
    /// Gets the OpenAPI schema for the roles class.
    /// </summary>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Title = "Roles",
        Type = OpenApiDataType.Object,
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["isSecurityAdmin"] = new OpenApiSchema
            {
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    ["x-group"] = new OpenApiString("Role")
                },
                Title = "Security admin",
                Type = OpenApiDataType.Boolean
            },
            ["isUserAdmin"] = new OpenApiSchema
            {
                Title = "User admin",
                Type = OpenApiDataType.Boolean
            }
        }
    };
}
