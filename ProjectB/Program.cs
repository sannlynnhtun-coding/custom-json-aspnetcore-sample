using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

const string projectName = "ProjectB";

var builder = Host.CreateApplicationBuilder(args);
var configPath = FindSharedConfigPath(builder.Environment.ContentRootPath);

builder.Configuration.AddJsonFile(configPath, optional: false, reloadOnChange: true);
builder.Services.Configure<SampleConfig>(
    builder.Configuration.GetRequiredSection("SampleConfig"));
builder.Services.AddTransient<SampleConfigService>();

using var host = builder.Build();

host.Services.GetRequiredService<SampleConfigService>().PrintConfig(projectName);

static string FindSharedConfigPath(string startPath)
{
    for (var directory = new DirectoryInfo(startPath); directory is not null; directory = directory.Parent)
    {
        var configPath = Path.Combine(directory.FullName, "Config", "custom-setting.json");

        if (File.Exists(configPath))
        {
            return configPath;
        }
    }

    throw new FileNotFoundException("Config/custom-setting.json was not found.");
}

public sealed class SampleConfig
{
    public string ApplicationName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int RetryCount { get; set; }
    public SampleApiConfig Api { get; set; } = new();
}

public sealed class SampleApiConfig
{
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; }
}

public sealed class SampleConfigService(IOptions<SampleConfig> options)
{
    private readonly SampleConfig _config = options.Value;

    public void PrintConfig(string projectName)
    {
        Console.WriteLine($"Project: {projectName}");
        Console.WriteLine($"Application: {_config.ApplicationName}");
        Console.WriteLine($"Enabled: {_config.IsEnabled}");
        Console.WriteLine($"Retry Count: {_config.RetryCount}");
        Console.WriteLine($"API Base URL: {_config.Api.BaseUrl}");
        Console.WriteLine($"API Timeout Seconds: {_config.Api.TimeoutSeconds}");
    }
}
