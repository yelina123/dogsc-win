using System;
using System.Collections.Generic;
using DogSC.Core.Models;

namespace DogSC.Core.Effects;

/// <summary>
/// 屏幕运镜动画
/// </summary>
public class ScreenMotion
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public MotionType Type { get; set; } = MotionType.None;
    public double StartZoom { get; set; } = 1.0;
    public double EndZoom { get; set; } = 1.0;
    public double StartX { get; set; } = 0.5;
    public double StartY { get; set; } = 0.5;
    public double EndX { get; set; } = 0.5;
    public double EndY { get; set; } = 0.5;
    public double Tilt { get; set; } = 0.0;
    public double Perspective { get; set; } = 0.0;
    public EasingCurve Easing { get; set; } = EasingCurve.EaseInOut;
    public bool IsAutoFollow { get; set; } = false;
}

public enum MotionType
{
    None,
    Zoom,
    Pan,
    ZoomAndPan,
    AutoFollow,
    ThreeD
}

public enum EasingCurve
{
    Linear,
    EaseIn,
    EaseOut,
    EaseInOut,
    Bounce,
    Elastic
}
