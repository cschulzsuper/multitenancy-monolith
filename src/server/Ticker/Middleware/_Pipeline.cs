using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Data.StaticDictionary;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.Middleware;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Pipeline
{
    public static IApplicationBuilder UseAuthenticationScope(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {

            if (context.GetEndpoint()?.Metadata.GetMetadata<AuthenticationAttribute>() == null ||
                !context.Request.HasJsonContentType())
            {
                await next.Invoke(context);
                return;
            }

            var buffer = new MemoryStream();
            await context.Request.Body.CopyToAsync(buffer);
            buffer.Position = 0;

            var @object = await JsonSerializer.DeserializeAsync<JsonObject>(buffer);

            buffer.Position = 0;
            context.Request.Body = buffer;

            if (@object?.ContainsKey("group") != true)
            {
                await next.Invoke(context);
                return;
            }

            var group = @object["group"]!.GetValue<string>();

            var existingServices = context.RequestServices;
            var existingFeature = context.Features.Get<IServiceProvidersFeature>();
            if (existingFeature == null)
            {
                await next.Invoke(context);
                return;
            }

            using (var scope = existingServices.CreateMultitenancyScope(group))
            {
                try
                {
                    existingFeature.RequestServices = scope.ServiceProvider;
                    await next.Invoke(context);
                }
                finally
                {
                    existingFeature.RequestServices = existingServices;
                }
            }
        });

        return app;
    }
}
