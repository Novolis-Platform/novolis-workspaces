using Novolis.Workspaces.DotNet;
using Novolis.Workspaces.DotNet.Indexing;
using Novolis.Workspaces.DotNet.MSBuild;
using Novolis.Workspaces.DotNet.Roslyn;
using Novolis.Workspaces.DotNet.Slnx;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.DotNet;

public sealed class SolutionExplorationGeneratorTests
{
    [Test]
    public async Task Generate_EmitsTypedProjectNamespaceAndTypeMembers()
    {
        var generated = SolutionExplorationGenerator.Generate(CreateCatalog());

        await Assert.That(generated.Source.Contains("public DemoLibProject DemoLib { get; }", StringComparison.Ordinal)).IsTrue();
        await Assert.That(generated.Source.Contains("public DemoLibProject_Novolis_SampleNamespace Sample { get; }", StringComparison.Ordinal)).IsTrue();
        await Assert.That(generated.Source.Contains("public SemanticType Widget { get; }", StringComparison.Ordinal)).IsTrue();
        await Assert.That(generated.Source.Contains(
            "solution.Projects.DemoLib.Novolis.Sample.Widget",
            StringComparison.Ordinal)).IsTrue();
        await Assert.That(generated.Source.Contains("HiddenPart", StringComparison.Ordinal)).IsFalse();
    }

    [Test]
    public async Task Compile_WalksGeneratedTypedMembers()
    {
        var catalog = CreateCatalog();
        var compiled = SolutionExplorationCompiler.Compile(SolutionExplorationGenerator.Generate(catalog));
        var walk = compiled.Walk(catalog);

        await Assert.That(walk.Contains("solution.Projects.DemoLib.RelativePath => DemoLib/DemoLib.csproj", StringComparison.Ordinal)).IsTrue();
        await Assert.That(walk.Contains("solution.Projects.DemoLib.Novolis.Sample.Widget => Novolis.Sample.Widget", StringComparison.Ordinal)).IsTrue();
        await Assert.That(walk.Contains("solution.Projects.DemoLib.Novolis.Sample.WidgetKind => Novolis.Sample.WidgetKind", StringComparison.Ordinal)).IsTrue();
        await Assert.That(walk.Contains("HiddenPart", StringComparison.Ordinal)).IsFalse();
    }

    private static SolutionCatalog CreateCatalog()
    {
        const string projectPath = @"D:\demo\DemoLib\DemoLib.csproj";
        return new SolutionCatalog(
            "snapshot",
            new SolutionCatalogProvenance(@"D:\demo", @"D:\demo\Demo.slnx", DateTimeOffset.UnixEpoch),
            new EvaluationContext(),
            [
                new SolutionCatalogProject(
                    new SlnxProjectEntry("DemoLib/DemoLib.csproj", projectPath, "DemoLib"),
                    new EvaluatedProject(
                        projectPath,
                        new EvaluationContext(),
                        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                        [],
                        [],
                        []),
                    new SemanticProject(
                        projectPath,
                        ["Widget.cs"],
                        [
                            new SemanticType("Novolis.Sample.Widget", "Novolis.Sample", "Class", true, "Widget.cs"),
                            new SemanticType("Novolis.Sample.WidgetKind", "Novolis.Sample", "Enum", true, "Widget.cs"),
                            new SemanticType("Novolis.Sample.HiddenPart", "Novolis.Sample", "Class", false, "Widget.cs"),
                        ],
                        [])),
            ],
            []);
    }
}
