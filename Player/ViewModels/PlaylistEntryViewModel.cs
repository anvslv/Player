using System.Globalization;
using Player.Core;

namespace Player.View.ViewModels
{
    public class PlaylistEntryViewModel  
    {
        private PlaylistEntry entry;

        public PlaylistEntryViewModel(PlaylistEntry e)
        {
            this.entry = e;
        }

        public string AlbumAndYearString
        {
            get { return string.Format("(“{0}”, {1})", entry.Song.Album, entry.Song.Year); }
        }

        public string DurationString
        {
            get { return entry.Song.Duration.ToString("m':'ss"); }
        }

        public string GlobalTrackNumberString
        {
            get { return entry.Index.ToString(CultureInfo.InvariantCulture); }
        }

        public string FullSongTitleString
        {
            get { return string.Format("{0} {1} — {2}", entry.Song.TrackNumber, entry.Song.Artist, entry.Song.Title); }
        } 
    }
}