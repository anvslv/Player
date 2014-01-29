 
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using MicroMvvm;
using Player.Core;
using Player.Model;

namespace Player.ViewModels
{
    public class PlaylistViewModel : ObservableObject
    {
        private IEnumerable<PlaylistEntryViewModel> selectedPlaylistEntries;
        private readonly Library library;
      
        public PlaylistViewModel(Library l)
        {
            this.library = l;
            this.library.PlaylistChanged += (sender, e) => this.UpdatePlaylist();
            this.library.SongAdded += (sender, e) => this.UpdatePlaylist();
            this.library.SongStarted += (sender, e) => this.UpdatePlaylist();
        }

        public IEnumerable<PlaylistEntryViewModel> Songs
        {
            get
            {
                var count = this.library.CurrentPlaylist.Count();
                
                var songs = library.CurrentPlaylist.Select(entry => new PlaylistEntryViewModel(entry, count)).ToList();

                if (this.library.CurrentPlaylist.CurrentSongIndex.HasValue && songs.Count > 0)
                {
                    PlaylistEntryViewModel entry = songs[this.library.CurrentPlaylist.CurrentSongIndex.Value];

                    if (!entry.IsCorrupted)
                    {
                        entry.IsPlaying = true;
                    }
                }

                return songs;
            }
        }
          
        private void UpdatePlaylist( )
        { 
            this.RaisePropertyChanged( () => Songs );
        }

        public ICommand PlayCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => this.library.PlaySong(this.SelectedPlaylistEntries.First().Index),
                    () => this.SelectedPlaylistEntries != null && this.SelectedPlaylistEntries.Count() == 1
               );
            }
        }

        public ICommand RemoveSelectedPlaylistEntriesCommand
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        this.library.RemoveFromPlaylist(this.SelectedPlaylistEntries.Select(entry => entry.Index));

                        this.RaisePropertyChanged(() => this.Songs); 
                    },
                    () => this.SelectedPlaylistEntries != null
                        && this.SelectedPlaylistEntries.Any() 
                );
            }
        }

        public IEnumerable<PlaylistEntryViewModel> SelectedPlaylistEntries
        {
            get { return this.selectedPlaylistEntries; }
            set
            {
                if (this.selectedPlaylistEntries != value)
                {
                    this.selectedPlaylistEntries = value;
                    this.RaisePropertyChanged(() => this.SelectedPlaylistEntries);
                    this.RaisePropertyChanged(() => this.PlayCommand);
                }
            }
        }


    }
}