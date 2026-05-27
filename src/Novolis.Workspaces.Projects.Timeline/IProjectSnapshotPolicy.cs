using System.IO.Abstractions;

namespace Novolis.Workspaces.Projects.Timeline;

/// <summary>Decides which files under a project root are included in project-scoped snapshots.</summary>
public interface IProjectSnapshotPolicy
{
    bool ShouldInclude(IProject project, IFileInfo file);
}
