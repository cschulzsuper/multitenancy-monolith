using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

public interface IEventPublisher
{
    Task PublishAsync(string @event, long snowflake);
}
