using Player.Model;
using Player.Settings; 

namespace Player.Services
{
    internal static class LibraryManager
    {
        private static Library _instance;

        public static Library Instance()
        {
            if (_instance == null)
            { 
                _instance = new Library(new PlayerFileStateManager(SettingsPath.AudioSettingsFilePath));
            }

            return _instance;
        }
    } 

}
