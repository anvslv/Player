using System;
using System.Collections.Generic;
using System.IO;
using Player.Core;
using Player.Model;
using Player.Settings;
using Moq;

namespace Player.Tests
{
    internal static class Helpers
    {
        public static readonly LocalSong LocalSong1 = new LocalSong("Path1", TimeSpan.FromTicks(1))
        {
            Album = "Album1",
            Artist = "Artist1",
            Year = 2005,
            Title = "Title1",
            TrackNumber = 1
        };

        public static readonly LocalSong LocalSong2 = new LocalSong("Path2", TimeSpan.FromTicks(2))
        {
            Album = "Album2",
            Artist = "Artist2",
            Year = 2006, 
            Title = "Title2",
            TrackNumber = 2
        };

        public static readonly Playlist Playlist1;
         
        static Helpers()
        {
            Playlist1 = new Playlist();
            Playlist1.AddSongs(new[] { LocalSong1, LocalSong2 }); 
        }

        public static Library CreateLibrary()
        {
            return new Library(new Mock<IPlayerStateManager>().Object);
        }

        public static Library CreateLibraryWithPlaylist()
        {
            var library = CreateLibrary();
            library.CreateNewPlaylist();

            return library;
        }

        public static Mock<Song> CreateSongMock(string name = "Song", bool callBase = false, TimeSpan duration = new TimeSpan())
        {
            return new Mock<Song>(name, duration) { CallBase = callBase };
        }

        public static Mock<Song>[] CreateSongMocks(int count, bool callBase)
        {
            var songs = new Mock<Song>[count];

            for (int i = 0; i < count; i++)
            {
                songs[i] = CreateSongMock("Song" + i, callBase);
            }

            return songs;
        }

        public static string GenerateSaveFile()
        { 
            return "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                    "<Root>" +
                    "  <Playlist>" +
                    "    <Volume Value=\"0.5\" />" + 
                    "    <CurrentTime Value=\"0\" />" +
                    "    <Entries>" +
                    "      <Entry FilePath=\"Path1\" Album=\"Album1\" Artist=\"Artist1\" Title=\"Title1\" Year=\"2005\" TrackNumber=\"1\" Duration=\"1\" />" +
                    "      <Entry FilePath=\"Path2\" Album=\"Album2\" Artist=\"Artist2\" Title=\"Title2\" Year=\"2006\" TrackNumber=\"2\" Duration=\"2\" />" +
                    "    </Entries>" +
                    "  </Playlist>" +
                    "</Root>";
        }

        public static Song SetupSongMock(string name = "Song", bool callBase = false, TimeSpan duration = new TimeSpan())
        {
            return CreateSongMock(name, callBase, duration).Object;
        }

        public static Song[] SetupSongMocks(int count, bool callBase = false)
        {
            var songs = new Song[count];

            for (int i = 0; i < count; i++)
            {
                songs[i] = SetupSongMock("Song" + i, callBase);
            }

            return songs;
        }

        public static string StreamToString(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                stream.Position = 0;

                return reader.ReadToEnd();
            }
        }

        public static Stream ToStream(this string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        internal static Playlist SetupPlaylist(Song song)
        {
            return SetupPlaylist(new[] { song });
        }

        internal static Playlist SetupPlaylist(IEnumerable<Song> songs)
        {
            var playlist = new Playlist();

            playlist.AddSongs(songs);

            return playlist;
        }
    }
}