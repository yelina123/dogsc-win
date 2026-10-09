using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DogSC.Core.Timeline;
using DogSC.Core.Models;

namespace DogSC.Editor.Timeline;

public partial class TimelineControl : UserControl
{
    private readonly List<TimelineTrack> _tracks = new();
    private double _pixelsPerSecond = 100.0;
    private TimeSpan _playheadPosition = TimeSpan.Zero;
    private TimeSpan _duration = TimeSpan.FromMinutes(1);
    private bool _isDraggingPlayhead;

    public event EventHandler<TimeSpan>? PlayheadChanged;
    public event EventHandler<TimelineClip>? ClipSelected;

    public double PixelsPerSecond
    {
        get => _pixelsPerSecond;
        set
        {
            _pixelsPerSecond = value;
            Redraw();
        }
    }

    public TimeSpan PlayheadPosition
    {
        get => _playheadPosition;
        set
        {
            _playheadPosition = value;
            UpdatePlayheadPosition();
            PlayheadChanged?.Invoke(this, value);
        }
    }

    public TimelineControl()
    {
        InitializeComponent();
        SizeChanged += (s, e) => Redraw();
    }

    public void SetTracks(IEnumerable<TimelineTrack> tracks)
    {
        _tracks.Clear();
        _tracks.AddRange(tracks);
        Redraw();
    }

    public void SetDuration(TimeSpan duration)
    {
        _duration = duration;
        Redraw();
    }

    private void Redraw()
    {
        TimelineCanvas.Children.Clear();
        DrawTracks();
        DrawTimeRuler();
        UpdateCanvasSize();
        UpdatePlayheadPosition();
    }

    private void DrawTracks()
    {
        var y = 0.0;
        foreach (var track in _tracks)
        {
            var trackHeight = 60.0;

            var trackBg = new Rectangle
            {
                Width = TimelineCanvas.ActualWidth,
                Height = trackHeight,
                Fill = new SolidColorBrush(Color.FromRgb(0x2D, 0x2D, 0x30)),
                Stroke = new SolidColorBrush(Color.FromRgb(0x3E, 0x3E, 0x40)),
                StrokeThickness = 0.5
            };
            Canvas.SetLeft(trackBg, 0);
            Canvas.SetTop(trackBg, y);
            TimelineCanvas.Children.Add(trackBg);

            var label = new TextBlock
            {
                Text = track.Name,
                Foreground = Brushes.LightGray,
                FontSize = 11,
                Margin = new Thickness(5, 3, 0, 0)
            };
            Canvas.SetLeft(label, 0);
            Canvas.SetTop(label, y + 3);
            TimelineCanvas.Children.Add(label);

            foreach (var clip in track.Clips)
            {
                DrawClip(clip, y + 20);
            }

            y += trackHeight;
        }
    }

    private void DrawClip(TimelineClip clip, double y)
    {
        var x = clip.TimelineRange.Start.TotalSeconds * _pixelsPerSecond;
        var width = clip.TimelineRange.Duration.TotalSeconds * _pixelsPerSecond;

        var clipRect = new Rectangle
        {
            Width = Math.Max(width, 10),
            Height = 38,
            Fill = new SolidColorBrush(Color.FromRgb(0x3C, 0x3C, 0x4E)),
            Stroke = new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC)),
            StrokeThickness = 1,
            RadiusX = 3,
            RadiusY = 3,
            Cursor = Cursors.Hand,
            Tag = clip
        };

        clipRect.MouseLeftButtonDown += (s, e) =>
        {
            ClipSelected?.Invoke(this, clip);
            e.Handled = true;
        };

        Canvas.SetLeft(clipRect, x);
        Canvas.SetTop(clipRect, y);
        TimelineCanvas.Children.Add(clipRect);

        var clipLabel = new TextBlock
        {
            Text = clip.Name,
            Foreground = Brushes.White,
            FontSize = 10,
            Margin = new Thickness(5, 2, 0, 0),
            IsHitTestVisible = false
        };
        Canvas.SetLeft(clipLabel, x + 5);
        Canvas.SetTop(clipLabel, y + 2);
        TimelineCanvas.Children.Add(clipLabel);
    }

    private void DrawTimeRuler()
    {
        TimeRulerCanvas.Children.Clear();
        var width = TimeRulerCanvas.ActualWidth;
        var seconds = _duration.TotalSeconds;
        var totalWidth = seconds * _pixelsPerSecond;

        var tickInterval = GetTickInterval(_pixelsPerSecond);
        var tickSeconds = tickInterval.TotalSeconds;

        for (double s = 0; s <= seconds; s += tickSeconds)
        {
            var x = s * _pixelsPerSecond;

            var line = new Line
            {
                X1 = x,
                Y1 = 14,
                X2 = x,
                Y2 = 22,
                Stroke = Brushes.Gray,
                StrokeThickness = 1
            };
            TimeRulerCanvas.Children.Add(line);

            var label = new TextBlock
            {
                Text = $"{(int)s / 60}:{(int)s % 60:D2}",
                Foreground = Brushes.Gray,
                FontSize = 10
            };
            Canvas.SetLeft(label, x + 3);
            Canvas.SetTop(label, 0);
            TimeRulerCanvas.Children.Add(label);
        }
    }

    private TimeSpan GetTickInterval(double pps)
    {
        if (pps >= 200) return TimeSpan.FromSeconds(1);
        if (pps >= 100) return TimeSpan.FromSeconds(5);
        if (pps >= 50) return TimeSpan.FromSeconds(10);
        if (pps >= 20) return TimeSpan.FromSeconds(30);
        return TimeSpan.FromMinutes(1);
    }

    private void UpdateCanvasSize()
    {
        var totalWidth = _duration.TotalSeconds * _pixelsPerSecond;
        var totalHeight = _tracks.Count * 60;
        TimelineCanvas.Width = Math.Max(totalWidth, ActualWidth);
        TimelineCanvas.Height = Math.Max(totalHeight, 100);
    }

    private void UpdatePlayheadPosition()
    {
        var x = _playheadPosition.TotalSeconds * _pixelsPerSecond;
        Canvas.SetLeft(Playhead, x);
    }

    private void TimeRuler_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDraggingPlayhead = true;
        UpdatePlayheadFromMouse(e.GetPosition(TimeRulerCanvas).X);
    }

    private void TimeRuler_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDraggingPlayhead)
        {
            UpdatePlayheadFromMouse(e.GetPosition(TimeRulerCanvas).X);
        }
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonUp(e);
        _isDraggingPlayhead = false;
    }

    private void UpdatePlayheadFromMouse(double x)
    {
        var seconds = x / _pixelsPerSecond;
        PlayheadPosition = TimeSpan.FromSeconds(Math.Max(0, seconds));
    }

    private void TimelineCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Redraw();
    }
}
