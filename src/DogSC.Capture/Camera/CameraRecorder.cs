using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using DogSC.Core.Models;
using DogSC.Core.Diagnostics;

namespace DogSC.Capture.Camera;

/// <summary>
/// 摄像头录制器
/// </summary>
public class CameraRecorder : IDisposable
{
    private bool _isRecording;
    private readonly CameraCaptureDiagnostics _diagnostics = new();

    public event Action<Bitmap>? FrameCaptured;
    public event Action<Exception>? ErrorOccurred;

    public bool IsRecording => _isRecording;
    public CameraCaptureDiagnostics Diagnostics => _diagnostics;

    public async Task StartAsync(CaptureConfiguration config)
    {
        if (_isRecording) return;
        _isRecording = true;
        _diagnostics.Reset();

        try
        {
            await Task.Run(() => CaptureLoop(config));
        }
        catch (Exception ex)
        {
            ErrorOccurred?.Invoke(ex);
            _isRecording = false;
        }
    }

    public Task StopAsync()
    {
        _isRecording = false;
        return Task.CompletedTask;
    }

    private void CaptureLoop(CaptureConfiguration config)
    {
        var frameInterval = 1000.0 / 30.0;

        while (_isRecording)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            var bitmap = new Bitmap(640, 480);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(System.Drawing.Color.DarkGray);
                g.DrawString("Camera Preview", new Font("Arial", 16),
                    Brushes.White, 20, 20);
            }

            FrameCaptured?.Invoke(bitmap);
            _diagnostics.CapturedFrames++;
            _diagnostics.ActualFps = 30.0;

            var elapsed = sw.ElapsedMilliseconds;
            var delay = Math.Max(0, (int)(frameInterval - elapsed));
            System.Threading.Thread.Sleep(delay);
        }
    }

    public static List<CameraDeviceInfo> GetDevices()
    {
        return new List<CameraDeviceInfo>
        {
            new() { Id = "default", Name = "默认摄像头", IsDefault = true }
        };
    }

    public void Dispose()
    {
        StopAsync().Wait();
    }
}

public class CameraDeviceInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsDefault { get; set; }
    public List<CameraResolution> Resolutions { get; set; } = new();
}

public class CameraResolution
{
    public int Width { get; set; }
    public int Height { get; set; }
    public double FrameRate { get; set; }
}

public static class CameraCaptureDiagnosticsExtensions
{
    public static void Reset(this CameraCaptureDiagnostics diagnostics)
    {
        diagnostics.DroppedFrames = 0;
        diagnostics.ActualFps = 0;
        diagnostics.LatencyMs = 0;
    }
}
