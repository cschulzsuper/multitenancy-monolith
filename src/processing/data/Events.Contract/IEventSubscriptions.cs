﻿using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public interface IEventSubscriptions
{
    void Map<THandler>(string @event, Func<THandler, long, Task> subscription)
        where THandler : class;

    Task InvokeAsync(string @event, IServiceProvider services, long snowflake);
}