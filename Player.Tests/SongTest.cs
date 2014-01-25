using System;
using Player.Core;
using Moq;
using Player.Model;
using Xunit;

namespace Player.Tests
{
    public sealed class SongTest
    {
        
        [Fact]
        public void EqualsNullIsFalse()
        {
            var song = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            Assert.False(song.Equals(null));
        }

        [Fact]
        public void EqualsSamePathIsTrue()
        {
            var song1 = new Mock<Song>("TestPath", TimeSpan.Zero).Object;
            var song2 = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            Assert.True(song1.Equals(song2));
        }

        [Fact]
        public void EqualsSameReferenceIsTrue()
        {
            var song = new Mock<Song>("TestPath", TimeSpan.Zero).Object;

            Assert.True(song.Equals(song));
        }

        [Fact]
        public void EqualsSongWithDifferentPathIsFalse()
        {
            var song1 = new Mock<Song>("TestPath", TimeSpan.Zero).Object;
            var song2 = new Mock<Song>("TestPath1", TimeSpan.Zero).Object;

            Assert.False(song1.Equals(song2));
        }

        [Fact]
        public void GetHashcode_EqualObjects_ReturnsEqualHashCodes()
        {
            var song1 = new Mock<Song>("TestPath", TimeSpan.Zero) { CallBase = true }.Object;
            var song2 = new Mock<Song>("TestPath", TimeSpan.Zero) { CallBase = true }.Object;

            Assert.Equal(song1.GetHashCode(), song2.GetHashCode());
        }
    }
}