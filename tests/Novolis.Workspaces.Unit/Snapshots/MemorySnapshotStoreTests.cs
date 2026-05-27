using Novolis.Snapshots;
using Novolis.Snapshots.Json;
using Novolis.Snapshots.Memory;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.Snapshots;

public sealed class MemorySnapshotStoreTests
{
    [Test]
    public async Task SaveAndRestore_RoundTripsState()
    {
        var serializer = new JsonStateSerializer<SampleState>();
        var store = new MemorySnapshotStore<SampleState>(serializer, () => new SampleState());
        var state = new SampleState { Name = "alpha", Count = 3 };

        var snapshot = await store.SaveAsync(state, new SnapshotRequest("test", SnapshotKinds.Manual));
        var target = new SampleState();
        await store.RestoreAsync(target, snapshot);

        await Assert.That(target.Name).IsEqualTo("alpha");
        await Assert.That(target.Count).IsEqualTo(3);
    }

    public sealed class SampleState
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
