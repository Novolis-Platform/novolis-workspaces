using Novolis.Snapshots;
using Novolis.Timeline;

namespace Novolis.Workspaces.Timeline;

/// <summary>Request to create a workspace save point.</summary>
public sealed record SavePointRequest(
    string? Label,
    string Kind,
    IReadOnlyDictionary<string, string>? Properties = null)
{
    public TimelineMetadata ToMetadata() =>
        new(Label, Kind, Properties ?? new Dictionary<string, string>());

    public SnapshotRequest ToSnapshotRequest() =>
        new(Label, Kind, Properties);
}
