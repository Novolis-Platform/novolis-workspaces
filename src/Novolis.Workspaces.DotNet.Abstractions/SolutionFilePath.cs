using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>Absolute path to a .slnx or .sln solution file.</summary>
public sealed record SolutionFilePath
{
    public SolutionFilePath(string fullPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fullPath);
        FullPath = Path.GetFullPath(fullPath);
        if (!Path.HasExtension(FullPath)
            || (!FullPath.EndsWith(".slnx", StringComparison.OrdinalIgnoreCase)
                && !FullPath.EndsWith(".sln", StringComparison.OrdinalIgnoreCase)))
        {
            throw new ArgumentException("A solution path must end in .slnx or .sln.", nameof(fullPath));
        }
    }

    public string FullPath { get; }
}
