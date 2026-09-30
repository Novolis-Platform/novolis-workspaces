using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>An arbitrary set of solutions with no shared-root guarantee.</summary>
public sealed record SolutionWorkspaceSet(IReadOnlyList<SolutionWorkspace> Members);
