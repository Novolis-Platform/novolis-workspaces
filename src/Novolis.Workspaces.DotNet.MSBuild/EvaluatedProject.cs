namespace Novolis.Workspaces.DotNet.MSBuild;

/// <summary>An immutable effective view of one project under one evaluation context.</summary>
public sealed record EvaluatedProject(
    string ProjectPath,
    EvaluationContext Context,
    IReadOnlyDictionary<string, string> Properties,
    IReadOnlyList<EvaluatedProjectItem> Items,
    IReadOnlyList<string> Imports,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);
