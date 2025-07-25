﻿using Microsoft.Extensions.Logging;
using Microsoft.TemplateEngine.Authoring.TemplateVerifier;
using Shouldly;

namespace Template.Tests;

public class TemplateTests : IAsyncLifetime {
    private VerificationEngine? _engine;
    private string? _outputDir;

    private string? _nunitTemplateName;
    private string? _xunitTemplateName;
    private string? _tunitTemplateName;
    private string? _xunit3TemplateName;
    private static readonly string _nunitTemplateFolder = GetFolderPath("NUnit");
    private static readonly string _xunitTemplateFolder = GetFolderPath("XUnit");
    private static readonly string _tunitTemplateFolder = GetFolderPath("TUnit");
    private static readonly string _xunit3TemplateFolder = GetFolderPath("XUnit3");

    public Task InitializeAsync() {
        _outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _nunitTemplateName = "saucery-nunit";
        _xunitTemplateName = "saucery-xunit";
        _tunitTemplateName = "saucery-tunit";
        _xunit3TemplateName = "saucery-xunit3";

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        _engine = new VerificationEngine(loggerFactory);

        return Task.CompletedTask;
    }

    [Fact]
    [Trait("Group", "NUnit")]
    public async Task GeneratesExpectedNUnitProject() {
        var options = GetVerifierOptions(_nunitTemplateName!, _nunitTemplateFolder);

        await _engine!.Execute(options);

        AssertFiles([
            [options.OutputDirectory!, _nunitTemplateName!, "MyTestProject.csproj"],
            [options.OutputDirectory!, _nunitTemplateName!, "NuGetIntegrationTests.cs"],
            [options.OutputDirectory!, _nunitTemplateName!, "RequestedPlatformData.cs"],
            [options.OutputDirectory!, _nunitTemplateName!, "PageObjects", "GuineaPigPage.cs"]
        ]);
    }

    [Fact]
    [Trait("Group", "XUnit")]
    public async Task GeneratesExpectedXUnitProject() {
        var options = GetVerifierOptions(_xunitTemplateName!, _xunitTemplateFolder);

        await _engine!.Execute(options);

        AssertFiles([
            [options.OutputDirectory!, _xunitTemplateName!, "MyTestProject.csproj"],
            [options.OutputDirectory!, _xunitTemplateName!, "ClickLinkTests.cs"],
            [options.OutputDirectory!, _xunitTemplateName!, "DataDrivenTests.cs"],
            [options.OutputDirectory!, _xunitTemplateName!, "RequestedPlatformData.cs"],
            [options.OutputDirectory!, _xunitTemplateName!, "Usings.cs"],
            [options.OutputDirectory!, _xunitTemplateName!, "PageObjects", "GuineaPigPage.cs"]
        ]);
    }

    [Fact]
    [Trait("Group", "TUnit")]
    public async Task GeneratesExpectedTUnitProject() {
        var options = GetVerifierOptions(_tunitTemplateName!, _tunitTemplateFolder);

        await _engine!.Execute(options);

        AssertFiles([
            [options.OutputDirectory!, _tunitTemplateName!, "PageObjects", "GuineaPigPage.cs"],
            [options.OutputDirectory!, _tunitTemplateName!, "ClickLinkTests.cs"],
            [options.OutputDirectory!, _tunitTemplateName!, "DataDrivenTests.cs"],
            [options.OutputDirectory!, _tunitTemplateName!, "MyParallelLimit.cs"],
            [options.OutputDirectory!, _tunitTemplateName!, "MyTestProject.csproj"],
            [options.OutputDirectory!, _tunitTemplateName!, "RequestedPlatformData.cs"]
        ]);
    }

    [Fact]
    [Trait("Group", "XUnit3")]
    public async Task GeneratesExpectedXUnit3Project() {
        var options = GetVerifierOptions(_xunit3TemplateName!, _xunit3TemplateFolder);

        await _engine!.Execute(options);

        AssertFiles([
            [options.OutputDirectory!, _xunit3TemplateName!, "PageObjects", "GuineaPigPage.cs"],
            [options.OutputDirectory!, _xunit3TemplateName!, "ClickLinkTests.cs"],
            [options.OutputDirectory!, _xunit3TemplateName!, "DataDrivenTests.cs"],
            [options.OutputDirectory!, _xunit3TemplateName!, "MyTestProject.csproj"],
            [options.OutputDirectory!, _xunit3TemplateName!, "RequestedPlatformData.cs"],
            [options.OutputDirectory!, _xunit3TemplateName!, "Usings.cs"],
            [options.OutputDirectory!, _xunit3TemplateName!, "xunit.runner.json"]
        ]);
    }

    public Task DisposeAsync() {
        if(Directory.Exists(_outputDir)) {
            Directory.Delete(_outputDir, recursive: true);
        }

        _engine = null!;

        return Task.CompletedTask;
    }

    private static string GetFolderPath(string templateName)
        => Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Templates", templateName));

    private TemplateVerifierOptions GetVerifierOptions(string templateName, string templateFolder)
        => new(templateName) {
            TemplatePath = templateFolder,
            OutputDirectory = _outputDir,
            TemplateSpecificArgs = ["--name", "MyTestProject"],
            DisableDiffTool = true
        };

    private static void AssertFiles(string[][] fileSet) {
        foreach(var file in fileSet) {
            var filePath = Path.Combine(file);
            File.Exists(filePath).ShouldBeTrue($"Expected file to exist: {filePath}");
        }
    }
}