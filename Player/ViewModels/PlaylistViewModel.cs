 
using Player.Core;

namespace Player.View.ViewModels
{
    public class PlaylistViewModel
    {
        private readonly Library library;

        public PlaylistViewModel(Library l)
        {
            this.library = l;
        }
    }
}