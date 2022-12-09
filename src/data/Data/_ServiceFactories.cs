using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Data;

public static partial class _Services
{
    private static Func<IServiceProvider, IRepository<Member>> MemberRepositoryFactory =>
        (services) =>
        {
            var multitenancyContext = services.GetRequiredService<MultitenancyContext>();
            var multitenancyDiscriminator = multitenancyContext.MultitenancyDiscriminator;

            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Member>>();
            var repositoryContext = repositoryContextFactory.Create(multitenancyDiscriminator, member => member.UniqueName);

            return new Repository<Member>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Membership>> MembershipRepositoryFactory =>
        (services) =>
        {
            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Membership>>();
            var repositoryContext = repositoryContextFactory.Create(membership => (membership.Group, membership.Member, membership.Identity));

            return new Repository<Membership>(repositoryContext);
        };

    private static Func<IServiceProvider, IRepository<Identity>> IdentityRepositoryFactory =>
        (services) =>
        {
            var repositoryContextFactory = services.GetRequiredService<RepositoryContextFactory<Identity>>();
            var repositoryContext = repositoryContextFactory.Create(Identity => Identity.UniqueName);

            return new Repository<Identity>(repositoryContext);
        };
}
