using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IMemberManager
{
    Member Get(string uniqueName);

    IQueryable<Member> GetAll();
}