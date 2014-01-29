using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Player.Audio;
using Player.Core;
using Player.Model;
using Player.Tests.Mocks;
using Moq;
using Xunit;

namespace Player.Tests
{
    public sealed class LibraryTest
    {
        [Fact]
        public void AddAndSwitchToPlaylist_SomeGenericName_WorksAsExpected()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                library.CreateNewPlaylist(); 
                Assert.NotNull(library.CurrentPlaylist);
            }
        }
          
        [Fact]
        public void AddLocalSongsAsync_PathIsNull_ThrowsArgumentNullException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentNullException>(() => library.AddLocalSongsAsync(null));
            }
        }
          
        [Fact]
        public void AddSongsToPlaylist_SongListIsNull_ThrowsArgumentNullException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentNullException>(() => library.AddSongsToPlaylist(null));
            }
        }

        [Fact]
        public void AddSongToPlaylist_SongIsNull_ThrowsArgumentNullException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentNullException>(() => library.AddSongToPlaylist(null));
            }
        }
           
        [Fact]
        public void ContinueSongCallsAudioPlayerPlay()
        {
            var handle = new ManualResetEvent(false);

            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                Mock<Song> song = Helpers.CreateSongMock();
                var audioPlayer = new Mock<AudioPlayer>();
                audioPlayer.Setup(p => p.Play()).Callback(() => handle.Set());

                song.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayer.Object);

                library.AddSongToPlaylist(song.Object);

                library.PlaySong(0);

                // The library starts a new thread when playing a song, we want to wait till it called the audio player
                // to avoid threading issues and a wrong test result
                handle.WaitOne();

                library.ContinueSong();

                audioPlayer.Verify(p => p.Play(), Times.Exactly(2));
            }
        }
         
        [Fact]
        public void GetPlaylist_PlaylistIsNotInitialized_ReturnsNull()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Null(library.CurrentPlaylist);
            }
        } 
  
        [Fact]
        public void PauseSongCallsAudioPlayerPause()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                Mock<Song> song = Helpers.CreateSongMock();
                var audioPlayer = new Mock<AudioPlayer>();

                song.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayer.Object);

                library.AddSongToPlaylist(song.Object);

                library.PlaySong(0);

                library.PauseSong();

                audioPlayer.Verify(p => p.Pause(), Times.Once());
            }
        }

        [Fact] 
        public void Play_SongIsCorrupted_PlaysNextSong()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                var audioPlayer = new Mock<AudioPlayer>();
                audioPlayer.Setup(p => p.Play()).Throws<PlaybackException>();

                Mock<Song> corruptedSong = Helpers.CreateSongMock();
                corruptedSong.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayer.Object);

                Mock<Song> nextSong = Helpers.CreateSongMock();
                nextSong.Setup(p => p.CreateAudioPlayer()).Returns(new JumpAudioPlayer());

                library.AddSongsToPlaylist(new[] { corruptedSong.Object, nextSong.Object });

                var handle = new AutoResetEvent(false);

                library.SongCorrupted += (sender, args) => handle.Set();
                library.SongStarted += (sender, args) => handle.Set();

                library.PlaySong(0);

                handle.WaitOne();
                handle.WaitOne();

                // The test will fail, if the last wait timeouts
            }
        }

        [Fact]
        public void Play_ThrowsPlaybackException_SetsSongIsCorruptedToTrue()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                var audioPlayer = new Mock<AudioPlayer>();
                audioPlayer.Setup(p => p.Play()).Throws<PlaybackException>();

                Mock<Song> song = Helpers.CreateSongMock();
                song.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayer.Object);

                library.AddSongToPlaylist(song.Object);

                var handle = new ManualResetEvent(false);

                song.Object.Corrupted += (sender, args) => handle.Set();

                library.PlaySong(0);

                handle.WaitOne();

                Assert.True(song.Object.IsCorrupted);
            }
        }

        [Fact]
        public void Play_ThrowsSongLoadException_SetsSongIsCorruptedToTrue()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                var audioPlayer = new Mock<AudioPlayer>();
                audioPlayer.Setup(p => p.Load()).Throws<SongLoadException>();

                Mock<Song> song = Helpers.CreateSongMock();
                song.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayer.Object);

                library.AddSongToPlaylist(song.Object);

                var handle = new ManualResetEvent(false);

                song.Object.Corrupted += (sender, args) => handle.Set();

                library.PlaySong(0);

                handle.WaitOne();

                Assert.True(song.Object.IsCorrupted);
            }
        }
         
        [Fact]
        public void PlayPreviousSong_PlaylistIsEmpty_ThrowsInvalidOperationException()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                Assert.Throws<InvalidOperationException>(() => library.PlayPreviousSong());
            }
        }

        [Fact]
        public void PlaysNextSongAutomatically()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                var song1 = new Mock<Song>("TestPath", TimeSpan.Zero);
                song1.Setup(p => p.CreateAudioPlayer()).Returns(() => new JumpAudioPlayer());

                var song2 = new Mock<Song>("TestPath2", TimeSpan.Zero);
                song2.Setup(p => p.CreateAudioPlayer()).Returns(() => new JumpAudioPlayer());

                library.AddSongsToPlaylist(new[] { song1.Object, song2.Object });

                var handle = new ManualResetEvent(false);
                int played = 0;

                library.SongStarted += (sender, e) =>
                {
                    played++;

                    if (played == 2)
                    {
                        handle.Set();
                    }
                };

                library.PlaySong(0);

                if (!handle.WaitOne(5000))
                {
                    Assert.True(false, "Timout");
                }
            }
        }

        [Fact]
        public void PlaySong_IndexIsLessThanZero_ThrowsArgumentOutOfRangeException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => library.PlaySong(-1));
            }
        }
             
        [Fact]
        public void RemoveFromPlaylist_IndexesIsNull_ThrowsArgumentNullException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentNullException>(() => library.RemoveFromPlaylist((IEnumerable<int>)null));
            }
        }

        [Fact]
        public void RemoveFromPlaylist_RemoveByIndexes_SongsAreRemovedFromPlaylist()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                Song[] songs = Helpers.SetupSongMocks(4);

                library.AddSongsToPlaylist(songs);

                library.RemoveFromPlaylist(new[] { 0, 2 });

                Song[] remaining = library.CurrentPlaylist.Select(entry => entry.Song).ToArray();

                Assert.Equal(songs[1], remaining[0]);
                Assert.Equal(songs[3], remaining[1]);
            }
        }

        [Fact]
        public void RemoveFromPlaylist_RemoveBySongReference_SongsAreRemovedFromPlaylist()
        {
            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                Song[] songs = Helpers.SetupSongMocks(4, true);

                library.AddSongsToPlaylist(songs);

                library.RemoveFromPlaylist(new[] { songs[0], songs[2] });

                Song[] remaining = library.CurrentPlaylist.Select(entry => entry.Song).ToArray();

                Assert.Equal(songs[1], remaining[0]);
                Assert.Equal(songs[3], remaining[1]);
            }
        }

        [Fact]
        public void RemoveFromPlaylist_SongIsPlaying_CurrentPlayerIsNotStopped()
        {
            var audioPlayerMock = new Mock<AudioPlayer>();

            var songMock = new Mock<Song>("TestPath", TimeSpan.Zero);
            songMock.Setup(p => p.CreateAudioPlayer()).Returns(audioPlayerMock.Object);

            using (Library library = Helpers.CreateLibraryWithPlaylist())
            {
                library.AddSongsToPlaylist(new[] { songMock.Object });

                library.PlaySong(0);

                library.RemoveFromPlaylist(new[] { 0 });

                audioPlayerMock.Verify(p => p.Stop(), Times.Never);
            }
        }

        [Fact]
        public void RemoveFromPlaylist_SongListIsNull_ThrowsArgumentNullException()
        {
            using (Library library = Helpers.CreateLibrary())
            {
                Assert.Throws<ArgumentNullException>(() => library.RemoveFromPlaylist((IEnumerable<Song>)null));
            }
        } 
    }
}