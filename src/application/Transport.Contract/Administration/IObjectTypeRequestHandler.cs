using ChristianSchulz.MultitenancyMonolith.Application.Administration.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Administration;

public interface IObjectTypeRequestHandler
{
    ObjectTypeResponse Get(string uniqueName);

    IEnumerable<ObjectTypeResponse> GetAll();

}