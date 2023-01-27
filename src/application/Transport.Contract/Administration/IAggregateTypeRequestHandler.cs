using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IAggregateTypeRequestHandler
{
    AggregateTypeResponse Get(string uniqueName);

    IEnumerable<AggregateTypeResponse> GetAll();

}