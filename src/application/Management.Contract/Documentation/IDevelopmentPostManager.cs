using ChristianSchulz.MultitenancyMonolith.Objects.Documentation;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Documentation;

public interface IDevelopmentPostManager
{
    IQueryable<DevelopmentPost> GetQueryable();
}