using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>A portable diagnostic tied to a workspace file or root.</summary>
public sealed record WorkspaceDiagnostic(
    string Code,
    string Message,
    WorkspaceDiagnosticSeverity Severity,
    string? Path = null);
