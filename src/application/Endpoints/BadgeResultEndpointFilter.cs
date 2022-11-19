using ChristianSchulz.MultitenancyMonolith.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public class BadgeResultEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is ClaimsIdentity claimsIdentity)
        {
            var claims = claimsIdentity.Claims as Claim[] ?? claimsIdentity.Claims.ToArray();

            var claimsSerialized = JsonSerializer.SerializeToUtf8Bytes(claims, 
                ClaimsJsonSerializerOptions.Options);

            result = WebEncoders.Base64UrlEncode(claimsSerialized);
        }

        return result;
    }
}
