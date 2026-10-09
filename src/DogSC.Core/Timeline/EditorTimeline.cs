using System;
using System.Collections.ObjectModel;
using System.Linq;
using DogSC.Core.Models;

namespace DogSC.Core.Timeline;

/// <summary>
/// 编辑器时间线
/// </summary>
public class EditorTimeline
{
    public ObservableCollection<TimelineTrack> Tracks { get; set; } = new();
    public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    public TimeSpan PlayheadPosition { get; set; } = TimeSpan.Zero;
    public double ZoomLevel { get; set; } = 1.0;
    public double ScrollPosition { get; set; }

    public TimelineClip? SelectedClip { get; set; }
    public List<TimelineClip> SelectedClips { get; set; } = new();

    public void AddTrack(TimelineTrack track)
    {
        track.TrackOrder = Tracks.Count;
        Tracks.Add(track);
    }

    public void RemoveTrack(string trackId)
    {
        var track = Tracks.FirstOrDefault(t => t.Id == trackId);
        if (track != null)
        {
            Tracks.Remove(track);
        }
    }

    public void SplitClip(TimelineClip clip, TimeSpan atTime)
    {
        if (!clip.TimelineRange.Contains(atTime) || atTime == clip.TimelineRange.Start || atTime == clip.TimelineRange.End)
            return;

        var splitOffset = atTime - clip.TimelineRange.Start;
        var newClip = new TimelineClip
        {
            Name = clip.Name,
            MediaFilePath = clip.MediaFilePath,
            Speed = clip.Speed,
            IsMuted = clip.IsMuted,
            Volume = clip.Volume,
            TrackIndex = clip.TrackIndex,
            TimelineRange = new MediaTimeRange(atTime, clip.TimelineRange.End - atTime),
            SourceRange = new MediaTimeRange(
                clip.SourceRange.Start + splitOffset,
                clip.SourceRange.End - atTime)
        };

        clip.TimelineRange = new MediaTimeRange(clip.TimelineRange.Start, splitOffset);
    }

    public void RecalculateDuration()
    {
        Duration = Tracks.SelectMany(t => t.Clips)
            .Select(c => c.TimelineRange.End)
            .DefaultIfEmpty(TimeSpan.Zero)
            .Max();
    }
}
