using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapAccessEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var authorization = endpoints
            .MapGroup("access")
            .WithGroupName("access");

        authorization.MapAccountGroup();
        authorization.MapAccountMemberResource();
        authorization.MapAccountMemberCommands();
        authorization.MapAccountRegistrationCommands();

        return endpoints;
    }
}