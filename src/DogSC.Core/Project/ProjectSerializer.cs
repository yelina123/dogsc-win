using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DogSC.Core.Models;
using DogSC.Core.Timeline;

namespace DogSC.Core.Project;

/// <summary>
/// 项目文件序列化器 - 处理 .dogscproject 文件的保存与加载
/// </summary>
public class ProjectSerializer
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new TimeSpanJsonConverter() }
    };

    public async Task SaveAsync(ProjectModel project, string filePath)
    {
        project.Touch();

        var dto = ProjectDto.FromModel(project);
        var json = JsonSerializer.Serialize(dto, _options);

        var projectDir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(projectDir) && !Directory.Exists(projectDir))
        {
            Directory.CreateDirectory(projectDir);
        }

        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<ProjectModel> LoadAsync(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("项目文件不存在", filePath);
        }

        var json = await File.ReadAllTextAsync(filePath);
        var dto = JsonSerializer.Deserialize<ProjectDto>(json, _options)
            ?? throw new InvalidDataException("项目文件格式无效");

        var project = dto.ToModel();
        project.FilePath = filePath;
        return project;
    }
}

/// <summary>
/// 项目 DTO - 用于序列化
/// </summary>
public class ProjectDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
    public string BackgroundColor { get; set; } = "#000000";
    public ProjectTimelineDto Timeline { get; set; } = new();
    public List<MediaAssetDto> Assets { get; set; } = new();
    public List<RecordingMarkerDto> Markers { get; set; } = new();
    public Dictionary<string, string> Metadata { get; set; } = new();

    public static ProjectDto FromModel(ProjectModel model)
    {
        return new ProjectDto
        {
            Id = model.Id,
            Name = model.Name,
            CreatedAt = model.CreatedAt,
            ModifiedAt = model.ModifiedAt,
            Width = model.Width,
            Height = model.Height,
            FrameRate = model.FrameRate,
            BackgroundColor = model.BackgroundColor,
            Timeline = ProjectTimelineDto.FromModel(model.Timeline),
            Assets = model.Media.Assets.Select(MediaAssetDto.FromModel).ToList(),
            Markers = model.Markers.Select(RecordingMarkerDto.FromModel).ToList(),
            Metadata = new Dictionary<string, string>(model.Metadata)
        };
    }

    public ProjectModel ToModel()
    {
        return new ProjectModel
        {
            Id = Id,
            Name = Name,
            CreatedAt = CreatedAt,
            ModifiedAt = ModifiedAt,
            Width = Width,
            Height = Height,
            FrameRate = FrameRate,
            BackgroundColor = BackgroundColor,
            Timeline = Timeline.ToModel(),
            Media = new ProjectMediaModel
            {
                Assets = Assets.Select(a => a.ToModel()).ToList()
            },
            Markers = Markers.Select(m => m.ToModel()).ToList(),
            Metadata = new Dictionary<string, string>(Metadata)
        };
    }
}

public class ProjectTimelineDto
{
    public List<TimelineTrackDto> Tracks { get; set; } = new();
    public double ZoomLevel { get; set; } = 1.0;
    public double ScrollPosition { get; set; }
    public bool IsSnappingEnabled { get; set; } = true;

    public static ProjectTimelineDto FromModel(ProjectTimeline model)
    {
        return new ProjectTimelineDto
        {
            Tracks = model.Tracks.Select(TimelineTrackDto.FromModel).ToList(),
            ZoomLevel = model.ZoomLevel,
            ScrollPosition = model.ScrollPosition,
            IsSnappingEnabled = model.IsSnappingEnabled
        };
    }

    public ProjectTimeline ToModel()
    {
        var model = new ProjectTimeline
        {
            ZoomLevel = ZoomLevel,
            ScrollPosition = ScrollPosition,
            IsSnappingEnabled = IsSnappingEnabled
        };
        foreach (var trackDto in Tracks)
        {
            model.Tracks.Add(trackDto.ToModel());
        }
        return model;
    }
}

