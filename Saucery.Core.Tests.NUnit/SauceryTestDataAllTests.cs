using NUnit.Framework;
using Saucery.Core.DataSources;
using Saucery.Core.Tests.NUnit.DataProviders;
using Saucery.Core.Tests.NUnit.Fixtures;
using Shouldly;

namespace Saucery.Core.Tests.NUnit;

public class SauceryTestDataAllTests : SauceryTestData
{
    private PlatformConfiguratorAllFixture _fixture = null!;

    [OneTimeSetUp]
    public void SetupFixture() => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    public void AllPlatformsTest()
    {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, _fixture.PlatformConfigurator);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBe(34); // Due to platform expansion.
    }
}
