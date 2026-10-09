using System;
using System.Collections.Generic;
using DogSC.Core.Models;
using DogSC.Core.Timeline;

namespace DogSC.Core.Effects;

/// <summary>
/// 自动缩放规划器 - 根据鼠标位置自动规划运镜
/// </summary>
public class AutoZoomPlanner
{
    public double ZoomStrength { get; set; } = 1.2;
    public double Smoothness { get; set; } = 0.05;
    public double MinZoom { get; set; } = 1.0;
    public double MaxZoom { get; set; } = 2.0;
    public double DeadZone { get; set; } = 0.1;

    public List<ScreenMotion> Plan(PointerTrack pointerTrack, TimeSpan totalDuration)
    {
        var motions = new List<ScreenMotion>();
        if (pointerTrack.Points.Count < 2) return motions;

        var currentMotion = new ScreenMotion
        {
            Type = MotionType.AutoFollow,
            IsAutoFollow = true,
            StartZoom = 1.0,
            EndZoom = ZoomStrength
        };

        var lastSignificantMove = pointerTrack.Points[0];
        var moveStart = pointerTrack.Points[0].Time;

        for (int i = 1; i < pointerTrack.Points.Count; i++)
        {
            var point = pointerTrack.Points[i];
            var distance = Math.Sqrt(
                Math.Pow(point.Position.X - lastSignificantMove.Position.X, 2) +
                Math.Pow(point.Position.Y - lastSignificantMove.Position.Y, 2));

            if (distance > DeadZone)
            {
                if ((point.Time - moveStart).TotalSeconds > 0.5)
                {
                    currentMotion.Range = new MediaTimeRange(moveStart, point.Time - moveStart);
                    currentMotion.StartX = lastSignificantMove.Position.X;
                    currentMotion.StartY = lastSignificantMove.Position.Y;
                    currentMotion.EndX = point.Position.X;
                    currentMotion.EndY = point.Position.Y;
                    motions.Add(currentMotion);

                    currentMotion = new ScreenMotion
                    {
                        Type = MotionType.AutoFollow,
                        IsAutoFollow = true,
                        StartZoom = ZoomStrength,
                        EndZoom = 1.0
                    };
                    moveStart = point.Time;
                }
                lastSignificantMove = point;
            }
        }

        if (motions.Count > 0)
        {
            var last = motions[^1];
            last.Range = new MediaTimeRange(last.Range.Start, totalDuration - last.Range.Start);
        }

        return motions;
    }
}
