using System.Windows;
using Player.Services;
using Player.Settings;

namespace Player
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            WindowStateManager.Instance.CreateWindows(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Tray.Instance.DisposeIcon();
            WindowStateManager.Instance.Dispose(); 
            LibraryManager.Instance().Dispose();
        }
    }
}