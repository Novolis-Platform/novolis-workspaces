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
