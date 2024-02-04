using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Contract;
using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Git;
using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Outputs;
using ChristianSchulz.MultitenancyMonolith.Tools.PreBuild.Serializations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;

namespace ChristianSchulz.MultitenancyMonolith.Tools.PreBuild;

public sealed class Program
{
    static async Task Main(string[] args)
    {
        await ConfigureServices(args)
            .GetRequiredService<PreBuildService>()
            .RunAsync();
    }

    private static IConfigurationRoot ConfigureConfiguration(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(setup =>
            {
                setup.Args = args;
                setup.SwitchMappings = new Dictionary<string, string>
                {
                    ["--source-directory"] = "Settings:SourceDirectoryPath",
                    ["--output-directory"] = "Settings:OutputDirectoryPath",
                    ["--output-filename"] = "Settings:OutputFilenameFormat",
                    ["--output-format"] = "Settings:OutputFormat",
                    ["--branch-name"] = "Settings:BranchName",
                    ["--commit-hash"] = "Settings:CommitHash",
                    ["--short-commit-hash"] = "Settings:ShortCommitHash",
                    ["--build-number"] = "Settings:BuildNumber"
                };
            })
            .AddEnvironmentVariables("PreBuild__")
#if DEBUG
            .AddJsonFile("appsettings.Debug.json")
#endif
            .Build();

        return configuration;
    }

    private static PreBuildSettings ResolveSettings(string[] args)
    {
        var settings = ConfigureConfiguration(args)
            .GetSection("Settings")
            .Get<PreBuildSettings>() ?? new PreBuildSettings();

        Validator.ValidateObject(settings, new ValidationContext(settings), true);

        return settings;
    }

    private static ServiceProvider ConfigureServices(string[] args)
    {
        var settings = ResolveSettings(args);

        var services = new ServiceCollection()

            .AddGitClient(setup =>
            {
                setup.RepositoryPath = settings.SourceDirectoryPath ?? Directory.GetCurrentDirectory();
            })

            .AddPreBuildSerialization(setup =>
            {
                setup.SerializationClientType = settings.OutputFormat switch
                {
                    "json" => typeof(JsonSerializationClient),
                    "values" => typeof(KeyPerValueSerializationClient),

                    _ => null!
                };
            })
            .AddPreBuildSerializationClient<JsonSerializationClient>()
            .AddPreBuildSerializationClient<KeyPerValueSerializationClient>()

            .AddPreBuildOutputs(setup =>
            {
                setup.OutputType = settings.OutputDirectoryPath != null
                    ? typeof(FileOutput)
                    : settings.OutputFormat switch
                    {
                        "json" => typeof(RawConsoleOutput),
                        "values" => typeof(KeyValuePairConsoleOutput),

                        _ => null!
                    };
            })
            .AddPreBuildOutput<KeyValuePairConsoleOutput>()
            .AddPreBuildOutput<RawConsoleOutput>()
            .AddPreBuildOutput<FileOutput, FileOutputOptions>(setup =>
            {
                setup.OutputDirectoryPath = settings.OutputDirectoryPath ?? string.Empty;
                setup.OutputFilenameFormat = settings.OutputFilenameFormat ?? "{0}";
            })

            .AddSingleton(settings)
            .AddSingleton<PreBuildService>()
            .BuildServiceProvider();

        return services;
    }
}