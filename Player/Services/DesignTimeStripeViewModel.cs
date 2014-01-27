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
            get { return ResizeMode.CanResize; }
        }

        private double thisWidth = 450;
        public new double ThisWidth
        {
            get { return thisWidth; }
            set { thisWidth = value; }
        }
    }
}