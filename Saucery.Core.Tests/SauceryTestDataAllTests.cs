using Saucery.Core.DataSources;
using Saucery.Core.Tests.DataProviders;
using Saucery.Core.Tests.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests;

public class SauceryTestDataAllTests() : SauceryTestData {
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) {
        // This will ensure the fixture is created
        _fixture = new PlatformConfiguratorAllFixture();
    }

    [Test]
    public void AllPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }
}
