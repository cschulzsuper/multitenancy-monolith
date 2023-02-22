using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.EventBus;

public interface IEventStorage
{
    void Add(string @event, long snowflake);

    Task FlushAsync();

}