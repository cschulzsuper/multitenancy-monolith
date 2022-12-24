using ChristianSchulz.MultitenancyMonolith.Aggregates.Authentication;
using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityManager : IIdentityManager
{
    private readonly IRepository<Identity> _repository;

    public IdentityManager(IRepository<Identity> repository)
    {
        _repository = repository;
    }

    public ValueTask<Identity> GetAsync(long snowflake)
    {
        IdentityValidator.EnsureSnowflake(snowflake);

        var identity = _repository.GetAsync(snowflake);

        return identity;
    }

    public ValueTask<Identity> GetAsync(string uniqueName)
    {
        IdentityValidator.EnsureUniqueName(uniqueName);

        var identity = _repository.GetAsync(x => x.UniqueName == uniqueName);

        return identity;
    }

    public IQueryable<Identity> GetAll()
        => _repository.GetQueryable();

    public async ValueTask InsertAsync(Identity identity)
    {
        IdentityValidator.EnsureInsertable(identity);

        await _repository.InsertAsync(identity);
    }

    public async ValueTask DeleteAsync(long snowflake)
    {
        IdentityValidator.EnsureSnowflake(snowflake);

        await _repository.DeleteOrThrowAsync(snowflake);
    }

    public async ValueTask DeleteAsync(string uniqueName)
    {
        IdentityValidator.EnsureUniqueName(uniqueName);

        await _repository.DeleteOrThrowAsync(x => x.UniqueName == uniqueName);
    }
}