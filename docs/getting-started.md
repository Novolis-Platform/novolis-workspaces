# Getting started

## Create a workspace

```csharp
using System.IO.Abstractions;
using Novolis.Workspaces;
using Novolis.Workspaces.FileSystem;

var fs = new FileSystem();
var service = new WorkspaceFileSystemService(fs);
var workspace = await service.CreateAsync(@"C:\work\my-studio", "My Studio");
var project = await service.AddProjectAsync(workspace, "Voice Pack", ProjectKind.VoicePack);
```

## Save points and timeline

```csharp
using Novolis.Snapshots;
using Novolis.Timeline.FileSystem;
using Novolis.Workspaces.Snapshots;
using Novolis.Workspaces.Timeline;

var snapshotsRoot = fs.DirectoryInfo.New(Path.Combine(workspace.Root.FullName, ".novolis", "snapshots"));
var timelineRoot = fs.DirectoryInfo.New(WorkspaceLayout.TimelinePath(workspace.Root.FullName));

var store = new ZipWorkspaceSnapshotStore(fs, snapshotsRoot);
var timeline = new FileSystemTimeline<ZipSnapshotRef>(fs, timelineRoot);
var workspaceTimeline = new WorkspaceTimeline(timeline, store);

await workspaceTimeline.SavePointAsync(workspace, new SavePointRequest("Before export", SnapshotKinds.ExportCheckpoint));
```

## UI projection

```csharp
using Novolis.Timeline.Presentation;

var projector = new TimelineTreeProjector<ZipSnapshotRef>();
var rows = projector.ToRows(await timeline.GetNodesAsync(), await timeline.GetBranchesAsync(), await timeline.GetHeadAsync());
```

See `novolis-dogfooding/apps/workspaces/MinimalWorkspaceTimeline` for a full console walkthrough.
