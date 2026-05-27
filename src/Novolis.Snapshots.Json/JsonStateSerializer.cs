using System.Reflection;
using System.Text.Json;

namespace Novolis.Snapshots.Json;

/// <summary>JSON <see cref="IStateSerializer{TState}"/> using <see cref="System.Text.Json"/>.</summary>
public sealed class JsonStateSerializer<TState> : IStateSerializer<TState>
    where TState : class
{
    private readonly JsonSerializerOptions _options;

    public JsonStateSerializer(JsonSerializerOptions? options = null) =>
        _options = options ?? new JsonSerializerOptions { WriteIndented = false };

    public async ValueTask WriteAsync(TState state, Stream destination, CancellationToken cancellationToken = default) =>
        await JsonSerializer.SerializeAsync(destination, state, _options, cancellationToken).ConfigureAwait(false);

    public async ValueTask ReadAsync(TState target, Stream source, CancellationToken cancellationToken = default)
    {
        var loaded = await JsonSerializer.DeserializeAsync<TState>(source, _options, cancellationToken).ConfigureAwait(false);
        if (loaded is null)
            throw new SnapshotException($"Failed to deserialize {typeof(TState).Name} from JSON.");

        CopyInto(loaded, target);
    }

    private static void CopyInto(TState source, TState target)
    {
        if (target is IDictionary<string, string> targetDict && source is IDictionary<string, string> sourceDict)
        {
            targetDict.Clear();
            foreach (var pair in sourceDict)
                targetDict[pair.Key] = pair.Value;
            return;
        }

        foreach (var prop in typeof(TState).GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!prop.CanWrite || prop.GetIndexParameters().Length > 0)
                continue;
            prop.SetValue(target, prop.GetValue(source));
        }
    }
}
