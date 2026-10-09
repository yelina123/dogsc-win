using System;
using System.Collections.Generic;

namespace DogSC.Core.Timeline;

/// <summary>
/// 输出帧调度器 - 管理导出时的帧输出计划
/// </summary>
public class OutputFrameSchedule
{
    public double OutputFps { get; set; } = 60.0;
    public TimeSpan TotalDuration { get; set; }
    public int TotalFrames => (int)(TotalDuration.TotalSeconds * OutputFps);

    public IEnumerable<TimeSpan> EnumerateFrameTimes()
    {
        var frameDuration = TimeSpan.FromSeconds(1.0 / OutputFps);
        for (var i = 0; i < TotalFrames; i++)
        {
            yield return frameDuration * i;
        }
    }

    public TimeSpan GetTimeForFrame(int frameIndex) =>
        TimeSpan.FromSeconds(frameIndex / OutputFps);

    public int GetFrameIndexForTime(TimeSpan time) =>
        (int)(time.TotalSeconds * OutputFps);
}
