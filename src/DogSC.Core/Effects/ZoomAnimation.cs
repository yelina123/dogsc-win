using System;
using DogSC.Core.Models;

namespace DogSC.Core.Effects;

/// <summary>
/// 缩放动画
/// </summary>
public class ZoomAnimation
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public double StartScale { get; set; } = 1.0;
    public double EndScale { get; set; } = 1.5;
    public double CenterX { get; set; } = 0.5;
    public double CenterY { get; set; } = 0.5;
    public EasingCurve Easing { get; set; } = EasingCurve.EaseInOut;

    public double GetScaleAt(TimeSpan time)
    {
        if (time < Range.Start) return StartScale;
        if (time > Range.End) return EndScale;
        var progress = (time - Range.Start).TotalMilliseconds / Range.Duration.TotalMilliseconds;
        return StartScale + (EndScale - StartScale) * EasingUtilities.ApplyEasing(progress, Easing);
    }
}

public static class EasingUtilities
{
    public static double ApplyEasing(double t, EasingCurve curve) =>
        curve switch
        {
            EasingCurve.Linear => t,
            EasingCurve.EaseIn => t * t,
            EasingCurve.EaseOut => 1 - (1 - t) * (1 - t),
            EasingCurve.EaseInOut => t < 0.5 ? 2 * t * t : 1 - Math.Pow(-2 * t + 2, 2) / 2,
            EasingCurve.Bounce => BounceOut(t),
            EasingCurve.Elastic => ElasticOut(t),
            _ => t
        };

    private static double BounceOut(double t)
    {
        const double n1 = 7.5625;
        const double d1 = 2.75;
        if (t < 1 / d1) return n1 * t * t;
        if (t < 2 / d1) return n1 * (t -= 1.5 / d1) * t + 0.75;
        if (t < 2.5 / d1) return n1 * (t -= 2.25 / d1) * t + 0.9375;
        return n1 * (t -= 2.625 / d1) * t + 0.984375;
    }

    private static double ElasticOut(double t)
    {
        const double c4 = (2 * Math.PI) / 3;
        return t == 0 ? 0 : t == 1 ? 1 :
            Math.Pow(2, -10 * t) * Math.Sin((t * 10 - 0.75) * c4) + 1;
    }
}
