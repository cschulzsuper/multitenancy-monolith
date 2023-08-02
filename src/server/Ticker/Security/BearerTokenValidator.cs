using ChristianSchulz.MultitenancyMonolith.Application.Ticker;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Ticker.Security
{
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

                case "ticker":
                    ValidateTicker(context, ticket);
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

            using var client = context.HttpContext.RequestServices
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient("server");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerTokenDefaults.AuthenticationScheme, context.Request.Headers.Authorization);

            var response = await client.PostAsync("/api/admission/authentication-identities/_/verify", null);
            if (!response.IsSuccessStatusCode)
            {
                context.Fail("Verifying token failed");
                return;
            };

            context.Principal = ticket.Principal;
            context.Success();
        }

        protected virtual async Task ValidateMemberAsync(MessageReceivedContext context, AuthenticationTicket ticket)
        {
            var isIdentityToken = ticket.Principal.HasClaim(x => x.Type == "member");
            if (!isIdentityToken)
            {
                context.NoResult();
                return;
            }

            using var client = context.HttpContext.RequestServices
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient("server");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(BearerTokenDefaults.AuthenticationScheme, context.Request.Headers.Authorization);

            var response = await client.PostAsync("/api/access/account-members/_/verify", null);
            if (!response.IsSuccessStatusCode)
            {
                context.Fail("Verifying token failed");
                return;
            };

            context.Principal = ticket.Principal;
            context.Success();

        }

        protected virtual void ValidateTicker(MessageReceivedContext context, AuthenticationTicket ticket)
        {
            var clientClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "client");
            if (clientClaim == null)
            {
                context.Fail("Token has no client");
                return;
            }

            var groupClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "group");
            if (groupClaim == null)
            {
                context.Fail("Token has no group");
                return;
            }

            var mailClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "mail");
            if (mailClaim == null)
            {
                context.Fail("Token has no mail");
                return;
            }

            var verificationKey = new TickerUserVerificationKey
            {
                ClientName = clientClaim.Value,
                AccountGroup = groupClaim.Value,
                Mail = mailClaim.Value
            };

            var verificationClaimString = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "verification");
            if (verificationClaimString == null)
            {
                context.Fail("Token has no verification");
                return;
            }

            var verificationClaim = Convert.FromBase64String(verificationClaimString.Value);

            var valid = context.HttpContext.RequestServices
                .GetRequiredService<ITickerUserVerificationManager>()
                .Has(verificationKey, verificationClaim);

            if (!valid)
            {
                context.Fail("Token has invalid verification");
                return;
            }

            context.Principal = ticket.Principal;
            context.Success();
        }
    }
}
