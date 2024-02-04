using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class PreBuildSerializationClientResolver
{
    private readonly IServiceProvider _services;

    public PreBuildSerializationClientResolver(IServiceProvider services)
    {
        _services = services;
    }

    public IPreBuildSerializationClient Resolve(Type type)
        => (IPreBuildSerializationClient)_services.GetRequiredService(type);
}