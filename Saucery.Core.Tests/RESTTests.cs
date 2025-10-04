using Saucery.Core.Dojo;
using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Apple;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.Linux;
using Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;
using Saucery.Core.RestAPI.FlowControl;
using Shouldly;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace Saucery.Core.Tests;

public class RestTests(PlatformConfiguratorFixture fixture) : IClassFixture<PlatformConfiguratorFixture>
{
    private readonly PlatformConfiguratorFixture _fixture = fixture;

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FlowControlTest(bool isRealDevice) {
        var flowController = new SauceLabsFlowController();
        flowController.ControlFlow(isRealDevice);
    }

    [Fact]
    public void SupportedRealDevicePlatformTest()
    {
        PlatformConfigurator configurator = new(PlatformFilter.RealDevice);
        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldBeEmpty();
        realDevices.ShouldNotBeNull();
    }

    [Fact]
    public void SupportedEmulatedPlatformTest()
    {
        PlatformConfigurator configurator = new(PlatformFilter.Emulated);

        var availablePlatforms = configurator.AvailablePlatforms;
        var realDevices = configurator.AvailableRealDevices;

        availablePlatforms.ShouldNotBeNull();
        realDevices.ShouldBeEmpty();
    }

    [Theory]
    [MemberData(nameof(SupportedPlatformTypes))]
    public void SupportedPlatformTheory(Type platformType) {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        availablePlatforms.ShouldNotBeNull();

        var platformList = InvokeGetPlatformGeneric(availablePlatforms, platformType);

        // Null Check
        platformList.ShouldNotBeNull();

        // Count Checks
        platformList.Count.ShouldBe(1);

        // TypeOf Checks: List<T>
        platformList.GetType().ShouldBe(typeof(List<>).MakeGenericType(platformType));
    }

    public static IEnumerable<object[]> SupportedPlatformTypes =>
        new[]
        {
            typeof(LinuxPlatform),
            typeof(Windows11Platform),
            typeof(Windows10Platform),
            typeof(Windows81Platform),
            typeof(Windows8Platform),
            typeof(Mac15Platform),
            typeof(Mac14Platform),
            typeof(Mac13Platform),
            typeof(Mac12Platform),
            typeof(Mac11Platform),
            typeof(IOS175Platform),
            typeof(IOS17Platform),
            typeof(IOS162Platform),
            typeof(IOS161Platform),
            typeof(IOS16Platform),
            typeof(IOS155Platform),
            typeof(IOS154Platform),
            typeof(IOS152Platform),
            typeof(IOS15Platform),
            typeof(IOS145Platform),
            typeof(IOS144Platform),
            typeof(IOS143Platform),
            typeof(IOS14Platform),
            typeof(Android16Platform),
            typeof(Android15Platform),
            typeof(Android14Platform),
            typeof(Android13Platform),
            typeof(Android12Platform),
            typeof(Android11Platform),
            typeof(Android10Platform),
            typeof(Android9Platform),
            typeof(Android81Platform),
            typeof(Android8Platform),
            typeof(Android71Platform),
            typeof(Android7Platform),
            typeof(Android6Platform),
            typeof(Android51Platform),
        }.Select(t => new object[] { t });


    [Theory]
    [MemberData(nameof(SupportedRealDeviceTypes))]
    public void SupportedRealDeviceTheory(Type realDeviceType) {
        var realDevices = _fixture.PlatformConfigurator!.AvailableRealDevices;
        
        realDevices.ShouldNotBeNull();

        var platformList = InvokeGetPlatformGeneric(realDevices, realDeviceType);

        //Null Check
        platformList.ShouldNotBeNull();

        //Count Checks
        platformList.Count.ShouldBe(1);

        //TypeOf Checks
        platformList.GetType().ShouldBe(typeof(List<>).MakeGenericType(realDeviceType));
    }

    public static IEnumerable<object[]> SupportedRealDeviceTypes =>
        new[]
        {
            typeof(IOS26Platform),
            typeof(IOS18Platform),
            typeof(IOS17Platform),
            typeof(IOS16Platform),
            typeof(IOS15Platform),
            typeof(IOS14Platform),
            typeof(IOS13Platform),
            typeof(Android16Platform),
            typeof(Android15Platform),
            typeof(Android14Platform),
            typeof(Android13Platform),
            typeof(Android12Platform),
            typeof(Android11Platform),
            typeof(Android10Platform),
            typeof(Android9Platform)
        }.Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(PlatformsWithBrowsersTypes))]
    public void BrowserCountTest(Type platformsWithBrowsersType)
    {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        var platformList = InvokeGetPlatformGeneric(availablePlatforms, platformsWithBrowsersType);

        //Browser Count Checks
        var first = (PlatformBase)platformList[0]!;
        first.Browsers.Count.ShouldBeEquivalentTo(first.Selenium4BrowserNames!.Count);
    }

    public static IEnumerable<object[]> PlatformsWithBrowsersTypes =>
        new[]
        {
            typeof(LinuxPlatform),
            typeof(Windows11Platform),
            typeof(Windows10Platform),
            typeof(Windows81Platform),
            typeof(Windows8Platform),
            typeof(Mac15Platform),
            typeof(Mac14Platform),
            typeof(Mac13Platform),
            typeof(Mac12Platform),
            typeof(Mac11Platform)
        }.Select(t => new object[] { t });

    private static IList InvokeGetPlatformGeneric(object groups, Type platformType) {
        var groupsType = groups.GetType();

        // 1) Try an instance generic method named GetPlatform<T>() with no parameters
        var inst = groupsType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m =>
                m.Name == "GetPlatform" &&
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters().Length == 0);

        if(inst is not null) {
            return (IList)inst.MakeGenericMethod(platformType).Invoke(groups, null)!;
        }

        // 2) Otherwise, locate an extension method: public static List<T> GetPlatform<T>(this X groups)
        var ext = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                   from type in GetLoadableTypes(asm)
                   where type.IsSealed && type.IsAbstract // static class
                   from m in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                   where m.Name == "GetPlatform"
                         && m.IsGenericMethodDefinition
                         && m.GetGenericArguments().Length == 1
                         && m.GetCustomAttributes(typeof(ExtensionAttribute), false).Any()
                   let pars = m.GetParameters()
                   where pars.Length == 1 && ParameterAccepts(pars[0].ParameterType, groupsType)
                   select m).FirstOrDefault()
                   ?? throw new InvalidOperationException($"Cannot find GetPlatform<T> as instance or extension for receiver type {groupsType.FullName}.");
        var gm = ext.MakeGenericMethod(platformType);
        
        return (IList)gm.Invoke(null, [groups])!;
    }

    private static bool ParameterAccepts(Type paramType, Type actual) =>
        paramType.IsAssignableFrom(actual) ||
        actual.GetInterfaces().Any(paramType.IsAssignableFrom);

    private static IEnumerable<Type> GetLoadableTypes(Assembly asm) {
        try {
            return asm.GetTypes();
        } catch(ReflectionTypeLoadException ex) {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}