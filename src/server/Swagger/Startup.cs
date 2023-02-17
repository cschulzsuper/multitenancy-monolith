using ChristianSchulz.MultitenancyMonolith.Server.Swagger.SwaggerUI;
using Microsoft.AspNetCore.Builder;

namespace ChristianSchulz.MultitenancyMonolith.Server.Swagger;

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        app.UseSwaggerUI(options =>
        {
            options.ConfigureSwaggerEndpoints();
            options.UseAccessTokenRequestInterceptor();
        });
    }
}