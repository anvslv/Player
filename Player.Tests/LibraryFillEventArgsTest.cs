using System;
using Player.Core;
using Moq;
using Xunit;

namespace Player.Tests
{
    public class LibraryFillEventArgsTest
    {
        [Fact]
        public void Constructor()
        {
            Song song = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            var args = new LibraryFillEventArgs(song, 5, 10);

            Assert.Equal(song, args.Song);
            Assert.Equal(5, args.ProcessedTagCount);
            Assert.Equal(10, args.TotalTagCount);
        }

        [Fact]
        public void Constructor_SongIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LibraryFillEventArgs(null, 0, 0));
        }

        [Fact]
        public void Constructor_ProcessedTagCountIsLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            Song song = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            Assert.Throws<ArgumentOutOfRangeException>(() => new LibraryFillEventArgs(song, -1, 0));
        }

        [Fact]
        public void Constructor_TotalTagCountIsLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            Song song = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            Assert.Throws<ArgumentOutOfRangeException>(() => new LibraryFillEventArgs(song, 0, -1));
        }
    }
}