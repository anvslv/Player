using System;
using System.IO;
using System.Runtime.InteropServices;
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
               
                //var e = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Root>" +
                //        "  <Playlist>" +
                //        "    <Volume Value=\"0.5\" />" + 
                //        "    <CurrentTime Value=\"0\" />" +
                //        "    <Entries>" +
                //        "      <Entry FilePath=\"Path1\" Album=\"Album1\" Artist=\"Artist1\" Title=\"Title1\" Year=\"2005\" TrackNumber=\"1\" Duration=\"1\" />" +
                //        "      <Entry FilePath=\"Path2\" Album=\"Album2\" Artist=\"Artist2\" Title=\"Title2\" Year=\"2006\" TrackNumber=\"2\" Duration=\"2\" />" +
                //        "    </Entries>" +
                //        "  </Playlist>" +
                //        "</Root>"
                //var a = "<?xml version=\"1.0\" encoding=\"utf-8\"?><Root>" +
                //        "  <Playlist>" +
                //        "    <Volume Value=\"0.5\" />" +
                //        "    <CurrentTime Value=\"0\" />" +
                //        "    <Entries>" +
                //        "      <Entry FilePath=\"Path1\" Album=\"Album1\" Artist=\"Artist1\" Title=\"Title1\" Year=\"2005\" TrackNumber=\"1\" Duration=\"1\" />" +
                //        "      <Entry FilePath=\"Path2\" Album=\"Album2\" Artist=\"Artist2\" Title=\"Title2\" Year=\"2006\" TrackNumber=\"2\" Duration=\"2\" />" +
                //        "    </Entries>" +
                //        "  </Playlist>" +
                //        "</Root>";
                
                Assert.Equal(expected, actual);
            }
        }
    }
}