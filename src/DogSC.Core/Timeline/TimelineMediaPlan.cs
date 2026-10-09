using System;
using System.Collections.Generic;
using System.Linq;
using DogSC.Core.Models;
using DogSC.Core.Timeline;

namespace DogSC.Core.Timeline;

/// <summary>
/// 时间线媒体计划 - 规划某一时刻需要渲染哪些媒体
/// </summary>
public class TimelineMediaPlan
{
    public TimeSpan Time { get; set; }
    public List<MediaClipPlan> VideoClips { get; set; } = new();
    public List<MediaClipPlan> AudioClips { get; set; } = new();
    public List<OverlayClip> Overlays { get; set; } = new();
    public PointerTrackPoint? CursorPoint { get; set; }

    public static TimelineMediaPlan Build(
        MediaTimeline timeline,
        PointerTrack? pointerTrack,
        TimeSpan time)
    {
        var plan = new TimelineMediaPlan { Time = time };

        foreach (var clip in timeline.GetClipsAt(time))
        {
            var sourceOffset = time - clip.TimelineRange.Start + clip.SourceRange.Start;
            var clipPlan = new MediaClipPlan
            {
                Clip = clip,
                SourceTime = sourceOffset
            };

            if (clip.TrackIndex < 5)
                plan.VideoClips.Add(clipPlan);
            else
                plan.AudioClips.Add(clipPlan);
        }

        if (pointerTrack != null)
        {
            plan.CursorPoint = pointerTrack.GetPointAt(time);
        }

        return plan;
    }
}

public class MediaClipPlan
{
    public TimelineClip Clip { get; set; } = null!;
    public TimeSpan SourceTime { get; set; }
}
