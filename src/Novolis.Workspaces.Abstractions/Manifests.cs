namespace Novolis.Workspaces;

/// <summary>Workspace-level manifest persisted as <c>workspace.json</c>.</summary>
public sealed record WorkspaceManifest(
    WorkspaceId Id,
    string Name,
    int SchemaVersion,
    IReadOnlyList<ProjectReference> Projects);
