using ChristianSchulz.MultitenancyMonolith.Data;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Authentication;

internal sealed class IdentityManager : IIdentityManager
{
    private readonly IRepository<Identity> _repository;

    public IdentityManager(IRepository<Identity> repository)
    {
        _repository = repository;
    }

    public Identity Get(string uniqueName)
        => _repository.Get(uniqueName);

    public IQueryable<Identity> GetAll()
        => _repository.GetQueryable();
}