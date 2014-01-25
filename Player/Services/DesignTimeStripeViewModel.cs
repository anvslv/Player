using System.Windows;
using Player.ViewModels;

namespace Player.Services
{
    public class DesignTimeStripeViewModel : StripeViewModel
    {
        public DesignTimeStripeViewModel()
            : base(LibraryManager.Instance())
        { 
        }

        public ResizeMode ThisResizeMode
        {
            get { return ResizeMode.NoResize; }
        } 
    }
}