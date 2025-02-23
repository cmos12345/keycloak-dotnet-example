using KeycloakExample.Web;
using KeycloakExample.Web.Auth;
using KeycloakExample.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

// KeycloakExample
// This allows us to use the HttpContextAccessor from the DI Container, aswell as addiong our AuthorizationHandler.
builder.Services.AddHttpContextAccessor()
                .AddTransient<AuthorizationHandler>()
                .AddScoped<LogoutService>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new("https+http://apiservice");
    })
    .AddHttpMessageHandler<AuthorizationHandler>(); // KeycloakExample: Adds our AuthorizationHandler to the HttpClient pipeline.

// KeycloakExample
// Add Keycloak authentication with OpenID Connect.
var oidcScheme = OpenIdConnectDefaults.AuthenticationScheme;
builder.Services.AddAuthentication(oidcScheme)
                .AddKeycloakOpenIdConnect("keycloak", realm: "WeatherShop", oidcScheme, options =>
                {
                    options.ClientId = "WeatherWeb";
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.Scope.Add("weather:all");
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                    options.SaveTokens = true;
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.AddCascadingAuthenticationState(); // KeycloakExample: Ensures that the authentication state is available to all components.

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();
app.MapLoginAndLogout(); // KeycloakExample: Adds the login and logout endpoints for Keycloak identity provider.

app.Run();
