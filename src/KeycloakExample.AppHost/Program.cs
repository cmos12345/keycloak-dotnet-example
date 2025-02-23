using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

IConfiguration config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

// KeycloakExample
// Adding the keycloak container to our aspire app.
// This also imports the realm configuration from the Realms folder so keycloak is correctly configured.
// The AddKeycloak extension is provided by the Aspire.Hosting.Keycloak NuGet package.
var keycloak = builder.AddKeycloak("keycloak", 8080)
    .WithDataVolume()
    .WithRealmImport("../Realms");

var apiService = builder.AddProject<Projects.KeycloakExample_ApiService>("apiservice")
    .WithReference(keycloak);  // KeycloakExample: ServiceDiscovvery for the keycloak container in our API Service.

builder.AddProject<Projects.KeycloakExample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(keycloak) // KeycloakExample: ServiceDiscovvery for the keycloak container in our web frontend.
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
