using System;
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
}