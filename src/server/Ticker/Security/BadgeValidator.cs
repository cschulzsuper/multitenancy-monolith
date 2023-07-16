using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using System.Net.Http.Headers;
using System.Net.Http;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security;

public class BadgeValidator
{
    public virtual async Task<bool> ValidateAsync(BadgeValidatePrincipalContext context)
    {
        var badgeClaims =
            context.Principal?.Claims as ICollection<Claim> ??
            context.Principal?.Claims.ToArray() ??
            Array.Empty<Claim>();

        var badgeType = badgeClaims.SingleOrDefault(x => x.Type == "badge");
        if (badgeType == null)
        {
            return false;
        }

        var badgeValid = badgeType.Value switch
        {
            "member" => await ValidateMemberAsync(context),
            "ticker" => ValidateTicker(context, badgeClaims),

            _ => false
        };

        return badgeValid;
    }

    protected virtual async Task<bool> ValidateMemberAsync(BadgeValidatePrincipalContext context)
    {
        var client = new HttpClient();

        // TODO Hard-coded url must be moved to configuration

        client.BaseAddress = new Uri("https://localhost:7207");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", context.HttpContext.Request.Headers.Authorization);

        var response = await client.PostAsync("/access/account-members/#/verify", null);

        var badgeValid = response.IsSuccessStatusCode;

        return badgeValid;
    }

    private static bool ValidateTicker(BadgeValidatePrincipalContext context, ICollection<Claim> badgeClaims)
    {
        var badgeClient = badgeClaims.SingleOrDefault(x => x.Type == "client");
        if (badgeClient == null)
        {
            return false;
        }

        var badgeGroup = badgeClaims.SingleOrDefault(x => x.Type == "group");
        if (badgeGroup == null)
        {
            return false;
        }

        var badgeMail = badgeClaims.SingleOrDefault(x => x.Type == "mail");
        if (badgeMail == null)
        {
            return false;
        }

        var verificationKey = new TickerUserVerificationKey
        {
            ClientName = badgeClient.Value,
            AccountGroup = badgeGroup.Value,
            Mail = badgeMail.Value
        };

        var badgeVerificationString = badgeClaims.SingleOrDefault(x => x.Type == "verification");
        if (badgeVerificationString == null)
        {
            return false;
        }

        var badgeVerification = Convert.FromBase64String(badgeVerificationString.Value);

        var badgeValid = context.HttpContext.RequestServices
            .GetRequiredService<ITickerUserVerificationManager>()
            .Has(verificationKey, badgeVerification);

        return badgeValid;
    }
}
