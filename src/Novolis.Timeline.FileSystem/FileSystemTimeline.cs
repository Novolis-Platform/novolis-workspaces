using System.IO.Abstractions;
using System.Text.Json;

namespace Novolis.Timeline.FileSystem;

/// <summary>Persists timeline state under <c>branches.json</c>, <c>head.json</c>, and <c>nodes/*.json</c>.</summary>
public sealed class FileSystemTimeline<TSnapshotRef> : ITimeline<TSnapshotRef>
{
    private readonly IFileSystem _fileSystem;
    private readonly IDirectoryInfo _root;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly object _gate = new();

    public FileSystemTimeline(IFileSystem fileSystem, IDirectoryInfo root, JsonSerializerOptions? jsonOptions = null)
    {
        _fileSystem = fileSystem;
        _root = root;
        _jsonOptions = jsonOptions ?? TimelineJsonSerializerOptions.Create();
        EnsureLayout();
        EnsureMainBranch();
    }

    public async ValueTask<TimelineNode<TSnapshotRef>> AddAsync(
        TSnapshotRef snapshot,
        TimelineMetadata metadata,
        TimelineNodeId? parentId = null,
        BranchId? branchId = null,
        CancellationToken cancellationToken = default)
    {
        TimelineNode<TSnapshotRef> node;
        lock (_gate)
        {
            var state = LoadState();
            var branch = branchId ?? BranchId.Main;
            if (!state.Branches.Any(b => b.Id == branch))
                throw new TimelineException($"Branch '{branch}' does not exist.");

            var resolvedParent = parentId;
            if (resolvedParent is null && state.Heads.TryGetValue(branch, out var head))
                resolvedParent = head;

            node = new TimelineNode<TSnapshotRef>(
                TimelineNodeId.New(),
                resolvedParent,
                branch,
                snapshot,
                metadata,
                DateTimeOffset.UtcNow);

            state.Heads[branch] = node.Id;
            SaveState(state);
        }

        await WriteNodeFileAsync(node, cancellationToken).ConfigureAwait(false);
        return node;
    }

    public ValueTask<Branch> BranchAsync(BranchName name, TimelineNodeId from, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        lock (_gate)
        {
            if (!_fileSystem.File.Exists(NodePath(from)))
                throw new TimelineException($"Node '{from}' was not found.");

            var state = LoadState();
            var branch = new Branch(BranchId.New(), name, from);
            state.Branches.Add(new BranchRecord(branch.Id, branch.Name.Value, from));
            state.Heads[branch.Id] = from;
            SaveState(state);
            return ValueTask.FromResult(branch);
        }
    }

    public ValueTask MoveHeadAsync(BranchId branch, TimelineNodeId node, CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        lock (_gate)
        {
            if (!_fileSystem.File.Exists(NodePath(node)))
                throw new TimelineException($"Node '{node}' was not found.");

            var state = LoadState();
            if (state.Branches.All(b => b.Id != branch))
                throw new TimelineException($"Branch '{branch}' does not exist.");

            var nodeFile = JsonSerializer.Deserialize<NodeFile<TSnapshotRef>>(
                _fileSystem.File.ReadAllText(NodePath(node)), _jsonOptions)
                ?? throw new TimelineException($"Node '{node}' is invalid.");

            if (nodeFile.BranchId != branch)
                throw new TimelineException($"Node '{node}' is not on branch '{branch}'.");

            state.Heads[branch] = node;
            SaveState(state);
        }

        return ValueTask.CompletedTask;
    }

    public async ValueTask<IReadOnlyList<TimelineNode<TSnapshotRef>>> GetNodesAsync(CancellationToken cancellationToken = default)
    {
        if (!_fileSystem.Directory.Exists(NodesPath()))
            return [];

        var nodes = new List<TimelineNode<TSnapshotRef>>();
        foreach (var file in _fileSystem.Directory.EnumerateFiles(NodesPath(), "*.json"))
        {
            await using var stream = _fileSystem.File.OpenRead(file);
            var nodeFile = await JsonSerializer.DeserializeAsync<NodeFile<TSnapshotRef>>(stream, _jsonOptions, cancellationToken).ConfigureAwait(false);
            if (nodeFile is null)
                continue;

            nodes.Add(new TimelineNode<TSnapshotRef>(
                nodeFile.Id,
                nodeFile.ParentId,
                nodeFile.BranchId,
                nodeFile.Snapshot,
                nodeFile.Metadata,
                nodeFile.CreatedAt));
        }

        return nodes.OrderBy(n => n.CreatedAt).ToArray();
    }

