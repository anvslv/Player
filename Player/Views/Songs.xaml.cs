using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input; 
using Player.Services; 
using Player.ViewModels;

namespace Player.Views
{  
    public partial class Songs : BaseWindow
    {
        private readonly PlaylistViewModel viewmodel;
        
        public Songs()
        {
            InitializeComponent();
            Draggable.PreviewMouseLeftButtonDown += DraggableOnMouseLeftButtonDown;
            Draggable.PreviewMouseLeftButtonUp += DraggableOnMouseLeftButtonUp; 
            Loaded += OnLoaded;
            ResizeMode = GetResizeMode();

            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new DesignTimePlaylistViewModel();
            else
            {
                viewmodel = new PlaylistViewModel(LibraryManager.Instance());
                DataContext = viewmodel;
            }
        }
          
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            StickyWindow.StickToOther = true;
            StickyWindow.StickOnResize = true; 
        }
         

        public override void ShowHideWindow()
        {
            if (IsHidden || IsVisible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        public bool IsHidden { get; set; }

        public override sealed ResizeMode GetResizeMode()
        {
            return ResizeMode.CanResizeWithGrip;
        }
        
        private void PlaylistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.viewmodel.SelectedPlaylistEntries = ((ListView)sender).SelectedItems.Cast<PlaylistEntryViewModel>();
        }
         
        private void PlaylistDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.viewmodel.PlayCommand.CanExecute(null))
            {
                this.viewmodel.PlayCommand.Execute(null);
            }
        }

        private void PlaylistKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                if (this.viewmodel.RemoveSelectedPlaylistEntriesCommand.CanExecute(null))
                {
                    this.viewmodel.RemoveSelectedPlaylistEntriesCommand.Execute(null);
                }
            }

            else if (e.Key == Key.Enter)
            {
                if (this.viewmodel.PlayCommand.CanExecute(null))
                {
                    this.viewmodel.PlayCommand.Execute(null);
                }
            }
        }
    }
}
