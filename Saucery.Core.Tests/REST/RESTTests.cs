using Saucery.Core.Dojo.Platforms.Base;
using Saucery.Core.RestAPI.FlowControl;
using Saucery.Core.Tests.Fixtures;
using Saucery.Core.Tests.REST.Data;
using Shouldly;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Saucery.Core.Tests.REST;

public class RestTests()
{
    private static PlatformConfiguratorAllFixture _fixture = null!;

    [Before(Class)]
    public static void SetupFixture(ClassHookContext context) => _fixture = new PlatformConfiguratorAllFixture();

    [Test]
    [Arguments(true)]
    [Arguments(false)]
    public void FlowControlTest(bool isRealDevice) {
        var flowController = new SauceLabsFlowController();
        flowController.ControlFlow(isRealDevice);
    }

    [Test]
    [MethodDataSource(typeof(PlatformTypes), nameof(PlatformTypes.SupportedPlatformTypes))]
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

    [Test]
    [MethodDataSource(typeof(PlatformTypes), nameof(PlatformTypes.SupportedRealDeviceTypes))]
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

    [Test]
    [MethodDataSource(typeof(PlatformTypes), nameof(PlatformTypes.PlatformsWithBrowsersTypes))]
    public void BrowserCountTest(Type platformsWithBrowsersType)
    {
        var availablePlatforms = _fixture.PlatformConfigurator!.AvailablePlatforms;
        var platformList = InvokeGetPlatformGeneric(availablePlatforms, platformsWithBrowsersType);

        //Browser Count Checks
        var first = (PlatformBase)platformList[0]!;
        first.Browsers.Count.ShouldBeEquivalentTo(first.Selenium4BrowserNames!.Count);
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

        // 2) Otherwise, locate an extension method: public static List<T> GetPlatform<T>(this X groups)
        var ext = (from asm in AppDomain.CurrentDomain.GetAssemblies()
                   from type in GetLoadableTypes(asm)
                   where type.IsSealed && type.IsAbstract // static class
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