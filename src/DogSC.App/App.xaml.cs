using System;
using System.Windows;

namespace DogSC.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}", "DogSC",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        };
    }
}
