using ChristianSchulz.MultitenancyMonolith.Objects.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

public interface IIdentityManager
{
    Task<Identity> GetAsync(long snowflake);

    Task<Identity> GetAsync(string uniqueName);

    IQueryable<Identity> GetAll();

    Task<bool> ExistsAsync(string identity, string secret);

    Task InsertAsync(Identity identity);

    Task UpdateAsync(long snowflake, Action<Identity> action);

    Task UpdateAsync(string uniqueName, Action<Identity> action);

    Task DeleteAsync(long snowflake);

    Task DeleteAsync(string uniqueName);
    
}