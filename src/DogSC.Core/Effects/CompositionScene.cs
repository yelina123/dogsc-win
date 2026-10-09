using System;
using System.Collections.Generic;
using System.Windows;

namespace DogSC.Core.Effects;

/// <summary>
/// 合成场景 - 描述某一时刻所有元素的合成状态
/// </summary>
public class CompositionScene
{
    public TimeSpan Time { get; set; }
    public FrameScene ScreenFrame { get; set; } = new();
    public FrameScene? CameraFrame { get; set; }
    public CursorRenderState Cursor { get; set; } = new();
    public List<OverlayElement> Overlays { get; set; } = new();
    public double BackgroundOpacity { get; set; } = 1.0;
    public string BackgroundColor { get; set; } = "#000000";
}

/// <summary>
/// 帧场景 - 单个视频帧的渲染状态
/// </summary>
public class FrameScene
{
    public double Scale { get; set; } = 1.0;
    public double OffsetX { get; set; } = 0.5;
    public double OffsetY { get; set; } = 0.5;
    public double Rotation { get; set; } = 0.0;
    public double Tilt { get; set; } = 0.0;
    public double Perspective { get; set; } = 0.0;
    public double CornerRadius { get; set; } = 0.0;
    public double ShadowBlur { get; set; } = 0.0;
    public double ShadowOpacity { get; set; } = 0.3;
    public Point ShadowOffset { get; set; } = new(0, 10);
    public double BorderWidth { get; set; } = 0.0;
    public string BorderColor { get; set; } = "#FFFFFF";
    public double Padding { get; set; } = 0.0;
    public string? DeviceFrame { get; set; }
}

/// <summary>
/// 光标渲染状态
/// </summary>
public class CursorRenderState
{
    public Point Position { get; set; }
    public bool IsVisible { get; set; } = true;
    public double Size { get; set; } = 1.0;
    public bool ShowClickRipple { get; set; } = false;
    public double RippleProgress { get; set; } = 0.0;
}

/// <summary>
/// 叠加元素
/// </summary>
public class OverlayElement
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = "image";
    public string SourcePath { get; set; } = "";
    public Rect Bounds { get; set; }
    public double Opacity { get; set; } = 1.0;
    public double Rotation { get; set; } = 0.0;
    public double CornerRadius { get; set; } = 0.0;
}
