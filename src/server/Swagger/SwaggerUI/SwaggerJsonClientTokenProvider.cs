using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;

internal sealed class SwaggerJsonClientTokenProvider
{
    private readonly TransportWebServiceClientFactory _transportWebServiceClientFactory;
    private readonly AdmissionServer _admissionServer;

    public SwaggerJsonClientTokenProvider(
        TransportWebServiceClientFactory transportWebServiceClientFactory,
        IAdmissionServerProvider admissionServerProvider)
    {
        _transportWebServiceClientFactory = transportWebServiceClientFactory;
        _admissionServer = admissionServerProvider.Get();
    }

    public async Task<string> GetAsync()
    {
        using var client = _transportWebServiceClientFactory
            .Create<IContextAuthenticationIdentityCommandClient>(_admissionServer.Service);

        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = "swagger-ui-host",
            AuthenticationIdentity = _admissionServer.MaintenanceAuthenticationIdentity,
            Secret = _admissionServer.MaintenanceAuthenticationIdentitySecret
        };

        var tokenObject = await client.AuthAsync(command);
        var token = $"{tokenObject}";

        return token;
    }
}
