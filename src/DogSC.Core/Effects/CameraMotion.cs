using System;
using DogSC.Core.Models;

namespace DogSC.Core.Effects;

/// <summary>
/// 相机运动（摄像头画面动画）
/// </summary>
public class CameraMotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public double StartScale { get; set; } = 1.0;
    public double EndScale { get; set; } = 1.0;
    public double StartX { get; set; } = 0.5;
    public double StartY { get; set; } = 0.5;
    public double EndX { get; set; } = 0.5;
    public double EndY { get; set; } = 0.5;
    public EasingCurve Easing { get; set; } = EasingCurve.EaseInOut;
}

/// <summary>
/// 元素运动（贴图等元素的动画）
/// </summary>
public class ElementMotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public double StartOpacity { get; set; } = 1.0;
    public double EndOpacity { get; set; } = 1.0;
    public double StartScale { get; set; } = 1.0;
    public double EndScale { get; set; } = 1.0;
    public double StartRotation { get; set; } = 0.0;
    public double EndRotation { get; set; } = 0.0;
    public EasingCurve Easing { get; set; } = EasingCurve.EaseInOut;
}

/// <summary>
/// 贴图过渡效果
/// </summary>
public class StickerTransition
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public TransitionType Type { get; set; } = TransitionType.Fade;
    public double Duration { get; set; } = 0.3;
}

public enum TransitionType
{
    None,
    Fade,
    Slide,
    Zoom,
    Bounce
}
