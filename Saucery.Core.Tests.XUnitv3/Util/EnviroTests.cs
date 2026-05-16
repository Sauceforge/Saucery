using Saucery.Core.Util;
using Xunit;

namespace Saucery.Core.Tests.XUnitv3.Util;

public sealed class EnviroTests : IDisposable {
    private readonly string? _originalSauceryBuildName;
    private readonly string? _originalSauceryTestFramework;
    private readonly string? _originalBuildNumber;

    public EnviroTests() {
        _originalSauceryBuildName = Environment.GetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME);
        _originalSauceryTestFramework = Environment.GetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK);
        _originalBuildNumber = Environment.GetEnvironmentVariable(SauceryConstants.BUILD_NUMBER);

        ClearBuildNameEnvironment();
    }

    public void Dispose() {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, _originalSauceryBuildName);
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, _originalSauceryTestFramework);
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, _originalBuildNumber);

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void BuildName_WhenSauceryBuildNameIsSet_ReturnsExplicitBuildName() {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, "Desktop_Custom_Build");
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, "XUnit3");
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        Assert.Equal("Desktop_Custom_Build", buildName);
    }

    [Fact]
    public void BuildName_WhenFrameworkAndBuildNumberAreSet_ReturnsFrameworkSpecificBuildName() {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, "XUnit3");
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        Assert.Equal("Desktop_XUnit3_12345", buildName);
    }

    [Fact]
    public void BuildName_WhenOnlyBuildNumberIsSet_ReturnsOriginalDesktopBuildName() {
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        Assert.Equal("Desktop_12345", buildName);
    }

    [Fact]
    public void BuildName_WhenNoEnvironmentValuesAreSet_ReturnsDesktopWithGeneratedId() {
        var buildName = Enviro.BuildName;

        Assert.StartsWith("Desktop_", buildName);
        Assert.True(buildName.Length > "Desktop_".Length);
    }

    private static void ClearBuildNameEnvironment() {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, null);
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, null);
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, null);
    }
}