namespace Novolis.Workspaces;

/// <summary>Project-level manifest persisted as <c>project.json</c>.</summary>
public sealed record ProjectManifest(
    ProjectId Id,
    string Name,
    ProjectKind Kind,
    int SchemaVersion,
    IReadOnlyDictionary<string, string> Properties);
