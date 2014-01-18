using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Blue.Private.Win32Imports;
using Blue.Windows;
using Player.Core; 
using Player.Settings;

namespace Player.View.Services 
{
    public abstract class BaseWindow : Window, IDisposable
    {
        public StickyWindow StickyWindow;
        private bool _isHidden;

        protected BaseWindow()
        {
            Loaded += OnLoaded;
            Closing += OnClosing;
            Tray.Instance.AddMouseDoubleClick(TrayIconMouseDoubleClick);
        }

        public void Dispose()
        {
            Tray.Instance.DisposeIcon();
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            State state = State.Instance;

            state.WindowsStates.Add(new PlayerWindow
            {
                Window = Title,
                Height = Height,
                Width = Width,
                Top = Top,
                Left = Left,
                IsVisible = IsVisible
            });
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            StickyWindow = new StickyWindow(this);
            StickyWindow.StickToScreen = true;

            LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object sender, EventArgs e)
        {
            Point mousePoint = Mouse.GetPosition(this);
            Point screenPoint = PointToScreen(mousePoint);

            Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_NCLBUTTONDOWN, Win32.HT.HTCAPTION,
                Win32.MakeLParam(Convert.ToInt32(screenPoint.X), Convert.ToInt32(screenPoint.Y)));
            Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_MOUSEMOVE, Win32.HT.HTCAPTION,
                Win32.MakeLParam(Convert.ToInt32(mousePoint.X), Convert.ToInt32(mousePoint.Y)));
        }

        public abstract ResizeMode ThisResizeMode();

        public void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (ResizeMode != ResizeMode.NoResize)
                {
                    ResizeMode = ResizeMode.NoResize;
                    UpdateLayout();
                }
                DragMove();
            }
        }

        public void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == ResizeMode.NoResize)
            {
                ResizeMode = ThisResizeMode();
                UpdateLayout();
            }
        }

        private void TrayIconMouseDoubleClick(object sender, EventArgs e)
        {
            if (_isHidden == false)
            {
                _isHidden = true;
                Hide();
            }
            else
            {
                _isHidden = false;
                Show();
            }
        }
    }
}