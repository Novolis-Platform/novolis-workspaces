using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces;

/// <summary>Top-level container for projects, settings, and workspace-local metadata.</summary>
public interface IProjectWorkspace : RootWorkspace
{
    WorkspaceId Id { get; }
    string Name { get; }
    WorkspaceManifest Manifest { get; }
    IReadOnlyList<IProject> Projects { get; }
}

/// <summary>
/// Compatibility name for <see cref="IProjectWorkspace"/>.
/// </summary>
[Obsolete("Use IProjectWorkspace. IWorkspace is the typed directory-root abstraction in Novolis.IO.Workspace.")]
public interface IWorkspace : IProjectWorkspace;

/// <summary>Unit of meaningful work inside a workspace.</summary>
public interface IProject
{
    ProjectId Id { get; }
    string Name { get; }
    ProjectKind Kind { get; }
    IDirectoryInfo Root { get; }
    ProjectManifest Manifest { get; }
}
