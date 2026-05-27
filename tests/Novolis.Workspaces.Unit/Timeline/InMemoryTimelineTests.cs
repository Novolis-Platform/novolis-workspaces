using Novolis.Snapshots;
using Novolis.Timeline;
using Novolis.Timeline.Memory;
using TUnit.Core;

namespace Novolis.Workspaces.Unit.Timeline;

public sealed class InMemoryTimelineTests
{
    [Test]
    public async Task Branch_CreatesAlternateHead()
    {
        var timeline = new InMemoryTimeline<MemorySnapshotRef>();
        var first = await timeline.AddAsync(
            new MemorySnapshotRef(Guid.NewGuid()),
            new TimelineMetadata("first", TimelineKinds.SavePoint, new Dictionary<string, string>()));

        var branch = await timeline.BranchAsync(new BranchName("experiment"), first.Id);
        var head = await timeline.GetHeadAsync();

        await Assert.That(head.NodesByBranch[branch.Id]).IsEqualTo(first.Id);
    }
}
