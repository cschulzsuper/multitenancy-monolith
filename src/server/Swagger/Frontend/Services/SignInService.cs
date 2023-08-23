using ChristianSchulz.MultitenancyMonolith.Application.Admission;
using ChristianSchulz.MultitenancyMonolith.Application.Admission.Commands;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Frontend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger.Frontend.Services
{
    public class SignInService
    {
        private readonly IContextAuthenticationIdentityCommandClient _authenticationClient;
        private readonly IHttpContextAccessor _contextAccessor;

        public SignInService(
            IContextAuthenticationIdentityCommandClient authenticationClient,
            IHttpContextAccessor contextAccessor)
        {
            _authenticationClient = authenticationClient;
            _contextAccessor = contextAccessor;
        }

        public async Task SignInAsync(SignInModel model)
        {
            var command = new ContextAuthenticationIdentityAuthCommand
            {
                ClientName = "swagger-ui",
                AuthenticationIdentity = model.Username,
                Secret = model.Password,
            };

            var tokenObject = await _authenticationClient.AuthAsync(command);
            var token = $"{tokenObject}";

            _contextAccessor.HttpContext?.Response.Cookies.Append("access_token", token);
        }
    }
}
