using System.Windows;
using Player.Core;
using Player.Settings;
using Player.View.ViewModels;

namespace Player.View.Services
{
    public class DesignTimePlaylistViewModel : PlaylistViewModel
    {
        public DesignTimePlaylistViewModel()
            : base(DesignTime.LoadLibrary())
        { 
        } 
    }

    public class DesignTimeStripeViewModel : StripeViewModel
    {
        public DesignTimeStripeViewModel()
            : base(DesignTime.LoadLibrary())
        { 
        }
    }

    internal static class DesignTime
    {
        private static Library library;

        public static Library LoadLibrary()
        {
            if (library == null)
            { 
                library = new Library(new PlayerFileStateManager(SettingsPath.AudioSettingsFilePath));
            }

            return library;
        }
    } 

}
