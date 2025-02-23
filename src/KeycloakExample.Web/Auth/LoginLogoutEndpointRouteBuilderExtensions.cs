using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace KeycloakExample.Web.Auth;

// KeycloakExample
// This class enables us to add login and logout endpoints to the application.
// The endpoints will be available at /authentication/login and /authentication/logout.
internal static class LoginLogoutEndpointRouteBuilderExtensions
{
    internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("authentication");

        // This redirects the user to the Keycloak login page and, after successful login, redirects them to the home page.
        group.MapGet("/login", () =>
        {
            var test = TypedResults.Challenge(new AuthenticationProperties { RedirectUri = "/" });
            return test;
        })
        .AllowAnonymous();


        // This logs the user out of the application and redirects them to the home page.
        group.MapGet("/logout", () => TypedResults.SignOut(new AuthenticationProperties { RedirectUri = "/" },
        [CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme]));

        return group;
    }
}
