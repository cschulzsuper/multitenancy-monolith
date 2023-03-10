using ChristianSchulz.MultitenancyMonolith.Objects.Access;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IAccountRegistrationManager
{
    Task<AccountRegistration> GetAsync(Guid processToken);

    IQueryable<AccountRegistration> GetAll();

    Task InsertAsync(AccountRegistration @object);

    Task UpdateAsync(Guid processToken, Action<AccountRegistration> action);

    Task DeleteAsync(Guid processToken);
}