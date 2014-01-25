// http://programminghacks.net/2009/10/19/download-snapping-sticky-magnetic-windows-for-wpf/

using System; 
using System.Drawing; 

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
