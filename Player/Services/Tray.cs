using System;
using System.Drawing;
using System.Windows.Forms; 
using Application = System.Windows.Application;

namespace Player.Services
{
    public static class TimespanExtensions
    {
        public static string ToMinutesAndSeconds(this TimeSpan interval)
        {
            return string.Format("{0}:{1:D2}", interval.Days * 24 * 60 + interval.Hours * 60 + interval.Minutes, interval.Seconds);
        }
    }

    public sealed class Tray
    {
        private static readonly Tray instance = new Tray();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static Tray()
        {
        }

        private Tray()
        {
            var item = new MenuItem { Text = Resources.Exit };
            item.Click += ExitApplication;

            var menu = new ContextMenu();
            menu.MenuItems.Add(item); 
            
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = Icon.FromHandle(Resources.IconPng.GetHicon());
            _trayIcon.ContextMenu = menu;
            _trayIcon.Visible = true; 
        }

        private void ExitApplication(object sender, EventArgs eventArgs)
        {
            Application.Current.Shutdown();
        }

        public static Tray Instance
        {
            get
            {
                return instance;
            }
        }

        private NotifyIcon _trayIcon;

        public void DisposeIcon()
        {
            if (_trayIcon != null)
                _trayIcon.Dispose();
            
            _trayIcon = null;
        }

        public void AddMouseDoubleClick(MouseEventHandler click)
        {
            _trayIcon.MouseDoubleClick += click;
        }

    }
}