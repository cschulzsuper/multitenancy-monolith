using ChristianSchulz.MultitenancyMonolith.Application.Extension.Responses;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application.Extension;

public interface IObjectTypeRequestHandler
{
    ObjectTypeResponse Get(string uniqueName);

    IEnumerable<ObjectTypeResponse> GetAll();

}