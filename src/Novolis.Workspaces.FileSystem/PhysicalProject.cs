using System.IO.Abstractions;

namespace Novolis.Workspaces.FileSystem;

/// <summary>Disk-backed project.</summary>
public sealed class PhysicalProject : IProject
{
    public PhysicalProject(ProjectManifest manifest, IDirectoryInfo root)
    {
        Manifest = manifest;
        Root = root;
        Id = manifest.Id;
        Name = manifest.Name;
        Kind = manifest.Kind;
    }

    public ProjectId Id { get; }
    public string Name { get; }
    public ProjectKind Kind { get; }
    public IDirectoryInfo Root { get; }
    public ProjectManifest Manifest { get; }
}
