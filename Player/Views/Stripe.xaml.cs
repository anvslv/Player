 
using System.Windows; 
using Player.View.Services;
using Player.View.ViewModels;

namespace Player.View.Views
{
    public partial class Stripe : BaseWindow
    {
        private StripeViewModel viewModel;

        public Stripe()
        { 
            InitializeComponent(); 
            Draggable.PreviewMouseDown += OnPreviewMouseDown;
            Draggable.PreviewMouseUp += OnPreviewMouseUp; 
            Drop += OnDrop; 
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                viewModel.HandleDropFiles(files); 
            } 
        }
          
        public override ResizeMode ThisResizeMode()
        {
            return ResizeMode.NoResize;
        } 
    }
}
