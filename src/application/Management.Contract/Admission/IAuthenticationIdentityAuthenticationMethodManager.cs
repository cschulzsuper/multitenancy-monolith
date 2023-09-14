using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Admission;

public interface IAuthenticationIdentityAuthenticationMethodManager
{
    Task<bool> ExistsAsync(string authenticationIdentity, string clientName, string authenticationMethod);
}