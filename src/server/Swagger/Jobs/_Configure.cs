using ChristianSchulz.MultitenancyMonolith.Jobs;
using System.Diagnostics.CodeAnalysis;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Jobs;

[SuppressMessage("Style", "IDE1006:Naming Styles")]
internal static class _Configure
{
    public static JobsOptions Configure(this JobsOptions options)
    {
        return options;
    }
}