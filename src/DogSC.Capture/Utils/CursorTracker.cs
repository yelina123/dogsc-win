using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using DogSC.Core.Timeline;

namespace DogSC.Capture.Utils;

/// <summary>
/// 光标追踪器 - 录制期间记录鼠标位置和点击事件
/// </summary>
public class CursorTracker : IDisposable
{
    private readonly System.Windows.Forms.Timer _timer;
    private readonly List<PointerTrackPoint> _points = new();
    private DateTime _startTime;
    private bool _isTracking;

    public event Action<PointerTrackPoint>? PointAdded;

    public bool IsTracking => _isTracking;
    public IReadOnlyList<PointerTrackPoint> Points => _points;

    public CursorTracker()
    {
        _timer = new System.Windows.Forms.Timer();
        _timer.Interval = 16;
        _timer.Tick += OnTick;
    }

    public void Start()
    {
        if (_isTracking) return;
        _points.Clear();
        _startTime = DateTime.Now;
        _isTracking = true;
        _timer.Start();
    }

    public void Stop()
    {
        _isTracking = false;
        _timer.Stop();
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!_isTracking) return;

        var position = System.Windows.Forms.Cursor.Position;
        var elapsed = DateTime.Now - _startTime;

        var point = new PointerTrackPoint
        {
            Time = elapsed,
            Position = new System.Windows.Point(position.X, position.Y),
            IsClick = Mouse.LeftButton == MouseButtonState.Pressed,
            IsRightClick = Mouse.RightButton == MouseButtonState.Pressed
        };

        _points.Add(point);
        PointAdded?.Invoke(point);
    }

    public PointerTrack ToPointerTrack()
    {
        return new PointerTrack
        {
            Points = new List<PointerTrackPoint>(_points)
        };
    }

    public void Dispose()
    {
        _timer.Stop();
        _timer.Dispose();
    }
}
