using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;

public class RequestUserOptions
{
    public Func<ClaimsPrincipal, Task<ClaimsPrincipal>>? Transform { get; set; }
}
