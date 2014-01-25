using System;
using System.IO;

namespace Player.Settings
{
    public static class SettingsPath
    {
        public static string WindowSettingsFilePath;
        public static string AudioSettingsFilePath;

        static SettingsPath()
        {
            string settingsDirectoryPath = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"Player\");
          
            if (!Directory.Exists(settingsDirectoryPath)) 
                Directory.CreateDirectory(settingsDirectoryPath);

            WindowSettingsFilePath = Path.Combine(settingsDirectoryPath, "windowstate.xml");
            AudioSettingsFilePath = Path.Combine(settingsDirectoryPath, "audiostate.xml"); 
        } 
    }
}