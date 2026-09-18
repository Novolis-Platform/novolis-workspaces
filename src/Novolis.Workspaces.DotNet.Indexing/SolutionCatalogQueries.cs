using Novolis.Workspaces.DotNet.Roslyn;

namespace Novolis.Workspaces.DotNet.Indexing;

/// <summary>Allocation-light queries over an immutable solution catalog.</summary>
public static class SolutionCatalogQueries
{
    public static SolutionCatalogProject? FindProject(this SolutionCatalog catalog, string projectPath)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        var fullPath = Path.GetFullPath(projectPath);
        return catalog.Projects.FirstOrDefault(project =>
            string.Equals(project.Project.FullPath, fullPath, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<SemanticType> FindTypesInNamespace(this SolutionCatalog catalog, string @namespace)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(@namespace);
        return catalog.Projects
            .SelectMany(project => project.Semantic.Types)
            .Where(type => string.Equals(type.Namespace, @namespace, StringComparison.Ordinal));
    }

    public static IEnumerable<SemanticType> FindPublicTypes(this SolutionCatalog catalog)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        return catalog.Projects
            .SelectMany(project => project.Semantic.Types)
            .Where(type => type.IsPublic);
    }
}
