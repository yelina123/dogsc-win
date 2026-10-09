using System.Windows;
using System.Windows.Input;

namespace DogSC.App.Windows;

public partial class MemoWindow : Window
{
    public MemoWindow()
    {
        InitializeComponent();
        MouseLeftButtonDown += (s, e) => DragMove();
    }
}
