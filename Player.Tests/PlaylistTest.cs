using System;
using System.Collections.Generic;
using System.Linq;
using Player.Core;
using Player.Model;
using Xunit;

namespace Player.Tests
{
    public sealed class PlaylistTest
    {
        [Fact]
        public void AddSongs_AddTwoSong_IndexesAreCorrect()
        {
            Song[] songs = Helpers.SetupSongMocks(2);

            var playlist = new Playlist();

            playlist.AddSongs(songs);

            Assert.Equal(0, playlist[0].Index);
            Assert.Equal(1, playlist[1].Index);
        }

        [Fact]
        public void AddSongs_ArgumentIsNull_ThrowsArgumentNullException()
        {
            var playlist = new Playlist();

            Assert.Throws<ArgumentNullException>(() => playlist.AddSongs(null));
        }

        [Fact]
        public void AddSongs_PlaylistContainsSongs()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            Assert.Equal(4, playlist.Count());
            Assert.Equal(songs[0], playlist[0].Song);
            Assert.Equal(songs[1], playlist[1].Song);
            Assert.Equal(songs[2], playlist[2].Song);
            Assert.Equal(songs[3], playlist[3].Song);
        }

        [Fact]
        public void CanPlayNextSong_CurrentSongIndexIsLastSong_ReturnsFalse()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 3;

