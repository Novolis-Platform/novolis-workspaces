namespace Novolis.Workspaces;

/// <summary>Reference to a project from the workspace manifest.</summary>
public sealed record ProjectReference(ProjectId Id, string FolderName, string Name, ProjectKind Kind);
