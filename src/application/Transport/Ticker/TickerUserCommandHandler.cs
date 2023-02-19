using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
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

        var updateAction = (TickerUser @object) => 
        {
            switch (@object.SecretState)
            {
                case TickerUserSecretStates.Invalid:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is invalid");
                    break;

                case TickerUserSecretStates.Temporary:
                    @object.Secret = command.Secret;
                    @object.SecretState = TickerUserSecretStates.Pending;
                    @object.SecretToken = Guid.NewGuid();
                    break;

                case TickerUserSecretStates.Pending:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is pending");
                    break;

                case TickerUserSecretStates.Confirmed:
                    if (@object.Secret != command.Secret)
                    {
                        TransportException.ThrowSecurityViolation($"Secret of ticker user '{command.Mail}' is not correct");
                    }
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' has unexpected value");
                    break;
            }
        };

        var defaultAction = () => TransportException.ThrowSecurityViolation($"Ticker user '{command.Mail}' does not exist");

        await _tickerUserManager.UpdateOrDefaultAsync(command.Mail, updateAction, defaultAction);

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

    public async ValueTask<ClaimsIdentity> ConfirmAsync(TickerUserConfirmCommand command)
    {
        var client = command.Client;
        if (_allowedClientsProvider.Get().All(x => x.UniqueName != client))
        {
            TransportException.ThrowSecurityViolation($"Client '{client}' is not allowed to sign in");
        }

        var updateAction = (TickerUser @object) =>
        {
            switch (@object.SecretState)
            {
                case TickerUserSecretStates.Invalid:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is invalid");
                    break;

                case TickerUserSecretStates.Temporary:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is temporary");
                    break;

                case TickerUserSecretStates.Pending:
                    if (@object.Secret != command.Secret)
                    {
                        TransportException.ThrowSecurityViolation($"Secret of ticker user '{command.Mail}' is not correct");
                    }
                    if (@object.SecretToken != command.SecretToken)
                    {
                        TransportException.ThrowSecurityViolation($"Secret token of ticker user '{command.Mail}' is not correct");
                    }
                    @object.SecretState = TickerUserSecretStates.Confirmed;
                    break;

                case TickerUserSecretStates.Confirmed:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is confirmed");
                    break;

                default:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' has unexpected value");
                    break;
            }
        };

        var defaultAction = () => TransportException.ThrowSecurityViolation($"Ticker user '{command.Mail}' does not exist");

        await _tickerUserManager.UpdateOrDefaultAsync(command.Mail, updateAction, defaultAction);

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