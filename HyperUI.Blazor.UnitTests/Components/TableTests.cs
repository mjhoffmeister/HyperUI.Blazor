using Bunit;
using HyperUI.Blazor.Internal;
using HyperUI.Blazor.TestData;
using Microsoft.OpenApi.Models;
using MudBlazor.Services;
using System.Text.Json;

namespace HyperUI.Blazor.UnitTests.Components;

public class TableTests : TestContext
{
    public TableTests()
    {
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Render_UserTable_AddIconRenderedWhenOnAddCallbackProvided()
    {
        // Arrange

        string addIconSvgPath = "path[d=\"M19 13h-6v6h-2v-6H5v-2h6V5h2v6h6v2z\"]";

        int expectedAddIconCount = 1;

        // Act

        var userTable = RenderUserTable(setOnAddCallback: true);

        // Assert

        var addIcons = userTable.FindAll(addIconSvgPath);

        Assert.Equal(expectedAddIconCount, addIcons.Count);
    }

    [Fact]
    public void Render_UserTable_DeleteIconRenderedForDeleteOperations()
    {
        // Arrange

        string deleteIconSvgPath = "path[d=\"M6 19c0 1.1.9 2 2 2h8c1.1 0 2-.9 2-2V7H6v12zM19 " +
            "4h-3.5l-1-1h-5l-1 1H5v2h14V4z\"]";

        int expectedDeleteIconCount = 4;

        // Act

        var userTable = RenderUserTable();

        // Assert

        var deleteIcons = userTable.FindAll(deleteIconSvgPath);

        Assert.Equal(expectedDeleteIconCount, deleteIcons.Count);
    }

    [Theory]
    [InlineData(1, 18)]
    public void Render_UserTable_RendersExpectedDisabledCheckboxCount(
        int expectedIsActiveCheckboxCount, int expectedUserCount)
    {
        // Arrange

        int expectedDisabledCheckboxCount = expectedIsActiveCheckboxCount * expectedUserCount;

        // Act

        var userTable = RenderUserTable();

        // Assert

        var disabledCheckboxes = userTable.FindAll("input[type=\"checkbox\"]:disabled");

        Assert.Equal(expectedDisabledCheckboxCount, disabledCheckboxes.Count);
    }

    [Fact]
    public void Render_UserTable_RendersExpectedRowCount()
    {
        // Arrange

        int expectedRowCount = 7;

        // Act

        var userTable = RenderUserTable();

        // Assert

        var rows = userTable.FindAll("tr");

        Assert.Equal(expectedRowCount, rows.Count);
    }

    [Theory]
    [InlineData("Users")]
    public void Render_UserTable_RendersExpectedTitle(string expectedTitle)
    {
        // Act

        var userTable = RenderUserTable();

        // Assert

        var title = userTable.Find("h6");

        Assert.Equal(expectedTitle, title.TextContent);
    }



    [Fact]
    public void OnClickDeleteButton_UserTable_FiresOnDeleteCallback()
    {
        // Arrange

        bool isOnDeleteFired = false;

        void onDelete(string id)
        {
            // The first row with a delete button rendered should be for a user with the below
            // id. See User.GetSampleCollection() for more info.
            isOnDeleteFired = id == "https://api.example.com/users/5";
        }

        var userTable = RenderUserTable(onDelete: onDelete);

        var firstDeleteButton = userTable.Find("button[aria-label=\"delete\"]");

        // Act

        firstDeleteButton.Click();

        // Assert

        Assert.True(isOnDeleteFired);
    }

    [Fact]
    public void OnClickFirstRow_UserTable_RendersThreeEnabledCheckboxes()
    {
        // Arrange

        var userTable = RenderUserTable();

        var firstRow = userTable.Find("tbody > tr");

        // Act

        firstRow.Click();

        // Assert

        var enabledCheckboxes = userTable.FindAll("input[type=\"checkbox\"]:enabled");

        Assert.Equal(3, enabledCheckboxes.Count);
    }

    [Fact]
    public void OnClickFirstRow_UserTable_RendersOneSelect()
    {
        // Arrange

        var userTable = RenderUserTable();

        var firstRow = userTable.Find("tbody > tr");

        int expectedSelectCount = 1;

        // Act

        firstRow.Click();

        // Assert

        // List items are rendered as divs with p elements containing the text content
        var selects = userTable.FindAll("input.mud-select-input");

        Assert.Equal(expectedSelectCount, selects.Count);
    }

    [Fact]
    public void OnClickFirstRow_UserTable_RendersThreeTextFields()
    {
        // Arrange

        var userTable = RenderUserTable();

        var firstRow = userTable.Find("tbody > tr");

        // Act

        firstRow.Click();

        // Assert

        var textInputs = userTable.FindAll("input[type=\"text\"]");

        Assert.Equal(3, textInputs.Count);
    }

    /// <summary>
    /// Renders a user table for testing.
    /// </summary>
    /// <returns><see cref="IRenderedComponent{Table}"/></returns>
    private IRenderedComponent<Table> RenderUserTable(
        string? addItemTooltip = null,
        Action<string>? onDelete = null,
        bool setOnAddCallback = false)
    {
        // Get the user schema and sample collection
        OpenApiSchema userSchema = User.GetOpenApiSchema();
        IEnumerable<User> users = UserApi.GetUsers();

        // Convert user objects to maps/dictionaries
        string usersJson = JsonSerializer.Serialize(users, options: new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var userMaps = JsonSerializer
            .Deserialize<IEnumerable<Dictionary<string, JsonElement>>>(usersJson)?
            .ConvertJson();

        // Return the rendered user table
        return RenderComponent<Table>(p =>
        {
            p.Add(t => t.Items, userMaps);
            p.Add(t => t.ReferencedSchemas, new()
            {
                ["Roles"] = Roles.GetOpenApiSchema()
            });
            p.Add(t => t.Schema, userSchema);
            p.Add(t => t.Title, "Users");

            if (addItemTooltip != null)
                p.Add(t => t.AddItemTooltip, addItemTooltip);

            if (onDelete != null)
                p.Add(t => t.OnDeleteCallback, onDelete);

            if (setOnAddCallback)
            {
                // Set a boolean in the Action to avoid an ArgumentNullException when adding
                // parameters
                // TODO: is there a way to create a no-op Action that isn't passed as null?
#pragma warning disable CS0219 // Variable is assigned but its value is never used
                bool isOnAddFired = false;
#pragma warning restore CS0219 // Variable is assigned but its value is never used

                void onAdd(Dictionary<string, object?> newUser) => isOnAddFired = true;

                p.Add(t => t.OnAddCallback, onAdd);
            }
        });
    }
}
