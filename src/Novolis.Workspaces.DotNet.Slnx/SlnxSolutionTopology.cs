using System.Xml.Linq;

namespace Novolis.Workspaces.DotNet.Slnx;

/// <summary>Read-only physical topology projected from an SLNX file.</summary>
public sealed record SlnxSolutionTopology(
    SolutionWorkspace Workspace,
    IReadOnlyList<SlnxProjectEntry> Projects,
    IReadOnlyList<SlnxSolutionFolder> Folders,
    IReadOnlyList<WorkspaceDiagnostic> Diagnostics);
