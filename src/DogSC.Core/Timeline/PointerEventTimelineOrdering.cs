using System;
using System.Collections.Generic;
using System.Linq;

namespace DogSC.Core.Timeline;

/// <summary>
/// 指针事件时间线排序
/// </summary>
public static class PointerEventTimelineOrdering
{
    public static List<PointerTrackPoint> OrderByTime(IEnumerable<PointerTrackPoint> points)
    {
        return points.OrderBy(p => p.Time).ToList();
    }
}

/// <summary>
/// 指针时间线对齐
/// </summary>
public static class PointerTimelineAlignment
{
    public static void AlignToRecordingStart(List<PointerTrackPoint> points, TimeSpan recordingStart)
    {
        foreach (var point in points)
        {
            point.Time -= recordingStart;
        }
    }

    public static void AlignToClipStart(List<PointerTrackPoint> points, TimeSpan clipStart, TimeSpan sourceClipStart)
    {
        var offset = clipStart - sourceClipStart;
        foreach (var point in points)
        {
            point.Time += offset;
        }
    }
}

/// <summary>
/// 项目指针轨道
/// </summary>
public class ProjectPointerTrack
{
    public string TrackId { get; set; } = Guid.NewGuid().ToString();
    public List<PointerTrackPoint> Points { get; set; } = new();
    public double Smoothness { get; set; } = 0.5;
    public bool IsVisible { get; set; } = true;
    public double CursorSize { get; set; } = 1.0;
    public CursorStyle Style { get; set; } = CursorStyle.System;
    public bool HideWhenIdle { get; set; } = false;
    public TimeSpan IdleHideDelay { get; set; } = TimeSpan.FromSeconds(3);
    public bool ShowClickRipple { get; set; } = true;
}