            Assert.False(playlist.CanPlayNextSong);
        }

        [Fact]
        public void CanPlayNextSong_CurrentSongIndexIsNull_ReturnsFalse()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = null;

            Assert.False(playlist.CanPlayNextSong);
        }

        [Fact]
        public void CanPlayNextSong_CurrentSongIndexIsZero_ReturnsTrue()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 0;

            Assert.True(playlist.CanPlayNextSong);
        }

        [Fact]
        public void CanPlayNextSong_PlaylistIsEmpty_ReturnsFalse()
        {
            var playlist = new Playlist();

            Assert.False(playlist.CanPlayNextSong);
        }

        [Fact]
        public void CanPlayPreviousSong_CurrentSongIndexIsLastSong_ReturnsTrue()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 3;

            Assert.True(playlist.CanPlayPreviousSong);
        }

        [Fact]
        public void CanPlayPreviousSong_CurrentSongIndexIsNull_ReturnsFalse()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = null;

            Assert.False(playlist.CanPlayPreviousSong);
        }

        [Fact]
        public void CanPlayPreviousSong_CurrentSongIndexIsZero_ReturnsFalse()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 0;

            Assert.False(playlist.CanPlayPreviousSong);
        }

        [Fact]
        public void CanPlayPreviousSong_PlaylistIsEmpty_ReturnsFalse()
        {
            var playlist = new Playlist();

            Assert.False(playlist.CanPlayPreviousSong);
        }

        [Fact]
        public void CurrentSongIndexSetter_PlaylistIsEmptyAndSetToNull_Passes()
        {
            new Playlist()
            {
                CurrentSongIndex = null
            };
        }

        [Fact]
        public void CurrentSongIndexSetter_PlaylistIsEmptyAndSetToZero_ThrowsArgumentOutOfRangeException()
        {
            var playlist = new Playlist();

            Assert.Throws<ArgumentOutOfRangeException>(() => playlist.CurrentSongIndex = 0);
        }

        [Fact]
        public void CurrentSongIndexSetter_ValueIsNotInPlaylistRange_ThrowsArgumentOutOfRangeException()
        {
            Song[] songs = Helpers.SetupSongMocks(3);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            Assert.Throws<ArgumentOutOfRangeException>(() => playlist.CurrentSongIndex = 3);
        }

        [Fact]
        public void GetIndexes_MultipleSongs_ReturnsCorrectIndexes()
        {
            Song[] songs = Helpers.SetupSongMocks(3, true);

            Playlist playlist = Helpers.SetupPlaylist(songs);

            IEnumerable<int> indexes = playlist.GetIndexes(songs);

            Assert.True(indexes.SequenceEqual(new[] { 0, 1, 2 }));
        }

        [Fact]
        public void GetIndexes_OneSong_ReturnsCorrectIndexes()
        {
            Song song = Helpers.SetupSongMock("Song", true);

            Playlist playlist = Helpers.SetupPlaylist(song);

            int index = playlist.GetIndexes(new[] { song }).Single();

            Assert.Equal(0, index);
        }

        [Fact]
        public void GetIndexes_OneSongWithMultipleReferences_ReturnsCorrectIndexes()
        {
            Song song = Helpers.SetupSongMock("Song", true);

            Playlist playlist = Helpers.SetupPlaylist(Enumerable.Repeat(song, 3));

            IEnumerable<int> indexes = playlist.GetIndexes(new[] { song });

            Assert.True(indexes.SequenceEqual(new[] { 0, 1, 2 }));
        }

        [Fact]
        public void GetIndexes_PassSongsThatAreNotInPlaylist_ReturnsNoIndexes()
        {
            Song[] songs = Helpers.SetupSongMocks(4, true);

            Playlist playlist = Helpers.SetupPlaylist(songs.Take(2));

            IEnumerable<int> indexes = playlist.GetIndexes(songs.Skip(2));

            Assert.Empty(indexes);
        }

        [Fact]
        public void Indexer_LessThanZero_ThrowsArgumentOutOfRangeException()
        {
            var playlist = new Playlist();

            PlaylistEntry temp;

            Assert.Throws<ArgumentOutOfRangeException>(() => temp = playlist[-1]);
        }

        [Fact]
        public void Indexer_MoreThanZero_ThrowsArgumentOutOfRangeException()
        {
            Song song = Helpers.SetupSongMock();
            var playlist = Helpers.SetupPlaylist(song);

            PlaylistEntry temp;

            Assert.Throws<ArgumentOutOfRangeException>(() => temp = playlist[1]);
        }
           
        [Fact]
        public void RemoveSongs_ArgumentIsNull_ThrowsArgumentNullException()
        {
            var playlist = new Playlist();

            Assert.Throws<ArgumentNullException>(() => playlist.RemoveSongs(null));
        }

        [Fact]
        public void RemoveSongs_RemoveMultipleSongs_OrderIsCorrect()
        {
            Song[] songs = Helpers.SetupSongMocks(7);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.RemoveSongs(new[] { 1, 3, 4 });

            Assert.Equal(4, playlist.Count());
            Assert.Equal(songs[0], playlist[0].Song);
            Assert.Equal(songs[2], playlist[1].Song);
            Assert.Equal(songs[5], playlist[2].Song);
            Assert.Equal(songs[6], playlist[3].Song);
        }

        [Fact]
        public void RemoveSongs_RemoveOneSong_OrderIsCorrect()
        {
            Song[] songs = Helpers.SetupSongMocks(4);
            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.RemoveSongs(new[] { 1 });

            Assert.Equal(3, playlist.Count());
            Assert.Equal(songs[0], playlist[0].Song);
            Assert.Equal(songs[2], playlist[1].Song);
            Assert.Equal(songs[3], playlist[2].Song);
        }

        [Fact]
        public void RemoveSongsCorrectsCurrentSongIndex()
        {
            Song[] songs = Helpers.SetupSongMocks(2);

            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 1;

            playlist.RemoveSongs(new[] { 0 });

            Assert.Equal(0, playlist.CurrentSongIndex);
        }

        [Fact]
        public void ShuffleMigratesCurrentSongIndex()
        {
            Song[] songs = Helpers.SetupSongMocks(100, true);

            Playlist playlist = Helpers.SetupPlaylist(songs);

            playlist.CurrentSongIndex = 0;

            playlist.Shuffle();

            int newIndex = playlist.GetIndexes(new[] { songs[0] }).First();

            Assert.Equal(newIndex, playlist.CurrentSongIndex);
        }
    }
}