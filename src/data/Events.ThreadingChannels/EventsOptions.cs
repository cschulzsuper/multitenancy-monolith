using System;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public sealed class EventsOptions
{
    public Func<IServiceProvider, string> PublicationChannelNameResolver { get; set; } = _ => string.Empty;

    public Action<string, string, long> PublicationInterceptor { get; set; } = (_,_,_) => { };

    public Action<IServiceProvider, string> SubscriptionInvocationSetup { get; set; } = (_,_) => { };
}