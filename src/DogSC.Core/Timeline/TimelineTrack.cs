using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DogSC.Core.Models;

namespace DogSC.Core.Timeline;

/// <summary>
/// 轨道类型
/// </summary>
public enum TrackType
{
    Video,
    Audio,
    Camera,
    Cursor,
    Overlay,
    Sticker
}

/// <summary>
/// 时间线轨道
/// </summary>
public class TimelineTrack
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public TrackType Type { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; } = false;
    public double Volume { get; set; } = 1.0;
    public ObservableCollection<TimelineClip> Clips { get; set; } = new();
    public int TrackOrder { get; set; }

    public TimelineTrack(TrackType type, string name)
    {
        Type = type;
        Name = name;
    }
}

/// <summary>
/// 时间线上的片段
/// </summary>
public class TimelineClip
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public MediaTimeRange SourceRange { get; set; }
    public MediaTimeRange TimelineRange { get; set; }
    public string MediaFilePath { get; set; } = "";
    public double Speed { get; set; } = 1.0;
    public bool IsMuted { get; set; }
    public double Volume { get; set; } = 1.0;
    public List<ClipEffect> Effects { get; set; } = new();
    public List<RecordingMarker> Markers { get; set; } = new();
    public int TrackIndex { get; set; }

    public TimeSpan Duration => TimelineRange.Duration;
    public TimeSpan Start => TimelineRange.Start;
    public TimeSpan End => TimelineRange.End;
}

/// <summary>
/// 片段上的效果
/// </summary>
public class ClipEffect
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = "";
    public Dictionary<string, object> Parameters { get; set; } = new();
    public MediaTimeRange? EffectRange { get; set; }
}
