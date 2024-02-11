using ChristianSchulz.MultitenancyMonolith.Application.Access.Commands;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Access;

public interface IContextAccountMemberCommandHandler
{
    Task<object> AuthAsync(ContextAccountMemberAuthCommand command);
    Task VerifyAsync();
}