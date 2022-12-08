using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{

    private const string MemberConfigurationKey = "SeedData:Administration:Members";

    private static Func<IServiceProvider, IRepository<Member>> MemberRepositoryFactory =>
        (services) =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var user = services.GetRequiredService<ClaimsPrincipal>();
            var userGroup = user.GetClaim("Group");

            var groupSections = configuration.GetRequiredSection(MemberConfigurationKey).GetChildren();

            var members = groupSections
                .Where(x => x.Key != userGroup)
                .SelectMany(x => x.Get<Member[]>() ?? throw new RepositoryException($"Could not get `{MemberConfigurationKey}` configuration for group `{x.Key}`"))
                .ToDictionary(
                    group => (object)group.UniqueName,
                    group => group);

            return new Repository<Member>(members);
        };

    private const string MembershipConfigurationKey = "SeedData:Administration:Memberships";

    private static Func<IServiceProvider, IRepository<Membership>> MembershipRepositoryFactory =>
    (services) =>
    {
        var configuration = services.GetRequiredService<IConfiguration>();

        var membershipSection = configuration.GetRequiredSection(MembershipConfigurationKey);

        var membership = membershipSection.Get<Membership[]>()?
            .ToDictionary(
                membership => (object)(membership.Group, membership.Identity, membership.Member),
                membership => membership)

             ?? throw new RepositoryException($"Could not get `{MembershipConfigurationKey}` configuration");

        return new Repository<Membership>(membership);
    };

    private const string IdentitiesConfigurationKey = "SeedData:Authentication:Identities";

    private static Func<IServiceProvider, IRepository<Identity>> IdentityRepositoryFactory =>
        (services) =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();

            var membershipSection = configuration.GetRequiredSection(MembershipConfigurationKey);

            var identities = configuration.GetRequiredSection(IdentitiesConfigurationKey).Get<Identity[]>()?
                .ToDictionary(
                    identity => (object)identity.UniqueName,
                    identity => identity)

                ?? throw new RepositoryException($"Could not get `{IdentitiesConfigurationKey}` configuration");

            return new Repository<Identity>(identities);
        };
}
