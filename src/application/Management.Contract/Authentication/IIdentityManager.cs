using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityManager
{
    ValueTask<Identity> GetAsync(long snowflake);

    ValueTask<Identity> GetAsync(string uniqueName);

    IQueryable<Identity> GetAll();

    ValueTask InsertAsync(Identity identity);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
}