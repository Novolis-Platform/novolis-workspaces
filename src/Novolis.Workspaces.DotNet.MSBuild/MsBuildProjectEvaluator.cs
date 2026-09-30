namespace Novolis.Workspaces.DotNet.MSBuild;

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
