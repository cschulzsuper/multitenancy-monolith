﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Events;

[SuppressMessage("Style", "IDE1006:NamingRuleViolation")]
public static class _Services
{
    public static IServiceCollection AddEvents(this IServiceCollection services, Action<EventsOptions> setup)
    {
        services.Configure(setup);
        services.AddSingleton<EventsOptions>(provider => provider.GetRequiredService<IOptions<EventsOptions>>().Value);

        services.AddScoped<IEventStorage, EventStorage>();
        services.AddScoped<IEventPublisher, EventPublisher>();

        services.AddSingleton<IEventSubscriptions, EventSubscriptions>();

        services.AddSingleton<NamedChannelDictionary<EventValue>>();
        services.AddSingleton<TaskCollection>();

        services.AddHostedService<EventService>();

        return services;
    }
}