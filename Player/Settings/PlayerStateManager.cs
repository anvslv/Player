using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xaml.Permissions;
using System.Xml.Linq;
using Player.Core;

namespace Player.Settings
{
    public class PlayerFileStateManager : IPlayerStateManager
    {
        private string path;

        public PlayerFileStateManager(string filePath)
        {
            if(string.IsNullOrEmpty(filePath))
                throw new ArgumentNullException("filePath");

            path = filePath; 
        }
          
        public void Save(Playlist playlist)
        {
            using (FileStream s = File.Create(path))
            {
                PlayerStateManager.Save(playlist, s);
            }
        }

        public Playlist GetPlaylist()
        {
            if (!File.Exists(path))
                return new Playlist();

            using (FileStream s = File.OpenRead(path))
            {
                return PlayerStateManager.GetPlaylist(s);
            }
        } 
    }

    public static class PlayerStateManager
    {
        public static void Save(Playlist playlist, Stream stream)
        { 
            var document = new XDocument(
                new XElement("Root",
                    new XElement("Playlist",
                        new XElement("Volume",
                            new XAttribute("Value", playlist.Volume)),
                        new XElement("Entries", playlist.Select(entry =>
                            new XElement("Entry",
                                new XAttribute("FilePath", entry.Song.FilePath),
                                new XAttribute("Album", entry.Song.Album),
                                new XAttribute("Artist", entry.Song.Artist),
                                new XAttribute("Title", entry.Song.Title),
                                new XAttribute("Year", entry.Song.Year),
                                new XAttribute("TrackNumber", entry.Song.TrackNumber),
                                entry.Song.IsCurrent ? new XAttribute("IsCurrent", entry.Song.IsCurrent) : null,
                                new XAttribute("Duration", entry.Song.Duration.Ticks),
                                entry.Song.IsCurrent ? new XAttribute("CurrentPosition", entry.Song.CurrentPosition.Ticks) : null
            ))))));

            document.Save(stream);
           
        }

        public static Playlist GetPlaylist(Stream s)
        { 
            IEnumerable<XElement> playlist = XDocument.Load(s)
                .Descendants("Root")
                .Descendants("Playlist").ToList();

            IEnumerable<Song> songs = playlist
                .Descendants("Entries")
                .Elements("Entry")
                .Select(song => {
                    var localsong = new LocalSong(
                        song.Attribute("FilePath").Value,
                        TimeSpan.FromTicks(Int64.Parse(song.Attribute("Duration").Value)))
                    {
                        Album = song.Attribute("Album").Value,
                        Artist = song.Attribute("Artist").Value,
                        Title = song.Attribute("Title").Value,
                        Year = Int32.Parse(song.Attribute("Year").Value),
                        TrackNumber = Int32.Parse(song.Attribute("TrackNumber").Value)
                    };
                    if (song.Attribute("IsCurrent") !=null)
                        localsong.IsCurrent = 
                            Convert.ToBoolean(song.Attribute("IsCurrent").Value);
                    if (song.Attribute("CurrentPosition") !=null)
                        localsong.CurrentPosition =
                            TimeSpan.FromTicks(Int64.Parse(song.Attribute("CurrentPosition").Value));

                    return localsong;
                });

            float volume = playlist
                .Descendants("Volume")
                .Select(v =>
                    float.Parse(v.Attribute("Value").Value))
                .FirstOrDefault();

            var p = new Playlist();
            p.AddSongs(songs);
            p.Volume = volume;
            return p; 
        } 
    }
}