using ChristianSchulz.MultitenancyMonolith.Application.Documentation;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Configuration.Proxies;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.DataProtection;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Endpoints;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Security;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Services;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using ChristianSchulz.MultitenancyMonolith.Data.EntityFramework;
using ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite;
using ChristianSchulz.MultitenancyMonolith.Data.EntityFramework.Sqlite.Documentation;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using ChristianSchulz.MultitenancyMonolith.Frontend.DevLog.Data;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog;

public sealed class Startup
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public Startup(
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;

        if (!environment.IsDevelopment())
        {
            // TODO https://github.com/dotnet/aspnetcore/issues/49577
            StaticWebAssetsLoader.UseStaticWebAssets(environment, configuration);
        }
    }

    private void LoadRequiredConfiguration(
        out AllowedClient[] allowedClients,
        out string[] allowedClientHosts,
        out string admissionClientName,
        out string admissionFrontendUrl)
    {
        var configurationProxyProvider = new ConfigurationProxyProvider(_configuration);

        var configuredAdmissionPortal = configurationProxyProvider.GetAdmissionPortal();
        var configuredAllowedClients = configurationProxyProvider.GetAllowedClients();
        var configuredServicesMappings = configurationProxyProvider.GetServiceMappings();

        allowedClients = configuredAllowedClients;

        allowedClientHosts = configuredServicesMappings
            .Where(serviceMapping => configuredAllowedClients
                .Select(allowedClient => allowedClient.Service)
                .Contains(serviceMapping.UniqueName))
            .Select(servicesMapping => servicesMapping.Url)
            .ToArray();

        admissionClientName = configuredAdmissionPortal.ClientName;

        admissionFrontendUrl = configuredServicesMappings
            .Where(servicesMapping =>
                servicesMapping.UniqueName == configuredAdmissionPortal.Service)
            .Select(x => x.Url)
            .Single();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        LoadRequiredConfiguration(
            out var allowedClients,
            out var allowedClientHosts,
            out var admissionClientName,
            out var admissionFrontendUrl);

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddDataProtection().Configure(_environment, _configuration);

        services.AddAuthentication(BearerTokenDefaults.AuthenticationScheme)
            .AddBearerToken(options =>
                {
                    options.Configure();
                    options.ForwardChallenge = CookieAuthenticationDefaults.AuthenticationScheme;
                })
            .AddCookie(options =>
                {
                    options.LoginPath = "/auth";
                    options.ReturnUrlParameter = "return";
                    options.Cookie.Name = BearerTokenConstants.CookieName;
                    options.Events = new CookieAuthenticationEvents()
                    {
                        OnRedirectToLogin = (context) =>
                        {
                            var _originalScheme = context.Request.Scheme;
                            var _originalHost = context.Request.Host;
                            var _originalPath = context.Request.Path;
                            var _originalPathBase = context.Request.PathBase;

                            var redirectUri = $"{_originalScheme}://{_originalHost}{_originalPathBase}{_originalPath}{context.Request.QueryString}";

                            var queryParameter = new[]
                            {
                                new KeyValuePair<string, string?>(options.ReturnUrlParameter, redirectUri),
                                new KeyValuePair<string, string?>("client-name", admissionClientName)
                            };

                            var loginUri = $"{admissionFrontendUrl}{options.LoginPath}{QueryString.Create(queryParameter)}";

                            context.HttpContext.Response.Redirect(loginUri);

                            return Task.CompletedTask;
                        }
                    };
                });

        services.AddRequestLocalization(options =>
        {
            var supportedCultures = new List<CultureInfo> { new("en-US") };

            options.DefaultRequestCulture = new RequestCulture("en-US");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
        });

        services.AddAuthorization();

        services
            .AddRazorComponents()
            .AddDevLogServices();

        services.AddCors(setup => setup
            .AddDefaultPolicy(config => config
                .WithOrigins(allowedClientHosts)
                .WithMethods(HttpMethods.Get)));

        services.AddRequestUser(options => options.Configure(allowedClients));
        services.AddConfiguration();

        services.AddDataEntityFramework();
        services.AddDataEntityFrameworkSqlite();
        services.AddDataEntityFrameworkSqliteDocumentation();

        services.AddDocumentationManagement();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
        app.UseRequestLocalization();

        if (!_environment.IsProduction())
        {
            app.ApplicationServices.ConfigureDevelopmentPosts();
        }

        app.UseCors();

        app.Map("/dev-log", builder =>
        {
            builder.UseStaticFiles();
            builder.UseRouting();
            builder.UseAntiforgery();

            builder.UseAuthentication();
            builder.UseAuthorization();

            builder.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorComponents<App>();
                endpoints.MapSignIn();
            });
        });
    }
}
