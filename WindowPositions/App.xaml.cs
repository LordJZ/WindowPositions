using System;
using System.Linq;
using System.Windows;

namespace WindowPositions
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            LordJZ.WinAPI.Process.SetDpiAwareness(LordJZ.WinAPI.ProcessDpiAwareness.PerMonitorDpiAware);
            LordJZ.Presentation.Controls.BaseWindow.PerMonitorDpiAware = true;

            System.Threading.Thread.CurrentThread.Name = "WindowPositionsUI";

            base.OnStartup(e);

            if (e.Args.Any(arg => arg == "/RestoreAllPositions"))
            {
                WindowPositionRepository.RestoreAllPositions();
                Environment.Exit(0);
            }
        }
    }
}
