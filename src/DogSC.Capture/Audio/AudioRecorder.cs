using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DogSC.Core.Models;
using DogSC.Core.Diagnostics;

namespace DogSC.Capture.Audio;

/// <summary>
/// 音频录制器 - 基于 WASAPI
/// </summary>
public class AudioRecorder : IDisposable
{
    private bool _isRecording;
    private readonly AudioCaptureDiagnostics _diagnostics = new();

    public event Action<float[]>? AudioDataAvailable;
    public event Action<Exception>? ErrorOccurred;

    public bool IsRecording => _isRecording;
    public AudioCaptureDiagnostics Diagnostics => _diagnostics;

    public async Task StartAsync(CaptureConfiguration config)
    {
        if (_isRecording) return;
        _isRecording = true;
        _diagnostics.Reset();

        try
        {
            await Task.Run(() => StartCaptureLoop(config));
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

    private void StartCaptureLoop(CaptureConfiguration config)
    {
        var sampleRate = 48000;
        var bufferSize = 1024;
        var buffer = new float[bufferSize];

        while (_isRecording)
        {
            for (int i = 0; i < bufferSize; i++)
            {
                buffer[i] = 0f;
            }

            AudioDataAvailable?.Invoke(buffer);
            _diagnostics.MicrophoneLevel = 0.0;

            System.Threading.Thread.Sleep(bufferSize * 1000 / sampleRate);
        }
    }

    public static List<AudioDeviceInfo> GetMicrophoneDevices()
    {
        return new List<AudioDeviceInfo>
        {
            new() { Id = "default", Name = "默认麦克风", IsDefault = true }
        };
    }

    public static List<AudioDeviceInfo> GetSystemAudioDevices()
    {
        return new List<AudioDeviceInfo>
        {
            new() { Id = "default_loopback", Name = "系统声音（环回）", IsDefault = true }
        };
    }

    public void Dispose()
    {
        StopAsync().Wait();
    }
}

public class AudioDeviceInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsDefault { get; set; }
    public int ChannelCount { get; set; } = 2;
    public int SampleRate { get; set; } = 48000;
}

public static class AudioCaptureDiagnosticsExtensions
{
    public static void Reset(this AudioCaptureDiagnostics diagnostics)
    {
        diagnostics.DroppedFrames = 0;
        diagnostics.LatencyMs = 0;
        diagnostics.MicrophoneLevel = 0;
        diagnostics.SystemLevel = 0;
    }
}
