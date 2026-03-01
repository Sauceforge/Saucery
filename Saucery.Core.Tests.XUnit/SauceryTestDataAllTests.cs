using Saucery.Core.DataSources;
using Saucery.Core.Tests.XUnit.DataProviders;
using Saucery.Core.Tests.XUnit.Fixtures;
using Shouldly;
using Xunit;

namespace Saucery.Core.Tests.XUnit;

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
