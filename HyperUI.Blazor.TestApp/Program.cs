using Bunit.TestDoubles;
using HyperUI.Blazor.TestApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Moq;
using MudBlazor.Services;
using RichardSzalay.MockHttp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string apiBaseUrl = "https://api.example.com";

MockHttpMessageHandler mockHttpMessageHandler = new();

builder.Services.AddScoped(_ =>
{
    HttpClient httpClient = mockHttpMessageHandler.ToHttpClient();
    httpClient.BaseAddress = new Uri(apiBaseUrl);

    Mock<IHttpClientFactory> mock = new();

    mock
        .Setup(_ => _.CreateClient(It.IsAny<string>()))
        .Returns(httpClient);

    return mock.Object;
});

mockHttpMessageHandler.AddMockUserApiResponses(
    $"{apiBaseUrl}/access-reviews",
    $"{apiBaseUrl}/api-docs/current/openapi.json",
    $"{apiBaseUrl}/environments",
    $"https://cdn.example.com/images",
    $"{apiBaseUrl}/users");

TestAuthorizationContext testAuthorizationContext = new();

testAuthorizationContext.SetAuthorized("Zhang Xia");
testAuthorizationContext.RegisterAuthorizationServices(builder.Services);

builder.Services.AddMudServices();

await builder.Build().RunAsync();