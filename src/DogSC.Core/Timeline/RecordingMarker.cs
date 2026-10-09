using System;

namespace DogSC.Core.Timeline;

/// <summary>
/// 录制标记/书签
/// </summary>
public class RecordingMarker
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TimeSpan Time { get; set; }
    public string Note { get; set; } = "";
    public string Color { get; set; } = "#FF6B6B";
    public int TrackIndex { get; set; }
}
