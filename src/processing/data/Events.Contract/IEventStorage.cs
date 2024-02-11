using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Events;

public interface IEventStorage
{
    void Add(string @event, long snowflake);

    Task FlushAsync();
}