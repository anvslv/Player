using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers; 
using System.Windows.Input; 
using MicroMvvm;
using Player.Core;
using Player.Model;
using Player.Settings;
using Player.Services;

namespace Player.ViewModels
{
    public class StripeViewModel : ObservableObject, IDisposable
    {
        private readonly Library library;
        private readonly Timer updateTimer;
        private readonly Timer showVolumeTimer;
       
        public StripeViewModel(Library library)
        {
            this.library = library;
            this.library.Initialize();

            this.library.SongStarted += (sender, args) => this.HandleSongStarted();
            this.library.SongFinished += (sender, args) => this.HandleSongFinished();
            this.library.SongCorrupted += (sender, args) => this.HandleSongCorrupted();
            this.library.FirstSongSelected += (sender, e) => this.UpdateStripe();
          
            if (this.library.CurrentPlaylist == null)
            {
                this.library.CreateNewPlaylist();
            }

            this.updateTimer = new Timer(1000);
            this.updateTimer.Elapsed += (sender, e) => this.UpdateCurrentTime();

            this.showVolumeTimer = new Timer(2000);
            this.showVolumeTimer.AutoReset = false;
            this.showVolumeTimer.Elapsed += (sender, e) => this.UpdateCurrentTime(); 
        }

        private double thisWidth;

        private Double StripeMinWidth = 300;
        private Double ShortTimeStripeMaxWidth = 350;
        private Double StripeMaxWidth = 700;

        public double ThisWidth
        {
            get { return thisWidth; }
            set
            {
                if (Math.Abs(thisWidth - value) > 1)
                { 
                    thisWidth = value > StripeMaxWidth ? StripeMaxWidth : value;
                    thisWidth = value < StripeMinWidth ? StripeMinWidth : value;

                    RaisePropertyChanged(() => ThisWidth);
                    RaisePropertyChanged(() => RightBlock);
                }
            }
        }


        public int CurrentSeconds
        {
            get { return (int)this.CurrentTime.TotalSeconds; }
            set { this.library.CurrentTime = TimeSpan.FromSeconds(value); }
        }
          
        public TimeSpan CurrentTime
        {
            get { return this.library.CurrentTime; }
        }

        public int TotalSeconds
        {
            get { return (int)this.TotalTime.TotalSeconds; }
        }

        public TimeSpan TotalTime
        {
            get { return this.library.TotalTime; }
        }

        public bool IsPlaying
        {
            get { return this.library.IsPlaying; }
        }

        public bool SongLoaded
        {
            get { return this.library.LoadedSong != null; }
        }

        public string Title
        {
            get { 
                return string.Format("{0} — {1}", 
                    this.library.LoadedSong.Artist,
                    this.library.LoadedSong.Title); 
            }
        }

        public string Time
        {
            get
            {
                if (library.LoadedSong != null)
                    return string.Format("{0} -{1} {2}",
                    this.CurrentTime.ToMinutesAndSeconds(),
                    this.TotalTime.Subtract(this.CurrentTime).ToMinutesAndSeconds(),
                    this.TotalTime.ToMinutesAndSeconds());
                return string.Empty;
            }
        }

        public string ShortTime
        {
            get
            {
                if (library.LoadedSong != null)
                    return string.Format("{0}", this.CurrentTime.ToMinutesAndSeconds());
                return string.Empty;
            }
        }

        public string RightBlock
        {
            get
            {
                if (showVolumeTimer.Enabled)
                    return string.Format("{0}%", (int)(this.Volume * 100));
                if (thisWidth > ShortTimeStripeMaxWidth) 
                    return Time;
                return ShortTime;
            }
        }

        public string LeftBlock
        {
            get
            {
                if (SongLoaded)
                    return Title;
                return Resources.DropHere; 
            }
        }

        public float Volume
        {
            get { return this.library.Volume; }
            set
            {
                this.library.Volume = (float)value;
                this.RaisePropertyChanged(() => this.Volume);
                this.showVolumeTimer.Start();
            }
        }

        #region Commands

