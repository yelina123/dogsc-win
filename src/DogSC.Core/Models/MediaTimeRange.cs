using System;

namespace DogSC.Core.Models;

/// <summary>
/// 时间范围，表示媒体中的一段时间区间
/// </summary>
public readonly struct MediaTimeRange : IEquatable<MediaTimeRange>
{
    public TimeSpan Start { get; }
    public TimeSpan Duration { get; }
    public TimeSpan End => Start + Duration;

    public MediaTimeRange(TimeSpan start, TimeSpan duration)
    {
        Start = start;
        Duration = duration;
    }

    public MediaTimeRange(double startSeconds, double durationSeconds)
        : this(TimeSpan.FromSeconds(startSeconds), TimeSpan.FromSeconds(durationSeconds))
    {
    }

    public bool Contains(TimeSpan time) => time >= Start && time <= End;

    public bool Overlaps(MediaTimeRange other) =>
        Start < other.End && other.Start < End;

    public MediaTimeRange Intersect(MediaTimeRange other)
    {
        var newStart = Start > other.Start ? Start : other.Start;
        var newEnd = End < other.End ? End : other.End;
        return newEnd > newStart
            ? new MediaTimeRange(newStart, newEnd - newStart)
            : new MediaTimeRange(TimeSpan.Zero, TimeSpan.Zero);
    }

    public bool Equals(MediaTimeRange other) =>
        Start.Equals(other.Start) && Duration.Equals(other.Duration);

    public override bool Equals(object? obj) => obj is MediaTimeRange other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Start, Duration);

    public static bool operator ==(MediaTimeRange left, MediaTimeRange right) => left.Equals(right);
    public static bool operator !=(MediaTimeRange left, MediaTimeRange right) => !left.Equals(right);

    public override string ToString() => $"[{Start.TotalSeconds:F2}s, {End.TotalSeconds:F2}s]";
}
