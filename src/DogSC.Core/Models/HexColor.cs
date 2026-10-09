using System;
using System.Globalization;
using System.Windows.Media;

namespace DogSC.Core.Models;

/// <summary>
/// 十六进制颜色值
/// </summary>
public readonly struct HexColor : IEquatable<HexColor>
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public HexColor(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static HexColor FromString(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
        {
            var r = byte.Parse(hex[..2], NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new HexColor(r, g, b);
        }
        if (hex.Length == 8)
        {
            var r = byte.Parse(hex[..2], NumberStyles.HexNumber);
            var g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            var b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            var a = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);
            return new HexColor(r, g, b, a);
        }
        return new HexColor(0, 0, 0);
    }

    public Color ToColor() => Color.FromArgb(A, R, G, B);

    public override string ToString() => $"#{R:X2}{G:X2}{B:X2}{A:X2}";

    public bool Equals(HexColor other) => R == other.R && G == other.G && B == other.B && A == other.A;

    public override bool Equals(object? obj) => obj is HexColor other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(R, G, B, A);
}
