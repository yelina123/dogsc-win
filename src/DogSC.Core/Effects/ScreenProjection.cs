using System;
using System.Windows;

namespace DogSC.Core.Effects;

/// <summary>
/// 屏幕投影 - 处理屏幕画面的 3D 投影变换
/// </summary>
public class ScreenProjection
{
    public double FOV { get; set; } = 60.0;
    public double TiltX { get; set; } = 0.0;
    public double TiltY { get; set; } = 0.0;
    public double TiltZ { get; set; } = 0.0;
    public double Perspective { get; set; } = 0.0;

    public Point ProjectPoint(Point point, double width, double height)
    {
        var cx = width / 2.0;
        var cy = height / 2.0;

        var x = point.X - cx;
        var y = point.Y - cy;

        var tiltXRad = TiltX * Math.PI / 180.0;
        var tiltYRad = TiltY * Math.PI / 180.0;

        var newY = y * Math.Cos(tiltXRad) - 0 * Math.Sin(tiltXRad);
        var newX = x * Math.Cos(tiltYRad) - 0 * Math.Sin(tiltYRad);

        var perspectiveFactor = 1.0 / (1.0 + (newY / height) * Perspective);

        return new Point(
            cx + newX * perspectiveFactor,
            cy + newY * perspectiveFactor);
    }

    public Rect ProjectRect(Rect rect, double width, double height)
    {
        var topLeft = ProjectPoint(rect.TopLeft, width, height);
        var topRight = ProjectPoint(rect.TopRight, width, height);
        var bottomLeft = ProjectPoint(rect.BottomLeft, width, height);
        var bottomRight = ProjectPoint(rect.BottomRight, width, height);

        var minX = Math.Min(Math.Min(topLeft.X, topRight.X), Math.Min(bottomLeft.X, bottomRight.X));
        var maxX = Math.Max(Math.Max(topLeft.X, topRight.X), Math.Max(bottomLeft.X, bottomRight.X));
        var minY = Math.Min(Math.Min(topLeft.Y, topRight.Y), Math.Min(bottomLeft.Y, bottomRight.Y));
        var maxY = Math.Max(Math.Max(topLeft.Y, topRight.Y), Math.Max(bottomLeft.Y, bottomRight.Y));

        return new Rect(minX, minY, maxX - minX, maxY - minY);
    }
}

/// <summary>
/// 屏幕帧默认值
/// </summary>
public static class ScreenFrameDefaults
{
    public static double DefaultCornerRadius => 12.0;
    public static double DefaultShadowBlur => 24.0;
    public static double DefaultShadowOpacity => 0.25;
    public static Point DefaultShadowOffset => new(0, 16);
    public static double DefaultPadding => 40.0;
    public static double DefaultBorderWidth => 0.0;
}

/// <summary>
/// 屏幕运动创建样式
/// </summary>
public enum ScreenMotionCreationStyle
{
    AutoFollow,
    ManualZoom,
    ThreeDTilt,
    Preset
}
