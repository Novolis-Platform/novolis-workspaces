using System.IO.Abstractions;
using System.IO.Compression;
using Novolis.Snapshots;
using Novolis.Timeline;
using Novolis.Workspaces.Timeline;

namespace Novolis.Workspaces.Projects.Timeline;

/// <summary>Save and restore points scoped to one <see cref="IProject"/>.</summary>
public sealed class ProjectTimeline
{
    private readonly IFileSystem _fileSystem;
    private readonly IDirectoryInfo _snapshotsRoot;
    private readonly ITimeline<ZipSnapshotRef> _timeline;
    private readonly IProjectSnapshotPolicy _policy;

    public ProjectTimeline(
        IFileSystem fileSystem,
        IDirectoryInfo snapshotsRoot,
        ITimeline<ZipSnapshotRef> timeline,
        IProjectSnapshotPolicy? policy = null)
    {
        _fileSystem = fileSystem;
        _snapshotsRoot = snapshotsRoot;
        _timeline = timeline;
        _policy = policy ?? new DefaultProjectSnapshotPolicy();
        if (!_snapshotsRoot.Exists)
            _snapshotsRoot.Create();
    }

    public async ValueTask<TimelineNode<ZipSnapshotRef>> SavePointAsync(
        IProject project,
        SavePointRequest request,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await SaveProjectZipAsync(project, request, cancellationToken).ConfigureAwait(false);
        return await _timeline.AddAsync(snapshot, request.ToMetadata(), cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask RestorePointAsync(
        IProject project,
        TimelineNodeId nodeId,
        bool moveHead = true,
        CancellationToken cancellationToken = default)
    {
        var nodes = await _timeline.GetNodesAsync(cancellationToken).ConfigureAwait(false);
        var node = nodes.FirstOrDefault(n => n.Id == nodeId)
            ?? throw new TimelineException($"Timeline node '{nodeId}' was not found.");

        await SavePointAsync(
            project,
            new SavePointRequest("Before restore", SnapshotKinds.Safety),
            cancellationToken).ConfigureAwait(false);

        await RestoreProjectZipAsync(project, node.Snapshot, cancellationToken).ConfigureAwait(false);

        if (moveHead)
            await _timeline.MoveHeadAsync(node.BranchId, node.Id, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<ZipSnapshotRef> SaveProjectZipAsync(
        IProject project,
        SavePointRequest request,
        CancellationToken cancellationToken)
    {
        _ = request;
        var objectId = Guid.NewGuid().ToString("N");
        var relativePath = Path.Combine("projects", project.Id.Value.ToString("N")[..2], $"{objectId}.zip");
        var fullPath = _fileSystem.Path.Combine(_snapshotsRoot.FullName, relativePath);
        _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(fullPath)!);

        await using (var fileStream = _fileSystem.File.Create(fullPath))
        await using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: false))
        {
            foreach (var file in _fileSystem.Directory.EnumerateFiles(project.Root.FullName, "*", SearchOption.AllDirectories))
            {
                var info = _fileSystem.FileInfo.New(file);
                if (!_policy.ShouldInclude(project, info))
                    continue;

                cancellationToken.ThrowIfCancellationRequested();
                var relative = GetRelativePath(project.Root.FullName, file);
                var entry = archive.CreateEntry(relative.Replace('\\', '/'), CompressionLevel.Optimal);
                await using var entryStream = entry.Open();
                await using var source = _fileSystem.File.OpenRead(file);
                await source.CopyToAsync(entryStream, cancellationToken).ConfigureAwait(false);
            }
        }

        return new ZipSnapshotRef(objectId, relativePath.Replace('\\', '/'));
    }

    private async ValueTask RestoreProjectZipAsync(
        IProject project,
        ZipSnapshotRef snapshot,
        CancellationToken cancellationToken)
    {
        var fullPath = _fileSystem.Path.Combine(_snapshotsRoot.FullName, snapshot.RelativePath);
        if (!_fileSystem.File.Exists(fullPath))
            throw new SnapshotException($"Project zip snapshot '{snapshot.RelativePath}' was not found.");

        foreach (var file in _fileSystem.Directory.EnumerateFiles(project.Root.FullName, "*", SearchOption.AllDirectories))
        {
            var info = _fileSystem.FileInfo.New(file);
            if (_policy.ShouldInclude(project, info))
                _fileSystem.File.Delete(file);
        }

        await using var fileStream = _fileSystem.File.OpenRead(fullPath);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen: false);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            cancellationToken.ThrowIfCancellationRequested();
            var destination = _fileSystem.Path.Combine(project.Root.FullName, entry.FullName);
            _fileSystem.Directory.CreateDirectory(_fileSystem.Path.GetDirectoryName(destination)!);
            await using var entryStream = entry.Open();
            await using var output = _fileSystem.File.Create(destination);
            await entryStream.CopyToAsync(output, cancellationToken).ConfigureAwait(false);
        }
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
