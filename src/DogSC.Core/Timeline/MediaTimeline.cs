using System;
using System.Collections.Generic;
using System.Linq;
using DogSC.Core.Models;
using DogSC.Core.Timeline;

namespace DogSC.Core.Timeline;

/// <summary>
/// 媒体时间线 - 管理所有媒体轨道的时间线布局
/// </summary>
public class MediaTimeline
{
    public List<TimelineTrack> Tracks { get; set; } = new();
    public TimeSpan Duration { get; set; }

    public void Rebuild()
    {
        Duration = TimeSpan.Zero;
        foreach (var track in Tracks)
        {
            foreach (var clip in track.Clips)
            {
                if (clip.TimelineRange.End > Duration)
                    Duration = clip.TimelineRange.End;
            }
        }
    }

    public IEnumerable<TimelineClip> GetClipsAt(TimeSpan time)
    {
        return Tracks.SelectMany(t => t.Clips)
            .Where(c => c.TimelineRange.Contains(time));
    }

    public IEnumerable<TimelineClip> GetClipsInRange(MediaTimeRange range)
    {
        return Tracks.SelectMany(t => t.Clips)
            .Where(c => c.TimelineRange.Overlaps(range));
    }
}

/// <summary>
/// 叠加层时间线 - 管理贴图、标注等叠加元素
/// </summary>
public class OverlayTimeline
{
    public List<OverlayTrack> Tracks { get; set; } = new();
}

public class OverlayTrack
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public bool IsVisible { get; set; } = true;
    public List<OverlayClip> Clips { get; set; } = new();
}

public class OverlayClip
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public string Type { get; set; } = "image";
    public string SourcePath { get; set; } = "";
    public double PositionX { get; set; } = 0.5;
    public double PositionY { get; set; } = 0.5;
    public double Scale { get; set; } = 1.0;
    public double Opacity { set; get; } = 1.0;
}
