using System;
using System.Drawing;
using System.Threading.Tasks;
using DogSC.Core.Models;
using DogSC.Capture.Screen;
using DogSC.Capture.Audio;
using DogSC.Capture.Camera;

namespace DogSC.Capture;

/// <summary>
/// 录制管理器 - 协调屏幕、音频、摄像头的同步录制
/// </summary>
public class CaptureManager : IDisposable
{
    private readonly ScreenRecorder _screenRecorder = new();
    private readonly AudioRecorder _audioRecorder = new();
    private readonly CameraRecorder _cameraRecorder = new();

    private bool _isRecording;
    private DateTime _recordingStartTime;

    public event Action<Bitmap>? ScreenFrameCaptured;
    public event Action<Bitmap>? CameraFrameCaptured;
    public event Action<float[]>? AudioDataAvailable;
    public event Action<TimeSpan>? RecordingTick;
    public event Action? RecordingStarted;
    public event Action? RecordingStopped;
    public event Action<Exception>? ErrorOccurred;

    public bool IsRecording => _isRecording;
    public TimeSpan Elapsed => _isRecording ? DateTime.Now - _recordingStartTime : TimeSpan.Zero;

    public CaptureManager()
    {
        _screenRecorder.FrameCaptured += OnScreenFrameCaptured;
        _screenRecorder.RecordingTick += OnRecordingTick;
        _screenRecorder.ErrorOccurred += OnError;

        _audioRecorder.AudioDataAvailable += OnAudioDataAvailable;
        _audioRecorder.ErrorOccurred += OnError;

        _cameraRecorder.FrameCaptured += OnCameraFrameCaptured;
        _cameraRecorder.ErrorOccurred += OnError;
    }

    public async Task StartAsync(CaptureConfiguration config)
    {
        if (_isRecording) return;

        _recordingStartTime = DateTime.Now;
        _isRecording = true;
        RecordingStarted?.Invoke();

        try
        {
            var tasks = new System.Collections.Generic.List<Task>();

            tasks.Add(_screenRecorder.StartAsync(config));

            if (config.CaptureMicrophone || config.CaptureSystemAudio)
            {
                tasks.Add(_audioRecorder.StartAsync(config));
            }

            if (config.CaptureCamera)
            {
                tasks.Add(_cameraRecorder.StartAsync(config));
            }

            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            OnError(ex);
            await StopAsync();
        }
    }

    public async Task StopAsync()
    {
        if (!_isRecording) return;

        _isRecording = false;

        var tasks = new System.Collections.Generic.List<Task>
        {
            _screenRecorder.StopAsync(),
            _audioRecorder.StopAsync(),
            _cameraRecorder.StopAsync()
        };

        await Task.WhenAll(tasks);
        RecordingStopped?.Invoke();
    }

    private void OnScreenFrameCaptured(Bitmap frame) => ScreenFrameCaptured?.Invoke(frame);
    private void OnCameraFrameCaptured(Bitmap frame) => CameraFrameCaptured?.Invoke(frame);
    private void OnAudioDataAvailable(float[] data) => AudioDataAvailable?.Invoke(data);
    private void OnRecordingTick(TimeSpan elapsed) => RecordingTick?.Invoke(elapsed);
    private void OnError(Exception ex) => ErrorOccurred?.Invoke(ex);

    public void Dispose()
    {
        _screenRecorder.Dispose();
        _audioRecorder.Dispose();
        _cameraRecorder.Dispose();
    }
}
