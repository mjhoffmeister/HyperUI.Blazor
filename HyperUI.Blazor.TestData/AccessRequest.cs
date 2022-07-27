using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

namespace HyperUI.Blazor.TestData;

public class AccessRequest
{
    [JsonPropertyName("environment")]
    public string? Environment { get; set; }

    [JsonPropertyName("expirationDate")]
    public DateOnly? ExpirationDate { get; set; }

    [JsonPropertyName("isPermanent")]
    public bool? IsPermanent { get; set; }

    [JsonPropertyName("isTemporary")]
    public bool? IsTemporary { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("system")]
    public string? System { get; set; }

    public static OpenApiSchema GetOpenApiSchema() => new()
    {
        Extensions = new Dictionary<string, IOpenApiExtension>()
        {
            ["x-dependencies"] = new OpenApiArray()
            {
                new OpenApiString("OnlyOne(isPermanent, isTemporary);"),
                new OpenApiString("IF isTemporary==true THEN expirationDate;")
            }
        },
        Title = "Access request",
        Type = "object",
        Required = new HashSet<string> { "environment", "system", "role" },
        Properties = new Dictionary<string, OpenApiSchema>()
        {
            ["system"] = new OpenApiSchema()
            {
                Title = "System",
                Type = "string"
            },
            ["environment"] = new OpenApiSchema()
            {
                Title = "Environment",
                Type = "string",
                Enum = new List<IOpenApiAny>()
                {
                    new OpenApiObject()
                    {
                        ["@id"] = new OpenApiString("https://api.example.com/environments")
                    }
                }
            },
            ["role"] = new OpenApiSchema()
            {
                Title = "Role",
                Type = "string",
                Enum = new List<IOpenApiAny>()
                {
                    new OpenApiString("Standard"),
                    new OpenApiString("Elevated")
                }
            }
        }
    };
}
