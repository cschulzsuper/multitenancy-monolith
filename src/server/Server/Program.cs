using ChristianSchulz.MultitenancyMonolith.Application.Authentication;
using ChristianSchulz.MultitenancyMonolith.Application.Weather;
using ChristianSchulz.MultitenancyMonolith.Server.BadgeIdentity;
using ChristianSchulz.MultitenancyMonolith.Server.SwaggerGen;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChristianSchulz.MultitenancyMonolith.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthentication(setup =>
        {
            setup.DefaultScheme = "Badge";
            setup.AddScheme<BadgeAuthenticationHandler>("Badge", "Badge Authentication");
        });

        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new() { Title = "Multitenancy Monolith", Version = "v1" });

            options.ConfigureAuthentication();
            options.ConfigureAuthorization();
        });

        builder.Services.AddAuthenticationTransport();
        builder.Services.AddWeatherForecastTransport();

        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Multitenancy Monolith");
            });
        }

        app.UseHttpsRedirection();

        app.MapAuthenticationEndpoints();
        app.MapWeatherForecastEndpoints();

        app.Run();
    }
}