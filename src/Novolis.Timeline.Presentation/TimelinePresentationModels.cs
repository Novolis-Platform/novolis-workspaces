namespace Novolis.Timeline.Presentation;

/// <summary>UI-safe metadata for a timeline node.</summary>
public sealed record TimelinePresentationMetadata(
    string Label,
    string Kind,
    IReadOnlyDictionary<string, string> Properties);