public class TimelineTrackDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Type { get; set; } = "Video";
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; }
    public double Volume { get; set; } = 1.0;
    public int TrackOrder { get; set; }
    public List<TimelineClipDto> Clips { get; set; } = new();

    public static TimelineTrackDto FromModel(TimelineTrack model)
    {
        return new TimelineTrackDto
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type.ToString(),
            IsVisible = model.IsVisible,
            IsLocked = model.IsLocked,
            Volume = model.Volume,
            TrackOrder = model.TrackOrder,
            Clips = model.Clips.Select(TimelineClipDto.FromModel).ToList()
        };
    }

    public TimelineTrack ToModel()
    {
        var track = new TimelineTrack(Enum.Parse<TrackType>(Type), Name)
        {
            Id = Id,
            IsVisible = IsVisible,
            IsLocked = IsLocked,
            Volume = Volume,
            TrackOrder = TrackOrder
        };
        foreach (var clipDto in Clips)
        {
            track.Clips.Add(clipDto.ToModel());
        }
        return track;
    }
}

public class TimelineClipDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public double SourceStartSeconds { get; set; }
    public double SourceDurationSeconds { get; set; }
    public double TimelineStartSeconds { get; set; }
    public double TimelineDurationSeconds { get; set; }
    public string MediaFilePath { get; set; } = "";
    public double Speed { get; set; } = 1.0;
    public bool IsMuted { get; set; }
    public double Volume { get; set; } = 1.0;
    public int TrackIndex { get; set; }

    public static TimelineClipDto FromModel(TimelineClip model)
    {
        return new TimelineClipDto
        {
            Id = model.Id,
            Name = model.Name,
            SourceStartSeconds = model.SourceRange.Start.TotalSeconds,
            SourceDurationSeconds = model.SourceRange.Duration.TotalSeconds,
            TimelineStartSeconds = model.TimelineRange.Start.TotalSeconds,
            TimelineDurationSeconds = model.TimelineRange.Duration.TotalSeconds,
            MediaFilePath = model.MediaFilePath,
            Speed = model.Speed,
            IsMuted = model.IsMuted,
            Volume = model.Volume,
            TrackIndex = model.TrackIndex
        };
    }

    public TimelineClip ToModel()
    {
        return new TimelineClip
        {
            Id = Id,
            Name = Name,
            SourceRange = new MediaTimeRange(SourceStartSeconds, SourceDurationSeconds),
            TimelineRange = new MediaTimeRange(TimelineStartSeconds, TimelineDurationSeconds),
            MediaFilePath = MediaFilePath,
            Speed = Speed,
            IsMuted = IsMuted,
            Volume = Volume,
            TrackIndex = TrackIndex
        };
    }
}

public class MediaAssetDto
{
    public string Id { get; set; } = "";
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public double DurationSeconds { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
    public bool HasAudio { get; set; }
    public string Type { get; set; } = "Video";

    public static MediaAssetDto FromModel(MediaAsset model)
    {
        return new MediaAssetDto
        {
            Id = model.Id,
            FilePath = model.FilePath,
            FileName = model.FileName,
            DurationSeconds = model.Duration.TotalSeconds,
            Width = model.Width,
            Height = model.Height,
            FrameRate = model.FrameRate,
            HasAudio = model.HasAudio,
            Type = model.Type.ToString()
        };
    }

    public MediaAsset ToModel()
    {
        return new MediaAsset
        {
            Id = Id,
            FilePath = FilePath,
            FileName = FileName,
            Duration = TimeSpan.FromSeconds(DurationSeconds),
            Width = Width,
            Height = Height,
            FrameRate = FrameRate,
            HasAudio = HasAudio,
            Type = Enum.Parse<MediaType>(Type)
        };
    }
}

public class RecordingMarkerDto
{
    public string Id { get; set; } = "";
    public double TimeSeconds { get; set; }
    public string Note { get; set; } = "";
    public string Color { get; set; } = "#FF6B6B";
    public int TrackIndex { get; set; }

    public static RecordingMarkerDto FromModel(RecordingMarker model)
    {
        return new RecordingMarkerDto
        {
            Id = model.Id,
            TimeSeconds = model.Time.TotalSeconds,
            Note = model.Note,
            Color = model.Color,
            TrackIndex = model.TrackIndex
        };
    }

    public RecordingMarker ToModel()
    {
        return new RecordingMarker
        {
            Id = Id,
            Time = TimeSpan.FromSeconds(TimeSeconds),
            Note = Note,
            Color = Color,
            TrackIndex = TrackIndex
        };
    }
}

/// <summary>
/// TimeSpan JSON 转换器
/// </summary>
public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return TimeSpan.FromSeconds(reader.GetDouble());
    }

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.TotalSeconds);
    }
}
