using NUnit.Framework;
using Saucery.Core.DataSources;
using Saucery.Core.Dojo;
using Saucery.Core.Tests.DataProviders;
using Shouldly;

namespace Saucery.Core.Tests;

[TestFixture]
public class SauceryTestDataTests : SauceryTestData {
    [Test]
    public void AllPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(34); //Due to platform expansion.
        GetAllPlatforms().Count().ShouldBeEquivalentTo(34);
    }

    [Test]
    public void DesktopPlatformsTest() {
        SetPlatforms(PlatformDataClass.DesktopPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(34); //Due to platform expansion.
        GetAllPlatforms().Count().ShouldBeEquivalentTo(34);
    }

    [Test]
    public void RealAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.RealAndroidDevices, PlatformFilter.RealDevice);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(PlatformDataClass.RealAndroidDevices.Count);
        GetAllPlatforms().Count().ShouldBeEquivalentTo(PlatformDataClass.RealAndroidDevices.Count);
    }

    [Test]
    public void RealAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.RealIOSDevices, PlatformFilter.RealDevice);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(PlatformDataClass.RealIOSDevices.Count);
        GetAllPlatforms().Count().ShouldBeEquivalentTo(PlatformDataClass.RealIOSDevices.Count);
    }

    [Test]
    public void EmulatedAndroidDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedAndroidPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(PlatformDataClass.EmulatedAndroidPlatforms.Count);
        GetAllPlatforms().Count().ShouldBeEquivalentTo(PlatformDataClass.EmulatedAndroidPlatforms.Count);
    }

    [Test]
    public void EmulatedAppleDeviceTest() {
        SetPlatforms(PlatformDataClass.EmulatedIOSPlatforms, PlatformFilter.Emulated);

        Items.ShouldNotBeNull();
        Items.Count().ShouldBeEquivalentTo(PlatformDataClass.EmulatedIOSPlatforms.Count);
        GetAllPlatforms().Count().ShouldBeEquivalentTo(PlatformDataClass.EmulatedIOSPlatforms.Count);
    }
}