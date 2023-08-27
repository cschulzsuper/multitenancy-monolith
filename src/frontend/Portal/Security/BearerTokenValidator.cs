using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Security;

public class BearerTokenValidator
{
    public virtual async Task ValidateAsync(MessageReceivedContext context, AuthenticationTicket ticket)
    {
        var typeClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "type");
        if (typeClaim == null)
        {
            context.Fail("Token has no type");
            return;
        }

        switch (typeClaim.Value)
        {
            case "identity":
                await ValidateIdentityAsync(context, ticket);
                return;

            case "member":
                await ValidateMemberAsync(context, ticket);
                return;
        }
    }

    protected virtual async Task ValidateIdentityAsync(MessageReceivedContext context, AuthenticationTicket ticket)
    {
        var isIdentityToken = ticket.Principal.HasClaim(x => x.Type == "identity");
        if (!isIdentityToken)
        {
            context.NoResult();
            return;
        }

        var admissionServer = context.HttpContext.RequestServices
            .GetRequiredService<IConfigurationProxyProvider>()
            .GetAdmissionServer();

        using var client = context.HttpContext.RequestServices
            .GetRequiredService<TransportWebServiceClientFactory>()
            .Create<IContextAuthenticationIdentityCommandClient>(admissionServer.Service, () => Task.FromResult(context.Token));

        try
        {
            await client.VerifyAsync();

            context.Principal = ticket.Principal;
            context.Success();
        }
        catch
        {
            context.Fail("Verifying token failed");
        }
    }

    protected virtual async Task ValidateMemberAsync(MessageReceivedContext context, AuthenticationTicket ticket)
    {
        var isIdentityToken = ticket.Principal.HasClaim(x => x.Type == "member");
        if (!isIdentityToken)
        {
            context.NoResult();
            return;
        }

        using var contextAuthenticationIdentityCommandHandler = context.HttpContext.RequestServices
            .GetRequiredService<TransportWebServiceClientFactory>()
            .Create<IContextAccountMemberCommandClient>("server", () => Task.FromResult(context.Token));

        try
        {
            await contextAuthenticationIdentityCommandHandler.VerifyAsync();

            context.Principal = ticket.Principal;
            context.Success();
        }
        catch
        {
            context.Fail("Verifying token failed");
        }

    }
}
