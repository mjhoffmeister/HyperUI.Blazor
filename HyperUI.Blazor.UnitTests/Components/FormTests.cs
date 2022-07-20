using Bunit;
using Hydra.NET;
using HyperUI.Blazor.Internal;
using HyperUI.Blazor.TestData;
using System.Text.Json;

namespace HyperUI.Blazor.UnitTests.Components;

/// <summary>
/// Tests the <see cref="Form"/> component.
/// </summary>
public class FormTests : TestContext
{
    /// <summary>
    /// Tests that a <see cref="Form"/> with an item without an id and a POST operation renders an
    /// "add" button.
    /// </summary>
    public void Render_ItemWithNoIdAndPostOperation_RendersAddButton()
    {
        // Arrange

        string expectedButtonText = "Add";

        var userMap = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
            JsonSerializer.Serialize(
                new User()
                {
                    Operations = new[] { new Operation(Method.Post) }
                }))?
            .ConvertJson()!;

        // Act

        IRenderedComponent<Form> form = RenderComponent<Form>(p => p
            .Add(p => p.Item, userMap)
            .Add(p => p.ItemSchema, User.GetOpenApiSchema()));


        // Assert

        Assert.Equal(expectedButtonText, form.Find(".mud-button-label").TextContent);
    }

}
