using ChristianSchulz.MultitenancyMonolith.Application.Access;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using System;
using System.Collections.Generic;

namespace ChristianSchulz.MultitenancyMonolith.Application;

public static class TransportWebServiceClientMappings
{
    public static Dictionary<Type, Type> Mappings { get; } = new Dictionary<Type, Type>()
    {
        [typeof(IContextAccountMemberCommandClient)] = typeof(ContextAccountMemberCommandWebServiceClient),
        [typeof(IContextAuthenticationIdentityCommandClient)] = typeof(ContextAuthenticationIdentityCommandHandlerWebServiceClient)
    };
}
