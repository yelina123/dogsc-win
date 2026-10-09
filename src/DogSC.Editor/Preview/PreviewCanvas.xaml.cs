using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DogSC.Core.Effects;
using DogSC.Core.Project;

namespace DogSC.Editor.Preview;

public partial class PreviewCanvas : UserControl
{
    private bool _isPlaying;
    private TimeSpan _currentTime = TimeSpan.Zero;
    private TimeSpan _duration = TimeSpan.Zero;
    private ProjectModel? _project;

    public event EventHandler<TimeSpan>? TimeChanged;
    public event EventHandler? PlaybackStarted;
    public event EventHandler? PlaybackPaused;

    public TimeSpan CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            UpdateTimeDisplay();
            TimeChanged?.Invoke(this, value);
        }
    }

    public TimeSpan Duration
    {
        get => _duration;
        set
        {
            _duration = value;
            UpdateTimeDisplay();
        }
    }

    public bool IsPlaying => _isPlaying;

    public PreviewCanvas()
    {
        InitializeComponent();
        Loaded += (s, e) => SetupDefaultStyles();
    }

    public void SetProject(ProjectModel project)
    {
        _project = project;
        Duration = TimeSpan.Zero;
    }

    public void SetPreviewFrame(BitmapSource frame)
    {
        PreviewImage.Source = frame;
    }

    public void ApplyTransform(FrameScene scene)
    {
        ScaleTransform.ScaleX = scene.Scale;
        ScaleTransform.ScaleY = scene.Scale;
        TranslateTransform.X = (scene.OffsetX - 0.5) * ActualWidth;
        TranslateTransform.Y = (scene.OffsetY - 0.5) * ActualHeight;
        RotateTransform.Angle = scene.Tilt;

        TransformBorder.CornerRadius = new CornerRadius(scene.CornerRadius);
    }

    public void ShowCameraOverlay(bool show)
    {
        CameraOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
    }

    public void SetCameraFrame(BitmapSource frame)
    {
        CameraImage.Source = frame;
    }

    public void ShowFocusEffect(FocusEffect? effect)
    {
        if (effect == null)
        {
            FocusEllipse.Visibility = Visibility.Collapsed;
            return;
        }

        FocusEllipse.Visibility = Visibility.Visible;
        FocusEllipse.Width = effect.FocusRadius * 2 * ActualWidth;
        FocusEllipse.Height = effect.FocusRadius * 2 * ActualHeight;
        Canvas.SetLeft(FocusEllipse, effect.FocusPoint.X * ActualWidth - FocusEllipse.Width / 2);
        Canvas.SetTop(FocusEllipse, effect.FocusPoint.Y * ActualHeight - FocusEllipse.Height / 2);
    }

    private async void PlayPause_Click(object sender, RoutedEventArgs e)
    {
        if (_isPlaying)
        {
            Pause();
        }
        else
        {
            await PlayAsync();
        }
    }

    public async Task PlayAsync()
    {
        if (_isPlaying) return;
        _isPlaying = true;
        PlayButton.Content = "⏸";
        PlaybackStarted?.Invoke(this, EventArgs.Empty);

        var frameDelay = 1000.0 / 60.0;
        while (_isPlaying && _currentTime < _duration)
        {
            CurrentTime += TimeSpan.FromMilliseconds(frameDelay);
            await Task.Delay((int)frameDelay);
        }

        if (_currentTime >= _duration)
        {
            Pause();
        }
    }

    public void Pause()
    {
        _isPlaying = false;
        PlayButton.Content = "▶";
        PlaybackPaused?.Invoke(this, EventArgs.Empty);
    }

    private void Rewind_Click(object sender, RoutedEventArgs e)
    {
        CurrentTime = TimeSpan.Zero;
    }

    private void Forward_Click(object sender, RoutedEventArgs e)
    {
        CurrentTime = _duration;
    }

    private void PreviousFrame_Click(object sender, RoutedEventArgs e)
    {
        CurrentTime -= TimeSpan.FromSeconds(1.0 / 60.0);
    }

    private void NextFrame_Click(object sender, RoutedEventArgs e)
    {
        CurrentTime += TimeSpan.FromSeconds(1.0 / 60.0);
    }

    private void UpdateTimeDisplay()
    {
        TimeDisplay.Text = $"{FormatTime(_currentTime)} / {FormatTime(_duration)}";
    }

    private string FormatTime(TimeSpan time) =>
        $"{(int)time.TotalMinutes:D2}:{time.Seconds:D2}.{time.Milliseconds / 100:D1}";

    private void SetupDefaultStyles()
    {
        var transportStyle = new Style(typeof(Button));
        transportStyle.Setters.Add(new Setter(Button.ContentProperty, "▶"));
        transportStyle.Setters.Add(new Setter(Button.WidthProperty, 30.0));
        transportStyle.Setters.Add(new Setter(Button.HeightProperty, 30.0));
        transportStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x3C, 0x3C, 0x4E))));
        transportStyle.Setters.Add(new Setter(Button.ForegroundProperty, Brushes.White));
        transportStyle.Setters.Add(new Setter(Button.BorderThicknessProperty, new Thickness(0)));
        transportStyle.Setters.Add(new Setter(Button.CursorProperty, Cursors.Hand));

        var playStyle = new Style(typeof(Button), transportStyle);
        playStyle.Setters.Add(new Setter(Button.WidthProperty, 40.0));
        playStyle.Setters.Add(new Setter(Button.HeightProperty, 40.0));
        playStyle.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(0x00, 0x7A, 0xCC))));

        Resources.Add("TransportButton", transportStyle);
        Resources.Add("PlayButton", playStyle);
    }
}
