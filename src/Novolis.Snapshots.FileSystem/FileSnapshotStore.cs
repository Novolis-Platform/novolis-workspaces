using System.IO.Abstractions;
using System.Security.Cryptography;

namespace Novolis.Snapshots.FileSystem;

/// <summary>Stores serialized state blobs under a root directory.</summary>
public sealed class FileSnapshotStore<TState> : ISnapshotStore<TState, FileSnapshotRef>
{
    private readonly IFileSystem _fileSystem;
    private readonly IDirectoryInfo _root;
    private readonly IStateSerializer<TState> _serializer;

    public FileSnapshotStore(IFileSystem fileSystem, IDirectoryInfo root, IStateSerializer<TState> serializer)
    {
        _fileSystem = fileSystem;
        _root = root;
        _serializer = serializer;
        if (!_root.Exists)
            _root.Create();
    }

    public async ValueTask<FileSnapshotRef> SaveAsync(
        TState state,
        SnapshotRequest request,
        CancellationToken cancellationToken = default)
    {
        _ = request;
        var id = Guid.NewGuid().ToString("N");
        var relativePath = Path.Combine(id[..2], $"{id}.dat");
        var fullPath = _fileSystem.Path.Combine(_root.FullName, relativePath);
        var directory = _fileSystem.Path.GetDirectoryName(fullPath)!;
        _fileSystem.Directory.CreateDirectory(directory);

        await using var stream = _fileSystem.File.Create(fullPath);
        await _serializer.WriteAsync(state, stream, cancellationToken).ConfigureAwait(false);
        await stream.FlushAsync(cancellationToken).ConfigureAwait(false);

        var hash = await ComputeHashAsync(fullPath, cancellationToken).ConfigureAwait(false);
        return new FileSnapshotRef(relativePath.Replace('\\', '/'), hash);
    }

    public async ValueTask RestoreAsync(
        TState target,
        FileSnapshotRef snapshot,
        CancellationToken cancellationToken = default)
    {
        var fullPath = _fileSystem.Path.Combine(_root.FullName, snapshot.RelativePath);
        if (!_fileSystem.File.Exists(fullPath))
            throw new SnapshotException($"File snapshot '{snapshot.RelativePath}' was not found.");

        await using var stream = _fileSystem.File.OpenRead(fullPath);
        await _serializer.ReadAsync(target, stream, cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<string> ComputeHashAsync(string fullPath, CancellationToken cancellationToken)
    {
        await using var stream = _fileSystem.File.OpenRead(fullPath);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken).ConfigureAwait(false);
        return Convert.ToHexString(hash);
    }
}
