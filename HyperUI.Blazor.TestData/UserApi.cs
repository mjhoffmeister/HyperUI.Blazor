using Hydra.NET;
using HyperUI.Core;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;

namespace HyperUI.Blazor.TestData;

/// <summary>
/// Fake User API.
/// </summary>
public static class UserApi
{
    /// <summary>
    /// Gets an <see cref="OpenApiDocument"/> documenting the example User API.
    /// </summary>
    /// <returns><see cref="OpenApiDocument"/>.</returns>
    public static OpenApiDocument GetOpenApiDocument() => new()
    {
        Info = new()
        {
            Version = "1.0.0",
            Title = "Example",
        },
        Paths = new()
        {
            ["/users"] = new()
            {
                Summary = "Users",
                Extensions = new Dictionary<string, IOpenApiExtension>()
                {
                    ["x-icon-hint"] = new OpenApiString("People"),
                    ["x-is-nav-menu-link"] = new OpenApiBoolean(true),
                },
                Operations = new Dictionary<OperationType, OpenApiOperation>
                {
                    [OperationType.Get] = new()
                    {
                        Responses = new()
                        {
                            ["200"] = new()
                            {
                                Content = new Dictionary<string, OpenApiMediaType>()
                                {
                                    ["application/ld+json"] = new()
                                    {
                                        Schema = new()
                                        {
                                            Type = "array",
                                            Items = new()
                                            {
                                                Reference = new()
                                                {
                                                    Id = "User",
                                                    Type = ReferenceType.Schema
                                                }
                                            }
                                        }
                                    }
                                },
                                Description = "OK",
                            }
                        }
                    }
                },
            }
        },
        Components = new()
        {
            Schemas = new Dictionary<string, OpenApiSchema>
            {
                ["LinkCollection"] = RichLinkCollection.GetOpenApiSchema(),
                ["RichLink"] = RichLink.GetOpenApiSchema(),
                ["Roles"] = Roles.GetOpenApiSchema(),
                ["User"] = User.GetOpenApiSchema()
            }
        }
    };

    /// <summary>
    /// Gets test users.
    /// </summary>
    /// <returns>Test users./returns>
    public static IEnumerable<User> GetUsers()
    {
        yield return new User
        {
            EmailAddress = "jane.doe@example.com",
            FullName = "Jane Doe",
            Id = "https://api.example.com/users/1",
            IsActive = true,
            IsSecurityAdmin = true,
            IsUserAdmin = true,
            Operations = new[]
            {
                new Operation(Method.Put)
            },
            Type = "Admin"
        };

        yield return new User
        {
            EmailAddress = "zhang.xia@example.com",
            FullName = "Zhang Xia",
            Id = $"https://api.example.com/users/2",
            IsActive = true,
            IsSecurityAdmin = true,
            IsUserAdmin = true,
            Type = "Admin"
        };

        yield return new User
        {
            EmailAddress = "panashe.mutsipa@example.com",
            FullName = "Panashe Mutsipa",
            Id = "https://api.example.com/users/3",
            IsActive = true,
            IsSecurityAdmin = true,
            IsUserAdmin = false,
            Operations = new[]
            {
                new Operation(Method.Delete),
                new Operation(Method.Put)
            },
            Type = "Basic"
        };

        yield return new User
        {
            EmailAddress = "vera.ilyinichna@example.com",
            FullName = "Vera Ilyinichna",
            Id = "https://api.example.com/users/4",
            IsActive = true,
            IsSecurityAdmin = false,
            IsUserAdmin = true,
            Operations = new[]
            {
                new Operation(Method.Delete),
                new Operation(Method.Put)
            },
            Type = "Basic"
        };

        yield return new User
        {
            EmailAddress = "harpa.stefansdottir@example.com",
            FullName = "Harpa Stefansdottir",
            Id = "https://api.example.com/users/5",
            IsActive = false,
            IsSecurityAdmin = false,
            IsUserAdmin = false,
            Operations = new[]
            {
                new Operation(Method.Delete),
                new Operation(Method.Put)
            },
            Type = "Basic"
        };

        yield return new User 
        { 
            EmailAddress = "sato.gota@example.com",
            FullName = "Sato Gota",
            Id = "https://api.example.com/users/6",
            IsActive = true,
            IsSecurityAdmin = false,
            IsUserAdmin = false,
            Operations = new[] 
            {
                new Operation(Method.Delete),
                new Operation(Method.Put)
            },
            Type = "Basic"
        };
    }

    /// <summary>
    /// Gets user requests.
    /// </summary>
    /// <returns><see cref="RichLinkCollection"/>.</returns>
    public static RichLinkCollection GetRequests()
    {
        IEnumerable<RichLink> links = new[]
        {
            new RichLink(
                "https://api.example.com/requests/add-user",
                "Add a user",
                "https://cdn.example.com/images/user.svg"),
            new RichLink(
                "https://api.example.com/requests/request-access",
                "Request access",
                "https://cdn.example.com/images/access.svg"),
            new RichLink(
                "https://api.example.com/requests/change-password",
                "Change your password",
                "https://cdn.example.com/images/password.svg")
        };

        return new(links, RichLinkCollectionRenderMode.Grid);
    }
}