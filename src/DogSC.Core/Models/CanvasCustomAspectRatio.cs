using System;

namespace DogSC.Core.Models;

/// <summary>
/// 画布自定义比例
/// </summary>
public struct CanvasCustomAspectRatio : IEquatable<CanvasCustomAspectRatio>
{
    public double Width { get; }
    public double Height { get; }
    public string Label { get; }

    public CanvasCustomAspectRatio(double width, double height, string label)
    {
        Width = width;
        Height = height;
        Label = label;
    }

    public double Ratio => Width / Height;

    public static readonly CanvasCustomAspectRatio Original = new(0, 0, "原始");
    public static readonly CanvasCustomAspectRatio Landscape16x9 = new(16, 9, "16:9");
    public static readonly CanvasCustomAspectRatio Portrait9x16 = new(9, 16, "9:16");
    public static readonly CanvasCustomAspectRatio Square1x1 = new(1, 1, "1:1");
    public static readonly CanvasCustomAspectRatio Landscape4x3 = new(4, 3, "4:3");
    public static readonly CanvasCustomAspectRatio Portrait3x4 = new(3, 4, "3:4");
    public static readonly CanvasCustomAspectRatio Story9x19 = new(9, 19, "9:19");
    public static readonly CanvasCustomAspectRatio YouTubeShorts = new(9, 16, "Shorts");
    public static readonly CanvasCustomAspectRatio TikTok = new(9, 16, "TikTok");

    public static CanvasCustomAspectRatio[] Presets => new[]
    {
        Original,
        Landscape16x9,
        Portrait9x16,
        Square1x1,
        Landscape4x3,
        Portrait3x4,
        Story9x19
    };

    public bool Equals(CanvasCustomAspectRatio other) =>
        Width.Equals(other.Width) && Height.Equals(other.Height);

    public override bool Equals(object? obj) => obj is CanvasCustomAspectRatio other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Width, Height);

    public override string ToString() => Label;
}
