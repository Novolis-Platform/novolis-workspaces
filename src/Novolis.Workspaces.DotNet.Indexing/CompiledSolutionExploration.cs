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
