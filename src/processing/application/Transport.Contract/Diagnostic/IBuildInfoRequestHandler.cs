using ChristianSchulz.MultitenancyMonolith.Application.Diagnostic.Responses;

namespace ChristianSchulz.MultitenancyMonolith.Application.Diagnostic;

public interface IBuildInfoRequestHandler
{
    BuildInfoResponse Get();
}
