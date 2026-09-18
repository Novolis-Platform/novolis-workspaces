using System.IO.Abstractions;

namespace Novolis.Workspaces.FileSystem;

/// <summary>
/// Compatibility name for <see cref="PhysicalProjectWorkspace"/>.
/// </summary>
[Obsolete("Use PhysicalProjectWorkspace. IWorkspace is the typed directory-root abstraction in Novolis.IO.Workspace.")]
public sealed class PhysicalWorkspace : PhysicalProjectWorkspace, IWorkspace
{
    public PhysicalWorkspace(WorkspaceManifest manifest, IDirectoryInfo root, IReadOnlyList<IProject> projects)
        : base(manifest, root, projects)
    {
    }
}
