using Saucery.Core.Util;

namespace Saucery.Core.Tests.Util;

[NotInParallel]
public class EnviroTests : IDisposable
{
    private readonly string? _originalSauceryBuildName;
    private readonly string? _originalSauceryTestFramework;
    private readonly string? _originalBuildNumber;

    public EnviroTests()
    {
        _originalSauceryBuildName = Environment.GetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME);
        _originalSauceryTestFramework = Environment.GetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK);
        _originalBuildNumber = Environment.GetEnvironmentVariable(SauceryConstants.BUILD_NUMBER);

        ClearBuildNameEnvironment();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, _originalSauceryBuildName);
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, _originalSauceryTestFramework);
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, _originalBuildNumber);

        GC.SuppressFinalize(this);
    }

    [Test]
    public async Task BuildName_WhenSauceryBuildNameIsSet_ReturnsExplicitBuildName()
    {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, "Desktop_Custom_Build");
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, "TUnit");
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        await Assert.That(buildName).IsEqualTo("Desktop_Custom_Build");
    }

    [Test]
    public async Task BuildName_WhenFrameworkAndBuildNumberAreSet_ReturnsFrameworkSpecificBuildName()
    {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, "TUnit");
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        await Assert.That(buildName).IsEqualTo("Desktop_TUnit_12345");
    }

    [Test]
    public async Task BuildName_WhenOnlyBuildNumberIsSet_ReturnsOriginalDesktopBuildName()
    {
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, "12345");

        var buildName = Enviro.BuildName;

        await Assert.That(buildName).IsEqualTo("Desktop_12345");
    }

    [Test]
    public async Task BuildName_WhenNoEnvironmentValuesAreSet_ReturnsDesktopWithGeneratedId()
    {
        var buildName = Enviro.BuildName;

        await Assert.That(buildName).StartsWith("Desktop_");
        await Assert.That(buildName.Length).IsGreaterThan("Desktop_".Length);
    }

    private static void ClearBuildNameEnvironment()
    {
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_BUILD_NAME, null);
        Environment.SetEnvironmentVariable(SauceryConstants.SAUCERY_TEST_FRAMEWORK, null);
        Environment.SetEnvironmentVariable(SauceryConstants.BUILD_NUMBER, null);
    }
}