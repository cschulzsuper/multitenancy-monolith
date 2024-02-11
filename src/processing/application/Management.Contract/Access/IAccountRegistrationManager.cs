using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountRegistrationManager
{
    Task<AccountRegistration> GetAsync(long accountRegistration);

    IQueryable<AccountRegistration> GetQueryable();

    Task InsertAsync(AccountRegistration @object);

    Task UpdateAsync(long accountRegistration, Action<AccountRegistration> action);

    Task UpdateAsync(string accountGroup, Action<AccountRegistration> action);

    Task DeleteAsync(long accountRegistration);
}