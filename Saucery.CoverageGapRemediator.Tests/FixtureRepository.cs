using System.Text.RegularExpressions;

namespace Saucery.CoverageGapRemediator.Tests;

/// <summary>
/// A miniature Saucery repository on disk with the same layout and declaration shapes as the
/// real one: simulators/emulators keyed by exact version, real devices by major version, desktop
/// platforms by SauceryConstants, and both Core test projects (TUnit and xUnit.v3), each in its
/// own row style. Deleted on dispose.
/// </summary>
internal sealed class FixtureRepository : IDisposable
{
    public const string TUnit = "Saucery.Core.Tests";
    public const string XUnit = "Saucery.Core.Tests.XUnitv3";
    public static readonly string[] TestProjects = [TUnit, XUnit];

    public FixtureRepository()
    {
        Root = Path.Combine(Path.GetTempPath(), "saucery-remediator-tests", Guid.NewGuid().ToString("N"));

        WriteCore();
        WriteTests(TUnit, xunit: false);
        WriteTests(XUnit, xunit: true);

        Context = new RepositoryContext(Root);
    }

    public string Root { get; }
    public RepositoryContext Context { get; }

    public ChangeCollector NewChanges() => new(Context, new RepositoryTransaction());

    /// <summary>File text with line endings normalized to \n.</summary>
    public string Read(string relativePath) =>
        File.ReadAllText(Path.Combine(Root, relativePath)).Replace("\r\n", "\n", StringComparison.Ordinal);

    public bool Exists(string relativePath) => File.Exists(Path.Combine(Root, relativePath));

