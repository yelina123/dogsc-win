using System;
using System.Collections.Generic;

namespace DogSC.Core.Project;

/// <summary>
/// 项目模型
/// </summary>
public class ProjectModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "未命名项目";
    public string FilePath { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public int Width { get; set; } = 1920;
    public int Height { set; get; } = 1080;
    public double FrameRate { get; set; } = 60.0;
    public string BackgroundColor { get; set; } = "#000000";
    public ProjectMediaModel Media { get; set; } = new();
    public ProjectTimeline Timeline { get; set; } = new();
    public List<RecordingMarker> Markers { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();

    public void Touch() => ModifiedAt = DateTime.Now;
}

/// <summary>
/// 项目媒体文件
/// </summary>
public class ProjectMediaModel
{
    public List<MediaAsset> Assets { get; set; } = new();
}

public class MediaAsset
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
    public bool HasAudio { get; set; }
    public MediaType Type { get; set; }
    public string ThumbnailPath { get; set; } = "";
}

public enum MediaType
{
    Video,
    Audio,
    Image,
    Camera
}
