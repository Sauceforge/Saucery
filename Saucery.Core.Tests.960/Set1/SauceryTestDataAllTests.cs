using DataProviders;
using Fixtures;
using Saucery.Core.DataSources;
using Shouldly;

namespace Saucery.Core.Tests;

public class SauceryTestDataAllTests() : SauceryTestData {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    public void AllPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }
}
