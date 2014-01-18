using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Linq; 

namespace Player.Core
{
    public class Playlist : IEnumerable<PlaylistEntry>
    {
        private int? currentSongIndex;
        private List<PlaylistEntry> playlist;
         
        internal Playlist()
        { 
            this.playlist = new List<PlaylistEntry>();
        }

        /// <summary>
        /// Gets a value indicating whether the next song in the playlist can be played.
        /// </summary>
        /// <value>
        /// true if the next song in the playlist can be played; otherwise, false.
        /// </value>
        public bool CanPlayNextSong
        {
            get { return this.CurrentSongIndex.HasValue && this.ContainsIndex(this.CurrentSongIndex.Value + 1); }
        }

        /// <summary>
        /// Gets a value indicating whether the previous song in the playlist can be played.
        /// </summary>
        /// <value>
        /// true if the previous song in the playlist can be played; otherwise, false.
        /// </value>
        public bool CanPlayPreviousSong
        {
            get { return this.CurrentSongIndex.HasValue && this.ContainsIndex(this.CurrentSongIndex.Value - 1); }
        }

        /// <summary>
        /// Gets the index of the currently played song in the playlist.
        /// </summary>
        /// <value>
        /// The index of the currently played song in the playlist. <c>null</c>, if no song is currently played.
        /// </value>
        /// <exception cref="ArgumentOutOfRangeException">The value is not in the range of the playlist's indexes.</exception>
        public int? CurrentSongIndex
        {
            get { return this.currentSongIndex; }
            internal set
            {
                if (value != null && !this.ContainsIndex(value.Value))
                    throw new ArgumentOutOfRangeException("value");

                this.currentSongIndex = value;
            }
        }

        public float Volume { get; set; } 

        public PlaylistEntry this[int index]
        {
            get
            {
                if (index < 0)
                    throw new ArgumentOutOfRangeException("index");

                int maxIndex = this.playlist.Count;

                if (index > maxIndex)
                    throw new ArgumentOutOfRangeException("index");

                return this.playlist[index];
            }
        }

        /// <summary>
        /// Gets a value indicating whether there exists a song at the specified index.
        /// </summary>
        /// <param name="songIndex">The index to look for.</param>
        /// <returns>True, if there exists a song at the specified index; otherwise, false.</returns>
        public bool ContainsIndex(int songIndex)
        {
            return this.playlist.Any(entry => entry.Index == songIndex);
        }

        public IEnumerator<PlaylistEntry> GetEnumerator()
        {
            return this.playlist.GetEnumerator();
        }

        /// <summary>
        /// Gets all indexes of the specified songs.
        /// </summary>
        public IEnumerable<int> GetIndexes(IEnumerable<Song> songs)
        {
            return this.playlist
                .Where(entry => songs.Contains(entry.Song))
                .Select(entry => entry.Index)
                .ToList();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified songs to end of the playlist.
        /// </summary>
        /// <param name="songList">The songs to add to the end of the playlist.</param>
        internal void AddSongs(IEnumerable<Song> songList)
        {
            if (songList == null)
                throw new ArgumentNullException("songList");

            foreach (Song song in songList)
            {
                this.playlist.Add(new PlaylistEntry(this.playlist.Count, song));
            }
        }

        internal void AddSong(Song song)
        {
            if (song == null)
                throw new ArgumentNullException("song");
             
            this.playlist.Add(new PlaylistEntry(this.playlist.Count, song));  
        }
         
        /// <summary>
        /// Removes the songs with the specified indexes from the <see cref="Playlist"/>.
        /// </summary>
        /// <param name="indexes">The indexes of the songs to remove.</param>
        internal void RemoveSongs(IEnumerable<int> indexes)
        {
            if (indexes == null)
                throw new ArgumentNullException("indexes");

            // Use a hashset for better lookup performance
            var indexList = new HashSet<int>(indexes);

            if (this.CurrentSongIndex.HasValue && indexList.Contains(this.CurrentSongIndex.Value))
            {
                this.CurrentSongIndex = null;
            }

            this.playlist.RemoveAll(entry => indexList.Contains(entry.Index));

            this.RebuildIndexes();
        }

        internal void Shuffle()
        {
            int count = this.playlist.Count;

            var random = new Random();

            for (int index = 0; index < count; index++)
            {
                int newIndex = random.Next(count);

                // Migrate the CurrentSongIndex to the new position
                if (index == this.CurrentSongIndex)
                {
                    this.CurrentSongIndex = newIndex;
                }

                else if (newIndex == this.CurrentSongIndex)
                {
                    this.CurrentSongIndex = index;
                }

                PlaylistEntry temp = this.playlist[index];

                this.playlist[newIndex].Index = index;
                this.playlist[index] = this.playlist[newIndex];

                temp.Index = newIndex;
                this.playlist[newIndex] = temp;
            }
        }

        private void RebuildIndexes()
        {
            int index = 0;
            int? migrateIndex = null;
            var current = this.playlist.ToList();

            this.playlist.Clear();

            foreach (var entry in current)
            {
                if (this.CurrentSongIndex == entry.Index)
                {
                    migrateIndex = index;
                }

                this.playlist.Add(entry);
                entry.Index = index;

                index++;
            }

            if (migrateIndex.HasValue)
            {
                this.CurrentSongIndex = migrateIndex;
            }
        }
    }
}