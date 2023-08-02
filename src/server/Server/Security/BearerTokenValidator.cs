using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.Security
{
    public class BearerTokenValidator
    {


        public virtual void Validate(MessageReceivedContext context, AuthenticationTicket ticket)
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
                    ValidateIdentity(context, ticket);
                    return;

                case "member":
                    ValidateMember(context, ticket);
                    return;
            }

            return;
        }
        protected virtual void ValidateIdentity(MessageReceivedContext context, AuthenticationTicket ticket)
        {
            var clientClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "client");
            if (clientClaim == null)
            {
                context.Fail("Token has no client");
                return;
            }

            var identityClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "identity");
            if (identityClaim == null)
            {
                context.Fail("Token has no identity");
                return;
            }

            var verificationKey = new AuthenticationIdentityVerificationKey
            {
                ClientName = clientClaim.Value,
                AuthenticationIdentity = identityClaim.Value,
            };

            var verificationClaimString = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "verification");
            if (verificationClaimString == null)
            {
                context.Fail("Token has no verification");
                return;
            }

            var verificationClaim = Convert.FromBase64String(verificationClaimString.Value);

            var valid = context.HttpContext.RequestServices
                .GetRequiredService<IAuthenticationIdentityVerificationManager>()
                .Has(verificationKey, verificationClaim);

            if (!valid)
            {
                context.Fail("Token has invalid verification");
                return;
            }

            context.Principal = ticket.Principal;
            context.Success();
        }
        protected virtual void ValidateMember(MessageReceivedContext context, AuthenticationTicket ticket)
        {
            var clientClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "client");
            if (clientClaim == null)
            {
                context.Fail("Token has no client");
                return;
            }

            var identityClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "identity");
            if (identityClaim == null)
            {
                context.Fail("Token has no identity");
                return;
            }

            var groupClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "group");
            if (groupClaim == null)
            {
                context.Fail("Token has no group");
                return;
            }

            var memberClaim = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "member");
            if (memberClaim == null)
            {
                context.Fail("Token has no member");
                return;
            }

            var verificationKey = new AccountMemberVerificationKey
            {
                ClientName = clientClaim.Value,
                AuthenticationIdentity = identityClaim.Value,
                AccountGroup = groupClaim.Value,
                AccountMember = memberClaim.Value,
            };

            var verificationClaimString = ticket.Principal.Claims.SingleOrDefault(x => x.Type == "verification");
            if (verificationClaimString == null)
            {
                context.Fail("Token has no verification");
                return;
            }

            var verificationClaim = Convert.FromBase64String(verificationClaimString.Value);

            var valid = context.HttpContext.RequestServices
                .GetRequiredService<IAccountMemberVerificationManager>()
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