    /// <summary>Lines of a static member, from its declaration to its terminating semicolon.</summary>
    public string Member(string relativePath, string memberName)
    {
        var lines = Read(relativePath).Split('\n');
        var start = Array.FindIndex(lines, l => Regex.IsMatch(l, $@"\b{memberName}\b") && Regex.IsMatch(l, @"\b(public|internal|private)\b"));
        if (start < 0)
            throw new InvalidOperationException($"{memberName} not found in {relativePath}.");

        var end = Array.FindIndex(lines, start + 1, l => l.TrimEnd().EndsWith(';'));
        return string.Join('\n', lines[start..(end + 1)]);
    }

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }

    private void Write(string relativePath, string content)
    {
        var path = Path.Combine(Root, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);
    }

    private void WriteCore()
    {
        Write("Saucery.Core/Util/SauceryConstants.cs", """
            namespace Saucery.Core.Util;

            public static class SauceryConstants
            {
                public const string PLATFORM_IOS = "iOS";
                public const string ANDROID = "Android";
                public const string PLATFORM_WINDOWS_10 = "Windows 10";
                public const string PLATFORM_WINDOWS_81 = "Windows 2012 R2";
                public const string PLATFORM_WINDOWS_8 = "Windows 2012";
                public const string PLATFORM_MAC_26 = "Mac 26";
                public const string BROWSER_CHROME = "chrome";
                public const string BROWSER_FIREFOX = "firefox";
                public const string BROWSER_IE = "internet explorer";
                public const string BROWSER_EDGE = "MicrosoftEdge";
                public const string BROWSER_EDGE_LOWER = "microsoftedge";
                public const string BROWSER_SAFARI = "safari";
                public const string SCREENRES_1024_768 = "1024x768";
                public const string SCREENRES_2048_1536 = "2048x1536";
                public const string DEVICE_ORIENTATION_LANDSCAPE = "landscape";
                public static readonly string BROWSER_VERSION_CURRENT_MAJOR = "current_major";
            }
            """);

        Write("Saucery.Core/Dojo/DojoExtensions.cs", """
            namespace Saucery.Core.Dojo;

            public static class DojoExtensions
            {
                public static PlatformBase? FindDesktopPlatform(this List<PlatformBase> platforms, string os) => os switch
                {
                    SauceryConstants.PLATFORM_WINDOWS_10 => platforms.GetPlatform<Windows10Platform>().FirstOrDefault(),
                    SauceryConstants.PLATFORM_MAC_26 => platforms.GetPlatform<Mac26Platform>().FirstOrDefault(),
                    _ => null
                };

                public static PlatformBase? FindAndroidBrowser(this List<PlatformBase> platforms, string label) => label switch
                {
                    "Linux 17.0" => platforms.GetPlatform<Android17Platform>().FirstOrDefault(),
                    "Linux 9.0" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
                    _ => null
                };

                public static PlatformBase? FindAndroidPlatform(this List<PlatformBase> platforms, string label) => label switch
                {
                    "Linux 17" => platforms.GetPlatform<Android17Platform>().FirstOrDefault(),
                    "Linux 9" => platforms.GetPlatform<Android9Platform>().FirstOrDefault(),
                    _ => null
                };

                public static PlatformBase? FindIOSBrowser(this List<PlatformBase> platforms, string label) => label switch
                {
                    "iOS 27.0" => platforms.GetPlatform<IOS27Platform>().FirstOrDefault(),
                    "iOS 16.2" => platforms.GetPlatform<IOS162Platform>().FirstOrDefault(),
                    "iOS 14.0" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                    _ => null
                };

                public static PlatformBase? FindIOSPlatform(this List<PlatformBase> platforms, string label) => label switch
                {
                    "iOS 27" => platforms.GetPlatform<IOS27Platform>().FirstOrDefault(),
                    "iOS 14" => platforms.GetPlatform<IOS14Platform>().FirstOrDefault(),
                    _ => null
                };
            }
            """);

        Write("Saucery.Core/Dojo/PlatformConfigurator.cs", """
            namespace Saucery.Core.Dojo;

            public class PlatformConfigurator
            {
                private static readonly string[] MacPlatforms =
                [
                    SauceryConstants.PLATFORM_MAC_26,
                ];
            }
            """);

        Write("Saucery.Core/Dojo/Platforms/ApplePlatformFactory.cs", """
            namespace Saucery.Core.Dojo.Platforms;

            public static class ApplePlatformFactory
            {
                public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.short_version switch
                {
                    "27.0" => new IOS27PlatformCreator(sp).Create(),
                    "16.2" => new IOS162PlatformCreator(sp).Create(),
                    "14.0" => new IOS14PlatformCreator(sp).Create(),
                    _ => null
                };

                public static PlatformBase? CreateRealPlatform(SupportedPlatform sp) => sp.OsVersion?.Split(".")[0] switch {
                    "27" => new IOS27PlatformCreator(sp).Create(),
                    "14" => new IOS14PlatformCreator(sp).Create(),
                    _ => null
                };
            }
            """);

        Write("Saucery.Core/Dojo/Platforms/AndroidPlatformFactory.cs", """
            namespace Saucery.Core.Dojo.Platforms;

            public static class AndroidPlatformFactory
            {
                public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.short_version switch
                {
                    "17.0" => new Android17PlatformCreator(sp).Create(),
                    "9.0" => new Android9PlatformCreator(sp).Create(),
                    _ => null
                };

                public static PlatformBase? CreateRealPlatform(SupportedPlatform sp) => sp.OsVersion?.Split(".")[0] switch {
                    "17" => new Android17PlatformCreator(sp).Create(),
                    "9" => new Android9PlatformCreator(sp).Create(),
                    _ => null
                };
            }
            """);

        Write("Saucery.Core/Dojo/Platforms/DesktopPlatformFactory.cs", """
            namespace Saucery.Core.Dojo.Platforms;

            public static class DesktopPlatformFactory {
                public static PlatformBase? CreatePlatform(SupportedPlatform sp) => sp.Os switch {
                    SauceryConstants.PLATFORM_WINDOWS_10 => new Windows10PlatformCreator(sp).Create(),
                    SauceryConstants.PLATFORM_MAC_26 => new Mac26PlatformCreator(sp).Create(),
                    _ => null
                };
            }
            """);

        Write("Saucery.Core/Dojo/Browsers/BrowserFactory.cs", """
            namespace Saucery.Core.Dojo.Browsers;

            public static class BrowserFactory
            {
                public static BrowserBase? Create(SupportedPlatform sp, List<string> screenResolutions, bool isArmRequired) => sp.Os switch
                {
                    SauceryConstants.PLATFORM_WINDOWS_10 => new Windows10BrowserCreator(sp).Create(SauceryConstants.PLATFORM_WINDOWS_10, screenResolutions, isArmRequired),
                    SauceryConstants.PLATFORM_MAC_26 => new Mac26BrowserCreator(sp).Create("macOS 26", screenResolutions, isArmRequired),
                    _ => null
                };
            }
            """);

        foreach (var (folder, type, arm) in new[]
                 {
                     ("Apple", "IOS27Platform", true),
                     ("Apple", "IOS162Platform", false),
                     ("Apple", "IOS14Platform", false)
                 })
        {
            Write($"Saucery.Core/Dojo/Platforms/ConcreteProducts/{folder}/{type}.cs", $$"""
                namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.{{folder}};

                public sealed class {{type}}(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.PLATFORM_IOS, []{{(arm ? ", null!, null!, true" : string.Empty)}})
                {
                }
                """);
        }

        foreach (var type in new[] { "Android17Platform", "Android9Platform" })
        {
            Write($"Saucery.Core/Dojo/Platforms/ConcreteProducts/Google/{type}.cs", $$"""
                namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.Google;

                public sealed class {{type}}(SupportedPlatform sp) : PlatformBase(sp, SauceryConstants.ANDROID, [])
                {
                }
                """);
        }

        Write("Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Windows10Platform.cs", """
            namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

            public sealed class Windows10Platform(SupportedPlatform sp)
                : PlatformBase(sp,
                               SauceryConstants.PLATFORM_WINDOWS_10,
                               [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_IE],
                               [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE],
                               [SauceryConstants.SCREENRES_1024_768,
                                SauceryConstants.SCREENRES_2048_1536])
            {
            }
            """);

        Write("Saucery.Core/Dojo/Platforms/ConcreteProducts/PC/Mac26Platform.cs", """
            namespace Saucery.Core.Dojo.Platforms.ConcreteProducts.PC;

            public sealed class Mac26Platform(SupportedPlatform sp)
                : PlatformBase(sp,
                               "macOS 26",
                               [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI],
                               [SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_FIREFOX, SauceryConstants.BROWSER_EDGE, SauceryConstants.BROWSER_SAFARI],
                               [SauceryConstants.SCREENRES_1024_768,
                                SauceryConstants.SCREENRES_2048_1536 ],
                               true)
            {
            }
            """);

        foreach (var (folder, type) in new[]
                 {
                     ("Apple", "IOS27Platform"), ("Apple", "IOS162Platform"), ("Apple", "IOS14Platform"),
                     ("Google", "Android17Platform"), ("Google", "Android9Platform"),
                     ("PC", "Windows10Platform"), ("PC", "Mac26Platform")
                 })
        {
            Write($"Saucery.Core/Dojo/Platforms/ConcreteCreators/{folder}/{type}Creator.cs", $$"""
                namespace Saucery.Core.Dojo.Platforms.ConcreteCreators.{{folder}};

                internal class {{type}}Creator(SupportedPlatform sp) : PlatformCreator(sp)
                {
                    public override PlatformBase Create() => new {{type}}(Platform);
                }
                """);
        }

        foreach (var (type, browsers) in new[]
                 {
                     ("Windows10BrowserCreator", new[] { ("BROWSER_CHROME", "ChromeBrowser"), ("BROWSER_EDGE", "EdgeBrowser"), ("BROWSER_FIREFOX", "FirefoxBrowser"), ("BROWSER_IE", "IEBrowser") }),
                     ("Mac26BrowserCreator", new[] { ("BROWSER_CHROME", "ChromeBrowser"), ("BROWSER_EDGE", "EdgeBrowser"), ("BROWSER_FIREFOX", "FirefoxBrowser"), ("BROWSER_SAFARI", "SafariBrowser") })
                 })
        {
            var arms = string.Join("\n", browsers.Select(b =>
                $"            SauceryConstants.{b.Item1} => new {b.Item2}(Platform, screenResolutions, platformNameForOption, isArmRequired),"));
            Write($"Saucery.Core/Dojo/Browsers/ConcreteCreators/PC/{type}.cs", $$"""
                namespace Saucery.Core.Dojo.Browsers.ConcreteCreators.PC;

                internal class {{type}}(SupportedPlatform sp) : BrowserCreator(sp)
                {
                    public override BrowserBase? Create(
                        string platformNameForOption,
                        List<string> screenResolutions,
                        bool isArmRequired = false) =>
                        Platform.api_name switch
                        {
                {{arms}}
                            _ => null,
                        };
                }
                """);
        }
    }

    private void WriteTests(string project, bool xunit)
    {
        // A row: TUnit lists bare instances, xUnit.v3 wraps each in an object array.
        string Row(string expression, bool last = false) =>
            (xunit ? $"[{expression}]" : expression) + (last ? string.Empty : ",");

        Write($"{project}/{project}.csproj", "<Project Sdk=\"Microsoft.NET.Sdk\" />");

        Write($"{project}/REST/PlatformTypes.cs", """
            public static class PlatformTypes
            {
                public static IEnumerable<object[]> SupportedPlatformTypes =>
                    Select(
                    [
                        typeof(Windows10Platform),
                        typeof(Mac26Platform),
                        typeof(IOS27Platform),
                        typeof(IOS162Platform),
                        typeof(IOS14Platform),
                        typeof(Android17Platform),
                        typeof(Android9Platform),
                    ]);

                public static IEnumerable<object[]> SupportedRealDeviceTypes =>
                    Select(
                    [
                        typeof(IOS27Platform),
                        typeof(IOS14Platform),
                        typeof(Android17Platform),
                        typeof(Android9Platform)
                    ]);

                public static IEnumerable<object[]> PlatformsWithBrowsersTypes =>
                    Select(
                    [
                        typeof(Windows10Platform),
                        typeof(Mac26Platform)
                    ]);

                private static IEnumerable<object[]> Select(Type[] types) => types.Select(t => new object[] { t });
            }
            """);

        Write($"{project}/IOSFactoryVersionTests.cs", """
            internal static class IOSDataClass
            {
                internal static readonly string[] IOSPlatformVersions =
                [
                    "14.0","14.3",
                    "16.0","16.2",
                ];
            }
            """);

        Write($"{project}/AndroidFactoryVersionTests.cs", $$"""
            public static class AndroidDataClass
            {
                public static IEnumerable<object> SupportedTestCases =>
                    [
                        {{Row("new AndroidPlatform(\"Google Pixel 9 Pro Emulator\", \"17.0\", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)")}}
                        {{Row("new AndroidPlatform(\"Google Pixel 3 Emulator\", \"9.0\", SauceryConstants.DEVICE_ORIENTATION_LANDSCAPE)", last: true)}}
                    ];
            }
            """);

        Write($"{project}/RealIOSFactoryVersionTests.cs", xunit
            ? """
              public class RealIOSDataClass
              {
                  public static IEnumerable<object?[]> SupportedTestCases => sourceArray.Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

                  public static IEnumerable<object?[]> NotSupportedTestCases =>
                      [[new IOSRealDevice("NonExistent", "14")]];

                  private static readonly string[] sourceArray = ["14", "17", "27"];
              }
              """
            : """
              public class RealIOSDataClass {
                  public static IEnumerable<object?[]> SupportedTestCases =>
                      new[] { "14", "17", "27" }
                          .Select(v => new object?[] { new IOSRealDevice("iPhone.*", v) });

                  public static IEnumerable<object?[]> NotSupportedTestCases
                      =>
                      [
                          [new IOSRealDevice("NonExistent", "14")]
                      ];
              }
              """);

        Write($"{project}/RealAndroidFactoryVersionTests.cs", $$"""
            public static class RealAndroidDataClass
            {
                public static IEnumerable<object> SupportedTestCases =>
                    [
                        {{Row("new AndroidRealDevice(\"Google Pixel 9\", \"17\")")}}
                        {{Row("new AndroidRealDevice(\"Samsung Galaxy Tab S3\", \"9\")", last: true)}}
                    ];

                public static IEnumerable<object> NotSupportedTestCases =>
                    [
                        {{Row("new AndroidRealDevice(\"NonExistent\", \"1\")", last: true)}}
                    ];
            }
            """);

        Write($"{project}/DataProviders/PlatformDataClass.cs", """
            public static class PlatformDataClass
            {
                public static List<SaucePlatform> RealIOSDevices =>
                [
                    new IOSRealDevice("iPhone 15 Pro Max", "27"),
                    new IOSRealDevice("iPhone 12 Pro Max", "14"),
                ];

                public static List<SaucePlatform> RealAndroidDevices =>
                [
                    new AndroidRealDevice("Google Pixel 9", "17"),
                    new AndroidRealDevice("Samsung Galaxy Tab S3", "9"),
                ];
            }
            """);

        Write($"{project}/DesktopFactoryVersionTests.cs", $$"""
            public static class DesktopDataClass
            {
                public static IEnumerable<object> SupportedTestCases =>
                    [
                        {{Row("new DesktopPlatform(SauceryConstants.PLATFORM_WINDOWS_10, SauceryConstants.BROWSER_CHROME, SauceryConstants.BROWSER_VERSION_CURRENT_MAJOR)")}}
                        {{Row("new DesktopPlatform(SauceryConstants.PLATFORM_MAC_26, SauceryConstants.BROWSER_SAFARI, \"26\")", last: true)}}
                    ];
            }
            """);
    }
}
