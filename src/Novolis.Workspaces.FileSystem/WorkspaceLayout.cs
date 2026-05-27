namespace Novolis.Workspaces.FileSystem;

/// <summary>Well-known paths within a Novolis workspace.</summary>
public static class WorkspaceLayout
{
    public const int CurrentSchemaVersion = 1;

    public const string NovolisFolder = ".novolis";
    public const string WorkspaceManifestFile = "workspace.json";
    public const string SettingsFile = "settings.json";
    public const string TimelineFolder = "timeline";
    public const string ProjectsFolder = "projects";
    public const string ProjectManifestFile = "project.json";
    public const string DocumentsFolder = "documents";
    public const string AssetsFolder = "assets";
    public const string OutputsFolder = "outputs";
    public const string CacheFolder = "cache";
    public const string TempFolder = "temp";

    public static string NovolisPath(string workspaceRoot) =>
        Path.Combine(workspaceRoot, NovolisFolder);

    public static string WorkspaceManifestPath(string workspaceRoot) =>
        Path.Combine(NovolisPath(workspaceRoot), WorkspaceManifestFile);

    public static string TimelinePath(string workspaceRoot) =>
        Path.Combine(NovolisPath(workspaceRoot), TimelineFolder);

    public static string ProjectsPath(string workspaceRoot) =>
        Path.Combine(workspaceRoot, ProjectsFolder);

    public static string ProjectRoot(string workspaceRoot, string folderName) =>
        Path.Combine(ProjectsPath(workspaceRoot), folderName);
}
