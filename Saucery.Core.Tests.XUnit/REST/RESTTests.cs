using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.Tests.XUnit.Fixtures;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using Xunit;

namespace Saucery.Core.Tests.XUnit.REST;

public class RestTests(PlatformConfiguratorAllFixture fixture) : IClassFixture<PlatformConfiguratorAllFixture>
{
    private readonly PlatformConfiguratorAllFixture _fixture = fixture;

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task FlowControlTest(bool isRealDevice) {
        var flowController = new SauceLabsFlowController();
        await flowController.ControlFlowAsync(isRealDevice);
    }

    [Theory]
    [MemberData(nameof(PlatformTypes.SupportedPlatformTypes), MemberType = typeof(PlatformTypes))]
    public void SupportedPlatformTheory(Type platformType) {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        Assert.NotNull(availablePlatforms);

        var platformList = InvokeGetPlatformGeneric(availablePlatforms, platformType);

        Assert.NotNull(platformList);
        Assert.Equal(1, platformList.Count);
        Assert.Equal(typeof(List<>).MakeGenericType(platformType), platformList.GetType());
    }

    [Theory]
    [MemberData(nameof(PlatformTypes.SupportedRealDeviceTypes), MemberType = typeof(PlatformTypes))]
    public void SupportedRealDeviceTheory(Type realDeviceType) {
        var realDevices = _fixture.PlatformConfigurator!.AvailableRealDevices;
        Assert.NotNull(realDevices);

        var platformList = InvokeGetPlatformGeneric(realDevices, realDeviceType);

        Assert.NotNull(platformList);
        Assert.Equal(1, platformList.Count);
        Assert.Equal(typeof(List<>).MakeGenericType(realDeviceType), platformList.GetType());
    }

    [Theory]
    [MemberData(nameof(PlatformTypes.PlatformsWithBrowsersTypes), MemberType = typeof(PlatformTypes))]
    public void BrowserCountTest(Type platformsWithBrowsersType)
    {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        var platformList = InvokeGetPlatformGeneric(availablePlatforms, platformsWithBrowsersType);

        var first = (PlatformBase)platformList[0]!;
        Assert.Equal(first.Browsers.Count, first.Selenium4BrowserNames!.Count);
    }

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

        // 2) Otherwise, locate an extension method
        var ext = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                   from type in GetLoadableTypes(asm)
                   where type.IsSealed && type.IsAbstract
                   from m in type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                   where m.Name == "GetPlatform"
                         && m.IsGenericMethodDefinition
                         && m.GetGenericArguments().Length == 1
                         && m.GetCustomAttributes(typeof(ExtensionAttribute), false).Length != 0
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