using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;
using System.Windows.Interop;
using Player.Services;
using Player.ViewModels;

namespace Player.Views
{ 
    public partial class Stripe : BaseWindow
    {
        private StripeViewModel viewModel;

        public Stripe()
        { 
            InitializeComponent();
             
            this.Draggable.MouseLeftButtonDown += DraggableOnMouseLeftButtonDown;
            this.Draggable.MouseLeftButtonUp += DraggableOnMouseLeftButtonUp; 
            this.Drop += OnDrop;
            this.Closing += OnClosing;
            
            this.Grip.PreviewMouseLeftButtonDown += GripOnMouseDown;
            this.Grip.PreviewMouseLeftButtonUp += GripOnMouseUp;
            this.Grip.PreviewMouseMove += GripOnMouseMove;

            if (DesignerProperties.GetIsInDesignMode(this))
                this.DataContext = new DesignTimeStripeViewModel();
            else
            {
                this.viewModel = new StripeViewModel(LibraryManager.Instance());
                this.DataContext = viewModel;
            }

            WireCommands();
        } 

        bool isResizing = false;
        Point startPt;

        public double ThisWidth
        {
            get { return viewModel.ThisWidth; }
            set
            {
                viewModel.ThisWidth = value; 
            }
        }

        private void GripOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Mouse.Capture(this.Grip);
                isResizing = true; 
                startPt = e.GetPosition(this.Grip); 
            }
        }
        private void GripOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isResizing)
            {
                isResizing = false; 
                this.Grip.ReleaseMouseCapture();
            }
        }

        private void GripOnMouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                Point newPt = e.GetPosition(this.Grip);
                var newWidth = this.Width + newPt.X - startPt.X;
                ThisWidth = newWidth; 
            }
        }
       
        private void WireCommands()
        { 
            this.Draggable.DoubleClick(viewModel.PauseContinueCommand);
            this.RightButtonDragLeft(viewModel.PreviousSongCommand);
            this.RightButtonDragRight(viewModel.NextSongCommand);
            this.RightButtonDragDown( viewModel.ShowHidePlaylist);
            this.ScrollDown (viewModel.DecreaseVolume);
            this.ScrollUp(viewModel.IncreaseVolume);

            this.PlayPause(viewModel.PauseContinueCommand);
            this.NextTrack(viewModel.NextSongCommand);
            this.PreviousTrack(viewModel.PreviousSongCommand);
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        { 
            viewModel.Dispose();
        }

        public override void ShowHideWindow()
        {
            if (IsVisible)
            { 
                Hide();
            }
            else
            { 
                Show();
            }
        }
         

        public override ResizeMode GetResizeMode()
        {
            return ResizeMode.CanResize;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                viewModel.HandleDropFiles(files); 
            } 
        }
           
    }
}
