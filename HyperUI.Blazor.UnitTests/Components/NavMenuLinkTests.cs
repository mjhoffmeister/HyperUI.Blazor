using Bunit;
using HyperUI.Blazor.Internal;

namespace HyperUI.Blazor.UnitTests.Components;

/// <summary>
/// <see cref="NavMenuLink"/> tests.
/// </summary>
public class NavMenuLinkTests : TestContext
{
    /// <summary>
    /// Tests icon rendering.
    /// </summary>
    /// <param name="iconHint">Icon hint.</param>
    [Theory]
    [InlineData("Add")]
    [InlineData("People")]
    [InlineData("Event")]
    [InlineData("QueueMusic")]
    [InlineData("Foobar")]
    public void Render_MutlipleIconsHints_RendersSvg(string iconHint)
    {
        // Act

        var navMenuLink = RenderComponent<NavMenuLink>(p => p
            .Add(p => p.IconHint, iconHint)
            .Add(p => p.Path, "/test")
            .Add(p => p.Title, "Test"));

        // Assert

        // A title element with the name of the icon should be the first child of the SVG icon
        var svgIcon = navMenuLink.Find("svg");

        Assert.NotNull(svgIcon);
    }
}