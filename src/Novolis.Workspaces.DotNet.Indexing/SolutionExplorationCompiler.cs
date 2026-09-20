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

/// <summary>Compiles a generated typed exploration façade with the C# compiler.</summary>
public static class SolutionExplorationCompiler
{
    public static CompiledSolutionExploration Compile(GeneratedSolutionExploration generated)
    {
        ArgumentNullException.ThrowIfNull(generated);
        return new CompiledSolutionExploration(generated, Emit(
            "Novolis.Workspaces.DotNet.Generated",
            [CSharpSyntaxTree.ParseText(generated.Source, path: "GeneratedExploration.g.cs")]));
    }

    /// <summary>
    /// Compiles <paramref name="consumerSource"/> in the same compilation as the generated façade
    /// so the C# compiler type-checks member access such as
    /// <c>solution.Projects.DemoLib.Services.IdentityService</c>. Invokes
    /// <c>Consumer.Run(SolutionCatalog)</c> and returns its result.
    /// </summary>
    public static T InvokeConsumer<T>(
        GeneratedSolutionExploration generated,
        SolutionCatalog catalog,
        string consumerSource,
        string consumerTypeName = "Consumer",
        string methodName = "Run")
    {
        ArgumentNullException.ThrowIfNull(generated);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerSource);
        ArgumentException.ThrowIfNullOrWhiteSpace(consumerTypeName);
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        var assembly = Emit(
            "Novolis.Workspaces.DotNet.Generated.Consumer",
            [
                CSharpSyntaxTree.ParseText(generated.Source, path: "GeneratedExploration.g.cs"),
                CSharpSyntaxTree.ParseText(consumerSource, path: "Consumer.cs"),
            ]);

        var type = assembly.GetType(consumerTypeName)
            ?? Array.Find(assembly.GetTypes(), candidate => candidate.Name == consumerTypeName)
            ?? throw new InvalidOperationException($"Consumer type '{consumerTypeName}' was not found.");
        var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException($"Consumer '{consumerTypeName}.{methodName}' was not found.");

        var result = method.Invoke(null, [catalog]);
        if (result is T typed)
            return typed;

        throw new InvalidOperationException(
            $"Consumer '{consumerTypeName}.{methodName}' returned '{result?.GetType().FullName ?? "null"}', not {typeof(T).FullName}.");
    }

    private static Assembly Emit(string assemblyName, IEnumerable<SyntaxTree> trees)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName,
            trees,
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

        return Assembly.Load(peStream.ToArray());
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
