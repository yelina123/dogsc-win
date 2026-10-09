namespace DogSC.Core.Export;

/// <summary>
/// 导出设置
/// </summary>
public class ExportSettings
{
    public ExportFormat Format { get; set; } = ExportFormat.MP4;
    public ExportResolution Resolution { get; set; } = ExportResolution.Source;
    public int FrameRate { get; set; } = 60;
    public int VideoBitrate { get; set; } = 10_000_000;
    public int AudioBitrate { get; set; } = 192_000;
    public string OutputPath { get; set; } = "";
    public bool HardwareAcceleration { get; set; } = true;
}

public enum ExportFormat
{
    MP4,
    MKV,
    MOV,
    M4A
}

public enum ExportResolution
{
    Source,
    _1080p,
    _1440p,
    _4K
}

public static class ResolutionExtensions
{
    public static (int width, int height)? GetResolution(this ExportResolution resolution) =>
        resolution switch
        {
            ExportResolution._1080p => (1920, 1080),
            ExportResolution._1440p => (2560, 1440),
            ExportResolution._4K => (3840, 2160),
            _ => null
        };
}
