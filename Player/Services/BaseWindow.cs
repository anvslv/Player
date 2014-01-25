// http://programminghacks.net/2009/10/19/download-snapping-sticky-magnetic-windows-for-wpf/

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Blue.Private.Win32Imports;
using Blue.Windows; 
using Player.Settings;

namespace Player.Services 
{
    public abstract class BaseWindow : Window, IDisposable
    {
        public StickyWindow StickyWindow;
        private bool _isHidden;
        public abstract ResizeMode GetResizeMode();

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
            StickyWindow.StickGap = 5;
            //LocationChanged += OnLocationChanged;
        }

        // todo causes jumpiness
        private void OnLocationChanged(object sender, EventArgs e)
        {
            Point mousePoint = Mouse.GetPosition(this);
            Point screenPoint = PointToScreen(mousePoint);

            Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_NCLBUTTONDOWN, Win32.HT.HTCAPTION,
                Win32.MakeLParam(Convert.ToInt32(screenPoint.X), Convert.ToInt32(screenPoint.Y)));
            Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_MOUSEMOVE, Win32.HT.HTCAPTION,
                Win32.MakeLParam(Convert.ToInt32(mousePoint.X), Convert.ToInt32(mousePoint.Y)));
        }

        public void DraggableOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (ResizeMode != ResizeMode.NoResize)
                {
                    UpdateLayout();
                }

                Point mousePoint = Mouse.GetPosition(this);
                Point screenPoint = PointToScreen(mousePoint);

                // this shomehow removes jumpiness, WM_SYSCOMMAND and HTCAPTIONBAR are from part DragMove() internals
                Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_SYSCOMMAND, Win32.HT.HTCAPTIONBAR,
                    Win32.MakeLParam(Convert.ToInt32(screenPoint.X), Convert.ToInt32(screenPoint.Y)));
                //Win32.SendMessage(StickyWindow.Handle, Win32.WM.WM_LBUTTONUP, Win32.HT.HTCAPTIONBAR,
                //   Win32.MakeLParam(Convert.ToInt32(screenPoint.X), Convert.ToInt32(screenPoint.Y)));

                //DragMove();
            }
        }
         
        public void DraggableOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == ResizeMode.NoResize)
            {
                ResizeMode = GetResizeMode();
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