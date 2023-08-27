using ChristianSchulz.MultitenancyMonolith.Data;
using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Documentation;

internal sealed class DevelopmentPostManager : IDevelopmentPostManager
{
    private readonly IRepository<DevelopmentPost> _repository;

    public DevelopmentPostManager(IRepository<DevelopmentPost> repository)
    {
        _repository = repository;
    }

    public IQueryable<DevelopmentPost> GetQueryable()
        => _repository.GetQueryable();
}