using System;
using Player.Audio;
using Player.Core;
using Xunit;

namespace Player.Tests
{
    public class LocalAudioPlayerTest
    {
        [Fact]
        public void Constructor_SongIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new LocalAudioPlayer(null));
        }

        [Fact]
        public void CurrentTime_NoSongLoaded_ReturnsTimeSpandZero()
        {
            var audioPlayer = new LocalAudioPlayer(Helpers.SetupSongMock());

            Assert.Equal(TimeSpan.Zero, audioPlayer.CurrentTime);
        }

        [Fact]
        public void PlaybackState_NoSongLoaded_ReturnsNone()
        {
            var audioPlayer = new LocalAudioPlayer(Helpers.SetupSongMock());

            Assert.Equal(AudioPlayerState.None, audioPlayer.PlaybackState);
        }

        [Fact]
        public void Stop_SongNotLoaded_PassesWithoutException()
        {
            var audioPlayer = new LocalAudioPlayer(Helpers.SetupSongMock());

            audioPlayer.Stop();
        }

        [Fact]
        public void TotalTime_NoSongLoaded_ReturnsTimeSpanZero()
        {
            var audioPlayer = new LocalAudioPlayer(Helpers.SetupSongMock());

            Assert.Equal(TimeSpan.Zero, audioPlayer.TotalTime);
        }

        [Fact]
        public void Volume_NoSongLoadedAndVolumeIsSet_ReturnsSettedVolume()
        {
            var audioPlayer = new LocalAudioPlayer(Helpers.SetupSongMock()) { Volume = 0.5f };

            Assert.Equal(0.5f, audioPlayer.Volume);
        }
    }
}