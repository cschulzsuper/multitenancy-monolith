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
    private readonly string[] _services;

    public Startup(IConfiguration configuration)
    {
        _services = new ServiceMappingsProvider(configuration)
            .GetUniqueNames()
            .Where(services => new SwaggerDocsProvider(configuration)
                .Get()
                .Select(swaggerDoc => swaggerDoc.Service)
                .Contains(services))
            .ToArray();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddWebServices(_services);

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