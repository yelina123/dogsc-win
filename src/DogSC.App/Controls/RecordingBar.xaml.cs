using System;
using System.Windows;
using System.Windows.Input;

namespace DogSC.App.Controls;

public partial class RecordingBar : Window
{
    private DateTime _startTime;
    private readonly System.Windows.Threading.DispatcherTimer _timer;
    public event EventHandler? StopRequested;
    public event EventHandler? MarkerAdded;
    public event EventHandler? MemoRequested;

    public RecordingBar()
    {
        InitializeComponent();
        _timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };
        _timer.Tick += (s, e) => UpdateTime();

        MouseLeftButtonDown += (s, e) => DragMove();
    }

    public void Start()
    {
        _startTime = DateTime.Now;
        _timer.Start();
        Show();
    }

    public new void Stop()
    {
        _timer.Stop();
        Hide();
    }

    private void UpdateTime()
    {
        var elapsed = DateTime.Now - _startTime;
        TimeText.Text = $"{(int)elapsed.TotalMinutes:D2}:{elapsed.Seconds:D2}";
    }

    private void Stop_Click(object sender, RoutedEventArgs e)
    {
        StopRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Marker_Click(object sender, RoutedEventArgs e)
    {
        MarkerAdded?.Invoke(this, EventArgs.Empty);
    }

    private void Memo_Click(object sender, RoutedEventArgs e)
    {
        MemoRequested?.Invoke(this, EventArgs.Empty);
    }
}
