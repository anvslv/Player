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
            Draggable.MouseLeftButtonDown += DraggableOnMouseLeftButtonDown;
            Draggable.MouseLeftButtonUp += DraggableOnMouseLeftButtonUp; 
            Drop += OnDrop;
            Closing += OnClosing;
            
            this.Grip.PreviewMouseLeftButtonDown += GripOnMouseDown;
            this.Grip.PreviewMouseLeftButtonUp += GripOnMouseUp;
            this.Grip.PreviewMouseMove += GripOnMouseMove;

            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new DesignTimeStripeViewModel();
            else
            {
                viewModel = new StripeViewModel(LibraryManager.Instance());
                DataContext = viewModel;
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
                Debug.WriteLine(Width);
                Debug.WriteLine(ThisWidth);
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
            this.Draggable.InputBindings.Add(new MouseBinding(viewModel.PauseContinueCommand, new MouseGesture(MouseAction.LeftDoubleClick)));
            this.RightButtonDragLeft(viewModel.PreviousSongCommand);
            this.RightButtonDragRight( viewModel.NextSongCommand);
            this.RightButtonDragDown( viewModel.ShowHidePlaylist);
            this.ScrollDown (viewModel.DecreaseVolume);
            this.ScrollUp(viewModel.IncreaseVolume);
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
