using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public interface IEventPublisher
{
    Task PublishAsync(string @event, long snowflake);
}