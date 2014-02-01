using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Player.Audio;
using Player.Core;
using Player.IO;
using Player.Settings;

namespace Player.Model
{ 
    public sealed class Library : IDisposable
    { 
        // We need a lock when disposing songs to prevent a modification of the enumeration
        private readonly object disposeLock;

        private readonly IPlayerStateManager playerStateManager;
        private readonly object songLock;
        private bool abortSongAdding;
        private AudioPlayer currentPlayer;
        
        public Library(IPlayerStateManager playerStateManager)
        {
            this.songLock = new object(); 
            this.playerStateManager = playerStateManager;
            this.disposeLock = new object();
        }

        /// <summary>
        /// Occurs when the playlist has changed.
        /// </summary>
        public event EventHandler PlaylistChanged;

        /// <summary>
        /// Occurs when a song has been added to the library.
        /// </summary>
        public event EventHandler<LibraryFillEventArgs> SongAdded;

        /// <summary>
        /// Occurs when a corrupted song has been attempted to be played.
        /// </summary>
        public event EventHandler SongCorrupted;
        
        public event EventHandler FirstSongSelected;

        /// <summary>
        /// Occurs when a song has finished the playback.
        /// </summary>
        public event EventHandler SongFinished;

        /// <summary>
        /// Occurs when a song has started the playback.
        /// </summary>
        public event EventHandler SongStarted;

        /// <summary>
        /// Gets a value indicating whether the next song in the playlist can be played.
        /// </summary>
        /// <value>
        /// true if the next song in the playlist can be played; otherwise, false.
        /// </value>
        public bool CanPlayNextSong
        {
            get { return this.CurrentPlaylist.CanPlayNextSong; }
        }

        /// <summary>
        /// Gets a value indicating whether the previous song in the playlist can be played.
        /// </summary>
        /// <value>
        /// true if the previous song in the playlist can be played; otherwise, false.
        /// </value>
        public bool CanPlayPreviousSong
        {
            get { return this.CurrentPlaylist.CanPlayPreviousSong; }
        }
         
        public Playlist CurrentPlaylist
        {
            get; private set;
        }

        /// <summary>
        /// Gets or sets the current song's elapsed time.
        /// </summary>
        public TimeSpan CurrentTime
        {
            get
            {
                if (this.currentPlayer != null)
                {
                    return this.currentPlayer.CurrentTime;
                }
                return this.CurrentPlaylist.CurrentTime;
            }
            set
            {
                if (this.currentPlayer != null)
                {
                    this.currentPlayer.CurrentTime = value;
                }
            }
        }
         
        /// <summary>
        /// Gets the duration of the current song.
        /// </summary>
        public TimeSpan TotalTime
        {
            get { return this.currentPlayer == null ? TimeSpan.Zero : this.currentPlayer.TotalTime; }
        }
          
        /// <summary>
        /// Gets a value indicating whether the playback is paused.
        /// </summary>
        /// <value>
        /// true if the playback is paused; otherwise, false.
        /// </value>
        public bool IsPaused
        {
            get { return this.currentPlayer != null && this.currentPlayer.PlaybackState == AudioPlayerState.Paused; }
        }

        /// <summary>
        /// Gets a value indicating whether the playback is started.
        /// </summary>
        /// <value>
        /// true if the playback is started; otherwise, false.
        /// </value>
        public bool IsPlaying
        {
            get { return this.currentPlayer != null && this.currentPlayer.PlaybackState == AudioPlayerState.Playing; }
        }

        /// <summary>
        /// Gets the song that is currently loaded.
        /// </summary>
        public Song LoadedSong
        {
            get
            {
                return this.currentPlayer == null ? null : this.currentPlayer.Song;
            }
        }

        /// <summary>
        /// Gets or sets the current volume.
        /// </summary>
        /// <value>
        /// The current volume.
        /// </value>
        public float Volume
        {
            get { return this.CurrentPlaylist.Volume; }
            set
            {
                this.CurrentPlaylist.Volume = value;

                if (this.currentPlayer != null)
                {
                    this.currentPlayer.Volume = value;
                }
            }
        }
         
        /// <summary>
        /// Adds the song that are contained in the specified directory recursively in an asynchronous manner to the library.
        /// </summary>
        /// <param name="paths">The paths of the directory to search.</param>
        /// <returns>The <see cref="Task"/> that did the work.</returns>
        public Task AddLocalSongsAsync(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            return Task.Factory.StartNew(() => this.AddLocalSongs(paths));
        }
         
