using System.Windows;
using Player.ViewModels;

namespace Player.Services
{
    public class DesignTimePlaylistViewModel : PlaylistViewModel
    {
        public DesignTimePlaylistViewModel()
            : base(LibraryManager.Instance())
        { 
        }

        public ResizeMode ThisResizeMode
        {
            get { return ResizeMode.CanResizeWithGrip; }
        } 
    }
}