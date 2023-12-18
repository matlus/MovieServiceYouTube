using System.Collections.Generic;
using DomainLayer;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Testing.Shared;

namespace ClassTests;

[TestClass]
public class ConfigurationProviderTests
{
    private const string ImdbServiceBaseUrlKey = "ImdbServiceBaseUrl";

    private static IConfigurationRoot CreateConfigurationRoot(string key, string value)
    {
        var appSettingsDictionary = new Dictionary<string, string?>()
        {
            { "AppSettings:" + key, value },
        };

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.AddInMemoryCollection(appSettingsDictionary);
        return configurationBuilder.Build();
    }

    private static ConfigurationProviderBase CreateConfigurationProvider(string key, string value)
    {
        var configurationRoot = CreateConfigurationRoot(key, value);
        return new DomainLayer.ConfigurationProvider(configurationRoot);
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public void GetImdbServiceBaseUrl_WhenValueExistsWithEndingForwardSlash_ShouldReturnValueAsIs()
    {
        // Arrange
        var expectedImdbServiceBaseUrl = "http://www.geico.com/";
        var configurationProvider = CreateConfigurationProvider(ImdbServiceBaseUrlKey, expectedImdbServiceBaseUrl);

        // Act
        var actualImdbServiceBaseUrl = configurationProvider.GetImdbServiceBaseUrl();

        // Assert
        Assert.AreEqual(expectedImdbServiceBaseUrl, actualImdbServiceBaseUrl, $"We were expecting the actualImdbServiceBaseUrl to be: {expectedImdbServiceBaseUrl}, but it was: {actualImdbServiceBaseUrl}");
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public void GetImdbServiceBaseUrl_WhenValueExistsWithoutEndingForwardSlash_ShouldReturnValueWithEndingForwardSlash()
    {
        // Arrange
        var someValidUrl = "http://www.geico.com";
        var expectedImdbServiceBaseUrl = someValidUrl + "/";
        var configurationProvider = CreateConfigurationProvider(ImdbServiceBaseUrlKey, someValidUrl);

        // Act
        var actualImdbServiceBaseUrl = configurationProvider.GetImdbServiceBaseUrl();

        // Assert
        Assert.AreEqual(expectedImdbServiceBaseUrl, actualImdbServiceBaseUrl, $"We were expecting the actualImdbServiceBaseUrl to be: {expectedImdbServiceBaseUrl}, but it was: {actualImdbServiceBaseUrl}");
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public void GetImdbServiceBaseUrl_WhenValueIsEmpty_ShouldThrow()
    {
        // Arrange
        var emptyImdbServiceBaseUrl = string.Empty;
        var configurationProvider = CreateConfigurationProvider(ImdbServiceBaseUrlKey, emptyImdbServiceBaseUrl);

        // Act
        try
        {
            configurationProvider.GetImdbServiceBaseUrl();
            Assert.Fail("We were expecting an exception of type ConfigurationSettingValueEmptyException to be thrown, but no exception was thrown");
        }
        catch (ConfigurationSettingValueEmptyException e)
        {
            // Assert
            StringAssert.Contains(e.Message, "Key: ImdbServiceBaseUrl, Exists but its value is Empty");
        }
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public void GetImdbServiceBaseUrl_WhenSettingIsMissing_ShouldThrow()
    {
        // Arrange
        var configurationProvider = CreateConfigurationProvider("IrrelevantKey", "irrelevantValue");

        // Act
        try
        {
            configurationProvider.GetImdbServiceBaseUrl();
            Assert.Fail("We were expecting an exception of type ConfigurationSettingMissingException to be thrown, but no exception was thrown");
        }
        catch (ConfigurationSettingMissingException e)
        {
            // Assert
            StringAssert.Contains(e.Message, "Key: ImdbServiceBaseUrl, is Missing");
        }
    }

    [TestMethod]
    [TestCategory("Class Test")]
    public void GetImdbServiceBaseUrl_WhenSettingIsWhitespaces_ShouldThrow()
    {
        // Arrange
        var whitespaces = "  \t \n       ";
        var configurationProvider = CreateConfigurationProvider(ImdbServiceBaseUrlKey, whitespaces);

        // Act
        try
        {
            configurationProvider.GetImdbServiceBaseUrl();
            Assert.Fail("We were expecting an exception of type ConfigurationSettingValueEmptyException to be thrown, but no exception was thrown");
        }
        catch (ConfigurationSettingValueEmptyException e)
        {
            // Assert
            AssertEx.EnsureExceptionMessageContains(e, "Key: ImdbServiceBaseUrl", "is White spaces");
        }
    }
}
