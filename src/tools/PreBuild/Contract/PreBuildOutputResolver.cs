using Microsoft.Extensions.DependencyInjection;
using System;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;

public class PreBuildOutputResolver
{
    private readonly IServiceProvider _services;

    public PreBuildOutputResolver(IServiceProvider services)
    {
        _services = services;
    }

    public IPreBuildOutput Resolve(Type type)
        => (IPreBuildOutput)_services.GetRequiredService(type);
}