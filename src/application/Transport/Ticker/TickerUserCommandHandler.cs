using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Events;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class TickerUserCommandHandler : ITickerUserCommandHandler
{
    private readonly ITickerUserManager _tickerUserManager;
    private readonly ITickerMessageManager _tickerMessageManager;
    private readonly ITickerUserVerificationManager _tickerUserVerificationManager;
    private readonly IAllowedClientsProvider _allowedClientsProvider;
    private readonly ClaimsPrincipal _user;
    private readonly IEventStorage _eventStorage;

    public TickerUserCommandHandler(
        ITickerUserManager tickerUserManager,
        ITickerUserVerificationManager tickerUserVerificationManager,
        ITickerMessageManager tickerMessageManager,
        IAllowedClientsProvider allowedClientsProvider,
        ClaimsPrincipal user,
        IEventStorage eventStorage)
    {
        _tickerUserManager = tickerUserManager;
        _tickerMessageManager = tickerMessageManager;
        _tickerUserVerificationManager = tickerUserVerificationManager;
        _allowedClientsProvider = allowedClientsProvider;
        _user = user;
        _eventStorage = eventStorage;
    }

    public async Task<ClaimsIdentity> AuthAsync(TickerUserAuthCommand command)
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

                    // TODO Add event to notify ticker user about pending secret
                    _eventStorage.Add("ticker-user-secret-pending", @object.Snowflake);

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

        await _tickerUserManager.UpdateAsync(command.Mail, updateAction, defaultAction);

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

    public async Task<ClaimsIdentity> ConfirmAsync(TickerUserConfirmCommand command)
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

        await _tickerUserManager.UpdateAsync(command.Mail, updateAction, defaultAction);

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

    public async Task PostAsync(TickerUserPostCommand command)
    {
        var @object = new TickerMessage
        {
            Text = command.Text,
            Priority = TickerMessagePriorities.Low,
            Timestamp = DateTime.UtcNow.Ticks,
            TickerUser = _user.GetClaim("mail")
        };

        await _tickerMessageManager.InsertAsync(@object);
    }

    public void Verify()
    {
    }
}