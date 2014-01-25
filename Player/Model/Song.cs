using System;
using System.Diagnostics;
using Player.Audio;

namespace Player.Model
{
    public sealed class LocalSong : Song
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalSong"/> class.
        /// </summary>
        /// <param name="path">The path of the file.</param> 
        /// <param name="duration">The duration of the song.</param> 
        public LocalSong(string path, TimeSpan duration)
            : base(path, duration)
        { 
        }
          
        internal override AudioPlayer CreateAudioPlayer()
        {
            return new LocalAudioPlayer(this);
        }
         
    }

    /// <summary>
    /// Represents a song
    /// </summary>
    [DebuggerDisplay("{Artist}-{Album}-{Title}")]
    public abstract class Song : IEquatable<Song>
    { 
        private bool isCorrupted;

        /// <summary>
        /// Initializes a new instance of the <see cref="Song"/> class.
        /// </summary>
        /// <param name="path">The path of the song.</param> 
        /// <param name="duration">The duration of the song.</param>
        /// <exception cref="ArgumentNullException"><c>path</c> is null.</exception>
        protected Song(string path, TimeSpan duration)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.FilePath = path; 
            this.Duration = duration;

            this.Album = String.Empty;
            this.Artist = String.Empty; 
            this.Title = String.Empty;
        }
          
        public event EventHandler Corrupted;

        public string Album { get; set; }

        public string Artist { get; set; } 
         
        public TimeSpan Duration { get; private set; }
          
        /// <summary>
        /// Gets a value indicating whether the song is corrupted and can't be played.
        /// </summary>
        /// <value><c>true</c> if the song is corrupted; otherwise, <c>false</c>.</value>
        public bool IsCorrupted
        {
            get { return this.isCorrupted; }
            internal set
            {
                this.isCorrupted = value;

                if (this.isCorrupted)
                {
                    if (this.Corrupted != null)
                    this.Corrupted(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets the path of the song on the local filesystem, or in the internet.
        /// </summary>
        public string FilePath { get; private set; }
         
        public string Title { get; set; }

        public int TrackNumber { get; set; } 
         
        public int Year { get; set; }
          
        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// true if the specified <see cref="System.Object"/> is equal to this instance; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as Song);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Song other)
        {
            return other != null && this.FilePath == other.FilePath;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return new { OriginalPath = this.FilePath, this.Duration }.GetHashCode();
        }
         
        /// <summary>
        /// Creates the audio player for the song.
        /// </summary>
        /// <returns>The audio player for playback.</returns>
        internal abstract AudioPlayer CreateAudioPlayer(); 
    } 
}