using Hydra.NET;
using HyperUI.Blazor.TestData;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace HyperUI.Blazor.TestApp;

/// <summary>
/// <see cref="IServiceCollection"/> extension methods.
/// </summary>
public static class MockHttpMessageHandlerExtensions
{
    /// <summary>
    /// Adds mock user API responses to the <see cref="MockHttpMessageHandler"/>.
    /// </summary>
    /// <param name="messageHandler"><see cref="MockHttpMessageHandler"/></param>
    /// <param name="accessReviewsUrl">Access reviews URL.</param>
    /// <param name="documentationUrl">Documentation URL.</param>
    /// <param name="environmentsUrl">Environments URL.</param>
    /// <param name="imagesUrl">Images URL.</param>
    /// <param name="usersUrl">Users URL.</param>
    public static void AddMockUserApiResponses(
        this MockHttpMessageHandler messageHandler,
        string accessReviewsUrl,
        string documentationUrl,
        string environmentsUrl,
        string imagesUrl,
        string usersUrl)
    {
        ConfigureAddResponse<AccessReview>(messageHandler, usersUrl);

        ConfigureAddResponse<User>(messageHandler, usersUrl);

        ConfigureDeleteResponse(messageHandler, accessReviewsUrl, "Access review");

        ConfigureDeleteResponse(messageHandler, usersUrl, "User");

        ConfigureGetImageResponse(messageHandler, imagesUrl);

        ConfigureGetOpenApiDocumentationResponse(messageHandler, documentationUrl);

        ConfigureGetResponse(
            messageHandler, environmentsUrl, new[] { "Development", "Test", "UAT", "Production" });

        ConfigureGetResponse(messageHandler, usersUrl, UserApi.GetUsers());

        ConfigurePutResponse(messageHandler, usersUrl, "User");
    }

    /// <summary>
    /// Configures an add (POST) response.
    /// </summary>
    /// <typeparam name="T">Type of the item to add.</typeparam>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="url">URL.</param>
    private static void ConfigureAddResponse<T>(MockHttpMessageHandler messageHandler, string url)
        where T : ApiObject
    {
        messageHandler
            .When(HttpMethod.Post, url)
            .Respond(async request =>
            {
                HttpContent? httpContent = request.Content;

                T? itemToAdd = await request.Content?.ReadFromJsonAsync<T>()!;

                if (itemToAdd == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                var apiObject = itemToAdd as ApiObject;

                apiObject.Id = new Uri($"{url}/{Guid.NewGuid()}").ToString();

                apiObject.Operations = new[]
                {
                    new Operation(Method.Delete),
                    new Operation(Method.Put),
                };

                HttpResponseMessage response = new(HttpStatusCode.Created);

                response.Content = new StringContent(JsonSerializer.Serialize(itemToAdd));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }

    /// <summary>
    /// Configures a DELETE response.
    /// </summary>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="url">URL.</param>
    /// <param name="schemaTitle">Schema title. Used for the delete message.</param>
    private static void ConfigureDeleteResponse(
        MockHttpMessageHandler messageHandler, string url, string schemaTitle)
    {
        messageHandler
            .When(HttpMethod.Delete, $"{url}/*")
            .Respond(request =>
            {
                Status status = new(
                    new Context(new Uri("http://www.w3.org/ns/hydra/context.jsonld")),
                    (int)HttpStatusCode.OK,
                    "OK",
                    $"{schemaTitle} removed.");

                HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(JsonSerializer.Serialize(status));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }

    /// <summary>
    /// Configures the response for getting images.
    /// </summary>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="imagesUrl">Images URL.</param>
    private static void ConfigureGetImageResponse(
        MockHttpMessageHandler messageHandler, string imagesUrl)
    {
        messageHandler
            .When(HttpMethod.Get, $"{imagesUrl}/*")
            .Respond(request =>
            {
                // Get the image name
                string? imageName = request.RequestUri?.Segments.Last();

                return imageName switch
                {
                    "access.svg" or "password.svg" or "user.svg" => 
                        GetImageResponse($"~/images/{imageName}"),
                    _ => new HttpResponseMessage(HttpStatusCode.NotFound)
                };
            });

        // Gets an image response
        static HttpResponseMessage GetImageResponse(string fileName)
        { 
            HttpResponseMessage response = new(HttpStatusCode.OK);
            response.Content = new StreamContent(new FileStream(fileName, FileMode.Open));
            response.Content.Headers.ContentType = new("image/svg+xml");

            return response;
        }
    }

    /// <summary>
    /// Configures a get OpenAPI documentation response.
    /// </summary>
    /// <param name="messageHandler"><see cref="MockHttpMessageHandler"/>.</param>
    /// <param name="documentationUrl">Documentation URL.</param>
    private static void ConfigureGetOpenApiDocumentationResponse(
        MockHttpMessageHandler messageHandler, string documentationUrl)
    {
        messageHandler
            .When(HttpMethod.Get, documentationUrl)
            .Respond(_ =>
            {
                HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(UserApi
                    .GetOpenApiDocument()
                    .Serialize(OpenApiSpecVersion.OpenApi3_0, OpenApiFormat.Json));

                response.Content.Headers.ContentType = new("application/json");

                return response;
            });
    }

    /// <summary>
    /// Configures a GET response.
    /// </summary>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="url">URL.</param>
    /// <param name="content">Content.</param>
    private static void ConfigureGetResponse(
        MockHttpMessageHandler messageHandler, string url, object content)
    {
        messageHandler
            .When(HttpMethod.Get, url)
            .Respond(_ =>
            {
                HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(JsonSerializer.Serialize(content));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }

    /// <summary>
    /// Configures a PUT response.
    /// </summary>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="url">URL.</param>
    /// <param name="content">Content.</param>
    private static void ConfigurePutResponse(
        MockHttpMessageHandler messageHandler, string url, string schemaTitle)
    {
        messageHandler
            .When(HttpMethod.Put, $"{url}/*")
            .Respond(_ =>
            {
				Status status = new(
					new Context(new Uri("http://www.w3.org/ns/hydra/context.jsonld")),
					(int)HttpStatusCode.OK,
					"OK",
					$"{schemaTitle} updated.");

				HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(JsonSerializer.Serialize(status));

				response.Content.Headers.ContentType = new("application/ld+json");

				return response;
			});
    }
}