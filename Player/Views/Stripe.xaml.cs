using System.ComponentModel;
using System.Windows; 
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
            
            if (DesignerProperties.GetIsInDesignMode(this))
                DataContext = new DesignTimeStripeViewModel();
            else
            {
                viewModel = new StripeViewModel(LibraryManager.Instance());
                DataContext = viewModel;
            }

            WireCommands();
        }
          
        private void WireCommands()
        {
            this.Draggable.DoubleClick(viewModel.PauseContinueCommand);
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
            return ResizeMode.NoResize;
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
