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
            MsBuildHostRegistration.EnsureRegistered();
            return ValueTask.FromResult(MsBuildProjectEvaluationCore.Evaluate(fullPath, context, diagnostics));
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
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
