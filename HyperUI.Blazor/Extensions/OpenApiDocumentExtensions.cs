using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for the <see cref="OpenApiDocument"/> class.
/// </summary>
internal static class OpenApiDocumentExtensions
{
    /// <summary>
    /// Gets schemas referenced by another schema.
    /// </summary>
    /// <param name="openApiDocument">OpenAPI document.</param>
    /// <param name="schemaName">Schema name.</param>
    /// <returns>Referenced schemas.</returns>
    public static Dictionary<string, OpenApiSchema>? TryGetReferencedSchemas(
        this OpenApiDocument openApiDocument, string schemaName)
    {
        // Get defined schemas
        IDictionary<string, OpenApiSchema> definedSchemas = openApiDocument
            .Components
            .Schemas;

        // If the property isn't found, return null
        if (!definedSchemas.TryGetValue(schemaName, out OpenApiSchema? schema))
            return null;

        // Search for schema references
        IEnumerable<OpenApiReference> schemaReferences = schema.Properties
            .Where(property => property.Value.Reference?.Type == ReferenceType.Schema)
            .Select(property => property.Value.Reference);

        // If the schema doesn't reference other schemas, return null
        if (!schemaReferences.Any())
            return null;

        // Return referenced schemas
        return new Dictionary<string, OpenApiSchema>(definedSchemas
            .Join(
                schemaReferences,
                definedSchema => definedSchema.Key,
                schemaReference => schemaReference.Id,
                (definedSchema, _) => definedSchema));
    }
}
