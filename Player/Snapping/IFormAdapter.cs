
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace StickyWindowLibrary
{
    public interface IFormAdapter
    {
        IntPtr Handle { get; }
        Rectangle Bounds { get; set; }
        Size MaximumSize { get; set; }
        Size MinimumSize { get; set; }
        bool Capture { get; set; }
        void Activate();
        Point PointToScreen(Point point);
    }
}
