using System;
using DogSC.Core.Models;

namespace DogSC.Core.Effects;

/// <summary>
/// 缩放过渡分辨率
/// </summary>
public class ZoomTransitionResolution
{
    public double RenderScale { get; set; } = 1.0;
    public bool UseHighQualityScaling { get; set; } = true;
    public bool UseMipmaps { get; set; } = true;
    public string ScalingMode { get; set; } = "HighQuality";

    public static ZoomTransitionResolution Preview => new()
    {
        RenderScale = 0.5,
        UseHighQualityScaling = false,
        UseMipmaps = false,
        ScalingMode = "Fast"
    };

    public static ZoomTransitionResolution Export => new()
    {
        RenderScale = 1.0,
        UseHighQualityScaling = true,
        UseMipmaps = true,
        ScalingMode = "HighQuality"
    };
}

/// <summary>
/// 相机内容填充方式
/// </summary>
public enum CameraContentFill
{
    Stretch,
    Fit,
    Cover,
    Crop
}
