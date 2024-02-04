using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ChristianSchulz.MultitenancyMonolith.Frontend.DevLog;

public sealed class Program
{
#if DEBUG
    private const bool _buildInfoOptions = true;
#else
    private const bool _buildInfoOptions = false;
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
                    .AddJsonFile("appsettings.BuildInfo.json", _buildInfoOptions)
                    .AddJsonFile("appsettings.SeedData.json", true)
                    .AddEnvironmentVariables("MM_DevLog_"));
            });
}