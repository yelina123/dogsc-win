using System;

namespace DogSC.Core.Export;

/// <summary>
/// 导出策略
/// </summary>
public class ExportPolicy
{
    public bool AllowCancellation { get; set; } = true;
    public bool ShowProgress { get; set; } = true;
    public bool PreserveProjectFile { get; set; } = true;
    public string ProjectFileExtension { get; set; } = ".dogscproject";

    public static ExportPolicy Default => new();
}
