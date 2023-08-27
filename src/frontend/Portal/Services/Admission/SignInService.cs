using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Portal.Services.Admission;

public class SignInService
{
    private readonly IContextAuthenticationIdentityCommandClient _authenticationClient;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IConfigurationProxyProvider _configurationProxyProvider;
    private readonly AdmissionPortal _admissionPortal;
    private readonly IWebHostEnvironment _environment;

    public SignInService(
        IContextAuthenticationIdentityCommandClient authenticationClient,
        IConfigurationProxyProvider configurationProxyProvider,
        IHttpContextAccessor contextAccessor, 
        IWebHostEnvironment environment)
    {
        _authenticationClient = authenticationClient;
        _contextAccessor = contextAccessor;
        _configurationProxyProvider = configurationProxyProvider;
        _admissionPortal = configurationProxyProvider.GetAdmissionPortal();
        _environment = environment;
    }

    public void InitializeModel(SignInModel model)
    {
        if (_environment.IsStaging()) 
        {
            var defaultStagingAuthenticationIdentity = _configurationProxyProvider.GetDefaultStagingAuthenticationIdentity();

            model.Username = string.IsNullOrWhiteSpace(model.Username)
                ? defaultStagingAuthenticationIdentity.UniqueName ?? string.Empty
                : model.Username;

            model.Password = string.IsNullOrWhiteSpace(model.Password)
                ? defaultStagingAuthenticationIdentity.Secret ?? string.Empty
                : model.Password;
        }
    }

    public async Task SignInAsync(SignInModel model)
    {
        var command = new ContextAuthenticationIdentityAuthCommand
        {
            ClientName = model.ClientName ?? _admissionPortal.ClientName,
            AuthenticationIdentity = model.Username,
            Secret = model.Password,
        };

        var tokenObject = await _authenticationClient.AuthAsync(command);
        var token = $"{tokenObject}";

        _contextAccessor.HttpContext?.Response.Cookies.Append("access_token", token);
    }
}