    public ValueTask<IReadOnlyList<Branch>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        lock (_gate)
        {
            var state = LoadState();
            IReadOnlyList<Branch> branches = state.Branches
                .Select(b => new Branch(b.Id, new BranchName(b.Name), b.ForkedFromNodeId))
                .ToArray();
            return ValueTask.FromResult(branches);
        }
    }

    public ValueTask<TimelineHead> GetHeadAsync(CancellationToken cancellationToken = default)
    {
        _ = cancellationToken;
        lock (_gate)
            return ValueTask.FromResult(LoadState().Head);
    }

    private void EnsureLayout()
    {
        if (!_root.Exists)
            _root.Create();
        _fileSystem.Directory.CreateDirectory(NodesPath());
    }

    private void EnsureMainBranch()
    {
        lock (_gate)
        {
            var state = LoadState();
            if (state.Branches.Any(b => b.Id == BranchId.Main))
                return;

            state.Branches.Add(new BranchRecord(BranchId.Main, "main", null));
            SaveState(state);
        }
    }

    private string NodesPath() => _fileSystem.Path.Combine(_root.FullName, "nodes");

    private string BranchesPath() => _fileSystem.Path.Combine(_root.FullName, "branches.json");

    private string HeadPath() => _fileSystem.Path.Combine(_root.FullName, "head.json");

    private string NodePath(TimelineNodeId id) => _fileSystem.Path.Combine(NodesPath(), $"{id.Value:D}.json");

    private TimelineState LoadState()
    {
        if (!_fileSystem.File.Exists(BranchesPath()) || !_fileSystem.File.Exists(HeadPath()))
            return new TimelineState([], new Dictionary<BranchId, TimelineNodeId>());

        var branches = JsonSerializer.Deserialize<BranchRecord[]>(
            _fileSystem.File.ReadAllText(BranchesPath()), _jsonOptions) ?? [];
        var head = JsonSerializer.Deserialize<Dictionary<BranchId, TimelineNodeId>>(
            _fileSystem.File.ReadAllText(HeadPath()), _jsonOptions)
            ?? new Dictionary<BranchId, TimelineNodeId>();

        return new TimelineState([.. branches], head);
    }

    private void SaveState(TimelineState state)
    {
        _fileSystem.File.WriteAllText(BranchesPath(), JsonSerializer.Serialize(state.Branches, _jsonOptions));
        _fileSystem.File.WriteAllText(HeadPath(), JsonSerializer.Serialize(state.Heads, _jsonOptions));
    }

    private async ValueTask WriteNodeFileAsync(TimelineNode<TSnapshotRef> node, CancellationToken cancellationToken)
    {
        var record = new NodeFile<TSnapshotRef>(
            node.Id,
            node.ParentId,
            node.BranchId,
            node.Snapshot,
            node.Metadata,
            node.CreatedAt);

        await using var stream = _fileSystem.File.Create(NodePath(node.Id));
        await JsonSerializer.SerializeAsync(stream, record, _jsonOptions, cancellationToken).ConfigureAwait(false);
    }

    private sealed class TimelineState
    {
        public List<BranchRecord> Branches { get; }
        public Dictionary<BranchId, TimelineNodeId> Heads { get; }

        public TimelineState(List<BranchRecord> branches, Dictionary<BranchId, TimelineNodeId> heads)
        {
            Branches = branches;
            Heads = heads;
        }

        public TimelineHead Head => new(Heads);
    }

    private sealed record BranchRecord(BranchId Id, string Name, TimelineNodeId? ForkedFromNodeId);

    private sealed record NodeFile<T>(
        TimelineNodeId Id,
        TimelineNodeId? ParentId,
        BranchId BranchId,
        T Snapshot,
        TimelineMetadata Metadata,
        DateTimeOffset CreatedAt);
}