        public ICommand IncreaseVolume
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        this.Volume = ChangeVolume(0.05f);
                    } 
                );
            }
        }

        public ICommand DecreaseVolume
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        this.Volume = ChangeVolume(-0.05f);
                    }
                );
            }
        }
          
        public ICommand PauseCommand
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        this.library.PauseSong();
                        this.updateTimer.Stop();
                        this.RaisePropertyChanged(() => this.IsPlaying); 
                    },
                    () => this.IsPlaying
                );
            }
        }
          
        public ICommand PlayCommand
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        if (this.library.IsPaused || this.library.LoadedSong != null)
                        {
                            this.library.ContinueSong();
                            this.updateTimer.Start();
                            this.RaisePropertyChanged(() => this.IsPlaying); 
                        }

                        else if (this.library.CurrentPlaylist.CurrentSongIndex != null)
                        {
                            this.library.PlaySong((int)this.library.CurrentPlaylist.CurrentSongIndex);
                        }
                    },
                    () =>
                         
                        (this.library.CurrentPlaylist.CurrentSongIndex != null ||
                         
                        (this.library.LoadedSong != null || this.library.IsPaused))
                );
            }
        }

        public ICommand PauseContinueCommand
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        if (this.IsPlaying)
                        {
                            this.PauseCommand.Execute(null);
                        }

                        else
                        {
                            this.PlayCommand.Execute(false);
                        }
                    },
                    () => this.IsPlaying ? this.PauseCommand.CanExecute(null) : this.PlayCommand.CanExecute(null)
                );
            }
        }
         
        public ICommand NextSongCommand
        {
            get
            {
                return new RelayCommand
                (
                    () => this.library.PlayNextSong(),
                    () => this.library.CanPlayNextSong
                );
            }
        }

        public ICommand PreviousSongCommand
        {
            get { return new RelayCommand(this.library.PlayPreviousSong, () => this.library.CanPlayPreviousSong);}
        }

        public ICommand ShowHidePlaylist
        {
            get
            {
                return new RelayCommand
                (
                    () =>
                    {
                        WindowStateManager.Instance.ShowHidePlaylist();
                    }
                );
            }
        }

        #endregion

        public void HandleDropFiles(IEnumerable<string> files)
        {
            library.AddLocalSongsAsync(files.ToArray());
        }

        public void Dispose()
        { 
            this.updateTimer.Dispose();
        }

        private float ChangeVolume(float delta)
        {
            var newone = this.Volume += delta;
            if (newone > 1.0f) return 1.0f;
            if (newone < 0f) return 0f;
            return newone;
        }
          
        private void HandleSongCorrupted()
        {
            this.RaisePropertyChanged(() => this.IsPlaying);
            this.RaisePropertyChanged(() => this.SongLoaded);
        }

        private void HandleSongFinished()
        { 
            UpdateStripe();
            if (!this.library.CanPlayNextSong)
            {
                this.RaisePropertyChanged(() => this.IsPlaying);
            }
             
            this.RaisePropertyChanged(() => this.PreviousSongCommand);

            this.updateTimer.Stop();
        }

        private void HandleSongStarted()
        {
            this.UpdateStripe();

            this.RaisePropertyChanged(() => this.IsPlaying); 
            this.RaisePropertyChanged(() => this.PlayCommand);

            this.updateTimer.Start();
        }
         
        private void UpdateStripe()
        {
            this.RaisePropertyChanged(() => this.SongLoaded);

            this.RaisePropertyChanged(() => this.CurrentSeconds);
            this.RaisePropertyChanged(() => this.TotalSeconds);
              
            this.RaisePropertyChanged(() => this.Title);
            this.RaisePropertyChanged(() => this.Time);

            this.RaisePropertyChanged(() => this.LeftBlock);
            this.RaisePropertyChanged(() => this.RightBlock);
        }

        private void UpdateCurrentTime()
        {
            this.RaisePropertyChanged(() => this.TotalSeconds);
            this.RaisePropertyChanged(() => this.CurrentSeconds);
            
            this.RaisePropertyChanged(() => this.Time);

            this.RaisePropertyChanged(() => this.RightBlock);
        } 
    }
}