using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission;

public class SignInService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly TransportWebServiceClientFactory _transportWebServiceClientFactory;
    private readonly IConfigurationProxyProvider _configurationProxyProvider;

    public SignInService(
        IConfigurationProxyProvider configurationProxyProvider,
        IHttpContextAccessor contextAccessor,
        TransportWebServiceClientFactory transportWebServiceClientFactory)
    {
        _contextAccessor = contextAccessor;
        _transportWebServiceClientFactory = transportWebServiceClientFactory;
        _configurationProxyProvider = configurationProxyProvider;
    }

    public void InitializeModel(SignInModel model)
    {
        var admissionPortal = _configurationProxyProvider.GetAdmissionPortal();

        model.Stage ??= "username";
        model.ClientName ??= admissionPortal.ClientName;
    }

    public string NextStage(string stage)
        => stage switch
        {
            "username" => "password",
            _ => throw new InvalidOperationException("No successor stage.")
        };

    public async Task<string> ResolveAuthenticationIdentityAsync(string username)
    {
        var maintenanceAuthenticationIdentity = _configurationProxyProvider.GetMaintenanceAuthenticationIdentity();

        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = maintenanceAuthenticationIdentity.ClientName,
            AuthenticationIdentity = maintenanceAuthenticationIdentity.UniqueName,
            Secret = maintenanceAuthenticationIdentity.Secret,
        };

        var token = await AuthAsync(command);

        var admissionServer = _configurationProxyProvider.GetAdmissionServer();

        using var client = _transportWebServiceClientFactory
            .Create<IAuthenticationIdentityRequestClient>(admissionServer.Service, () => Task.FromResult(token)!);

        var potentialIdentities = client.GetAll($"mail-address:{username} unique-name:{username}", 0, 2);

        string? identity = null;

        await foreach (var potentialIdentity in potentialIdentities)
        {
            if(identity != null) throw new Exception("User not found");

            identity = potentialIdentity.UniqueName;
        }

        if (identity == null) throw new Exception("User not found");

        return identity;
    }

    public async Task SignInAsync(SignInModel model)
    {
        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = model.ClientName!,
            AuthenticationIdentity = model.Username!,
            Secret = model.Password!,
        };

        var token = await AuthAsync(command);

        _contextAccessor.HttpContext?.Response.Cookies.Append("access_token", token);
    }

    private async Task<string> AuthAsync(ContextAuthenticationIdentityAuthCommand command)
    {
        var admissionServer = _configurationProxyProvider.GetAdmissionServer();

        using var client = _transportWebServiceClientFactory
            .Create<IContextAuthenticationIdentityCommandClient>(admissionServer.Service);

        var tokenObject = await client.AuthAsync(command);
        var token = $"{tokenObject}";

        return token;
    }
}
