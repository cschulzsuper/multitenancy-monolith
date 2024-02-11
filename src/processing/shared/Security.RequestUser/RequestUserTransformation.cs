using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

internal sealed class RequestUserTransformation : IClaimsTransformation
{
    private readonly RequestUserContext _context;
    private readonly RequestUserOptions _options;

    public RequestUserTransformation(
        RequestUserContext context,
        IOptions<RequestUserOptions> options)
    {
        _context = context;
        _options = options.Value;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (_options.Transform != null)
        {
            var user = await _options.Transform(principal);
            if (user == principal)
            {
                throw new UnreachableException("Please create a new principal.");
            }

            _context.User = user;
        }
        else
        {
            _context.User = principal;
        }

        return _context.User;
    }
}