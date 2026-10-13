using System;

namespace DogSC.Core.Models;

/// <summary>
/// 二维点 - 不依赖 WPF/WinForms
/// </summary>
public struct CorePoint : IEquatable<CorePoint>
{
    public double X { get; set; }
    public double Y { get; set; }

    public CorePoint(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static CorePoint operator +(CorePoint a, CorePoint b) => new(a.X + b.X, a.Y + b.Y);
    public static CorePoint operator -(CorePoint a, CorePoint b) => new(a.X - b.X, a.Y - b.Y);

    public bool Equals(CorePoint other) => X.Equals(other.X) && Y.Equals(other.Y);
    public override bool Equals(object? obj) => obj is CorePoint other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X:F1}, {Y:F1})";
}

/// <summary>
/// 矩形 - 不依赖 WPF/WinForms
/// </summary>
public struct CoreRect : IEquatable<CoreRect>
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }

    public CoreRect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public double Left => X;
    public double Top => Y;
    public double Right => X + Width;
    public double Bottom => Y + Height;
    public CorePoint TopLeft => new(X, Y);
    public CorePoint TopRight => new(X + Width, Y);
    public CorePoint BottomLeft => new(X, Y + Height);
    public CorePoint BottomRight => new(X + Width, Y + Height);
    public CorePoint Center => new(X + Width / 2, Y + Height / 2);

    public bool Contains(CorePoint point) =>
        point.X >= Left && point.X <= Right && point.Y >= Top && point.Y <= Bottom;

    public bool Equals(CoreRect other) =>
        X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);

    public override bool Equals(object? obj) => obj is CoreRect other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
    public override string ToString() => $"({X:F0}, {Y:F0}, {Width:F0}x{Height:F0})";
}
