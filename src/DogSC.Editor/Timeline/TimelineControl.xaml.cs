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
    private TimelineClip? _selectedClip;
    private TimelineClip? _draggingClip;
    private Point _dragStartPoint;
    private TimeSpan _dragOriginalStart;
    private bool _isResizingClip;
    private double _resizeOriginalDuration;

    public event EventHandler<TimeSpan>? PlayheadChanged;
    public event EventHandler<TimelineClip>? ClipSelected;
    public event EventHandler<TimelineClip>? ClipDoubleClicked;
    public event EventHandler<TimelineClip>? ClipDeleted;
    public event EventHandler<TimelineClip, TimeSpan>? ClipSplit;

    public double PixelsPerSecond
    {
        get => _pixelsPerSecond;
        set
        {
            _pixelsPerSecond = Math.Clamp(value, 10, 500);
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

    public TimelineClip? SelectedClip
    {
        get => _selectedClip;
        set
        {
            _selectedClip = value;
            Redraw();
        }
    }

    public TimelineControl()
    {
        InitializeComponent();
        SizeChanged += (s, e) => Redraw();
        MouseWheel += TimelineControl_MouseWheel;
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
        var isSelected = clip == _selectedClip;

        var clipRect = new Border
        {
            Width = Math.Max(width, 10),
            Height = 38,
            Background = new SolidColorBrush(isSelected
                ? Color.FromRgb(0x4C, 0x4C, 0x6E)
                : Color.FromRgb(0x3C, 0x3C, 0x4E)),
            BorderBrush = new SolidColorBrush(isSelected
                ? Color.FromRgb(0x00, 0xBC, 0xEE)
                : Color.FromRgb(0x00, 0x7A, 0xCC)),
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(3),
            Cursor = Cursors.SizeWE,
            Tag = clip
        };

        clipRect.MouseLeftButtonDown += (s, e) => OnClipMouseDown(clip, e);
        clipRect.MouseMove += (s, e) => OnClipMouseMove(clip, e);
        clipRect.MouseLeftButtonUp += (s, e) => OnClipMouseUp(clip, e);
        clipRect.MouseRightButtonUp += (s, e) => ShowClipContextMenu(clip, e);
        clipRect.MouseLeftButtonDoubleClick += (s, e) =>
        {
            ClipDoubleClicked?.Invoke(this, clip);
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

        if (clip.Speed != 1.0)
        {
            var speedLabel = new TextBlock
            {
                Text = $"{clip.Speed:F1}x",
                Foreground = new SolidColorBrush(Color.FromRgb(0xFF, 0xAA, 0x00)),
                FontSize = 9,
                IsHitTestVisible = false
            };
            Canvas.SetLeft(speedLabel, x + width - 30);
            Canvas.SetTop(speedLabel, y + 2);
            TimelineCanvas.Children.Add(speedLabel);
        }
    }

    private void OnClipMouseDown(TimelineClip clip, MouseButtonEventArgs e)
    {
        _selectedClip = clip;
        ClipSelected?.Invoke(this, clip);

        _dragStartPoint = e.GetToScreen();
        _draggingClip = clip;
        _dragOriginalStart = clip.TimelineRange.Start;

        var pos = e.GetPosition(TimelineCanvas);
        var clipRight = clip.TimelineRange.Start.TotalSeconds * _pixelsPerSecond +
                       clip.TimelineRange.Duration.TotalSeconds * _pixelsPerSecond;

        if (Math.Abs(pos.X - clipRight) < 5)
        {
            _isResizingClip = true;
            _resizeOriginalDuration = clip.TimelineRange.Duration.TotalSeconds;
        }

        Redraw();
        e.Handled = true;
    }

    private void OnClipMouseMove(TimelineClip clip, MouseEventArgs e)
    {
        if (_draggingClip != clip) return;

        var pos = e.GetPosition(TimelineCanvas);
        var deltaSeconds = (pos.X - _dragStartPoint.X) / _pixelsPerSecond;

        if (_isResizingClip)
        {
            var newDuration = Math.Max(0.5, _resizeOriginalDuration + deltaSeconds);
            clip.TimelineRange = new MediaTimeRange(
                clip.TimelineRange.Start,
                TimeSpan.FromSeconds(newDuration));
        }
        else
        {
            var newStart = TimeSpan.FromSeconds(
                Math.Max(0, _dragOriginalStart.TotalSeconds + deltaSeconds));
            clip.TimelineRange = new MediaTimeRange(
                newStart,
                clip.TimelineRange.Duration);
        }

        Redraw();
    }

    private void OnClipMouseUp(TimelineClip clip, MouseButtonEventArgs e)
    {
        _draggingClip = null;
        _isResizingClip = false;
    }

    private void ShowClipContextMenu(TimelineClip clip, RoutedEventArgs e)
    {
        var menu = new ContextMenu();

        var splitItem = new MenuItem { Header = "分割 (S)" };
        splitItem.Click += (s, args) =>
        {
            ClipSplit?.Invoke(this, clip, PlayheadPosition);
        };
        menu.Items.Add(splitItem);

        var deleteItem = new MenuItem { Header = "删除 (Del)" };
        deleteItem.Click += (s, args) => ClipDeleted?.Invoke(this, clip);
        menu.Items.Add(deleteItem);

        menu.Items.Add(new Separator());

        var speedItem = new MenuItem { Header = "速度" };
        foreach (var speed in new[] { 0.5, 1.0, 1.5, 2.0 })
        {
            var subItem = new MenuItem { Header = $"{speed:F1}x", IsCheckable = true, IsChecked = clip.Speed == speed };
            subItem.Click += (s, args) => clip.Speed = speed;
            speedItem.Items.Add(subItem);
        }
        menu.Items.Add(speedItem);

        menu.IsOpen = true;
    }

    private void DrawTimeRuler()
    {
        TimeRulerCanvas.Children.Clear();
        var seconds = _duration.TotalSeconds;

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
        _draggingClip = null;
        _isResizingClip = false;
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

    private void TimelineControl_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control)
        {
            var delta = e.Delta > 0 ? 10 : -10;
            PixelsPerSecond += delta;
            e.Handled = true;
        }
    }
}

internal static class MouseExtensions
{
    public static Point GetToScreen(this MouseButtonEventArgs e)
    {
        var pos = e.GetPosition((IInputElement)e.Source);
        return new Point(pos.X, pos.Y);
    }
}
