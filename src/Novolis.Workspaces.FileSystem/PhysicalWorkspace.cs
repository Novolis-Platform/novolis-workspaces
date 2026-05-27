using System.IO.Abstractions;

namespace Novolis.Workspaces.FileSystem;

/// <summary>Disk-backed workspace.</summary>
public sealed class PhysicalWorkspace : IWorkspace
{
    public PhysicalWorkspace(WorkspaceManifest manifest, IDirectoryInfo root, IReadOnlyList<IProject> projects)
    {
        Manifest = manifest;
        Root = root;
        Projects = projects;
        Id = manifest.Id;
        Name = manifest.Name;
    }

    public WorkspaceId Id { get; }
    public string Name { get; }
    public IDirectoryInfo Root { get; }
    public WorkspaceManifest Manifest { get; }
    public IReadOnlyList<IProject> Projects { get; }
}
