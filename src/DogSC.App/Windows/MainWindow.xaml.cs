using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DogSC.Capture;
using DogSC.Core.Project;
using DogSC.Core.Models;
using DogSC.Core.Timeline;
using DogSC.Core.Undo;
using DogSC.App.Windows;

namespace DogSC.App.Windows;

public partial class MainWindow : Window
{
    private readonly CaptureManager _captureManager = new();
    private ProjectModel _project = new();
    private readonly ProjectSerializer _serializer = new();
    private readonly UndoManager _undoManager = new();
    private ProjectTimelineEditing? _timelineEditing;
    private bool _isRecording;
    private bool _isDirty;

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

        _timelineEditing = new ProjectTimelineEditing(_project);
        _undoManager.Clear();

        TimelineControl.SetTracks(_project.Timeline.Tracks);
        PreviewCanvas.SetProject(_project);

        _isDirty = false;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var title = $"DogSC - {_project.Name}";
        if (_isDirty) title += "*";
        if (_isRecording) title += $" - 录制中 {_captureManager.Elapsed:hh\\:mm\\:ss}";
        Title = title;
    }

    private void InitializeCaptureEvents()
    {
        _captureManager.RecordingStarted += () =>
        {
            Dispatcher.Invoke(() =>
            {
                _isRecording = true;
                UpdateTitle();
            });
        };

        _captureManager.RecordingStopped += () =>
        {
            Dispatcher.Invoke(() =>
            {
                _isRecording = false;
                UpdateTitle();
            });
        };

        _captureManager.RecordingTick += elapsed =>
        {
            Dispatcher.Invoke(() => UpdateTitle());
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
        _isDirty = true;
        UpdateTitle();
        MessageBox.Show($"已添加标记 @ {PreviewCanvas.CurrentTime:mm\\:ss}", "DogSC");
    }

    private void NewProject_Click(object sender, RoutedEventArgs e)
    {
        if (_isDirty)
        {
            var result = MessageBox.Show("当前项目有未保存的更改，是否保存？", "确认",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;
            if (result == MessageBoxResult.Yes) SaveProject_Click(sender, e);
        }

        _project = new ProjectModel { Name = "未命名项目" };
        InitializeProject();
    }

    private async void OpenProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "DogSC 项目|*.dogscproject|所有文件|*.*"
        };
        if (dialog.ShowDialog() == true)
        {
            try
            {
                _project = await _serializer.LoadAsync(dialog.FileName);
                InitializeProject();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开项目失败: {ex.Message}", "错误",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private async void SaveProject_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_project.FilePath))
        {
            SaveAsProject_Click(sender, e);
            return;
        }

        await SaveProjectToFile();
    }

    private async void SaveAsProject_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "DogSC 项目|*.dogscproject",
            FileName = _project.Name + ".dogscproject"
        };
        if (dialog.ShowDialog() == true)
        {
            _project.FilePath = dialog.FileName;
            await SaveProjectToFile();
        }
    }

    private async Task SaveProjectToFile()
    {
        try
        {
            await _serializer.SaveAsync(_project, _project.FilePath);
            _isDirty = false;
            UpdateTitle();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存失败: {ex.Message}", "错误",
                MessageBoxButton.OK, MessageBoxImage.Error);
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
Ctrl+S - 保存项目
Ctrl+O - 打开项目
Ctrl+N - 新建项目
S - 分割片段
Delete - 删除选中片段
Ctrl+Z - 撤销
Ctrl+Y - 重做
空格 - 播放/暂停";
        MessageBox.Show(shortcuts, "快捷键");
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
        {
            _undoManager.Undo();
            e.Handled = true;
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
        {
            _undoManager.Redo();
            e.Handled = true;
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
        {
            SaveProject_Click(this, e);
            e.Handled = true;
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.O)
        {
            OpenProject_Click(this, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Space)
        {
            if (PreviewCanvas.IsPlaying)
                PreviewCanvas.Pause();
            else
                _ = PreviewCanvas.PlayAsync();
            e.Handled = true;
        }
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

        if (_isDirty)
        {
            var result = MessageBox.Show("有未保存的更改，是否保存？", "确认",
                MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }
            if (result == MessageBoxResult.Yes)
            {
                _ = SaveProjectToFile();
            }
        }

        _captureManager.Dispose();
        base.OnClosing(e);
    }
}
