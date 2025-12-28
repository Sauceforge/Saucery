using Microsoft.Extensions.Logging;
using Microsoft.TemplateEngine.Authoring.TemplateVerifier;
using Shouldly;

namespace Template.Tests;

public class TemplateTests {
    private static readonly string _nunitTemplateFolder = GetFolderPath("NUnit");
    private static readonly string _xunitTemplateFolder = GetFolderPath("XUnit");
    private static readonly string _tunitTemplateFolder = GetFolderPath("TUnit");
    private static readonly string _xunit3TemplateFolder = GetFolderPath("XUnit3");

    [Test]
    [Category("NUnit")]
    public async Task GeneratesExpectedNUnitProject() {
        var outputDir = CreateTempOutputDir();
        try {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var engine = new VerificationEngine(loggerFactory);

            var options = GetVerifierOptions("saucery-nunit", _nunitTemplateFolder, outputDir);

            await engine.Execute(options);

            AssertFiles([
                [options.OutputDirectory!, "saucery-nunit", "MyTestProject.csproj"],
                [options.OutputDirectory!, "saucery-nunit", "NuGetIntegrationTests.cs"],
                [options.OutputDirectory!, "saucery-nunit", "RequestedPlatformData.cs"],
                [options.OutputDirectory!, "saucery-nunit", "PageObjects", "GuineaPigPage.cs"]
            ]);
        } finally {
            CleanupOutputDir(outputDir);
        }
    }

    [Test]
    [Category("XUnit")]
    public async Task GeneratesExpectedXUnitProject() {
        var outputDir = CreateTempOutputDir();
        try {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var engine = new VerificationEngine(loggerFactory);

            var options = GetVerifierOptions("saucery-xunit", _xunitTemplateFolder, outputDir);

            await engine.Execute(options);

            AssertFiles([
                [options.OutputDirectory!, "saucery-xunit", "MyTestProject.csproj"],
                [options.OutputDirectory!, "saucery-xunit", "ClickLinkTests.cs"],
                [options.OutputDirectory!, "saucery-xunit", "DataDrivenTests.cs"],
                [options.OutputDirectory!, "saucery-xunit", "RequestedPlatformData.cs"],
                [options.OutputDirectory!, "saucery-xunit", "Usings.cs"],
                [options.OutputDirectory!, "saucery-xunit", "PageObjects", "GuineaPigPage.cs"]
            ]);
        } finally {
            CleanupOutputDir(outputDir);
        }
    }

    [Test]
    [Category("TUnit")]
    public async Task GeneratesExpectedTUnitProject() {
        var outputDir = CreateTempOutputDir();
        try {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var engine = new VerificationEngine(loggerFactory);

            var options = GetVerifierOptions("saucery-tunit", _tunitTemplateFolder, outputDir);

            await engine.Execute(options);

            AssertFiles([
                [options.OutputDirectory!, "saucery-tunit", "PageObjects", "GuineaPigPage.cs"],
                [options.OutputDirectory!, "saucery-tunit", "ClickLinkTests.cs"],
                [options.OutputDirectory!, "saucery-tunit", "DataDrivenTests.cs"],
                [options.OutputDirectory!, "saucery-tunit", "MyParallelLimit.cs"],
                [options.OutputDirectory!, "saucery-tunit", "MyTestProject.csproj"],
                [options.OutputDirectory!, "saucery-tunit", "RequestedPlatformData.cs"]
            ]);
        } finally {
            CleanupOutputDir(outputDir);
        }
    }

    [Test]
    [Category("XUnit3")]
    public async Task GeneratesExpectedXUnit3Project() {
        var outputDir = CreateTempOutputDir();
        try {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var engine = new VerificationEngine(loggerFactory);

            var options = GetVerifierOptions("saucery-xunit3", _xunit3TemplateFolder, outputDir);

            await engine.Execute(options);

            AssertFiles([
                [options.OutputDirectory!, "saucery-xunit3", "PageObjects", "GuineaPigPage.cs"],
                [options.OutputDirectory!, "saucery-xunit3", "ClickLinkTests.cs"],
                [options.OutputDirectory!, "saucery-xunit3", "DataDrivenTests.cs"],
                [options.OutputDirectory!, "saucery-xunit3", "MyTestProject.csproj"],
                [options.OutputDirectory!, "saucery-xunit3", "RequestedPlatformData.cs"],
                [options.OutputDirectory!, "saucery-xunit3", "Usings.cs"],
                [options.OutputDirectory!, "saucery-xunit3", "xunit.runner.json"]
            ]);
        } finally {
            CleanupOutputDir(outputDir);
        }
    }

    private static string GetFolderPath(string templateName)
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Templates", templateName));

    private static TemplateVerifierOptions GetVerifierOptions(string templateName, string templateFolder, string outputDir)
        => new(templateName) {
            TemplatePath = templateFolder,
            OutputDirectory = outputDir,
            TemplateSpecificArgs = ["--name", "MyTestProject"],
            DisableDiffTool = true
        };

    private static void AssertFiles(string[][] fileSet) {
        foreach(var file in fileSet) {
            var filePath = Path.Combine(file);
            File.Exists(filePath).ShouldBeTrue($"Expected file to exist: {filePath}");
        }
    }

    private static string CreateTempOutputDir() {
        var dir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(dir);
        return dir;
    }

    private static void CleanupOutputDir(string? dir) {
        if (string.IsNullOrEmpty(dir))
            return;

        try {
            if (Directory.Exists(dir)) {
                Directory.Delete(dir, recursive: true);
            }
        } catch {
            // best-effort cleanup; tests should not fail on cleanup error
        }
    }
}