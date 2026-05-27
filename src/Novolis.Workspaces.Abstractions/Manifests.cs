namespace Novolis.Workspaces;

/// <summary>Workspace-level manifest persisted as <c>workspace.json</c>.</summary>
public sealed record WorkspaceManifest(
    WorkspaceId Id,
    string Name,
    int SchemaVersion,
    IReadOnlyList<ProjectReference> Projects);

/// <summary>Reference to a project from the workspace manifest.</summary>
public sealed record ProjectReference(ProjectId Id, string FolderName, string Name, ProjectKind Kind);

/// <summary>Project-level manifest persisted as <c>project.json</c>.</summary>
public sealed record ProjectManifest(
    ProjectId Id,
    string Name,
    ProjectKind Kind,
    int SchemaVersion,
    IReadOnlyDictionary<string, string> Properties);
