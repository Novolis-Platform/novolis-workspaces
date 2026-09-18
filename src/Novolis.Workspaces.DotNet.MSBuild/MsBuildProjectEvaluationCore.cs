using Microsoft.Build.Evaluation;

namespace Novolis.Workspaces.DotNet.MSBuild;

internal static class MsBuildProjectEvaluationCore
{
    public static EvaluatedProject Evaluate(
        string fullPath,
        EvaluationContext context,
        ICollection<WorkspaceDiagnostic> diagnostics)
    {
        using var collection = new ProjectCollection(context.ToGlobalProperties()
            .ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.OrdinalIgnoreCase));
        var project = collection.LoadProject(fullPath);
        var properties = project.AllEvaluatedProperties
            .GroupBy(property => property.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Last().EvaluatedValue, StringComparer.OrdinalIgnoreCase);
        var items = project.AllEvaluatedItems
            .Select(item => new EvaluatedProjectItem(
                item.ItemType,
                item.EvaluatedInclude,
                item.Metadata.ToDictionary(metadata => metadata.Name, metadata => metadata.EvaluatedValue, StringComparer.OrdinalIgnoreCase),
                item.Xml?.ContainingProject?.FullPath))
            .ToArray();
        var imports = project.Imports
            .Select(import => import.ImportedProject.FullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        return new EvaluatedProject(fullPath, context, properties, items, imports, diagnostics.ToArray());
    }
}
