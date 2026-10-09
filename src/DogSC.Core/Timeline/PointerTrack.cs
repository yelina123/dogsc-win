using System;
using System.Collections.Generic;
using System.Windows;

namespace DogSC.Core.Timeline;

/// <summary>
/// 光标轨迹点
/// </summary>
public class PointerTrackPoint
{
    public TimeSpan Time { get; set; }
    public Point Position { get; set; }
    public bool IsClick { get; set; }
    public bool IsRightClick { get; set; }
    public int ScrollDelta { get; set; }
}

/// <summary>
/// 光标轨迹
/// </summary>
public class PointerTrack
{
    public List<PointerTrackPoint> Points { get; set; } = new();
    public double Smoothness { get; set; } = 0.5;
    public bool IsVisible { get; set; } = true;
    public double CursorSize { get; set; } = 1.0;
    public CursorStyle Style { get; set; } = CursorStyle.System;
    public bool HideWhenIdle { get; set; } = false;
    public TimeSpan IdleHideDelay { get; set; } = TimeSpan.FromSeconds(3);
    public bool ShowClickRipple { get; set; } = true;

    public PointerTrackPoint? GetPointAt(TimeSpan time)
    {
        if (Points.Count == 0) return null;
        PointerTrackPoint? prev = null;
        foreach (var point in Points)
        {
            if (point.Time > time) return prev;
            prev = point;
        }
        return prev;
    }
}

public enum CursorStyle
{
    System,
    Arrow,
    Hand,
    Custom
}
