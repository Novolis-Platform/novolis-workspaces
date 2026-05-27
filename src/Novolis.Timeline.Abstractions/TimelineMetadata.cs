namespace Novolis.Timeline;

/// <summary>User-facing metadata for a timeline node (save point / restore point).</summary>
public sealed record TimelineMetadata(
    string? Label,
    string Kind,
    IReadOnlyDictionary<string, string> Properties);
