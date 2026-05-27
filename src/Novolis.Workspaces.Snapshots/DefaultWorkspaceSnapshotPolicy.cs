using System.IO.Abstractions;
using Novolis.Workspaces.FileSystem;

namespace Novolis.Workspaces.Snapshots;

/// <summary>Default include/exclude rules for workspace snapshots.</summary>
public sealed class DefaultWorkspaceSnapshotPolicy : IWorkspaceSnapshotPolicy
{
    public bool ShouldInclude(IWorkspace workspace, IFileInfo file)
    {
        var relative = GetRelativePath(workspace.Root.FullName, file.FullName);
        if (string.IsNullOrEmpty(relative))
            return false;

        var normalized = relative.Replace('\\', '/');

        if (IsExcluded(normalized))
            return false;

        return IsIncluded(normalized);
    }

    private static bool IsExcluded(string relative)
    {
        if (relative.StartsWith(".novolis/timeline/", StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var segment in new[]
                 {
                     "/cache/", "/temp/", "/bin/", "/obj/", "/outputs/", "/logs/", "/model-files/",
                     "\\cache\\", "\\temp\\", "\\bin\\", "\\obj\\", "\\outputs\\", "\\logs\\", "\\model-files\\",
                 })
        {
            if (relative.Contains(segment, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        if (relative.Equals("cache", StringComparison.OrdinalIgnoreCase)
            || relative.Equals("temp", StringComparison.OrdinalIgnoreCase)
            || relative.Equals("outputs", StringComparison.OrdinalIgnoreCase))
            return true;

        return false;
    }

    private static bool IsIncluded(string relative)
    {
        if (relative.Equals($"{WorkspaceLayout.NovolisFolder}/{WorkspaceLayout.WorkspaceManifestFile}", StringComparison.OrdinalIgnoreCase))
            return true;

        if (relative.EndsWith($"/{WorkspaceLayout.ProjectManifestFile}", StringComparison.OrdinalIgnoreCase))
            return true;

        if (relative.Contains("/documents/", StringComparison.OrdinalIgnoreCase)
            || relative.Contains("/assets/", StringComparison.OrdinalIgnoreCase)
            || relative.Contains("/settings/", StringComparison.OrdinalIgnoreCase)
            || relative.Contains("/presets/", StringComparison.OrdinalIgnoreCase))
            return true;

        if (relative.Equals($"{WorkspaceLayout.NovolisFolder}/{WorkspaceLayout.SettingsFile}", StringComparison.OrdinalIgnoreCase))
            return true;

        return relative.StartsWith($"{WorkspaceLayout.ProjectsFolder}/", StringComparison.OrdinalIgnoreCase)
               && !relative.Contains("/cache/", StringComparison.OrdinalIgnoreCase)
               && !relative.Contains("/temp/", StringComparison.OrdinalIgnoreCase)
               && !relative.Contains("/outputs/", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetRelativePath(string root, string fullPath)
    {
        var rootWithSep = root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        return fullPath.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase)
            ? fullPath[rootWithSep.Length..]
            : string.Empty;
    }
}
