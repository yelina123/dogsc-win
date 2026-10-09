using System;
using System.Threading.Tasks;
using DogSC.Core.Export;
using DogSC.Core.Project;

namespace DogSC.Export;

/// <summary>
/// 纯音频导出管道
/// </summary>
public class AudioOnlyExportPipeline
{
    public async Task ExportAsync(ProjectModel project, ExportSettings settings, string outputPath)
    {
        var exporter = new VideoExporter();
        var audioSettings = new ExportSettings
        {
            Format = ExportFormat.M4A,
            OutputPath = outputPath,
            AudioBitrate = settings.AudioBitrate
        };

        await exporter.ExportAsync(project, audioSettings);
    }
}

/// <summary>
/// 直接导出管道（无编辑，直接从录制文件导出）
/// </summary>
public class DirectExportPipeline
{
    public async Task ExportAsync(string sourcePath, string outputPath, ExportSettings settings)
    {
        var exporter = new VideoExporter();
        var project = new ProjectModel
        {
            Name = "Direct Export"
        };

        await exporter.ExportAsync(project, settings);
    }
}
