using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>Severity of a workspace operation diagnostic.</summary>
public enum WorkspaceDiagnosticSeverity
{
    Info = 0,
    Warning = 1,
    Error = 2,
}
