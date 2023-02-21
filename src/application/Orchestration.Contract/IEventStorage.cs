using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application
{
    public interface IEventStorage
    {
        void Add(string @event, long snowflake);

        Task FlushAsync();

    }
}