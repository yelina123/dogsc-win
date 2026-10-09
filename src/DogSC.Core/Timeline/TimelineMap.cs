using System;
using System.Collections.Generic;
using System.Linq;
using DogSC.Core.Models;
using DogSC.Core.Timeline;

namespace DogSC.Core.Timeline;

/// <summary>
/// 时间线映射 - 处理时间坐标转换
/// </summary>
public class TimelineMap
{
    public double PixelsPerSecond { get; set; } = 100.0;
    public double ScrollOffset { get; set; } = 0.0;

    public double TimeToX(TimeSpan time) => time.TotalSeconds * PixelsPerSecond - ScrollOffset;

    public TimeSpan XToTime(double x) => TimeSpan.FromSeconds((x + ScrollOffset) / PixelsPerSecond);

    public double ClipToX(TimelineClip clip) => TimeToX(clip.TimelineRange.Start);

    public double ClipWidth(TimelineClip clip) => clip.TimelineRange.Duration.TotalSeconds * PixelsPerSecond;

    public TimeSpan SnapToGrid(TimeSpan time, double snapThreshold = 0.05)
    {
        var seconds = Math.Round(time.TotalSeconds / snapThreshold) * snapThreshold;
        return TimeSpan.FromSeconds(seconds);
    }

    public TimeSpan SnapToClipEdges(TimeSpan time, IEnumerable<TimelineClip> clips, double thresholdPixels = 10)
    {
        var thresholdSeconds = thresholdPixels / PixelsPerSecond;
        var bestSnap = time;
        var bestDistance = double.MaxValue;

        foreach (var clip in clips)
        {
            var startDistance = Math.Abs((clip.TimelineRange.Start - time).TotalSeconds);
            var endDistance = Math.Abs((clip.TimelineRange.End - time).TotalSeconds);

            if (startDistance < thresholdSeconds && startDistance < bestDistance)
            {
                bestDistance = startDistance;
                bestSnap = clip.TimelineRange.Start;
            }

            if (endDistance < thresholdSeconds && endDistance < bestDistance)
            {
                bestDistance = endDistance;
                bestSnap = clip.TimelineRange.End;
            }
        }

        return bestSnap;
    }
}

/// <summary>
/// 轨道排序
/// </summary>
public static class TimelineTrackOrdering
{
    public static readonly Dictionary<TrackType, int> DefaultOrder = new()
    {
        { TrackType.Video, 0 },
        { TrackType.Camera, 1 },
        { TrackType.Overlay, 2 },
        { TrackType.Sticker, 3 },
        { TrackType.Cursor, 4 },
        { TrackType.Audio, 5 }
    };

    public static int GetOrder(TrackType type) =>
        DefaultOrder.TryGetValue(type, out var order) ? order : 99;
}
