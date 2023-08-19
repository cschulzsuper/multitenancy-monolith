using ChristianSchulz.MultitenancyMonolith.Application.Ticker.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Events;
using ChristianSchulz.MultitenancyMonolith.Objects.Ticker;
using ChristianSchulz.MultitenancyMonolith.ObjectValidation.Ticker.ConcreteValidators;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.AspNetCore.Authentication.BearerToken;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Ticker;

internal sealed class ContextTickerUserCommandHandler : IContextTickerUserCommandHandler
{
    private readonly ITickerUserManager _tickerUserManager;
    private readonly ITickerMessageManager _tickerMessageManager;
    private readonly ITickerUserVerificationManager _tickerUserVerificationManager;
    private readonly AllowedClient[] _allowedClients;
    private readonly ClaimsPrincipal _user;
    private readonly IEventStorage _eventStorage;

    public ContextTickerUserCommandHandler(
        ITickerUserManager tickerUserManager,
        ITickerUserVerificationManager tickerUserVerificationManager,
        ITickerMessageManager tickerMessageManager,
        IConfigurationProxyProvider configurationProxyProvider,
        ClaimsPrincipal user,
        IEventStorage eventStorage)
    {
        _tickerUserManager = tickerUserManager;
        _tickerMessageManager = tickerMessageManager;
        _tickerUserVerificationManager = tickerUserVerificationManager;
        _allowedClients = configurationProxyProvider.GetAllowedClients();
        _user = user;
        _eventStorage = eventStorage;
    }

    public async Task<object> AuthAsync(ContextTickerUserAuthCommand command)
    {
        var clientName = command.ClientName;

        if (_allowedClients.All(x => x.Service != clientName))
        {
            TransportException.ThrowSecurityViolation($"Client '{clientName}' is not allowed to sign in");
        }

        var updateAction = (TickerUser @object) =>
        {
            switch (@object.SecretState)
            {
                case TickerUserSecretStates.Invalid:
                    TransportException.ThrowSecurityViolation($"Secret state of ticker user '{command.Mail}' is invalid");
                    break;

                case TickerUserSecretStates.Reset:
                    @object.Secret = command.Secret;
                    @object.SecretState = TickerUserSecretStates.Pending;
                    @object.SecretToken = Guid.NewGuid();

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
            AccountGroup = command.AccountGroup,
            ClientName = clientName,
            Mail = command.Mail
        };

        _tickerUserVerificationManager.Set(verificationKey, verification);

        var verificationValue = Convert.ToBase64String(verification);

        var claims = new Claim[]
        {
            new Claim("type", "ticker"),
            new Claim("group", command.AccountGroup),
            new Claim("client", clientName),
            new Claim("mail", command.Mail),
            new Claim("verification", verificationValue, ClaimValueTypes.Base64Binary)
        };

        var claimsIdentity = new ClaimsIdentity(claims, BearerTokenDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        return claimsPrincipal;
    }

    public async Task ConfirmAsync(ContextTickerUserConfirmCommand command)
    {
        var updateAction = (TickerUser @object) =>
        {
            switch (@object.SecretState)
            {
                case TickerUserSecretStates.Invalid:
                    TransportException.ThrowProcessViolation($"Secret state of ticker user '{command.Mail}' is invalid");
                    break;

                case TickerUserSecretStates.Reset:
                    TransportException.ThrowProcessViolation($"Secret state of ticker user '{command.Mail}' is reset");
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
                    TransportException.ThrowProcessViolation($"Secret state of ticker user '{command.Mail}' is confirmed");
                    break;

                default:
                    TransportException.ThrowProcessViolation($"Secret state of ticker user '{command.Mail}' has unexpected value");
                    break;
            }
        };

        var defaultAction = () => TransportException.ThrowSecurityViolation($"Ticker user '{command.Mail}' does not exist");

        await _tickerUserManager.UpdateAsync(command.Mail, updateAction, defaultAction);
    }

    public async Task PostAsync(ContextTickerUserPostCommand command)
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