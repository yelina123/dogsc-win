using System.Windows;
using DogSC.Core.Export;

namespace DogSC.App.Windows;

public partial class ExportDialog : Window
{
    public ExportDialog()
    {
        InitializeComponent();
    }

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "MP4 视频|*.mp4|MKV 视频|*.mkv|M4A 音频|*.m4a",
            FileName = "DogSC_Export.mp4"
        };
        if (dialog.ShowDialog() == true)
        {
            PathBox.Text = dialog.FileName;
        }
    }

    private async void Export_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(PathBox.Text))
        {
            MessageBox.Show("请选择输出位置", "提示");
            return;
        }

        var settings = new ExportSettings
        {
            OutputPath = PathBox.Text,
            Resolution = ResolutionBox.Text.Contains("1080") ? ExportResolution._1080p :
                         ResolutionBox.Text.Contains("1440") ? ExportResolution._1440p :
                         ResolutionBox.Text.Contains("4K") ? ExportResolution._4K :
                         ExportResolution.Source,
            FrameRate = FpsBox.Text.Contains("60") ? 60 : 30
        };

        var exporter = new Export.VideoExporter();
        exporter.ProgressChanged += progress =>
        {
            Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = progress;
                StatusText.Text = $"导出中... {progress:P0}";
            });
        };

        exporter.StatusChanged += status =>
        {
            Dispatcher.Invoke(() => StatusText.Text = status);
        };

        exporter.ExportCompleted += () =>
        {
            Dispatcher.Invoke(() =>
            {
                StatusText.Text = "导出完成！";
                MessageBox.Show("导出成功！", "DogSC");
            });
        };

        await exporter.ExportAsync(new Core.Project.ProjectModel(), settings);
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
