using System.Collections.ObjectModel;
using DogSC.Core.Timeline;

namespace DogSC.Core.Project;

/// <summary>
/// 项目时间线
/// </summary>
public class ProjectTimeline
{
    public ObservableCollection<TimelineTrack> Tracks { get; set; } = new();
    public double ZoomLevel { get; set; } = 1.0;
    public double ScrollPosition { get; set; }
    public int SnapThreshold { get; set; } = 10;
    public bool IsSnappingEnabled { get; set; } = true;
    public bool IsWaveformVisible { get; set; } = true;
    public bool IsThumbnailVisible { get; set; } = true;
}
