using System;
using System.Globalization;
using System.Linq;
using MicroMvvm;
using Player.Core;
using Player.Model;
using Player.Services;

namespace Player.ViewModels
{
    public class PlaylistEntryViewModel : ObservableObject, IDisposable
    {
        private readonly PlaylistEntry entry;
        private readonly int pad = 2;
        private bool isPlaying;

        public PlaylistEntryViewModel(PlaylistEntry e, int count)
        {
            this.entry = e;
            this.pad = (count + "").Count();
            this.entry.Song.Corrupted += OnCorrupted;
        }

        public string AlbumAndYearString
        {
            get
            {
                var result = string.Empty;
                if (!string.IsNullOrEmpty(entry.Song.Album) && entry.Song.Year != 0)
                    result = string.Format("(“{0}”, {1})", entry.Song.Album, entry.Song.Year);
                if (!string.IsNullOrEmpty(entry.Song.Album))
                    result = string.Format("(“{0}”)", entry.Song.Album);
                if (entry.Song.Year != 0)
                    result = string.Format("({0})", entry.Song.Year);
                return result;
            }
        }

        public int Index
        {
            get { return this.entry.Index; }
        }
        
        public string DurationString
        {
            get { return entry.Song.Duration.ToMinutesAndSeconds(); }
        }

        public string GlobalTrackNumberString
        {
            
            get { 
                return (entry.Index + 1).ToString(CultureInfo.InvariantCulture).PadLeft(pad, '0'); 
            }
        }

        public string FullSongTitleString
        {
            get { 
                return string.Format("{0} {1} — {2}", 
                    entry.Song.TrackNumber.ToString(CultureInfo.InvariantCulture).PadLeft(pad, '0'), 
                    entry.Song.Artist,
                    entry.Song.Title); 
            }
        }

        public bool IsPlaying
        {
            get { return this.isPlaying; }
            set
            {
                if (this.IsPlaying != value)
                {
                    this.isPlaying = value;
                    this.RaisePropertyChanged(() => this.IsPlaying);
                }
            }
        }

        public bool IsCorrupted
        {
            get { return this.entry.Song.IsCorrupted; }
        }

        public void Dispose()
        { 
            this.entry.Song.Corrupted -= this.OnCorrupted;
        }

        private void OnCorrupted(object sender, EventArgs eventArgs)
        {
            this.RaisePropertyChanged(() => this.IsCorrupted);
        }
    }
}