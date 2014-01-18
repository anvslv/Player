using System;
using System.Drawing;
using System.Windows.Forms;
using Application = System.Windows.Application;

namespace Player.View.Services
{
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
            _trayIcon = new NotifyIcon();
            _trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);
            var menu = new ContextMenu();
            var item = new MenuItem { Text = "Exit" };
            item.Click += ExitApplication;
            menu.MenuItems.Add(item);
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