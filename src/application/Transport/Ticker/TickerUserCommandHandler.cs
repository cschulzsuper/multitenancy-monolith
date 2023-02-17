using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserCommandHandler : ITickerUserCommandHandler
{
    private readonly ITickerUserManager _tickerUserManager;
    private readonly ITickerUserVerificationManager _tickerUserVerificationManager;

    private readonly string[] _allowedClients = { "swagger", "security-tests" };

    public TickerUserCommandHandler(
        ITickerUserManager tickerUserManager,
        ITickerUserVerificationManager tickerUserVerificationManager)
    {
        _tickerUserManager = tickerUserManager;
        _tickerUserVerificationManager = tickerUserVerificationManager;
    }

    public async ValueTask<ClaimsIdentity> AuthAsync(TickerUserAuthCommand command)
    {
        var client = command.Client;

        if (!_allowedClients.Contains(command.Client))
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