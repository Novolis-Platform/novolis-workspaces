using System.IO.Abstractions;
using System.IO.Compression;
using Novolis.Snapshots;
using Novolis.Workspaces.FileSystem;

namespace Novolis.Workspaces.Snapshots;

/// <summary>Captures workspace tree as zip archives using <see cref="IWorkspaceSnapshotPolicy"/>.</summary>
public sealed class ZipWorkspaceSnapshotStore : ISnapshotStore<IWorkspace, ZipSnapshotRef>
{
    private readonly IFileSystem _fileSystem;
    private readonly IDirectoryInfo _snapshotsRoot;
    private readonly IWorkspaceSnapshotPolicy _policy;

    public ZipWorkspaceSnapshotStore(
        IFileSystem fileSystem,
        IDirectoryInfo snapshotsRoot,
        IWorkspaceSnapshotPolicy? policy = null)
    {
        _fileSystem = fileSystem;
        _snapshotsRoot = snapshotsRoot;
        _policy = policy ?? new DefaultWorkspaceSnapshotPolicy();
        if (!_snapshotsRoot.Exists)
            _snapshotsRoot.Create();
    }

    public async ValueTask<ZipSnapshotRef> SaveAsync(
        IWorkspace workspace,
        SnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = request;
        var objectId = Guid.NewGuid().ToString("N");
        var relativePath = Path.Combine(objectId[..2], $"{objectId}.zip");
        var fullPath = _fileSystem.Path.Combine(_snapshotsRoot.FullName, relativePath);
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(fullPath)!);

        await using (var fileStream = _fileSystem.File.Create(fullPath))
        await using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: false))
        {
            foreach (var file in EnumerateIncludedFiles(workspace))
            {
                cancellationToken.ThrowIfCancellationRequested();
                var relative = GetRelativePath(workspace.Root.FullName, file.FullName);
                var entry = archive.CreateEntry(relative.Replace('\\', '/'), CompressionLevel.Optimal);
                await using var entryStream = entry.Open();
                await using var source = _fileSystem.File.OpenRead(file.FullName);
                await source.CopyToAsync(entryStream, cancellationToken).ConfigureAwait(false);
            }
        }

        return new ZipSnapshotRef(objectId, relativePath.Replace('\\', '/'));
    }

    public async ValueTask RestoreAsync(
        IWorkspace workspace,
        ZipSnapshotRef snapshot,
        CancellationToken cancellationToken = default)
    {
        var fullPath = _fileSystem.Path.Combine(_snapshotsRoot.FullName, snapshot.RelativePath);
        if (!_fileSystem.File.Exists(fullPath))
            throw new SnapshotException($"Workspace zip snapshot '{snapshot.RelativePath}' was not found.");

        var timelinePath = WorkspaceLayout.TimelinePath(workspace.Root.FullName);
        var timelineBackup = _fileSystem.Directory.Exists(timelinePath)
            ? BackupDirectory(timelinePath)
            : null;

        try
        {
            ClearIncludedPaths(workspace);
            await ExtractZipAsync(fullPath, workspace.Root.FullName, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (timelineBackup is not null)
                RestoreDirectoryBackup(timelinePath, timelineBackup);
        }
    }

    private IEnumerable<IFileInfo> EnumerateIncludedFiles(IWorkspace workspace)
    {
        var root = workspace.Root.FullName;
        foreach (var file in _fileSystem.Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var info = _fileSystem.FileInfo.New(file);
            if (_policy.ShouldInclude(workspace, info))
                yield return info;
        }
    }

    private void ClearIncludedPaths(IWorkspace workspace)
    {
        foreach (var file in EnumerateIncludedFiles(workspace))
            _fileSystem.File.Delete(file.FullName);
    }

    private async ValueTask ExtractZipAsync(string zipPath, string workspaceRoot, CancellationToken cancellationToken)
    {
        await using var fileStream = _fileSystem.File.OpenRead(zipPath);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen: false);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            cancellationToken.ThrowIfCancellationRequested();
            var destination = _fileSystem.Path.Combine(workspaceRoot, entry.FullName);
            var directory = _fileSystem.Path.GetDirectoryName(destination)!;
            _fileSystem.Directory.CreateDirectory(directory);
            await using var entryStream = entry.Open();
            await using var output = _fileSystem.File.Create(destination);
            await entryStream.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
        }
    }

    private string BackupDirectory(string path)
    {
        var backup = path + ".bak-" + Guid.NewGuid().ToString("N");
        _fileSystem.Directory.Move(path, backup);
        _fileSystem.Directory.CreateDirectory(path);
        return backup;
    }

    private void RestoreDirectoryBackup(string path, string backup)
    {
        if (_fileSystem.Directory.Exists(path))
            _fileSystem.Directory.Delete(path, recursive: true);
        _fileSystem.Directory.Move(backup, path);
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
