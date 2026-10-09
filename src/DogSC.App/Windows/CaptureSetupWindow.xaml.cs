using System.Windows;
using DogSC.Core.Models;

namespace DogSC.App.Windows;

public partial class CaptureSetupWindow : Window
{
    public CaptureConfiguration CaptureConfig { get; private set; } = new();

    public CaptureSetupWindow()
    {
        InitializeComponent();
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        CaptureConfig = new CaptureConfiguration
        {
            SourceType = ScreenRadio.IsChecked == true ? CaptureSourceType.Screen :
                         WindowRadio.IsChecked == true ? CaptureSourceType.Window :
                         CaptureSourceType.Region,
            CaptureMicrophone = MicCheck.IsChecked == true,
            CaptureSystemAudio = SystemAudioCheck.IsChecked == true,
            CaptureCamera = CameraCheck.IsChecked == true,
            CaptureCursor = CursorCheck.IsChecked == true,
            FrameRate = FpsBox.Text.Contains("60") ? 60 : 30
        };

        if (CaptureConfig.SourceType == CaptureSourceType.Region)
        {
            var region = Capture.Utils.CaptureAreaSelector.SelectArea();
            if (region.HasValue)
            {
                CaptureConfig.RegionBounds = region.Value;
            }
            else
            {
                return;
            }
        }

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
