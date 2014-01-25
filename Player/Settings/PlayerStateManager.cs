using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq; 
using System.Xml.Linq; 
using Player.Model;

namespace Player.Settings
{
    public interface IPlayerStateManager
    {
        void Save(Playlist playlist);
        Playlist GetPlaylist();
    }

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
                        new XElement("CurrentSongIndex",
                            new XAttribute("Value", playlist.CurrentSongIndex ?? 0)),
                        new XElement("CurrentTime",
                            new XAttribute("Value", playlist.CurrentTime.Ticks)),
                        new XElement("Entries", playlist.Select(entry =>
                            new XElement("Entry",
                                new XAttribute("FilePath", entry.Song.FilePath),
                                new XAttribute("Album", entry.Song.Album),
                                new XAttribute("Artist", entry.Song.Artist),
                                new XAttribute("Title", entry.Song.Title),
                                new XAttribute("Year", entry.Song.Year),
                                new XAttribute("TrackNumber", entry.Song.TrackNumber),
                                new XAttribute("Duration", entry.Song.Duration.Ticks)
            ))))));

            document.Save(stream);
           
        }

        public static Playlist GetPlaylist(Stream s)
        {
            IEnumerable<XElement> playlist;
            try
            { 
                playlist = XDocument.Load(s)
                    .Descendants("Root")
                    .Descendants("Playlist").ToList();
            } catch {
                return new Playlist();
            }

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
                    return localsong;
                });

            float volume = playlist
                .Descendants("Volume")
                .Select(v =>
                    float.Parse(v.Attribute("Value").Value, CultureInfo.InvariantCulture.NumberFormat))
                .FirstOrDefault();

            int currentSongIndex = playlist
               .Descendants("CurrentSongIndex")
               .Select(e =>
                   int.Parse(e.Attribute("Value").Value))
               .FirstOrDefault();
             
            TimeSpan currentTime = playlist
                .Descendants("CurrentTime")
                .Select(e => 
                    TimeSpan.FromTicks(Int64.Parse(e.Attribute("Value").Value)))
                .FirstOrDefault();

            var p = new Playlist();
            p.AddSongs(songs);
            p.Volume = volume;
            p.CurrentSongIndex = currentSongIndex;
            p.CurrentTime = currentTime;

            return p; 
        } 
    }
}