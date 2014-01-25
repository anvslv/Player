using System.IO;
using System.Linq;
using Player.Core;
using Player.Model;
using Player.Settings;
using Xunit;

namespace Player.Tests
{
    public class LibraryReaderTest
    {
        [Fact]
        public void ReadPlaylist()
        {
            using (Stream saveFileStream = Helpers.GenerateSaveFile().ToStream())
            {
                Playlist playlist1 = PlayerStateManager.GetPlaylist(saveFileStream);

                Song[] songs1 = playlist1.Select(entry => entry.Song).ToArray();
                Song localSong1 = Helpers.LocalSong1;
                Song localSong2 = Helpers.LocalSong2;
                 
                Assert.Equal(localSong1.FilePath, songs1[0].FilePath);
                Assert.IsType(localSong1.GetType(), songs1[0]);

                Assert.Equal(localSong2.FilePath, songs1[1].FilePath);
                Assert.IsType(localSong2.GetType(), songs1[1]); 
            }
        }  
    }
}