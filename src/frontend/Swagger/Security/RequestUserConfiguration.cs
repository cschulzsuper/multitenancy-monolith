using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.Security;

public static class RequestUserConfiguration
{
    public static Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal, ICollection<AllowedClient> allowedClients)
    {
        var claims = new List<Claim>(principal.Claims);

        TransformAdminRoleClaim(claims);
        TransformChiefRoleClaim(claims);
        TransformChiefObserverRoleClaim(claims);
        TransformMemberRoleClaim(claims);
        TransformMemberObserverRoleClaim(claims);

        TransformScopeClaims(claims, allowedClients);

        var user = new ClaimsPrincipal(
            new ClaimsIdentity(claims, principal.Identity!.AuthenticationType));

        return Task.FromResult(user);
    }

    private static void TransformAdminRoleClaim(List<Claim> claims)
    {
        var isAdmin = claims.Any(x => x.Type == "identity" && x.Value == "admin");

        if (isAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin", ClaimValueTypes.String));
        }
    }

    private static void TransformChiefRoleClaim(List<Claim> claims)
    {

        var isChief =
            claims.Any(x => x.Type == "type" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value != "demo") &&
            claims.Any(x => x.Type == "member" && x.Value.StartsWith("chief-"));

        if (isChief)
        {
            claims.Add(new Claim(ClaimTypes.Role, "chief", ClaimValueTypes.String));
        }
    }

    private static void TransformChiefObserverRoleClaim(List<Claim> claims)
    {

        var isChiefObserver =
            claims.Any(x => x.Type == "type" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value == "demo") &&
            claims.Any(x => x.Type == "member" && x.Value.StartsWith("chief-"));

        if (isChiefObserver)
        {
            claims.Add(new Claim(ClaimTypes.Role, "chief-observer", ClaimValueTypes.String));
        }
    }

    private static void TransformMemberRoleClaim(List<Claim> claims)
    {


        var isMember =
            claims.Any(x => x.Type == "type" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value != "demo") &&
            claims.Any(x => x.Type == "member" && !string.IsNullOrWhiteSpace(x.Value));

        if (isMember)
        {
            claims.Add(new Claim(ClaimTypes.Role, "member", ClaimValueTypes.String));
        }
    }

    private static void TransformMemberObserverRoleClaim(List<Claim> claims)
    {
        var isMemberObserver =
            claims.Any(x => x.Type == "type" && x.Value == "member") &&
            claims.Any(x => x.Type == "identity" && x.Value == "demo") &&
            claims.Any(x => x.Type == "member" && !string.IsNullOrWhiteSpace(x.Value));

        if (isMemberObserver)
        {
            claims.Add(new Claim(ClaimTypes.Role, "member-observer", ClaimValueTypes.String));
        }
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