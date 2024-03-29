﻿using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger.SwaggerUI;

internal sealed class SwaggerJsonClientTokenProvider
{
    private readonly TransportWebServiceClientFactory _transportWebServiceClientFactory;
    private readonly AdmissionServer _admissionServer;

    public SwaggerJsonClientTokenProvider(
        TransportWebServiceClientFactory transportWebServiceClientFactory,
        IConfigurationProxyProvider configurationProxyProvider)
    {
        _transportWebServiceClientFactory = transportWebServiceClientFactory;
        _admissionServer = configurationProxyProvider.GetAdmissionServer();
    }

    public async Task<string> GetAsync()
    {
        using var client = _transportWebServiceClientFactory
            .Create<IContextAuthenticationIdentityCommandClient>(_admissionServer.Service);

        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = _admissionServer.MaintenanceClient,
            AuthenticationIdentity = _admissionServer.MaintenanceIdentity,
            Secret = _admissionServer.MaintenanceSecret
        };

        var tokenObject = await client.AuthAsync(command);
        var token = $"{tokenObject}";

        return token;
    }
}
