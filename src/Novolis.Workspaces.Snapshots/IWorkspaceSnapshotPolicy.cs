using System.IO.Abstractions;

namespace Novolis.Workspaces.Snapshots;

/// <summary>Decides which workspace files are included in a snapshot.</summary>
public interface IWorkspaceSnapshotPolicy
{
    bool ShouldInclude(IWorkspace workspace, IFileInfo file);
}
