namespace Player.Services
{
    public class PlayerWindow
    {
        public string Window { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
        public bool IsVisible { get; set; }

        public static PlayerWindow GetStripeWindow()
        {
            return new PlayerWindow
            {
                Window = "Stripe",
                Width = 400,
                Height = 100,
                Top = 0,
                Left = System.Windows.SystemParameters.WorkArea.Width - 500,
                IsVisible = true
            };
        }

        public static PlayerWindow GetSongsWindow()
        {
            return new PlayerWindow
            {
                Window = "Songs",
                Width = 400,
                Height = 400,
                Top = 24,
                Left = System.Windows.SystemParameters.WorkArea.Width - 500,
                IsVisible = true
            };
        }
    }
}