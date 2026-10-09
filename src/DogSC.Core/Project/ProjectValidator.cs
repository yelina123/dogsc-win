using System;
using System.Collections.Generic;
using System.Linq;
using DogSC.Core.Project;

namespace DogSC.Core.Project;

/// <summary>
/// 项目验证器
/// </summary>
public class ProjectValidator
{
    public List<ProjectValidationIssue> Validate(ProjectModel project)
    {
        var issues = new List<ProjectValidationIssue>();

        if (string.IsNullOrWhiteSpace(project.Name))
            issues.Add(new ProjectValidationIssue(
                ProjectIssueLevel.Warning, "项目名称为空", "建议给项目起一个有意义的名字"));

        if (project.Width <= 0 || project.Height <= 0)
            issues.Add(new ProjectValidationIssue(
                ProjectIssueLevel.Error, "画布尺寸无效", $"当前尺寸: {project.Width}x{project.Height}"));

        if (project.FrameRate <= 0 || project.FrameRate > 120)
            issues.Add(new ProjectValidationIssue(
                ProjectIssueLevel.Warning, "帧率异常", $"当前帧率: {project.FrameRate}fps"));

        foreach (var track in project.Timeline.Tracks)
        {
            foreach (var clip in track.Clips)
            {
                if (string.IsNullOrWhiteSpace(clip.MediaFilePath))
                    issues.Add(new ProjectValidationIssue(
                        ProjectIssueLevel.Error, "片段缺少媒体文件", $"片段: {clip.Name}"));

                if (clip.Duration <= TimeSpan.Zero)
                    issues.Add(new ProjectValidationIssue(
                        ProjectIssueLevel.Error, "片段时长为零", $"片段: {clip.Name}"));
            }
        }

        return issues;
    }
}

public enum ProjectIssueLevel
{
    Info,
    Warning,
    Error
}

public record ProjectValidationIssue(
    ProjectIssueLevel Level,
    string Title,
    string Description);
