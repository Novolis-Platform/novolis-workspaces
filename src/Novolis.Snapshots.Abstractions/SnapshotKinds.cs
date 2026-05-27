namespace Novolis.Snapshots;

/// <summary>Well-known <see cref="SnapshotRequest.Kind"/> values.</summary>
public static class SnapshotKinds
{
    public const string Manual = "manual";
    public const string Autosave = "autosave";
    public const string Safety = "safety";
    public const string Quick = "quick";
    public const string ExportCheckpoint = "export-checkpoint";
}
