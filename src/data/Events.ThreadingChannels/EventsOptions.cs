using System;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public sealed class EventsOptions
{
    public Func<IServiceProvider, string> ChannelNameResolver { get; set; } = _ => string.Empty;

    public Action<EventSubscriptionInvocationContext> InvocationSetup { get; set; } = _ => { };
}