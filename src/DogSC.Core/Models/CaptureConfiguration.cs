using System;
using System.Collections.Generic;

namespace DogSC.Core.Models;

/// <summary>
/// 录制配置
/// </summary>
public class CaptureConfiguration
{
    public CaptureSourceType SourceType { get; set; } = CaptureSourceType.Screen;
    public string? DisplayId { get; set; }
    public string? WindowId { get; set; }
    public System.Drawing.Rectangle RegionBounds { get; set; }
    public bool CaptureCursor { get; set; } = true;
    public bool CaptureMicrophone { get; set; } = true;
    public bool CaptureSystemAudio { get; set; } = true;
    public string? MicrophoneDeviceId { get; set; }
    public bool CaptureCamera { get; set; } = false;
    public string? CameraDeviceId { get; set; }
    public int FrameRate { get; set; } = 60;
    public int Width { get; set; } = 1920;
    public int Height { get; set; } = 1080;
}

public enum CaptureSourceType
{
    Screen,
    Window,
    Region
}
