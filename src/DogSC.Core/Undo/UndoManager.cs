using System;
using System.Collections.Generic;

namespace DogSC.Core.Undo;

/// <summary>
/// 撤销/重做命令接口
/// </summary>
public interface IUndoCommand
{
    void Execute();
    void Unexecute();
    string Description { get; }
}

/// <summary>
/// 撤销/重做管理器
/// </summary>
public class UndoManager
{
    private readonly Stack<IUndoCommand> _undoStack = new();
    private readonly Stack<IUndoCommand> _redoStack = new();
    private readonly int _maxHistory;

    public event EventHandler? UndoStackChanged;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public int UndoCount => _undoStack.Count;
    public int RedoCount => _redoStack.Count;

    public UndoManager(int maxHistory = 100)
    {
        _maxHistory = maxHistory;
    }

    public void Execute(IUndoCommand command)
    {
        command.Execute();
        _undoStack.Push(command);

        if (_undoStack.Count > _maxHistory)
        {
            var temp = new Stack<IUndoCommand>();
            while (_undoStack.Count > 1)
            {
                temp.Push(_undoStack.Pop());
            }
            _undoStack.Clear();
            while (temp.Count > 0)
            {
                _undoStack.Push(temp.Pop());
            }
        }

        _redoStack.Clear();
        UndoStackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Undo()
    {
        if (!CanUndo) return;

        var command = _undoStack.Pop();
        command.Unexecute();
        _redoStack.Push(command);
        UndoStackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Redo()
    {
        if (!CanRedo) return;

        var command = _redoStack.Pop();
        command.Execute();
        _undoStack.Push(command);
        UndoStackChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
        UndoStackChanged?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>
/// 分割片段命令
/// </summary>
public class SplitClipCommand : IUndoCommand
{
    private readonly Timeline.ProjectTimelineEditing _editing;
    private readonly string _clipId;
    private readonly TimeSpan _atTime;
    private string? _newClipId;

    public string Description => "分割片段";

    public SplitClipCommand(Timeline.ProjectTimelineEditing editing, string clipId, TimeSpan atTime)
    {
        _editing = editing;
        _clipId = clipId;
        _atTime = atTime;
    }

    public void Execute()
    {
        var clip = _editing.FindClip(_clipId);
        if (clip == null) return;

        _editing.SplitClip(_clipId, _atTime);

        var newClip = _editing.FindClip(_clipId)?.TimelineRange.End;
        // 记录新创建的片段 ID（简化处理，实际应从结果获取）
    }

    public void Unexecute()
    {
        // 删除新创建的片段，恢复原片段长度
        if (_newClipId != null)
        {
            _editing.DeleteClip(_newClipId);
        }
    }
}

/// <summary>
/// 删除片段命令
/// </summary>
public class DeleteClipCommand : IUndoCommand
{
    private readonly Timeline.ProjectTimelineEditing _editing;
    private readonly string _clipId;
    private Timeline.TimelineClip? _deletedClip;
    private int _originalTrackIndex;
    private int _originalIndexInTrack;

    public string Description => "删除片段";

    public DeleteClipCommand(Timeline.ProjectTimelineEditing editing, string clipId)
    {
        _editing = editing;
        _clipId = clipId;
    }

    public void Execute()
    {
        _deletedClip = _editing.FindClip(_clipId);
        if (_deletedClip != null)
        {
            _originalTrackIndex = _deletedClip.TrackIndex;
            _editing.DeleteClip(_clipId);
        }
    }

    public void Unexecute()
    {
        if (_deletedClip != null)
        {
            // 恢复片段到原位置（简化实现）
        }
    }
}
