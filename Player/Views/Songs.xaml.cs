using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Blue.Private.Win32Imports;
using Blue.Windows;
using Player.Core;
using Player.Settings;
using Player.View.Services;
using Player.View.ViewModels;

namespace Player.View.Views
{  
    public partial class Songs : BaseWindow
    {

        public Songs()
        {
            InitializeComponent();
            Draggable.PreviewMouseDown += OnPreviewMouseDown;
            Draggable.PreviewMouseUp += OnPreviewMouseUp; 
            Loaded += OnLoaded;
             
            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new DesignTimePlaylistViewModel();
            else
                DataContext = new PlaylistViewModel(new Library(new PlayerFileStateManager(SettingsPath.AudioSettingsFilePath)));
        }
         
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            StickyWindow.StickToOther = true;
            StickyWindow.StickOnResize = true; 
        }

        public override ResizeMode ThisResizeMode()
        {
            return ResizeMode.CanResizeWithGrip;
        }
 
           
    }
}
