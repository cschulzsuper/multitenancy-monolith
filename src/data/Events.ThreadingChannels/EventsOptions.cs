using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public sealed class EventsOptions
{
    public Func<IServiceProvider, string> ChannelNameResolver { get; set; } = _ => string.Empty;

    public Action<string, string, long> PublicationInterceptor { get; set; } = (_,_,_) => { };

    public Func<IServiceProvider, string, Task> BeforeSubscriptionInvocation { get; set; } = (_,_) => Task.CompletedTask;

    public Func<IServiceProvider, string, Task> AfterSubscriptionInvocation { get; set; } = (_, _) => Task.CompletedTask;
}