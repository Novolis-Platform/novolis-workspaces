using Novolis.Workspaces.DotNet;
using Novolis.Workspaces.DotNet.Indexing;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.DotNet;

public sealed class SlnxSolutionReaderTests
{
    [Test]
    public async Task ReadAsync_ProjectsPhysicalSlnxTopology()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "novolis-slnx-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
        try
        {
            var solutionPath = Path.Combine(rootPath, "Demo.slnx");
            await File.WriteAllTextAsync(solutionPath, """
                <Solution>
                  <Folder Name="src">
                    <Project Path="src/Demo/Demo.csproj" />
                  </Folder>
                </Solution>
                """);
            var fileSystem = new System.IO.Abstractions.FileSystem();
            var workspace = new SolutionWorkspace(
                fileSystem.DirectoryInfo.New(rootPath),
                new SolutionFilePath(solutionPath));

            var topology = await new SlnxSolutionReader().ReadAsync(workspace);

            await Assert.That(topology.Projects).Count().IsEqualTo(1);
            await Assert.That(topology.Projects[0].RelativePath).IsEqualTo("src/Demo/Demo.csproj");
            await Assert.That(topology.Folders[0].Name).IsEqualTo("src");
            await Assert.That(topology.Diagnostics).Count().IsEqualTo(0);
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }

    [Test]
    public async Task EvaluateAsync_RequiresExplicitEvaluationConsent()
    {
        var evaluation = await new MsBuildProjectEvaluator().EvaluateAsync(
            Path.Combine(Path.GetTempPath(), "Missing.csproj"),
            new EvaluationContext());

        await Assert.That(evaluation.Diagnostics[0].Code).IsEqualTo("NWS2000");
    }

    [Test]
    public async Task BuildAsync_ProducesPortableCatalogProvenance()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "novolis-catalog-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
        try
        {
            var solutionPath = Path.Combine(rootPath, "Demo.slnx");
            await File.WriteAllTextAsync(solutionPath, "<Solution />");
            var fileSystem = new System.IO.Abstractions.FileSystem();
            var workspace = new SolutionWorkspace(
                fileSystem.DirectoryInfo.New(rootPath),
                new SolutionFilePath(solutionPath));

            var catalog = await new SolutionCatalogBuilder().BuildAsync(workspace, new EvaluationContext());

            await Assert.That(catalog.Provenance.WorkspaceRootPath).IsEqualTo(rootPath);
            await Assert.That(catalog.Provenance.SolutionPath).IsEqualTo(solutionPath);
            await Assert.That(catalog.Projects).Count().IsEqualTo(0);
            await Assert.That(catalog.SnapshotId.Length).IsEqualTo(64);
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }

    [Test]
    public async Task LoadAsync_ProjectsPublicCSharpTypesWhenDesignTimeLoadingIsAuthorized()
    {
        var rootPath = Path.Combine(Path.GetTempPath(), "novolis-roslyn-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(rootPath);
        try
        {
            var projectPath = Path.Combine(rootPath, "Demo.csproj");
            await File.WriteAllTextAsync(projectPath, """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <TargetFramework>net10.0</TargetFramework>
                  </PropertyGroup>
                </Project>
                """);
            await File.WriteAllTextAsync(
                Path.Combine(rootPath, "Greeting.cs"),
                "namespace Novolis.Sample; public sealed class Greeting { }");

            var semantic = await new RoslynSemanticProjectLoader().LoadAsync(
                projectPath,
                new EvaluationContext(AllowDesignTimeBuilds: true));

            await Assert.That(semantic.Types.Any(type => type.MetadataName == "Novolis.Sample.Greeting")).IsTrue();
            await Assert.That(semantic.Diagnostics.Any(diagnostic => diagnostic.Severity == WorkspaceDiagnosticSeverity.Error)).IsFalse();
        }
        finally
        {
            Directory.Delete(rootPath, recursive: true);
        }
    }
}
