using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;

namespace Novolis.Workspaces.DotNet.MSBuild;

/// <summary>An effective MSBuild item together with its source-project provenance.</summary>
public sealed record EvaluatedProjectItem(
    string ItemType,
    string EvaluatedInclude,
    IReadOnlyDictionary<string, string> Metadata,
    string? DefiningProjectFullPath);

/// <summary>An immutable effective view of one project under one evaluation context.</summary>
public sealed record EvaluatedProject(
    string ProjectPath,
    EvaluationContext Context,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyList<EvaluatedProjectItem> Items,
    IReadOnlyList<string> Imports,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);

/// <summary>Evaluates MSBuild projects without running build targets.</summary>
public sealed class MsBuildProjectEvaluator
{
    public ValueTask<EvaluatedProject> EvaluateAsync(
        string projectPath,
        EvaluationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(projectPath);
        ArgumentNullException.ThrowIfNull(context);
        cancellationToken.ThrowIfCancellationRequested();

        var fullPath = Path.GetFullPath(projectPath);
        var diagnostics = new List<WorkspaceDiagnostic>();
        if (!context.AllowEvaluation)
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS2000",
                "MSBuild evaluation requires EvaluationContext.AllowEvaluation.",
                WorkspaceDiagnosticSeverity.Warning,
                fullPath));
            return ValueTask.FromResult(new EvaluatedProject(
                fullPath,
                context,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                [],
                [],
                diagnostics));
        }

        try
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

            return ValueTask.FromResult(new EvaluatedProject(fullPath, context, properties, items, imports, diagnostics));
        }
        catch (Exception exception) when (exception is InvalidProjectFileException or IOException or UnauthorizedAccessException)
        {
            diagnostics.Add(new WorkspaceDiagnostic(
                "NWS2001",
                exception.Message,
                WorkspaceDiagnosticSeverity.Error,
                fullPath));
            return ValueTask.FromResult(new EvaluatedProject(
                fullPath,
                context,
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase),
                [],
                [],
                diagnostics));
        }
    }
}
