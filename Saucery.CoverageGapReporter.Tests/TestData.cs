namespace Saucery.CoverageGapReporter.Tests;

/// <summary>Sauce catalogue rows and a throwaway Saucery.Core source folder for reporter tests.</summary>
internal static class TestData
{
    public static SauceVirtualRow IOSSimulator(string version, string device = "iPhone Simulator") =>
        new("appium", "iphone", device, device, "iOS", version, version, "2.0");

    public static SauceVirtualRow AndroidEmulator(string version, string device = "Google Pixel 9 Emulator") =>
        new("appium", "android", device, device, "Linux", version, version, "2.0");

    public static SauceVirtualRow Desktop(string os, string browser) =>
        new("webdriver", browser, browser, string.Empty, os, "latest", "latest", string.Empty);

    public static SauceRealDevice RealIOS(string osVersion, string name = "iPhone 15") =>
        new(name.Replace(' ', '_'), name, "IOS", osVersion, "Apple");

    public static SauceRealDevice RealAndroid(string osVersion, string name = "Google Pixel 8") =>
        new(name.Replace(' ', '_'), name, "ANDROID", osVersion, "Google");
}

/// <summary>A temporary directory holding C# source, deleted on dispose.</summary>
internal sealed class SourceFolder : IDisposable
{
    public SourceFolder(params (string RelativePath, string Content)[] files)
    {
        Root = Path.Combine(Path.GetTempPath(), "saucery-reporter-tests", Guid.NewGuid().ToString("N"));
        foreach (var (relativePath, content) in files)
        {
            var path = Path.Combine(Root, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, content);
        }
    }

    public string Root { get; }

    public void Dispose()
    {
        if (Directory.Exists(Root))
            Directory.Delete(Root, recursive: true);
    }
}
