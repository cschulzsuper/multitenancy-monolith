﻿using ChristianSchulz.MultitenancyMonolith.Backend.Server;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]
internal static class MockWebApplication
{
    public const int MockMember = 1;

    public const string Client = "multitenancy-tests";
    public const string AuthenticationIdentity = "default";
    public const string Group1 = "group-1";
    public const string Group2 = "group-2";
    public const string AccountMember = "chief-default";

    private static readonly IDictionary<string, string> _configuration = new Dictionary<string, string>()
    {
        {"AllowedClients:0:Service", "multitenancy-tests"},
        {"AllowedClients:0:Scopes:1", "endpoints"}
    };

    public static WebApplicationFactory<Program> Mock(this WebApplicationFactory<Program> factory)
        => factory.WithWebHostBuilder(app => app
            .ConfigureServices(services =>
            {
                services.Configure<BearerTokenOptions>(BearerTokenDefaults.AuthenticationScheme, options =>
                {
                    options.Events.OnMessageReceived = context => Task.CompletedTask;
                });
            })
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
                config.AddInMemoryCollection(_configuration!);
            }));


    public static IServiceScope CreateMultitenancyScope(this WebApplicationFactory<Program> factory, string group)
        => factory.Services.CreateMultitenancyScope(group);

    private static string ProtectClaims(this WebApplicationFactory<Program> factory, Claim[] claims)
    {
        var options = factory.Services.GetRequiredService<IOptionsMonitor<BearerTokenOptions>>().Get(BearerTokenDefaults.AuthenticationScheme);

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        var authenticationProperties = new AuthenticationProperties
        {
            ExpiresUtc = DateTime.UtcNow + options.BearerTokenExpiration
        };

        var authenticationTicket = new AuthenticationTicket(claimsPrincipal, authenticationProperties, $"{BearerTokenDefaults.AuthenticationScheme}:AccessToken");

        var token = options.BearerTokenProtector.Protect(authenticationTicket);

        return token;
    }

    public static AuthenticationHeaderValue MockValidMemberAuthorizationHeader(this WebApplicationFactory<Program> factory, string accountGroup)
    {
        var claims = new Claim[]
        {
            new ("type", "member"),
            new ("client-name", Client),
            new ("authentication-identity", AuthenticationIdentity),
            new ("account-group", accountGroup),
            new ("account-member", AccountMember)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);
    }

    public static AuthenticationHeaderValue MockValidIdentityAuthorizationHeader(this WebApplicationFactory<Program> factory)
    {
        var claims = new Claim[]
        {
            new ("type", "identity"),
            new ("client-name", Client),
            new ("authentication-identity", AuthenticationIdentity)
        };

        var token = factory.ProtectClaims(claims);

        return new AuthenticationHeaderValue("Bearer", token);

    }
}