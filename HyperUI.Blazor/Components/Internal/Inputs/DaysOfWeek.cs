using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// Utility class for the days of the week.
/// </summary>
internal static class DaysOfWeek
{
    // Days of the week
    private static readonly string[] _daysOfWeek = new[]
    {
        "Sunday",
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday"
    };

    /// <summary>
    /// Renders days of the week as select items.
    /// </summary>
    /// <returns><see cref="RenderFragment"/>.</returns>
    public static RenderFragment RenderSelectItems() => builder =>
    {
        foreach (string dayOfWeek in _daysOfWeek)
        {
            builder.OpenComponent<MudSelectItem<object>>(0);
            builder.AddAttribute(1, "Value", dayOfWeek);
            builder.CloseComponent();
        }
    };
}
