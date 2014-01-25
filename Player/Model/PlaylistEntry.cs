using System;
using Player.Core;

namespace Player.Model
{
    public class PlaylistEntry
    {
        public Song Song { get; private set; }
        public int Index { get; set; }

        public PlaylistEntry(int index, Song song)
        {
            if(index < 0)
                throw new ArgumentOutOfRangeException("index");
            if(song == null)
                throw new ArgumentNullException("song");

            this.Song = song;
            this.Index = index;
        } 
    }
}