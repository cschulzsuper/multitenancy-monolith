using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
public static class _Endpoints
{
    public static IEndpointRouteBuilder MapTickerEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var business = endpoints
            .MapGroup("ticker")
            .WithGroupName("ticker");

        
        business.MapTickerMessageResource();
        business.MapTickerUserDependentBookmarkResource();
        business.MapTickerUserDependentCommands();

        return endpoints;
    }
}