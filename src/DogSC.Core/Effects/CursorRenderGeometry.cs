using System;
using System.Windows;

namespace DogSC.Core.Effects;

/// <summary>
/// 光标渲染几何 - 计算光标在画面中的位置和变换
/// </summary>
public class CursorRenderGeometry
{
    public Point ScreenPosition { get; set; }
    public double ScreenWidth { get; set; } = 1920;
    public double ScreenHeight { get; set; } = 1080;
    public double CanvasWidth { get; set; } = 1920;
    public double CanvasHeight { get; set; } = 1080;
    public double Zoom { get; set; } = 1.0;
    public double PanX { get; set; } = 0.5;
    public double PanY { get; set; } = 0.5;

    public Point GetCanvasPosition()
    {
        var relativeX = ScreenPosition.X / ScreenWidth;
        var relativeY = ScreenPosition.Y / ScreenHeight;

        var zoomedX = (relativeX - PanX) * Zoom + PanX;
        var zoomedY = (relativeY - PanY) * Zoom + PanY;

        return new Point(zoomedX * CanvasWidth, zoomedY * CanvasHeight);
    }

    public double GetCursorScale() => 1.0 / Zoom;
}
