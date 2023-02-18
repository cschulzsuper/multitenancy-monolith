using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserCommandHandler : ITickerUserCommandHandler
{
    private readonly ITickerUserManager _tickerUserManager;
    private readonly ITickerUserVerificationManager _tickerUserVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;

    public TickerUserCommandHandler(
        ITickerUserManager tickerUserManager,
        ITickerUserVerificationManager tickerUserVerificationManager,
        IAllowedClientsProvider allowedClientsProvider)
    {
        _tickerUserManager = tickerUserManager;
        _tickerUserVerificationManager = tickerUserVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
    }

    public async ValueTask<ClaimsIdentity> AuthAsync(TickerUserAuthCommand command)
    {
        var client = command.Client;

        if (_allowedClientsProvider.Get().All(x => x.UniqueName != client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        var found = await _tickerUserManager.ExistsAsync(command.Mail, command.Secret);

        if (!found)
        {
            TransportException.ThrowSecurityViolation($"Could not match ticker user '{command.Mail}' against secret");
        }

        var verification = Guid.NewGuid().ToByteArray();

        var verificationKey = new TickerUserVerificationKey
        {
            Group = command.Group,
            Client = client,
            Mail = command.Mail
        };

        _tickerUserVerificationManager.Set(verificationKey, verification);

        var verificationValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "ticker"),
            new Claim("group", command.Group),
            new Claim("client", client),
            new Claim("mail", command.Mail),
            new Claim("verification", verificationValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, "Badge");

        return claimsIdentity;
    }

    public void Verify() { }
}