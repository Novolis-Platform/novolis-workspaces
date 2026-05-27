namespace Novolis.Timeline;

/// <summary>Timeline operation failed.</summary>
public sealed class TimelineException : Exception
{
    public TimelineException(string message) : base(message) { }

    public TimelineException(string message, Exception inner) : base(message, inner) { }
}
