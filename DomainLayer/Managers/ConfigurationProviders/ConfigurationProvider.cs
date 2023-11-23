using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace DomainLayer;

internal sealed class ConfigurationProvider : ConfigurationProviderBase
{
    private readonly IConfigurationRoot _configurationRoot;

    public ConfigurationProvider()
    {
        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddJsonFile("appsettings.json");

        LoadEnvironmentSpecificAppSettings(configurationBuilder);

        _configurationRoot = configurationBuilder.Build();
    }

    [ExcludeFromCodeCoverage]
    internal ConfigurationProvider(IConfigurationRoot configurationRoot) => _configurationRoot = configurationRoot;

    private static void LoadEnvironmentSpecificAppSettings(ConfigurationBuilder configurationBuilder)
    {
        var aspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var environmentBasedSettingsFile = $"appsettings.{aspNetCoreEnvironment}.json";
        if (File.Exists(environmentBasedSettingsFile))
        {
            configurationBuilder.AddJsonFile(environmentBasedSettingsFile);
        }
    }

    protected override string? RetrieveConfigurationSettingValue(string key)
    {
        return _configurationRoot["AppSettings:" + key];
    }
}
