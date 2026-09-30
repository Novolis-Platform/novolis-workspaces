using System.IO.Abstractions;
using RootWorkspace = Novolis.IO.Workspace.IWorkspace;

namespace Novolis.Workspaces.DotNet;

/// <summary>A .NET solution rooted at the directory containing its solution file.</summary>
public sealed class SolutionWorkspace : RootWorkspace
{
    public SolutionWorkspace(IDirectoryInfo root, SolutionFilePath solutionFile)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(solutionFile);
        if (!IsContainedBy(root.FullName, solutionFile.FullPath))
            throw new ArgumentException("The solution file must be contained by the workspace root.", nameof(solutionFile));

        Root = root;
        SolutionFile = solutionFile;
    }

    public IDirectoryInfo Root { get; }
    public SolutionFilePath SolutionFile { get; }

    private static bool IsContainedBy(string rootPath, string filePath)
    {
        var normalizedRoot = rootPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return filePath.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase);
    }
}
