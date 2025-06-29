using Microsoft.Extensions.Logging;
using Microsoft.TemplateEngine.Authoring.TemplateVerifier;
using Shouldly;

namespace Template.Tests;

public class NUnitTemplateTests {

    private static readonly string TemplateFolder = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Templates", "NUnit"));

    [Fact]
    public async Task GeneratesExpectedProject() {
        var options = new TemplateVerifierOptions("saucery-nunit")
        {
            TemplatePath = TemplateFolder,
            //SnapshotsDirectory = Path.Combine(TestContext.CurrentContext.TestDirectory, "Snapshots"),
            OutputDirectory = Path.Combine(Path.GetTempPath(), "NUnit", Path.GetRandomFileName()),
            TemplateSpecificArgs = ["--name", "MyTestProject"],
            DisableDiffTool = true
        };

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var engine = new VerificationEngine(loggerFactory);

        await engine.Execute(options);

        // Assert that the project file and sample test file were created
        var projectFile = Path.Combine(options.OutputDirectory, "MyTestProject.csproj");
        var testFile = Path.Combine(options.OutputDirectory, "SampleTest.cs");

        File.Exists(projectFile).ShouldBeTrue("Expected project file to exist");
        File.Exists(testFile).ShouldBeTrue("Expected sample test to exist");
    }
}
