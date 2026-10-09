using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DogSC.Core.Export;
using DogSC.Core.Project;

namespace DogSC.Export;

/// <summary>
/// 视频导出器 - 基于 FFmpeg
/// </summary>
public class VideoExporter
{
    private Process? _ffmpegProcess;
    private CancellationTokenSource? _cts;

    public event Action<double>? ProgressChanged;
    public event Action<string>? StatusChanged;
    public event Action? ExportCompleted;
    public event Action<Exception>? ExportFailed;

    public bool IsExporting { get; private set; }

    public async Task ExportAsync(ProjectModel project, ExportSettings settings)
    {
        if (IsExporting) return;

        IsExporting = true;
        _cts = new CancellationTokenSource();

        try
        {
            StatusChanged?.Invoke("准备导出...");
            ProgressChanged?.Invoke(0);

            var outputPath = settings.OutputPath;
            var resolution = settings.Resolution.GetResolution();
            var width = resolution?.width ?? project.Width;
            var height = resolution?.height ?? project.Height;

            var ffmpegArgs = BuildFFmpegArgs(project, settings, width, height, outputPath);

            StatusChanged?.Invoke("正在编码...");

            var processStartInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = ffmpegArgs,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _ffmpegProcess = Process.Start(processStartInfo);

            if (_ffmpegProcess == null)
            {
                throw new InvalidOperationException("无法启动 FFmpeg，请确保 ffmpeg 已在 PATH 中");
            }

            var stderrTask = _ffmpegProcess.StandardError.ReadToEndAsync();

            await Task.Run(async () =>
            {
                try
                {
                    await _ffmpegProcess.WaitForExitAsync(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    _ffmpegProcess.Kill();
                }
            });

            var stderr = await stderrTask;

            if (_ffmpegProcess.ExitCode == 0)
            {
                ProgressChanged?.Invoke(1.0);
                StatusChanged?.Invoke("导出完成");
                ExportCompleted?.Invoke();
            }
            else
            {
                throw new InvalidOperationException($"FFmpeg 退出码: {_ffmpegProcess.ExitCode}\n{stderr}");
            }
        }
        catch (Exception ex)
        {
            ExportFailed?.Invoke(ex);
        }
        finally
        {
            IsExporting = false;
        }
    }

    private string BuildFFmpegArgs(ProjectModel project, ExportSettings settings, int width, int height, string outputPath)
    {
        var args = new System.Text.StringBuilder();

        args.Append("-y ");
        args.Append($"-f gdigrab ");
        args.Append($"-framerate {settings.FrameRate} ");
        args.Append($"-i desktop ");

        args.Append($"-c:v h264 ");
        args.Append($"-b:v {settings.VideoBitrate} ");
        args.Append($"-preset medium ");
        args.Append($"-crf 18 ");
        args.Append($"-s {width}x{height} ");

        if (settings.Format == ExportFormat.MP4)
        {
            args.Append("-movflags +faststart ");
        }

        args.Append($"-f {settings.Format.ToString().ToLower()} ");
        args.Append($"\"{outputPath}\"");

        return args.ToString();
    }

    public void Cancel()
    {
        _cts?.Cancel();
        _ffmpegProcess?.Kill();
        IsExporting = false;
    }
}
