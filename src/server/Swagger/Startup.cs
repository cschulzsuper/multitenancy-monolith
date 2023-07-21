using ChristianSchulz.MultitenancyMonolith.Application;
using ChristianSchulz.MultitenancyMonolith.Configuration;
using ChristianSchulz.MultitenancyMonolith.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.Jobs;
using ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger;

public sealed class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddConfiguration();
        services.AddJobs(options => options.Configure());
    }

    public void Configure(IApplicationBuilder app)
    {
        app.ApplicationServices
            .GetRequiredService<IJobScheduler>()
            .MapHeartbeat();

        app.UseHttpsRedirection();

        app.UseSwaggerUI(options =>
        {
            options.ConfigureSwaggerEndpoints(
                app.ApplicationServices
                    .GetRequiredService<ISwaggerDocsProvider>()
                    .Get());

            options.UseAccessTokenRequestInterceptor();
        });
    }
}