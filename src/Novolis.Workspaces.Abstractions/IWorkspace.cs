using System.IO.Abstractions;

namespace Novolis.Workspaces;

/// <summary>Top-level container for projects, settings, and workspace-local metadata.</summary>
public interface IWorkspace
{
    WorkspaceId Id { get; }
    string Name { get; }
    IDirectoryInfo Root { get; }
    WorkspaceManifest Manifest { get; }
    IReadOnlyList<IProject> Projects { get; }
}

/// <summary>Unit of meaningful work inside a workspace.</summary>
public interface IProject
{
    ProjectId Id { get; }
    string Name { get; }
    ProjectKind Kind { get; }
    IDirectoryInfo Root { get; }
    ProjectManifest Manifest { get; }
}
