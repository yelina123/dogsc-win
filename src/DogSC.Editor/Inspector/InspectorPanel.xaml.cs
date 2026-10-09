using System.Windows.Controls;

namespace DogSC.Editor.Inspector;

public partial class InspectorPanel : UserControl
{
    public InspectorPanel()
    {
        InitializeComponent();

        SpeedSlider.ValueChanged += (s, e) => SpeedLabel.Text = $"{SpeedSlider.Value:F1}x";
        VolumeSlider.ValueChanged += (s, e) => VolumeLabel.Text = $"{(int)(VolumeSlider.Value * 100)}%";
        CornerRadiusSlider.ValueChanged += (s, e) => CornerRadiusLabel.Text = $"{(int)CornerRadiusSlider.Value}";
        ShadowSlider.ValueChanged += (s, e) => ShadowLabel.Text = $"{(int)ShadowSlider.Value}";
        ZoomSlider.ValueChanged += (s, e) => ZoomLabel.Text = $"{ZoomSlider.Value:F1}x";
        TiltSlider.ValueChanged += (s, e) => TiltLabel.Text = $"{(int)TiltSlider.Value}°";
        PerspectiveSlider.ValueChanged += (s, e) => PerspectiveLabel.Text = $"{(int)PerspectiveSlider.Value}";
    }
}
