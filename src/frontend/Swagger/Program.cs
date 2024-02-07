using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.Swagger;

public sealed class Program
{

#if DEBUG
    private const bool _buildInfoOptional = true;
#else
    private const bool _buildInfoOptional = false;
#endif

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
                webBuilder.ConfigureAppConfiguration(c => c
                    .AddJsonFile("appsettings.BuildInfo.json", _buildInfoOptional)
                    .AddEnvironmentVariables("MM_Swagger_"));
            });
}