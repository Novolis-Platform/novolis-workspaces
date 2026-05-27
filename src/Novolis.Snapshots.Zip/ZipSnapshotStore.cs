using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;

namespace Novolis.Snapshots.Zip;

/// <summary>Stores state in zip archives under a snapshots root directory.</summary>
public sealed class ZipSnapshotStore<TState> : ISnapshotStore<TState, ZipSnapshotRef>
{
    public const string StateEntryName = "state.dat";
    public const string ManifestEntryName = "manifest.json";

    private readonly IFileSystem _fileSystem;
    private readonly IDirectoryInfo _root;
    private readonly IStateSerializer<TState> _serializer;

    public ZipSnapshotStore(IFileSystem fileSystem, IDirectoryInfo root, IStateSerializer<TState> serializer)
    {
        _fileSystem = fileSystem;
        _root = root;
        _serializer = serializer;
        if (!_root.Exists)
            _root.Create();
    }

    public async ValueTask<ZipSnapshotRef> SaveAsync(
        TState state,
        SnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        var objectId = Guid.NewGuid().ToString("N");
        var relativePath = Path.Combine(objectId[..2], $"{objectId}.zip");
        var fullPath = _fileSystem.Path.Combine(_root.FullName, relativePath);
        var directory = _fileSystem.Path.GetDirectoryName(fullPath)!;
        _fileSystem.Directory.CreateDirectory(directory);

        await using (var fileStream = _fileSystem.File.Create(fullPath))
        await using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create, leaveOpen: false))
        {
            var stateEntry = archive.CreateEntry(StateEntryName, CompressionLevel.Optimal);
            await using var stateStream = stateEntry.Open();
            await _serializer.WriteAsync(state, stateStream, cancellationToken).ConfigureAwait(false);

            var manifestEntry = archive.CreateEntry(ManifestEntryName, CompressionLevel.Optimal);
            await using var manifestStream = manifestEntry.Open();
            await JsonSerializer.SerializeAsync(
                manifestStream,
                new ZipSnapshotManifest(request.Label, request.Kind, request.Properties),
                cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        return new ZipSnapshotRef(objectId, relativePath.Replace('\\', '/'));
    }

    public async ValueTask RestoreAsync(
        TState target,
        ZipSnapshotRef snapshot,
        CancellationToken cancellationToken = default)
    {
        var fullPath = _fileSystem.Path.Combine(_root.FullName, snapshot.RelativePath);
        if (!_fileSystem.File.Exists(fullPath))
            throw new SnapshotException($"Zip snapshot '{snapshot.RelativePath}' was not found.");

        await using var fileStream = _fileSystem.File.OpenRead(fullPath);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read, leaveOpen: false);
        var stateEntry = archive.GetEntry(StateEntryName)
            ?? throw new SnapshotException($"Zip snapshot '{snapshot.RelativePath}' is missing '{StateEntryName}'.");

        await using var stateStream = stateEntry.Open();
        await _serializer.ReadAsync(target, stateStream, cancellationToken).ConfigureAwait(false);
    }

    private sealed record ZipSnapshotManifest(string? Label, string Kind, IReadOnlyDictionary<string, string>? Properties);
}
