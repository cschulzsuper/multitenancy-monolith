using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityManager
{
    ValueTask<Identity> GetAsync(long snowflake);

    ValueTask<Identity> GetAsync(string uniqueName);

    IQueryable<Identity> GetAll();

    ValueTask<bool> ExistsAsync(string identity, string secret);

    ValueTask InsertAsync(Identity identity);

    ValueTask UpdateAsync(long snowflake, Action<Identity> action);

    ValueTask UpdateAsync(string uniqueName, Action<Identity> action);

    ValueTask DeleteAsync(long snowflake);

    ValueTask DeleteAsync(string uniqueName);
    
}