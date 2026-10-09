using System;
using System.Collections.Generic;
using System.Linq;
using DogSC.Core.Models;
using DogSC.Core.Project;

namespace DogSC.Core.Timeline;

/// <summary>
/// 项目时间线编辑操作
/// </summary>
public class ProjectTimelineEditing
{
    private readonly ProjectModel _project;

    public ProjectTimelineEditing(ProjectModel project)
    {
        _project = project;
    }

    public void SplitClip(string clipId, TimeSpan atTime)
    {
        foreach (var track in _project.Timeline.Tracks)
        {
            var clip = track.Clips.FirstOrDefault(c => c.Id == clipId);
            if (clip == null) continue;

            if (!clip.TimelineRange.Contains(atTime) || atTime == clip.TimelineRange.Start || atTime == clip.TimelineRange.End)
                return;

            var splitOffset = atTime - clip.TimelineRange.Start;
            var newClip = new TimelineClip
            {
                Name = clip.Name + " 副本",
                MediaFilePath = clip.MediaFilePath,
                Speed = clip.Speed,
                IsMuted = clip.IsMuted,
                Volume = clip.Volume,
                TrackIndex = clip.TrackIndex,
                TimelineRange = new MediaTimeRange(atTime, clip.TimelineRange.End - atTime),
                SourceRange = new MediaTimeRange(
                    clip.SourceRange.Start + splitOffset,
                    clip.SourceRange.End - clip.SourceRange.Start - splitOffset)
            };

            clip.TimelineRange = new MediaTimeRange(clip.TimelineRange.Start, splitOffset);

            var index = track.Clips.IndexOf(clip);
            track.Clips.Insert(index + 1, newClip);
            break;
        }
    }

    public void DeleteClip(string clipId)
    {
        foreach (var track in _project.Timeline.Tracks)
        {
            var clip = track.Clips.FirstOrDefault(c => c.Id == clipId);
            if (clip != null)
            {
                track.Clips.Remove(clip);
                break;
            }
        }
    }

    public void MoveClip(string clipId, TimeSpan newStart, int newTrackIndex)
    {
        TimelineClip? clip = null;
        var oldTrack = _project.Timeline.Tracks.FirstOrDefault(t =>
        {
            clip = t.Clips.FirstOrDefault(c => c.Id == clipId);
            return clip != null;
        });

        if (clip == null || oldTrack == null) return;

        var duration = clip.TimelineRange.Duration;
        clip.TimelineRange = new MediaTimeRange(newStart, duration);
        clip.TrackIndex = newTrackIndex;

        if (oldTrack.Tracks?[newTrackIndex] is { } newTrack && newTrack != oldTrack)
        {
            oldTrack.Clips.Remove(clip);
            newTrack.Clips.Add(clip);
        }
    }

    public void TrimClipStart(string clipId, TimeSpan newStart)
    {
        var clip = FindClip(clipId);
        if (clip == null) return;

        if (newStart >= clip.TimelineRange.End) return;

        var delta = newStart - clip.TimelineRange.Start;
        clip.TimelineRange = new MediaTimeRange(newStart, clip.TimelineRange.End - newStart);
        clip.SourceRange = new MediaTimeRange(
            clip.SourceRange.Start + delta,
            clip.SourceRange.Duration - delta);
    }

    public void TrimClipEnd(string clipId, TimeSpan newEnd)
    {
        var clip = FindClip(clipId);
        if (clip == null) return;

        if (newEnd <= clip.TimelineRange.Start) return;

        clip.TimelineRange = new MediaTimeRange(clip.TimelineRange.Start, newEnd - clip.TimelineRange.Start);
    }

    public TimelineClip? FindClip(string clipId)
    {
        return _project.Timeline.Tracks
            .SelectMany(t => t.Clips)
            .FirstOrDefault(c => c.Id == clipId);
    }
}

/// <summary>
/// 项目时间线编辑变换
/// </summary>
public class ProjectTimelineEditingTransforms
{
    public static MediaTimeRange ApplyRetime(MediaTimeRange range, double speed)
    {
        var newDuration = TimeSpan.FromSeconds(range.Duration.TotalSeconds / speed);
        return new MediaTimeRange(range.Start, newDuration);
    }
}

/// <summary>
/// 项目时间线变速
/// </summary>
public class ProjectTimelineRetime
{
    public void SetSpeed(string clipId, double speed)
    {
        var editing = new ProjectTimelineEditing(null!);
        var clip = editing.FindClip(clipId);
        if (clip == null) return;

        var oldDuration = clip.TimelineRange.Duration;
        clip.Speed = speed;
        var newDuration = TimeSpan.FromSeconds(oldDuration.TotalSeconds / speed);
        clip.TimelineRange = new MediaTimeRange(clip.TimelineRange.Start, newDuration);
    }
}
