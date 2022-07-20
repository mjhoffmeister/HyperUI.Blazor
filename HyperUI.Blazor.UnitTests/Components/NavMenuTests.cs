using Bunit;
using HyperUI.Blazor.Internal;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Blazor.UnitTests.Components;

/// <summary>
/// <see cref="NavMenu"/> tests.
/// </summary>
public class NavMenuTests : TestContext
{
    /// <summary>
    /// Tests that paths configured as nav menu links are rendered.
    /// </summary>
    [Fact]
    public void Render_OpenApiPaths_RendersNavMenuLinks()
    {
        // Arrange

        int expectedNavMenuLinks = 2;

        OpenApiPaths paths = new()
        {
            ["/movies"] = new OpenApiPathItem
            {
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "x-icon-hint", new OpenApiString("List") },
                    { "x-is-nav-menu-link", new OpenApiBoolean(true) },
                },
                Summary = "Shows"
            },
            ["/shows"] = new OpenApiPathItem
            {
                Extensions = new Dictionary<string, IOpenApiExtension>
                {
                    { "x-icon-hint", new OpenApiString("List") },
                    { "x-is-nav-menu-link", new OpenApiBoolean(true) },
                },
                Summary = "Shows"
            }
        };

        // Act

        var navMenu = RenderComponent<NavMenu>(p => p
            .Add(p => p.NavMenuLinkPaths, paths));

        // Assert

        var navMenuLinks = navMenu.FindAll(".mud-nav-item");
        Assert.Equal(expectedNavMenuLinks, navMenuLinks.Count);
    }
}
