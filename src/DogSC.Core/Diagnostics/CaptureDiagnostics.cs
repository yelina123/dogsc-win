using System;
using System.Diagnostics;

namespace DogSC.Core.Diagnostics;

/// <summary>
/// 音频录制诊断
/// </summary>
public class AudioCaptureDiagnostics
{
    public string MicrophoneDeviceName { get; set; } = "";
    public string SystemAudioDeviceName { get; set; } = "";
    public int MicrophoneSampleRate { get; set; } = 48000;
    public int SystemSampleRate { get; set; } = 48000;
    public double MicrophoneLevel { get; set; } = 0.0;
    public double SystemLevel { get; set; } = 0.0;
    public long DroppedFrames { get; set; }
    public double LatencyMs { get; set; }
    public DateTime LastUpdate { get; set; } = DateTime.Now;

    public string GetSummary() =>
        $"Mic: {MicrophoneDeviceName} @ {MicrophoneSampleRate}Hz | " +
        $"Sys: {SystemAudioDeviceName} @ {SystemSampleRate}Hz | " +
        $"Dropped: {DroppedFrames} | Latency: {LatencyMs:F1}ms";
}

/// <summary>
/// 摄像头录制诊断
/// </summary>
public class CameraCaptureDiagnostics
{
    public string DeviceName { get; set; } = "";
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
    public long CapturedFrames { get; set; }
    public long DroppedFrames { get; set; }
    public double ActualFps { get; set; }
    public double LatencyMs { get; set; }
    public string Format { get; set; } = "NV12";

    public string GetSummary() =>
        $"{DeviceName} | {Width}x{Height} @ {ActualFps:F1}fps | " +
        $"Dropped: {DroppedFrames} | Latency: {LatencyMs:F1}ms";
}

/// <summary>
/// 录制性能诊断
/// </summary>
public class RecordingPerformanceDiagnostics
{
    public TimeSpan RecordingDuration { get; set; }
    public double Fps { get; set; } = 60.0;
    public long CapturedFrames { get; set; }
    public long DroppedFrames { get; set; }
    public double CpuUsage { get; set; }
    public double MemoryUsageMB { get; set; }
    public double DiskWriteMBps { get; set; }
    public double EncodingQueueSize { get; set; }

    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public double DroppedFrameRate => CapturedFrames == 0 ? 0 : (double)DroppedFrames / CapturedFrames * 100;

    public string GetSummary() =>
        $"Duration: {RecordingDuration:hh\\:mm\\:ss} | " +
        $"FPS: {Fps:F1} | Dropped: {DroppedFrameRate:F1}% | " +
        $"CPU: {CpuUsage:F0}% | Mem: {MemoryUsageMB:F0}MB | Disk: {DiskWriteMBps:F1}MB/s";

    public void Reset()
    {
        RecordingDuration = TimeSpan.Zero;
        CapturedFrames = 0;
        DroppedFrames = 0;
        CpuUsage = 0;
        MemoryUsageMB = 0;
        DiskWriteMBps = 0;
        EncodingQueueSize = 0;
        _stopwatch.Reset();
        _stopwatch.Start();
    }
}
