namespace HyperUI.Blazor.Internal;

/// <summary>
/// Edit item event.
/// </summary>
public class EditItemEvent
{
	/// <summary>
	/// Initializes a new instance of the <see cref="EditItemEvent"/> class.
	/// </summary>
	/// <param name="item">Item.</param>
	public EditItemEvent(Dictionary<string, object?> item)
	{
		Item = item;
	}

	/// <summary>
	/// Failed indicator
	/// </summary>
	public bool IsFailed { get; private set; }

	/// <summary>
	/// The edited item.
	/// </summary>
	public Dictionary<string, object?> Item { get; }

	/// <summary>
	/// Sets the result of the event as a failure.
	/// </summary>
	public void Failure() => IsFailed = true;

	/// <summary>
	/// Sets the result of the event as a success.
	/// </summary>
	public void Success() => IsFailed = false;
}
