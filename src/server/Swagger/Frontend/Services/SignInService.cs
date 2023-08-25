using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Frontend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Frontend.Services;

public class SignInService
{
    private readonly IContextAuthenticationIdentityCommandClient _authenticationClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly AdmissionServer _admissionServer;

    public SignInService(
        IContextAuthenticationIdentityCommandClient authenticationClient,
        IConfigurationProxyProvider configurationProxyProvider,
        IHttpContextAccessor contextAccessor)
    {
        _authenticationClient = authenticationClient;
        _contextAccessor = contextAccessor;
        _admissionServer = configurationProxyProvider.GetAdmissionServer();
    }

    public async Task SignInAsync(SignInModel model)
    {
        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = model.ClientName ?? _admissionServer.ClientName,
            AuthenticationIdentity = model.Username,
            Secret = model.Password,
        };

        var tokenObject = await _authenticationClient.AuthAsync(command);
        var token = $"{tokenObject}";

        _contextAccessor.HttpContext?.Response.Cookies.Append("access_token", token);
    }
}
