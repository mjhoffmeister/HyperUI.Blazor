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
    /// <param name="documentationUrl">Documentation URL.</param>
    /// <param name="environmentsUrl">Environments URL.</param>
    /// <param name="imagesUrl">Images URL.</param>
    /// <param name="usersUrl">Users URL.</param>
    public static void AddMockUserApiResponses(
        this MockHttpMessageHandler messageHandler,
        string documentationUrl,
        string environmentsUrl,
        string imagesUrl,
        string usersUrl)
    {
        ConfigureAddUserResponse(messageHandler, usersUrl);
        ConfigureDeleteUserResponse(messageHandler, usersUrl);
        ConfigureGetEnvironmentsResponse(messageHandler, environmentsUrl);
        ConfigureGetImageResponse(messageHandler, imagesUrl);
        ConfigureGetOpenApiDocumentationResponse(messageHandler, documentationUrl);
        ConfigureGetUsersResponse(messageHandler, usersUrl);
    }

    /// <summary>
    /// Configures the add user response.
    /// </summary>
    /// <param name="messageHandler"><see cref="MockHttpMessageHandler"/>.</param>
    /// <param name="usersUrl">Users URL.</param>
    private static void ConfigureAddUserResponse(
        MockHttpMessageHandler messageHandler, string usersUrl)
    {
        messageHandler
            .When(HttpMethod.Post, usersUrl)
            .Respond(async request =>
            {
                HttpContent? httpContent = request.Content;

                User? user = await request.Content?.ReadFromJsonAsync<User>()!;

                if (user == null)
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);

                user.Id = new Uri($"{usersUrl}/{Guid.NewGuid()}").ToString();

                user.Operations = new[]
                {
                    new Operation(Method.Delete),
                    new Operation(Method.Put),
                };

                HttpResponseMessage response = new(HttpStatusCode.Created);

                response.Content = new StringContent(JsonSerializer.Serialize(user));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }

    /// <summary>
    /// Configures the delete user response.
    /// </summary>
    /// <param name="messageHandler"></param>
    /// <param name="usersUrl"></param>
    private static void ConfigureDeleteUserResponse(
        MockHttpMessageHandler messageHandler, string usersUrl)
    {
        messageHandler
            .When(HttpMethod.Delete, $"{usersUrl}/*")
            .Respond(request =>
            {
                Status status = new(
                    new Context(new Uri("http://www.w3.org/ns/hydra/context.jsonld")),
                    (int)HttpStatusCode.OK,
                    "OK",
                    "User removed.");

                HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(JsonSerializer.Serialize(status));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }

    /// <summary>
    /// Configures the response for getting environments.
    /// </summary>
    /// <param name="messageHandler">Message handler.</param>
    /// <param name="environmentsUrl">Environments URL.</param>
    private static void ConfigureGetEnvironmentsResponse(
        MockHttpMessageHandler messageHandler, string environmentsUrl)
    {
        messageHandler
            .When(HttpMethod.Get, environmentsUrl)
            .Respond(_ =>
            {
                HttpResponseMessage response = new(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(
                        new[] { "Development", "Test", "UAT", "Production" })
                };

                response.Content.Headers.ContentType = new("application/json");

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
    /// Configures a get users response.
    /// </summary>
    /// <param name="messageHandler"><see cref="MockHttpMessageHandler"/>.</param>
    /// <param name="usersUrl">Users URL.</param>
    private static void ConfigureGetUsersResponse(
        MockHttpMessageHandler messageHandler, string usersUrl)
    {
        messageHandler
            .When(HttpMethod.Get, usersUrl)
            .Respond(_ =>
            {
                HttpResponseMessage response = new(HttpStatusCode.OK);

                response.Content = new StringContent(JsonSerializer.Serialize(UserApi.GetUsers()));

                response.Content.Headers.ContentType = new("application/ld+json");

                return response;
            });
    }
}