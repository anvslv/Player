using System.Windows; 
using Player.Settings;
using Player.View.Services;
using Player.View.Views;

namespace Player.View
{
    public partial class App : Application
    {
        private IWindowStateManager manager;

        public App()
        {
            this.manager = new WindowStateManager(SettingsPath.WindowSettingsFilePath);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var windowStates = manager.GetWindows();

            foreach (var windowState in windowStates) 
                CreateWindow(windowState); 
        }

        private void CreateWindow(PlayerWindow w)
        {
            BaseWindow window;
            if (w.Window == "Stripe") {
                window = new Stripe();
            } else {
                window = new Songs();
            }
            window.Height = w.Height;
            window.Width = w.Width;
            window.Left = w.Left;
            window.Top = w.Top;

            if(w.IsVisible)
                window.Show(); 
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Tray.Instance.DisposeIcon();
            manager.Save(State.Instance.WindowsStates);
        }
    }
}