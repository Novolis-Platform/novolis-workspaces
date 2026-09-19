using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>An in-memory assembly compiled from a generated typed exploration façade.</summary>
public sealed class CompiledSolutionExploration
{
    internal CompiledSolutionExploration(GeneratedSolutionExploration generated, Assembly assembly)
    {
        Generated = generated;
        Assembly = assembly;
    }

    public GeneratedSolutionExploration Generated { get; }
    public Assembly Assembly { get; }

    public string Walk(SolutionCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        var type = Assembly.GetType(Generated.WalkTypeName)
            ?? throw new InvalidOperationException($"Generated type '{Generated.WalkTypeName}' was not found.");
        var method = type.GetMethod("Walk", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Generated type '{Generated.WalkTypeName}' has no Walk method.");
        return method.Invoke(null, [catalog]) as string
            ?? throw new InvalidOperationException("Generated Walk returned no text.");
    }
}

/// <summary>Dynamically compiles a generated typed exploration façade.</summary>
public static class SolutionExplorationCompiler
{
    public static CompiledSolutionExploration Compile(GeneratedSolutionExploration generated)
    {
        ArgumentNullException.ThrowIfNull(generated);

        var compilation = CSharpCompilation.Create(
            "Novolis.Workspaces.DotNet.Generated",
            [CSharpSyntaxTree.ParseText(generated.Source)],
            CreateReferences(),
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

        using var peStream = new MemoryStream();
        var emit = compilation.Emit(peStream);
        if (!emit.Success)
        {
            var errors = emit.Diagnostics
                .Where(diagnostic => diagnostic.Severity is DiagnosticSeverity.Error)
                .Select(diagnostic => diagnostic.ToString());
            throw new InvalidOperationException(
                "Generated exploration failed to compile:" + Environment.NewLine + string.Join(Environment.NewLine, errors));
        }

        return new CompiledSolutionExploration(generated, Assembly.Load(peStream.ToArray()));
    }

    private static ImmutableArray<MetadataReference> CreateReferences()
    {
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trusted)
        {
            foreach (var path in trusted.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                paths.Add(path);
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
                paths.Add(assembly.Location);
        }

        foreach (var assembly in new[]
                 {
                     typeof(object).Assembly,
                     typeof(Enumerable).Assembly,
                     typeof(SolutionCatalog).Assembly,
                     typeof(Roslyn.SemanticType).Assembly,
                     typeof(Slnx.SlnxProjectEntry).Assembly,
                     typeof(EvaluationContext).Assembly,
                 })
        {
            if (!string.IsNullOrEmpty(assembly.Location))
                paths.Add(assembly.Location);
        }

        return [.. paths.Select(path => (MetadataReference)MetadataReference.CreateFromFile(path))];
    }
}
