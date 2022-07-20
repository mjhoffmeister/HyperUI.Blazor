using System.Diagnostics.CodeAnalysis;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Extension methods for <see cref="OpenApiPathItem"/>.
/// </summary>
internal static class OpenApiPathItemExtensions
{
    /// <summary>
    /// Determines whether the path's GET operation returns an array.
    /// </summary>
    /// <param name="path">The <see cref="OpenApiPathItem"/> to evaluate.</param>
    /// <param name="itemSchemaReferenceId">
    /// If the path's GET operation returns an array, the reference id of the array's item
    /// schema.
    /// </param>
    /// <returns>
    /// <see cref="true"/> if the path's GET operation returns an array; <see cref="false"/>, 
    /// otherwise.
    /// </returns>
    public static bool DoesGetOperationReturnArray(
        this OpenApiPathItem path, [NotNullWhen(true)] out string? itemSchemaReferenceId)
    {
        OpenApiSchema? schema = path
            .Operations.GetWritableValueOrDefault(OperationType.Get)?
            .Responses.GetValueOrDefault("200")?
            .Content.GetWritableValueOrDefault("application/ld+json")?
            .Schema;

        if (!string.IsNullOrEmpty(schema?.Items.Reference.Id))
        {
            itemSchemaReferenceId = schema.Items.Reference.Id;
            return true;
        }

        itemSchemaReferenceId = null;
        return false;
    }

    /// <summary>
    /// Returns true if the Open API path is designated as a nav menu link; false, otherwise.
    /// </summary>
    /// <param name="path">The <see cref="OpenApiPathItem"/> to evaluate.</param>
    /// <returns><see cref="bool"/></returns>
    public static bool IsNavMenuLink(this OpenApiPathItem path)
    {
        // TODO: create a shared library for magic strings like "x-is-nav-menu-link"
        if (path.Extensions.TryGetValue("x-is-nav-menu-link", out IOpenApiExtension? value))
        {
            if (value is OpenApiBoolean isNavMenuLink)
            {
                return isNavMenuLink.Value;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns the path's icon hint if present; null, otherwise.
    /// </summary>
    /// <param name="path">The <see cref="OpenApiPathItem"/> to evaluate.</param>
    /// <returns><see cref="string?"/></returns>
    public static string? TryGetIconHint(this OpenApiPathItem path)
    {
        // TODO: create a shared library for magic strings like "x-icon-hint"
        if (path.Extensions.TryGetValue("x-icon-hint", out IOpenApiExtension? value))
        {
            if (value is OpenApiString iconHint)
            {
                return iconHint.Value;
            }
        }

        return null;
    }
}
