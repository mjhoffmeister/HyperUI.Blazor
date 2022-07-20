using Hydra.NET;
using System.Net.Http.Json;
using System.Text.Json;

namespace HyperUI.Blazor.Internal;

/// <summary>
/// <see cref="HttpRequestMessage"/> extensions.
/// </summary>
internal static class HttpResponseMessageExtensions
{
	/// <summary>
	/// Tries to get the Hydra status description from the <see cref="HttpResponseMessage"/>.
	/// </summary>
	/// <param name="message">Message.</param>
	/// <returns>Status description.</returns>
	public static async Task<string?> TryGetStatusDescriptionAsync(this HttpResponseMessage message)
	{
		try
		{
			Status? status = await message.Content.ReadFromJsonAsync<Status>();
			return status?.Description;
		}
		catch (JsonException)
		{
			Console.WriteLine(GetFailureMessage("the JSON was invalid"));
		}
		catch (NotSupportedException)
		{
			Console.WriteLine(GetFailureMessage("the content type is not supported"));
		}

		return null;

		static string GetFailureMessage(string reason)
		{
			return $"Could not get status description from HTTP response message because {reason}.";
		}
	}
}
