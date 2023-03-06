using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using Microsoft.AspNetCore.Http.Features;
using System.IO;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace ChristianSchulz.MultitenancyMonolith.Server.Middleware;

/// <summary>
/// A middleware for handling CORS.
/// </summary>
public class AuthenticationScopeMiddleware
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