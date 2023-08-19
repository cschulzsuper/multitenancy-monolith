using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Endpoints;

internal static class SignInEndpoint
{
    public static IEndpointRouteBuilder MapSignIn(this IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapGet("sign-in", SignIn)
            .AllowAnonymous();

        return endpoints;
    }

    private static Delegate SignIn =>
        async (HttpContext context, [FromQuery(Name = "return")] string @return) =>
        {
            var admissionServer = context.RequestServices
                .GetRequiredService<IAdmissionServerProvider>()
                .Get();

            using var client = context.RequestServices
                .GetRequiredService<TransportWebServiceClientFactory>()
                .Create<IContextAuthenticationIdentityCommandClient>(admissionServer.Service);

            var command = new ContextAuthenticationIdentityAuthCommand
            {
                ClientName = "swagger-ui",
                AuthenticationIdentity = admissionServer.MaintenanceAuthenticationIdentity,
                Secret = admissionServer.MaintenanceAuthenticationIdentitySecret,
            };

            try
            {
                var tokenObject = await client.AuthAsync(command);
                var token = $"{tokenObject}";

                context.Response.Cookies.Append("access_token", token);
                return Results.LocalRedirect(@return);
            }
            catch
            {
                return Results.Unauthorized();
            }
        };
}