﻿// http://programminghacks.net/2009/10/19/download-snapping-sticky-magnetic-windows-for-wpf/

using System;
using System.Diagnostics;
using System.Drawing; 
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Point=System.Drawing.Point;
using Size=System.Drawing.Size;
using System.Windows.Media; 

namespace StickyWindowLibrary
{
    [DebuggerStepThrough]
    public class WpfFormAdapter : IFormAdapter
    {
        private readonly Window _window;
        private Point? _origin;

        public WpfFormAdapter(Window window)
        {
            _window = window;
        }

        #region IFormAdapter Members

        public IntPtr Handle
        {
            get
            {
                return new WindowInteropHelper(_window).Handle;
            }
        }

        public Rectangle Bounds
        {
            get
            {
                // converti width ed height ad absolute
                System.Windows.Point widthHeightPointConverted = fromRelativeToDevice(_window.ActualWidth, _window.ActualHeight, _window);

                // converti coordinate da relative a screen: la libreria lavora con quelle
                Point origin = getWindowOrigin();
                Point pStart = this.PointToScreen(origin);
                Point pEnd = this.PointToScreen(origin + new Size(Convert.ToInt32(widthHeightPointConverted.X), Convert.ToInt32(widthHeightPointConverted.Y)));
                pEnd.Offset(-pStart.X, -pStart.Y); // ora pend rappresenta width + height

                // imposta
                return new Rectangle(pStart.X, pStart.Y, pEnd.X, pEnd.Y);
            }
            set
            {
                // converti width ed height a relative
                System.Windows.Point widthHeightPointConverted = fromDeviceToRelative(value.Width, value.Height, _window);
                // converti coordinate da screen a relative: il video non si deve alterare!
                Point origin = getWindowOrigin();
                Point screenPointRef = new Point(-origin.X + value.X, -origin.Y + value.Y);
                var pStart = this.PointFromScreen(new Point(screenPointRef.X, screenPointRef.Y));

                // imposta
                _window.Left += pStart.X;
                _window.Top += pStart.Y;
                
                // todo replace condition with
                //  _window.Width = widthHeightPointConverted.X;
                //  _window.Height = widthHeightPointConverted.Y;
                if (widthHeightPointConverted.X >= 0.0f && widthHeightPointConverted.Y >= 0.0f)
                {
                    _window.Width = widthHeightPointConverted.X;
                    _window.Height = widthHeightPointConverted.Y;
                } 
            }
        }

        public Size MaximumSize
        {
            get
            {
                var width = _window.MaxWidth;
                var height = _window.MaxHeight;
                if (double.IsInfinity(width))
                {
                    width = int.MaxValue;
                }
                if (double.IsInfinity(height))
                {
                    height = int.MaxValue;
                }
                return new Size(Convert.ToInt32(width), Convert.ToInt32(height));
            }
            set {
                _window.MaxWidth = value.Width;
                _window.MaxHeight = value.Height;
            }
        }  

        public Size MinimumSize
        {
            get { return new Size(Convert.ToInt32(_window.MinWidth), Convert.ToInt32(_window.MinHeight)); }
            set
            {
                _window.MinWidth = value.Width;
                _window.MinHeight = value.Height;
            }
        }

        public bool Capture
        {
            get { return _window.IsMouseCaptured; }
            set
            {
                IInputElement targetToCapture = value ? _window : null;
                Mouse.Capture(targetToCapture);
            }
        }

        public void Activate()
        {
            _window.Activate();
        }

        public Point PointToScreen(Point pointWin)
        {
            System.Windows.Point p = new System.Windows.Point();
            Point resultWpf = toWinPoint(_window.PointToScreen(p));
            Point resultScaled = resultWpf + new Size(pointWin);
            return resultScaled;
        }

        #endregion

        #region Utility Methods

        private Point getWindowOrigin()
        {
            // TODO: on the first invocation to go to the cache to improve perf and avoid errors of approx
            //return new Point(-4, -28);
            if(!_origin.HasValue)
            {
                var currentWinPointConverted = fromRelativeToDevice(-_window.Left, -_window.Top, _window);
                Point locationFromScreen = this.PointToScreen(toWinPoint(currentWinPointConverted));
                _origin = new Point(-locationFromScreen.X, -locationFromScreen.Y);
            }

            return _origin.Value;
        }

        private static System.Windows.Point fromDeviceToRelative(double x, double y, Visual workingVisual)
        {
            Point widthHeightPoint = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
            PresentationSource source = PresentationSource.FromVisual(workingVisual);
            return source.CompositionTarget.TransformFromDevice.Transform(toWpfPoint(widthHeightPoint));
        }

        private static System.Windows.Point fromRelativeToDevice(double x, double y, Visual workingVisual)
        {
            Point widthHeightPoint = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
            PresentationSource source = PresentationSource.FromVisual(workingVisual);
            return source.CompositionTarget.TransformToDevice.Transform(toWpfPoint(widthHeightPoint));
        }

        public Point PointFromScreen(Point pointWin)
        {
            return toWinPoint(_window.PointFromScreen(toWpfPoint(pointWin)));
        }

        private static System.Windows.Point toWpfPoint(Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }

        private static Point toWinPoint(System.Windows.Point point)
        {
            return new Point(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        #endregion
    }
}