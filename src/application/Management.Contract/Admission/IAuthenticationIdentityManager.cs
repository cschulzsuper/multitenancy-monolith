using ChristianSchulz.MultitenancyMonolith.Objects.Admission;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityManager
{
    Task<bool> ExistsAsync(string authenticationIdentity);

    Task<bool> ExistsAsync(string authenticationIdentity, string secret);

    Task<AuthenticationIdentity> GetAsync(long authenticationIdentity);

    Task<AuthenticationIdentity> GetAsync(string authenticationIdentity);

    IQueryable<AuthenticationIdentity> GetQueryable();

    Task InsertAsync(AuthenticationIdentity @object);

    Task UpdateAsync(long authenticationIdentity, Action<AuthenticationIdentity> action);

    Task UpdateAsync(string authenticationIdentity, Action<AuthenticationIdentity> action);

    Task DeleteAsync(long authenticationIdentity);

    Task DeleteAsync(string authenticationIdentity);
}