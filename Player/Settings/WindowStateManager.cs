using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Player.View.Services;

namespace Player.Settings
{
    public class WindowStateManager : IWindowStateManager
    {
        private string path;

        public WindowStateManager(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            path = filePath;
        }

        public void Save(IEnumerable<PlayerWindow> windows)
        {
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

        public IEnumerable<PlayerWindow> GetWindows()
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

    }
}