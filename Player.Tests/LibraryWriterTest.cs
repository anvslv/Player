using System;
using System.IO;
using Player.Settings;
using Xunit;

namespace Player.Tests
{
    public class LibraryWriterTest
    {
        [Fact]
        public void Write()
        {
            using (Stream targetStream = new MemoryStream())
            { 
                PlayerStateManager.Save(Helpers.Playlist1, targetStream);

                string expected = Helpers.GenerateSaveFile();
                string actual = Helpers.StreamToString(targetStream).Replace("\r\n", String.Empty);

                Assert.Equal(expected, actual);
            }
        }
    }
}