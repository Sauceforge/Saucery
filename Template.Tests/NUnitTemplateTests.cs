using Microsoft.Extensions.Logging;
using Microsoft.TemplateEngine.Authoring.TemplateVerifier;
using Shouldly;

namespace Template.Tests;

public class NUnitTemplateTests : IAsyncLifetime {

    private static readonly string _templateFolder = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Templates", "NUnit"));

    private string? _outputDir;
    private string? _nUnitTemplateName;

    public Task InitializeAsync() {
        _outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _nUnitTemplateName = "saucery-nunit";
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GeneratesExpectedNUnitProject() {
        var options = new TemplateVerifierOptions(_nUnitTemplateName!)
        {
            TemplatePath = _templateFolder,
            OutputDirectory = _outputDir,
            TemplateSpecificArgs = ["--name", "MyTestProject"],
            DisableDiffTool = true
        };

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var engine = new VerificationEngine(loggerFactory);

        await engine.Execute(options);

        // Assert that the project file and sample test file were created
        var projectFile = Path.Combine(options.OutputDirectory!, _nUnitTemplateName!, "MyTestProject.csproj");
        var testFile1 = Path.Combine(options.OutputDirectory!, _nUnitTemplateName!, "NuGetIntegrationTests.cs");
        var testFile2 = Path.Combine(options.OutputDirectory!, _nUnitTemplateName!, "RequestedPlatformData.cs");
        var testFile3 = Path.Combine(options.OutputDirectory!, _nUnitTemplateName!, "PageObjects", "GuineaPigPage.cs");

        File.Exists(projectFile).ShouldBeTrue("Expected project file to exist");
        File.Exists(testFile1).ShouldBeTrue("Expected sample test to exist");
        File.Exists(testFile2).ShouldBeTrue("Expected sample test to exist");
        File.Exists(testFile3).ShouldBeTrue("Expected sample test to exist");
    }

    public Task DisposeAsync() {
        if(Directory.Exists(_outputDir)) {
            Directory.Delete(_outputDir, recursive: true);
        }

        return Task.CompletedTask;
    }
}
