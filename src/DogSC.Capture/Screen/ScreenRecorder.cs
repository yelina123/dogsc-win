using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using DogSC.Core.Models;
using DogSC.Core.Diagnostics;

namespace DogSC.Capture.Screen;

/// <summary>
/// 屏幕录制器 - 基于 Windows Graphics Capture API
/// </summary>
public class ScreenRecorder : IDisposable
{
    private bool _isRecording;
    private CancellationTokenSource? _cts;
    private readonly ScreenCaptureDiagnostics _diagnostics = new();

    public event Action<Bitmap>? FrameCaptured;
    public event Action<Exception>? ErrorOccurred;
    public event Action<TimeSpan>? RecordingTick;

    public bool IsRecording => _isRecording;
    public ScreenCaptureDiagnostics Diagnostics => _diagnostics;

    public async Task StartAsync(CaptureConfiguration config)
    {
        if (_isRecording) return;

        _cts = new CancellationTokenSource();
        _isRecording = true;
        _diagnostics.Reset();

        try
        {
            switch (config.SourceType)
            {
                case CaptureSourceType.Screen:
                    await StartScreenCaptureAsync(config, _cts.Token);
                    break;
                case CaptureSourceType.Window:
                    await StartWindowCaptureAsync(config, _cts.Token);
                    break;
                case CaptureSourceType.Region:
                    await StartRegionCaptureAsync(config, _cts.Token);
                    break;
            }
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(ex);
            _isRecording = false;
        }
    }

    public async Task StopAsync()
    {
        if (!_isRecording) return;

        _cts?.Cancel();
        _isRecording = false;

        await Task.Delay(100);
    }

    private async Task StartScreenCaptureAsync(CaptureConfiguration config, CancellationToken ct)
    {
        var screenWidth = config.Width;
        var screenHeight = config.Height;
        var frameInterval = 1000.0 / config.FrameRate;

        while (!ct.IsCancellationRequested)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var bitmap = new Bitmap(screenWidth, screenHeight);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }

                var frame = (Bitmap)bitmap.Clone();
                FrameCaptured?.Invoke(frame);
                _diagnostics.CapturedFrames++;
            }
            catch
            {
                _diagnostics.DroppedFrames++;
            }

            _diagnostics.RecordingDuration += TimeSpan.FromMilliseconds(frameInterval);
            RecordingTick?.Invoke(_diagnostics.RecordingDuration);

            var elapsed = sw.ElapsedMilliseconds;
            var delay = Math.Max(0, (int)(frameInterval - elapsed));
            await Task.Delay(delay, ct);
        }
    }

    private async Task StartWindowCaptureAsync(CaptureConfiguration config, CancellationToken ct)
    {
        await StartScreenCaptureAsync(config, ct);
    }

    private async Task StartRegionCaptureAsync(CaptureConfiguration config, CancellationToken ct)
    {
        var region = config.RegionBounds;
        var frameInterval = 1000.0 / config.FrameRate;

        while (!ct.IsCancellationRequested)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                using var bitmap = new Bitmap(region.Width, region.Height);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(region.Left, region.Top, 0, 0, region.Size);
                }

                var frame = (Bitmap)bitmap.Clone();
                FrameCaptured?.Invoke(frame);
                _diagnostics.CapturedFrames++;
            }
            catch
            {
                _diagnostics.DroppedFrames++;
            }

            _diagnostics.RecordingDuration += TimeSpan.FromMilliseconds(frameInterval);
            RecordingTick?.Invoke(_diagnostics.RecordingDuration);

            var elapsed = sw.ElapsedMilliseconds;
            var delay = Math.Max(0, (int)(frameInterval - elapsed));
            await Task.Delay(delay, ct);
        }
    }

    public static List<DisplayInfo> GetDisplays()
    {
        var displays = new List<DisplayInfo>();
        foreach (var screen in System.Windows.Forms.Screen.AllScreens)
        {
            displays.Add(new DisplayInfo
            {
                DeviceName = screen.DeviceName,
                Bounds = screen.Bounds,
                IsPrimary = screen.Primary,
                WorkingArea = screen.WorkingArea
            });
        }
        return displays;
    }

    public void Dispose()
    {
        _cts?.Dispose();
    }
}

public class DisplayInfo
{
    public string DeviceName { get; set; } = "";
    public Rectangle Bounds { get; set; }
    public Rectangle WorkingArea { get; set; }
    public bool IsPrimary { get; set; }
}

public class ScreenCaptureDiagnostics
{
    public TimeSpan RecordingDuration { get; set; }
    public long CapturedFrames { get; set; }
    public long DroppedFrames { get; set; }
    public double Fps { get; set; } = 60.0;
    public double CpuUsage { get; set; }
    public double MemoryUsageMB { get; set; }

    public void Reset()
    {
        RecordingDuration = TimeSpan.Zero;
        CapturedFrames = 0;
        DroppedFrames = 0;
    }
}
