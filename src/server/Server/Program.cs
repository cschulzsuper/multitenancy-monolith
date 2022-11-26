using ChristianSchulz.MultitenancyMonolith.Application.Administration;
using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Weather;
using ChristianSchulz.MultitenancyMonolith.Caching;
using ChristianSchulz.MultitenancyMonolith.Server.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.Authentication.Badge;
using ChristianSchulz.MultitenancyMonolith.Shared.Security.RequestUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChristianSchulz.MultitenancyMonolith.Server;

public sealed class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication().AddBadge(options => options.Configure());
        builder.Services.AddRequestUser();
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Multitenancy Monolith", Version = "v1" });

            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        builder.Services.AddCaching();

        builder.Services.AddAdministrationManagement();
        builder.Services.AddAdministrationTransport();

        builder.Services.AddAuthenticationManagement();
        builder.Services.AddAuthenticationTransport();

        builder.Services.AddWeatherForecastTransport();

        var app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseRequestUser();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Multitenancy Monolith");
            });
        }

        app.MapAdministrationEndpoints();
        app.MapAuthenticationEndpoints();
        app.MapWeatherForecastEndpoints();

        app.Run();
    }
}