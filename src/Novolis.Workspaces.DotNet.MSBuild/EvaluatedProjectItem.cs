namespace Novolis.Workspaces.DotNet.MSBuild;

/// <summary>An effective MSBuild item together with its source-project provenance.</summary>
public sealed record EvaluatedProjectItem(
    string ItemType,
    string EvaluatedInclude,
    IReadOnlyDictionary<string, string> Metadata,
    string? DefiningProjectFullPath);
