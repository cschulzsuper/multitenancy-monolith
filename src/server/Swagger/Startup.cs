using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;
using ChristianSchulz.MultitenancyMonolith.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger;

public sealed class Startup
{
    private readonly string[] _webServices;

    public Startup(IConfiguration configuration)
    {
        _webServices = new WebServicesProvider(configuration).GetUniqueNames();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddWebServices(_webServices);

        services.AddScoped<SwaggerUIOptionsConfiguration>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseSwaggerUI(options =>
        {
            using var scope = app.ApplicationServices.CreateScope();

            scope.ServiceProvider
                .GetRequiredService<SwaggerUIOptionsConfiguration>()
                .Configure(options);
        });
    }
}