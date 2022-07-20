using Bunit;
using Microsoft.AspNetCore.Components;

namespace HyperUI.Blazor.UnitTests;

/// <summary>
/// Extensions for <see cref="IRenderedComponent{TComponent}"/>.
/// </summary>
internal static class RenderedComponentExtensions
{
    /// <summary>
    /// Waits for the artificial delay in the OnAddButtonClicked method which is a workaround for
    /// https://github.com/MudBlazor/MudBlazor/issues/3279.
    /// </summary>
    /// <param name="component">The component on which to wait.</param>
    /// <param name="timeout">Timeout.</param>
    public static void WaitForOnAddArtificialDelay<T>(
        this IRenderedComponent<T>? component,
        TimeSpan? timeout = null)
        where T : IComponent
    {
        // This wait was added to account for the artificial delay as a workaround to 
        // https://github.com/MudBlazor/MudBlazor/issues/3279
        component?.WaitForState(
            () => component.FindAll(".mud-input-root-text").Count != 0,
            timeout);
    }
}
