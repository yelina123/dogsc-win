using System.Windows;
using DogSC.Capture;
using DogSC.Core.Project;
using DogSC.Core.Models;
using DogSC.Core.Timeline;
using DogSC.App.Windows;

namespace DogSC.App;

public partial class MainWindow : Window
{
    private readonly CaptureManager _captureManager = new();
    private ProjectModel _project = new();
    private bool _isRecording;

    public MainWindow()
    {
        InitializeComponent();
        InitializeProject();
        InitializeCaptureEvents();
    }

    private void InitializeProject()
    {
        _project.Name = "未命名项目";
        _project.Width = 1920;
        _project.Height = 1080;
        _project.FrameRate = 60;

        var videoTrack = new TimelineTrack(TrackType.Video, "屏幕录制");
        var audioTrack = new TimelineTrack(TrackType.Audio, "音频");
        var cursorTrack = new TimelineTrack(TrackType.Cursor, "光标");

        _project.Timeline.Tracks.Add(videoTrack);
        _project.Timeline.Tracks.Add(audioTrack);
        _project.Timeline.Tracks.Add(cursorTrack);

        TimelineControl.SetTracks(_project.Timeline.Tracks);
        PreviewCanvas.SetProject(_project);
    }

    private void InitializeCaptureEvents()
    {
        _captureManager.RecordingStarted += () =>
        {
            Dispatcher.Invoke(() =>
            {
                _isRecording = true;
                Title = "DogSC - 录制中...";
            });
        };

        _captureManager.RecordingStopped += () =>
        {
            Dispatcher.Invoke(() =>
            {
                _isRecording = false;
                Title = "DogSC";
            });
        };

        _captureManager.RecordingTick += elapsed =>
        {
            Dispatcher.Invoke(() =>
            {
                Title = $"DogSC - {elapsed:hh\\:mm\\:ss}";
            });
        };
    }

    private async void StartRecording_Click(object sender, RoutedEventArgs e)
    {
        var setupWindow = new CaptureSetupWindow();
        if (setupWindow.ShowDialog() == true)
        {
            var config = setupWindow.CaptureConfig;
            await _captureManager.StartAsync(config);
        }
    }

    private async void StopRecording_Click(object sender, RoutedEventArgs e)
    {
        await _captureManager.StopAsync();
    }

    private void AddMarker_Click(object sender, RoutedEventArgs e)
    {
        var marker = new RecordingMarker
        {
            Time = PreviewCanvas.CurrentTime,
            Note = "标记"
        };
        _project.Markers.Add(marker);
        MessageBox.Show($"已添加标记 @ {PreviewCanvas.CurrentTime:mm\\:ss}", "DogSC");
    }

    private void NewProject_Click(object sender, RoutedEventArgs e)
    {
        _project = new ProjectModel { Name = "未命名项目" };
        InitializeProject();
    }

    private void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "DogSC 项目|*.dogscproject|所有文件|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            MessageBox.Show($"打开项目: {dialog.FileName}", "DogSC");
        }
    }

    private void SaveProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "DogSC 项目|*.dogscproject",
            FileName = _project.Name + ".dogscproject"
        };
        if (dialog.ShowDialog() == true)
        {
            MessageBox.Show($"保存项目: {dialog.FileName}", "DogSC");
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        var exportDialog = new ExportDialog();
        exportDialog.ShowDialog();
    }

    private void Exit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "DogSC for Windows\n\n" +
            "macOS 版 DogSC 的 Windows 移植\n" +
            "原作者: laogou717\n\n" +
            "MIT License",
            "关于 DogSC");
    }

    private void Shortcuts_Click(object sender, RoutedEventArgs e)
    {
        var shortcuts = @"快捷键:
Ctrl+R - 开始/停止录制
Ctrl+Alt+M - 添加标记
Ctrl+E - 导出
S - 分割片段
Delete - 删除选中片段
Ctrl+C/V - 复制/粘贴效果
空格 - 播放/暂停";
        MessageBox.Show(shortcuts, "快捷键");
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        if (_isRecording)
        {
            var result = MessageBox.Show("正在录制中，确定要退出吗？", "确认",
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                return;
            }
        }

        _captureManager.Dispose();
        base.OnClosing(e);
    }
}
