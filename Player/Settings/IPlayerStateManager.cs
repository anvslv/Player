using Player.Core;

namespace Player.Settings
{
    public interface IPlayerStateManager
    {
        void Save(Playlist playlist); 
        Playlist GetPlaylist(); 
    }
}