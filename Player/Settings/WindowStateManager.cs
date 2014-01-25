using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Xml.Linq;
using Player.Model;
using Player.Services; 
using Player.Views;

namespace Player.Settings
{
    interface IWindowStateManager
    {
        void Dispose(); 
        void CreateWindows();
        void ShowHidePlaylist();
    }

    class WindowStateManager : IWindowStateManager
    {
        private string path;
        private Songs songs;
        private BaseWindow stripe;

        private static IWindowStateManager _instance;

        public static IWindowStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WindowStateManager(SettingsPath.WindowSettingsFilePath);
                }
                return _instance;
            }
        }
         
        private WindowStateManager(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            path = filePath;
        }

        public void Dispose()
        {
            var windows = State.Instance.WindowsStates;
            using (FileStream s = File.Create(path))
            {
                var document = new XDocument(
                    new XElement("Root",
                        new XElement("Windows",
                        new XElement("Entries", windows.Select(entry =>
                            new XElement("Entry",
                                new XAttribute("Window", entry.Window),
                                new XAttribute("Width", entry.Width),
                                new XAttribute("Height", entry.Height),
                                new XAttribute("Left", entry.Left),
                                new XAttribute("IsVisible", entry.IsVisible),
                                new XAttribute("Top", entry.Top)))))));

                document.Save(s);
            }
        }

        private IEnumerable<PlayerWindow> GetWindows()
        {
            if (!File.Exists(path))
                return new [] {
                    PlayerWindow.GetStripeWindow(), 
                    PlayerWindow.GetSongsWindow()
                };

            using (FileStream s = File.OpenRead(path))
            {
                IEnumerable<PlayerWindow> windows = XDocument.Load(s)
                    .Descendants("Root")
                    .Descendants("Windows")
                    .Descendants("Entries")
                    .Elements("Entry")
                    .Select(window =>
                        new PlayerWindow
                        {
                            Top = Convert.ToDouble(window.Attribute("Top").Value),
                            Left = Convert.ToDouble(window.Attribute("Left").Value),
                            Width = Convert.ToDouble(window.Attribute("Width").Value),
                            Height = Convert.ToDouble(window.Attribute("Height").Value),
                            Window =  window.Attribute("Window").Value,
                            IsVisible = Convert.ToBoolean(window.Attribute("IsVisible").Value), 
                        }
                    );
                return windows;
            }
        }

        public void CreateWindows()
        {  
            var windowStates = GetWindows();

            foreach (var windowState in windowStates) 
                CreateWindow(windowState); 
        }

        private void CreateWindow(PlayerWindow w)
        {
            if (w.Window == "Stripe")
            {
                stripe = new Stripe();
                stripe.Height = w.Height;
                stripe.Width = w.Width;
                stripe.Left = w.Left;
                stripe.Top = w.Top;
                stripe.Topmost = true;

                if (w.IsVisible)
                    stripe.Show();
            }
            else
            {
                songs = new Songs();
                songs.Height = w.Height;
                songs.Width = w.Width;
                songs.Left = w.Left;
                songs.Top = w.Top;
                songs.Topmost = true;

                if (w.IsVisible)
                    songs.Show();
            } 
        }

        public void ShowHidePlaylist()
        {
            songs.IsHidden = !songs.IsHidden;
            songs.ShowHideWindow();
        }
    } 
}