using System;
using System.Drawing;
using System.Windows.Forms;

namespace DogSC.Capture.Utils;

/// <summary>
/// 屏幕区域选择器 - 全屏遮罩，让用户拖拽选择录制区域
/// </summary>
public class CaptureAreaSelector : Form
{
    private bool _isDragging;
    private Point _startPoint;
    private Rectangle _selectionRect;

    public event Action<Rectangle>? AreaSelected;
    public event Action? SelectionCancelled;

    public CaptureAreaSelector()
    {
        FormBorderStyle = FormBorderStyle.None;
        WindowState = FormWindowState.Maximized;
        TopMost = true;
        Opacity = 0.3;
        BackColor = Color.Black;
        Cursor = Cursors.Cross;
        DoubleBuffered = true;

        MouseDown += OnMouseDown;
        MouseMove += OnMouseMove;
        MouseUp += OnMouseUp;
        KeyPreview = true;
        KeyDown += OnKeyDown;
    }

    private void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            _startPoint = e.Location;
            _selectionRect = new Rectangle(_startPoint, Size.Empty);
        }
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_isDragging) return;

        var x = Math.Min(_startPoint.X, e.X);
        var y = Math.Min(_startPoint.Y, e.Y);
        var width = Math.Abs(e.X - _startPoint.X);
        var height = Math.Abs(e.Y - _startPoint.Y);

        _selectionRect = new Rectangle(x, y, width, height);
        Invalidate();
    }

    private void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (!_isDragging) return;
        _isDragging = false;

        if (_selectionRect.Width > 10 && _selectionRect.Height > 10)
        {
            AreaSelected?.Invoke(_selectionRect);
            Close();
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            SelectionCancelled?.Invoke();
            Close();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_selectionRect.Width > 0 && _selectionRect.Height > 0)
        {
            using (var brush = new SolidBrush(Color.FromArgb(0, 0, 0, 0)))
            {
                e.Graphics.FillRectangle(brush, _selectionRect);
            }

            using var pen = new Pen(Color.Red, 2);
            e.Graphics.DrawRectangle(pen, _selectionRect);

            using var font = new Font("Arial", 10);
            var sizeText = $"{_selectionRect.Width} × {_selectionRect.Height}";
            e.Graphics.DrawString(sizeText, font, Brushes.Red,
                _selectionRect.X + 5, _selectionRect.Y - 20);
        }
    }

    public static Rectangle? SelectArea()
    {
        Rectangle? result = null;
        var selector = new CaptureAreaSelector();
        selector.AreaSelected += rect => result = rect;
        selector.ShowDialog();
        return result;
    }
}
