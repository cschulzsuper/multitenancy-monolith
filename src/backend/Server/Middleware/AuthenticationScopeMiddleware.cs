﻿using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Shared.Multitenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.Middleware;

public sealed class AuthenticationScopeMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationScopeMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context.GetEndpoint()?.Metadata.GetMetadata<AuthenticationAttribute>() == null ||
                        !context.Request.HasJsonContentType())
        {
            await _next.Invoke(context);
            return;
        }

        var buffer = new MemoryStream();
        await context.Request.Body.CopyToAsync(buffer);
        buffer.Position = 0;

        var @object = await JsonSerializer.DeserializeAsync<JsonObject>(buffer);

        buffer.Position = 0;
        context.Request.Body = buffer;

        if (@object?.ContainsKey("accountGroup") != true)
        {
            await _next.Invoke(context);
            return;
        }

        var group = @object["accountGroup"]!.GetValue<string>();

        var existingServices = context.RequestServices;
        var existingFeature = context.Features.Get<IServiceProvidersFeature>();
        if (existingFeature == null)
        {
            await _next.Invoke(context);
            return;
        }

        await using var scope = existingServices.CreateAsyncMultitenancyScope(group);

        try
        {
            existingFeature.RequestServices = scope.ServiceProvider;
            await _next.Invoke(context);
        }
        finally
        {
            existingFeature.RequestServices = existingServices;
        }
    }
}