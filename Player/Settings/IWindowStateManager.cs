using System.Collections.Generic;
using Player.View.Services;

namespace Player.Settings
{
    public interface IWindowStateManager
    { 
        void Save(IEnumerable<PlayerWindow> windows);
        IEnumerable<PlayerWindow> GetWindows(); 
    }
}