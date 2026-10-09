using System;
using System.Windows;
using DogSC.Core.Models;

namespace DogSC.Core.Effects;

/// <summary>
/// 聚焦虚化效果
/// </summary>
public class FocusEffect
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public MediaTimeRange Range { get; set; }
    public Point FocusPoint { get; set; } = new(0.5, 0.5);
    public double FocusRadius { get; set; } = 0.3;
    public double BlurAmount { get; set; } = 20.0;
    public double TransitionDuration { get; set; } = 0.5;
    public FocusShape Shape { get; set; } = FocusShape.Circle;
}

public enum FocusShape
{
    Circle,
    Rectangle,
    Linear
}
