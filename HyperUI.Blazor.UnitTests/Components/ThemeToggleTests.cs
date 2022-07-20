using Bunit;
using HyperUI.Blazor.Internal;
using MudBlazor.Services;

namespace HyperUI.Blazor.UnitTests.Components;

/// <summary>
/// <see cref="ThemeToggle"/> tests.
/// </summary>
public class ThemeToggleTests : TestContext
{
    /// <summary>
    /// Tests the toggling of the IsDarkMode property.
    /// </summary>
    /// <param name="isDarkMode">Initial IsDarkMode value.</param>
    /// <param name="expectedIsDarkModeAfterToggle">Expected IsDarkMode value after toggle.</param>
    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void OnClick_FromInitialUITheme_TogglesIsDarkMode(
        bool isDarkMode, bool expectedIsDarkModeAfterToggle)
    {
        // Arrange

        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;

        bool? actualIsDarkModeAfterToggle = null;

        var themeToggle = RenderComponent<ThemeToggle>(p => p
            .Add(p => p.IsDarkMode, isDarkMode)
            .Add(p => p.IsDarkModeChanged, toggledIsDarkMode =>
            {
                actualIsDarkModeAfterToggle = toggledIsDarkMode;
            }));

        // Act

        themeToggle.Find("button").Click();

        // Assert

        Assert.Equal(expectedIsDarkModeAfterToggle, actualIsDarkModeAfterToggle);
    }
}
