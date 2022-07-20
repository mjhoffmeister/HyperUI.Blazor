using Microsoft.AspNetCore.Components;
using Microsoft.OpenApi.Models;
using MudBlazor;
using System.Text.Json;

namespace HyperUI.Blazor.Internal;

public partial class Table : ComponentBase
{
    // Bool property converter for nullable objects
    private readonly BoolConverter<object?> _boolPropertyConverter = new()
    {
        GetFunc = value => value == true,
        SetFunc = boolObject => boolObject is bool value && value,
    };

    // A cache of the original values of an item being edited
    Dictionary<string, object?>? _cachedItem = null;

    // Indicates whether items can be added to the table
    private bool _canAdd = false;

    // Table edit state
    private TableEditState _editState = TableEditState.NotEditing;

    // Indicates whether there are items that have actions
    private bool _hasActions = false;

    // Loading state
    private bool _isLoading = false;

    // Item property schemas
    private IDictionary<string, OpenApiSchema> _propertySchemas =
        new Dictionary<string, OpenApiSchema>();

    // String property converter for nullable objects
    private readonly Converter<object?> _stringPropertyConverter = new()
    {
        SetFunc = stringObject => stringObject?.ToString(),
        GetFunc = text => (object?)text,
    };

    // MudTable reference
    private MudTable<Dictionary<string, object?>>? _table;

    /// <summary>
    /// Tooltip for the add item button.
    /// </summary>
    [Parameter]
    public string AddItemTooltip { get; set; } = "Add item";

    /// <summary>
    /// Event callback for adding a new item.
    /// </summary>
    [Parameter]
    public EventCallback<Dictionary<string, object?>> OnAddCallback { get; set; }

    /// <summary>
    /// Event callback for the initiation of an item add.
    /// </summary>
    [Parameter]
    public EventCallback<Dictionary<string, object?>> OnAddStartedCallback { get; set; }

    /// <summary>
    /// Event callback for deleting an item.
    /// </summary>
    [Parameter]
    public EventCallback<string> OnDeleteCallback { get; set; }

    /// <summary>
    /// Table items.
    /// </summary>
    [Parameter]
    public IEnumerable<Dictionary<string, object?>>? Items { get; set; }

    /// <summary>
    /// OpenAPI schemas referenced by the items schema.
    /// </summary>
    [Parameter]
    public Dictionary<string, OpenApiSchema>? ReferencedSchemas { get; set; }

    /// <summary>
    /// OpenAPI schema used for rendering items.
    /// </summary>
    [Parameter]
    public OpenApiSchema? Schema { get; set; }

    /// <summary>
    /// Title for the table.
    /// </summary>
    [Parameter]
    public string Title { get; set; } = "Items";

    /// <summary>
    /// Runs after properties are set on the table's first render.
    /// </summary>
    protected override void OnInitialized()
    {
        if (Schema?.Properties?.Any() == true)
            _propertySchemas = Schema.Properties;

        _canAdd = OnAddCallback.HasDelegate == true;
        _hasActions = Items?.Any(i => i.HasActions()) ?? false;
    }

    /// <summary>
    /// Caches an item.
    /// </summary>
    /// <param name="item">The item to cache.</param>
    private void CacheItem(object item)
    {
        if (item is Dictionary<string, object?> itemToCache)
            _cachedItem = new Dictionary<string, object?>(itemToCache);
    }

    /// <summary>
    /// Handles the OnClick event of the table's add button.
    /// </summary>
    private async Task OnAddButtonClicked()
    {
        if (Items?.Any() == true && _table != null)
        {
            // Create a new item
            Dictionary<string, object?> newItem =
                _propertySchemas.Keys.ToDictionary(key => key, _ => (object?)null);

            // TODO: use default values when present

            // Add the item to the top
            // TODO: handle sorting so that the new item doesn't disappear if paging is active
            //Items = new[] { newItem }.Concat(Items);
            await OnAddStartedCallback.InvokeAsync(newItem);

            // This is needed due to a bug in MudBlazor
            // See https://github.com/MudBlazor/MudBlazor/issues/3279
            // It can be removed and the method can be changed to a synchronous method when the
            // issue is resolved
            await Task.Delay(25);

            // Set the new item as being edited
            _table.SetEditingItem(newItem);

            // Set the table edit state
            _editState = TableEditState.EditingNewItem;
        }
    }

    /// <summary>
    /// Handles the OnClick event of the table's cancel row edit button.
    /// </summary>
    private void OnCancelEditButtonClicked()
    {
        // Adding a new item is cancelled
        if (_editState == TableEditState.EditingNewItem)
        {
            // Remove the new item
            Items = Items?.Skip(1);

            // TODO: re-apply sorting present before add?

            // Signal state change
            StateHasChanged();
        }
        // TODO: Editing an existing item is cancelled
        else if (_editState == TableEditState.EditingExistingItem)
        {

        }

        // Set edit state to not editing
        _editState = TableEditState.NotEditing;
    }

    /// <summary>
    /// Handles the OnClick event of a row's commit edit button
    /// </summary>
    /// <returns></returns>
    private async Task OnCommitEditButtonClicked()
    {
        if (_table != null && Items != null)
        {
            // Set loading indicator
            _isLoading = true;

            // New item edits are committed
            if (_editState == TableEditState.EditingNewItem)
            {
                await OnAddCallback.InvokeAsync(Items.First());
            }
            // TODO: Existing item edits are committed
            else if (_editState == TableEditState.EditingExistingItem)
            {

            }
        }

        // Set edit state to not editing
        _editState = TableEditState.NotEditing;

        // Unset loading indicator
        _isLoading = false;
    }

    /// <summary>
    /// Handles the OnClick event of a row's delete button.
    /// </summary>
    /// <param name="idObject">Id of the row item.</param>
    private async Task OnDeleteButtonClicked(object? idObject)
    {
        if (idObject is string id)
        {
            _isLoading = true;

            await OnDeleteCallback.InvokeAsync(id);

            _isLoading = false;
        }
    }
    
    /// <summary>
    /// Tries to resolve a schema reference.
    /// </summary>
    /// <param name="propertySchema">Property schema.</param>
    /// <returns><see cref="OpenApiSchema"/>.</returns>
    private OpenApiSchema? ResolveSchemaReference(OpenApiSchema propertySchema)
    {
        OpenApiSchema? resolvedSchemaReference = null;

        OpenApiReference? reference = propertySchema.Reference;

        if (reference?.Id != null)
            ReferencedSchemas?.TryGetValue(reference.Id, out resolvedSchemaReference);

        return resolvedSchemaReference;
    }

    /// <summary>
    /// Reverts the changes made to an item.
    /// </summary>
    /// <param name="itemToRevert">The item to revert.</param>
    private void RevertItemChanges(object item)
    {
        if (_cachedItem != null && item is Dictionary<string, object?> itemToRevert)
        {
            foreach (string key in _cachedItem.Keys)
            {
                itemToRevert[key] = _cachedItem[key];
            }
        }
    }
}