using Saucery.Core.DataSources;
using Saucery.Core.Tests.XUnitv3.DataProviders;
using Saucery.Core.Tests.XUnitv3.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnitv3;

public class SauceryTestDataAllTests(PlatformConfiguratorAllFixture fixture) : SauceryTestData, IClassFixture<PlatformConfiguratorAllFixture> 
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Fact]
    public void AllPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }
}