        /// <summary>
        /// Adds the specified song to the end of the playlist. 
        /// </summary>
        /// <param name="songList">The songs to add to the end of the playlist.</param>
        public void AddSongsToPlaylist(IEnumerable<Song> songList)
        {
            if (songList == null)
                throw new ArgumentNullException("songList");

            this.CurrentPlaylist.AddSongs(songList.ToList());
                // Copy the sequence to a list, so that the enumeration doesn't gets modified

            if (this.PlaylistChanged != null)
                this.PlaylistChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Adds the song to the end of the playlist.
        /// This method throws an exception, if there is an outstanding timeout.
        /// </summary>
        /// <param name="song">The song to add to the end of the playlist.</param>
        public void AddSongToPlaylist(Song song)
        {
            if (song == null)
                throw new ArgumentNullException("song"); 

            this.CurrentPlaylist.AddSongs(new[] { song });
             
            if (this.PlaylistChanged != null)
                this.PlaylistChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Continues the currently loaded song.
        /// </summary>
        public void ContinueSong()
        {
            this.currentPlayer.Play();
        }
         
        private void Save()
        {
            if(currentPlayer != null)
                this.CurrentPlaylist.CurrentTime = currentPlayer.CurrentTime;
            this.playerStateManager.Save(this.CurrentPlaylist);
        }

        private void Load()
        {
            this.CurrentPlaylist = this.playerStateManager.GetPlaylist();
            
            if (this.PlaylistChanged != null)
                this.PlaylistChanged(this, EventArgs.Empty);

            var currentSongIndex = this.CurrentPlaylist.CurrentSongIndex;
            if (currentSongIndex != null && this.CurrentPlaylist.Any())
            {
                InternSelectSong(currentSongIndex.Value, false, this.CurrentPlaylist.CurrentTime);
            }
        }

        public void Dispose()
        {
            Save();

            if (this.currentPlayer != null)
            {
                this.currentPlayer.Dispose();
            }
            this.abortSongAdding = true; 
        }
         
        public void Initialize()
        { 
            this.Load();
        }

        /// <summary>
        /// Pauses the currently loaded song.
        /// </summary>
        public void PauseSong()
        {
            this.currentPlayer.Pause();
        }

        /// <summary>
        /// Plays the next song in the playlist.
        /// </summary>
        public void PlayNextSong()
        {
            this.InternPlayNextSong();
        }

        /// <summary>
        /// Plays the previous song in the playlist.
        /// </summary>
        public void PlayPreviousSong()
        {
            if (!this.CurrentPlaylist.CanPlayPreviousSong || !this.CurrentPlaylist.CurrentSongIndex.HasValue)
                throw new InvalidOperationException("The previous song couldn't be played.");

            int prevIndex = this.CurrentPlaylist.GetPreviousIndex();
            this.InternSelectSong(prevIndex);
        }

        /// <summary>
        /// Plays the song with the specified index in the playlist.
        /// </summary>
        /// <param name="playlistIndex">The index of the song in the playlist.</param>
        public void PlaySong(int playlistIndex)
        {
            if (playlistIndex < 0)
                throw new ArgumentOutOfRangeException("playlistIndex");
             
            this.InternSelectSong(playlistIndex);
        }
         
        /// <summary>
        /// Removes the songs with the specified indexes from the playlist.
        /// </summary>
        /// <param name="indexes">The indexes of the songs to remove from the playlist.</param>
        public void RemoveFromPlaylist(IEnumerable<int> indexes)
        {
            if (indexes == null)
                throw new ArgumentNullException("indexes");

            this.RemoveFromPlaylist(this.CurrentPlaylist, indexes);
        }

        /// <summary>
        /// Removes the specified songs from the playlist.
        /// </summary>
        /// <param name="songList">The songs to remove.</param>
        public void RemoveFromPlaylist(IEnumerable<Song> songList)
        {
            if (songList == null)
                throw new ArgumentNullException("songList");

            this.RemoveFromPlaylist(this.CurrentPlaylist, songList);
        }
         
        public void ShufflePlaylist()
        {
            this.CurrentPlaylist.Shuffle();
        }
       
        public void CreateNewPlaylist()
        {
            CurrentPlaylist = new Playlist();
        }
          
        /// <summary>
        /// Adds the song that are contained in the specified directory recursively to the library.
        /// </summary>
        /// <param name="paths">The paths of the directory to search.</param>
        private void AddLocalSongs(params string[] paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            foreach (var path in paths)
            { 
                var finder = new SongFinder(path);

                finder.SongFound += (sender, e) =>
                {
                    if (this.abortSongAdding)
                    {
                        finder.Abort();
                        return;
                    }

                    lock (this.songLock)
                    {
                        lock (this.disposeLock)
                        {
                            this.CurrentPlaylist.AddSong(e.Song);
                            if (CurrentPlaylist.CurrentSongIndex == 0 && currentPlayer == null)
                            {
                                InternSelectSong(0, true);
                            }
                        }
                    }
                    
                    if (this.SongAdded != null)
                        this.SongAdded(this,
                            new LibraryFillEventArgs(e.Song, finder.TagsProcessed, finder.CurrentTotalSongs)); 
                };

                finder.Start(); 
            }
        }
         
        private void HandleSongCorruption()
        {
            if (!this.CurrentPlaylist.CanPlayNextSong)
            {
                this.CurrentPlaylist.CurrentSongIndex = null;
            }

            else
            {
                this.InternPlayNextSong();
            }
        }

        private void HandleSongFinish()
        {
            if (!this.CurrentPlaylist.CanPlayNextSong)
            {
                this.CurrentPlaylist.CurrentSongIndex = null;
            }

            this.currentPlayer.Dispose();
            this.currentPlayer = null;

            if (this.SongFinished != null)
                this.SongFinished(this, EventArgs.Empty);

            if (this.CurrentPlaylist.CanPlayNextSong)
            {
                this.InternPlayNextSong();
            }
        }

        private void InternPlayNextSong()
        {
            if (!this.CurrentPlaylist.CanPlayNextSong || !this.CurrentPlaylist.CurrentSongIndex.HasValue)
                throw new InvalidOperationException("The next song couldn't be played.");

            int nextIndex = this.CurrentPlaylist.GetNextIndex();

            this.InternSelectSong(nextIndex);
        }

        private void InternSelectSong(int playlistIndex, bool playThisSong = true, TimeSpan position = new TimeSpan())
        {
            if (playlistIndex < 0)
                throw new ArgumentOutOfRangeException("playlistIndex"); 
            
            this.CurrentPlaylist.CurrentSongIndex = playlistIndex;

            Song song = this.CurrentPlaylist[playlistIndex].Song;

            this.RenewCurrentPlayer(song);
               
            Task.Factory.StartNew(() =>
            {
                try
                {
                    this.currentPlayer.Load();
                    this.currentPlayer.CurrentTime = position;
                    if (this.FirstSongSelected != null)
                        this.FirstSongSelected(this, EventArgs.Empty);
                }

                catch (SongLoadException)
                {
                    song.IsCorrupted = true;
                    if (this.SongCorrupted != null)
                        this.SongCorrupted(this, EventArgs.Empty);

                    this.HandleSongCorruption();

                    return;
                }
                if (playThisSong)
                {
                    try
                    {
                        this.currentPlayer.Play();
                    }

                    catch (PlaybackException)
                    {
                        song.IsCorrupted = true;
                        if (this.SongCorrupted != null)
                            this.SongCorrupted(this, EventArgs.Empty);

                        this.HandleSongCorruption();

                        return;
                    }

                    if (this.SongStarted != null)
                        this.SongStarted(this, EventArgs.Empty);
                }
            }); 
        }
           
        private void RemoveFromPlaylist(Playlist playlist, IEnumerable<int> indexes)
        {
            //bool stopCurrentSong = playlist == this.CurrentPlaylist &&
            //                       indexes.Any(index => index == this.CurrentPlaylist.CurrentSongIndex);

            playlist.RemoveSongs(indexes);

            if (this.PlaylistChanged != null)
                this.PlaylistChanged(this, EventArgs.Empty);

            //if (stopCurrentSong)
            //{
            //    this.currentPlayer.Stop();
            //    if (this.SongFinished != null)
            //        this.SongFinished(this, EventArgs.Empty);
            //}
        }

        private void RemoveFromPlaylist(Playlist playlist, IEnumerable<Song> songList)
        {
            this.RemoveFromPlaylist(playlist, playlist.GetIndexes(songList));
        }

        private void RenewCurrentPlayer(Song song)
        {
            if (this.currentPlayer != null)
            {
                this.currentPlayer.Dispose();
            }

            this.currentPlayer = song.CreateAudioPlayer();

            this.currentPlayer.SongFinished += (sender, e) => this.HandleSongFinish();
            this.currentPlayer.Volume = this.Volume;
        }

    }

    public sealed class LibraryFillEventArgs : EventArgs
    {
        public LibraryFillEventArgs(Song song, int processedTagCount, int totalTagCount)
        {
            if (song == null)
                throw new ArgumentNullException("song");

            if (processedTagCount < 0)
                throw new ArgumentOutOfRangeException("processedTagCount");

            if (totalTagCount < 0)
                throw new ArgumentOutOfRangeException("totalTagCount");

            this.Song = song;
            this.TotalTagCount = totalTagCount;
            this.ProcessedTagCount = processedTagCount;
        }

        public int ProcessedTagCount { get; private set; }

        public Song Song { get; private set; }

        public int TotalTagCount { get; private set; }
    }
}
