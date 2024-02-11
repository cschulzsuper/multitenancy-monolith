using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityAuthenticationMethodManager
{
    Task<bool> ExistsAsync(long authenticationIdentity, string clientName, string authenticationMethod);
}