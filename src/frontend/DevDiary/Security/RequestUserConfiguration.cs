using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevDiary.Security;

public static class RequestUserConfiguration
{
    public static Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal, ICollection<AllowedClient> allowedClients)
    {
        var claims = new List<Claim>(principal.Claims);

        TransformScopeClaims(claims, allowedClients);

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(claims, principal.Identity!.AuthenticationType));

        return Task.FromResult(user);
    }

    private static void TransformScopeClaims(List<Claim> claims, ICollection<AllowedClient> allowedClients)
    {
        foreach (var allowedClient in allowedClients)
        {
            foreach (var scope in allowedClient.Scopes)
            {
                var isScopeFromAllowedClient =
                    claims.Any(x => x.Type == "client" && x.Value == allowedClient.Service);

                if (isScopeFromAllowedClient)
                {
                    claims.Add(new Claim("scope", scope, ClaimValueTypes.String));
                }
            }
        }
    }
}