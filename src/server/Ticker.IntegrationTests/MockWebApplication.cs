using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker;
using ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

internal static class MockWebApplication
{
    public const string Client = "integration-tests";

    public const string Group = "group";
    public const string Member = "member";

    public const string TicketUserMail = "default@localhost";
    public const string TicketUserSecret = "default";
    public readonly static Guid TicketUserSecretToken = Guid.NewGuid();

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:UniqueName", Client},
        {"AllowedClients:0:Scopes:0", "swagger-json"},
        {"AllowedClients:0:Scopes:1", "endpoints"},
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .ConfigureServices(services => 
            {
                services.AddSingleton<BadgeValidator, MockBadgeValidator>();
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));


    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory)
        => factory.Services.CreateMultitenancyScope(Group);
}