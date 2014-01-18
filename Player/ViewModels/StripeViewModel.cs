using System;
using System.Collections.Generic;
using MicroMvvm;
using Player.Core; 

namespace Player.View.ViewModels
{
    public class StripeViewModel : ObservableObject
    {
        private readonly Library library;
       
        public StripeViewModel(Library library)
        {
            this.library = library;

            //this.model.CurrentTime = TimeSpan.FromSeconds(CurrentSeconds);
          
            //this.Time = string.Format("{0} -{1} {2}",
            //    this.model.CurrentTime,
            //    this.model.TotalTime - this.model.CurrentTime,
            //    this.model.TotalTime);
              
            //this.Title = string.Format("{0} — {1}", this.model.CurrentPlaylistEntry.Song.Artist, this.model.CurrentPlaylistEntry.Song.Title); 
        }
            
        public void HandleDropFiles(IEnumerable<string> files)
        { 
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

        public bool IsPlaying
        {
            get { return this.library.IsPlaying; }
        }
          
        public double TotalSeconds { get; set; }
         
        public string Title { get; set; } 
         
        public string Time { get; set; }
    }
}