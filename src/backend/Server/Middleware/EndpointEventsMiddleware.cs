﻿using ChristianSchulz.MultitenancyMonolith.Events;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Backend.Server.Middleware;

public sealed class EndpointEventsMiddleware
{
    private readonly RequestDelegate _next;

    public EndpointEventsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IEventStorage eventStorage)
    {
        await _next.Invoke(context);

        await eventStorage.FlushAsync();
    }
}