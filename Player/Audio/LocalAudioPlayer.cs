using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio;
using NAudio.Wave;
using Player.Core;
using Player.Model;

namespace Player.Audio
{
    internal abstract class AudioPlayer : IDisposable
    {
        /// <summary>
        /// Occurs when the <see cref="Song"/> has finished it's playback.
        /// </summary>
        public event EventHandler SongFinished;

        /// <summary>
        /// Gets or sets the current time.
        /// </summary>
        /// <value>The current time.</value>
        public abstract TimeSpan CurrentTime { get; set; }

        /// <summary>
        /// Gets the current playback state.
        /// </summary>
        /// <value>
        /// The current playback state.
        /// </value>
        public abstract AudioPlayerState PlaybackState { get; }

        /// <summary>
        /// Gets the song that the <see cref="AudioPlayer"/> is assigned to.
        /// </summary>
        /// <value>The song that the <see cref="AudioPlayer"/> is assigned to.</value>
        public Song Song { get; protected set; }

        /// <summary>
        /// Gets the total time.
        /// </summary>
        /// <value>The total time.</value>
        public abstract TimeSpan TotalTime { get; }

        /// <summary>
        /// Gets or sets the volume (a value from 0.0 to 1.0).
        /// </summary>
        /// <value>The volume.</value>
        public abstract float Volume { get; set; }

        public abstract void Dispose();

        /// <summary>
        /// Loads the specified song into the <see cref="LocalAudioPlayer"/>. This is required before playing a new song.
        /// </summary>
        /// <exception cref="SongLoadException">The song could not be loaded.</exception>
        public virtual void Load()
        { }

        /// <summary>
        /// Pauses the playback of the <see cref="Song"/>.
        /// </summary>
        /// <remarks>
        /// This method has to ensure that the <see cref="PlaybackState"/> is set to <see cref="AudioPlayerState.Paused"/>
        /// before leaving the method.
        /// This method must always be callable, even if the <see cref="AudioPlayer"/> isn't loaded, is stopped or is paused.
        /// In this case it shouldn't perform any operation.
        /// </remarks>
        public abstract void Pause();

        /// <summary>
        /// Starts or continues the playback of the <see cref="Song"/>.
        /// </summary>
        /// <remarks>
        /// This method has to ensure that the <see cref="PlaybackState"/> is set to <see cref="AudioPlayerState.Playing"/>
        /// before leaving the method.
        /// </remarks>
        /// <exception cref="PlaybackException">The playback couldn't be started.</exception>
        public abstract void Play();

        /// <summary>
        /// Stops the playback of the <see cref="Song"/>.
        /// </summary>
        /// <remarks>
        /// This method has to ensure that the <see cref="PlaybackState"/> is set to <see cref="AudioPlayerState.Stopped"/>
        /// before leaving the method.
        /// This method must always be callable, even if the <see cref="AudioPlayer"/> isn't loaded, is stopped or is paused.
        /// In this case it shouldn't perform any operation.
        /// </remarks>
        public abstract void Stop();

        /// <summary>
        /// Raises the <see cref="SongFinished"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void OnSongFinished(EventArgs e)
        {
            if(this.SongFinished != null)
                this.SongFinished(this, e);
        }

    }

    internal class LocalAudioPlayer : AudioPlayer  
    { 
        private WaveChannel32 inputStream;
        private bool isLoaded;
        private object playerLock;
        private float volume;
        private IWavePlayer wavePlayer;

        public LocalAudioPlayer(Song song)
        {
            if (song == null)
                throw new ArgumentNullException("song");

            this.Song = song;
            this.Volume = 1.0f;

            this.playerLock = new object();
        }

        public override TimeSpan CurrentTime
        {
            get { return this.inputStream == null ? TimeSpan.Zero : this.inputStream.CurrentTime; }
            set { this.inputStream.CurrentTime = value; }
        }

        public override AudioPlayerState PlaybackState
        {
            get
            {
                if (this.wavePlayer != null)
                {
                    // We map the NAudio playbackstate to our own playback state,
                    // so that the NAudio API is not exposed outside of this class.
                    switch (this.wavePlayer.PlaybackState)
                    {
                        case NAudio.Wave.PlaybackState.Stopped:
                            return AudioPlayerState.Stopped;
                        case NAudio.Wave.PlaybackState.Playing:
                            return AudioPlayerState.Playing;
                        case NAudio.Wave.PlaybackState.Paused:
                            return AudioPlayerState.Paused;
                    }
                }

                return AudioPlayerState.None;
            }
        }

        public override TimeSpan TotalTime
        {
            get { return this.isLoaded ? this.inputStream.TotalTime : TimeSpan.Zero; }
        }

        public override float Volume
        {
            get { return this.volume; }
            set
            {
                this.volume = value;

                if (this.inputStream != null)
                {
                    this.inputStream.Volume = value;
                }
            }
        }

        public override void Dispose()
        {
            this.Stop();

            lock (this.playerLock)
            {
                if (wavePlayer != null)
                {
                    this.wavePlayer.Dispose();
                    this.wavePlayer = null;
                }

                if (inputStream != null)
                {
                    try
                    {
                        this.inputStream.Dispose();
                    }

                        // TODO: NAudio sometimes thows an exception here for unknown reasons
                    catch (MmException)
                    { }

                    this.inputStream = null;
                }
            }
        }

        public override void Load()
        {
            lock (this.playerLock)
            {
                this.wavePlayer = new WaveOutEvent();

                try
                {
                    this.CreateInputStream(this.Song);
                    this.wavePlayer.Init(inputStream);
                }

                    // NAudio can throw a broad range of exceptions when opening a song, so we catch everything
                catch (Exception ex)
                {
                    throw new SongLoadException("Song could not be loaded.", ex);
                }

                this.isLoaded = true;
            }
        }

        public override void Pause()
        {
            lock (this.playerLock)
            {
                if (this.wavePlayer == null || this.inputStream == null || this.wavePlayer.PlaybackState == NAudio.Wave.PlaybackState.Paused)
                    return;

                this.wavePlayer.Pause();

                this.EnsureState(AudioPlayerState.Paused);
            }
        }

        public override void Play()
        {
            lock (this.playerLock)
            {
                if (this.wavePlayer == null || this.inputStream == null || this.wavePlayer.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                    return;

                // Create a new thread, so that we can spawn the song state check on the same thread as the play method
                // With this, we can avoid cross-threading issues with the NAudio library
                Task.Factory.StartNew(() =>
                {
                    bool wasPaused = this.PlaybackState == AudioPlayerState.Paused;

                    try
                    {
                        this.wavePlayer.Play();
                    }

                    catch (MmException ex)
                    {
                        throw new PlaybackException("The playback couldn't be started.", ex);
                    }

                    if (!wasPaused)
                    {
                        while (this.PlaybackState != AudioPlayerState.Stopped && this.PlaybackState != AudioPlayerState.None)
                        {
                            this.UpdateSongState();
                            Thread.Sleep(250);
                        }
                    }
                });

                this.EnsureState(AudioPlayerState.Playing);
            }
        }

        public override void Stop()
        {
            lock (this.playerLock)
            {
                if (this.wavePlayer != null && this.wavePlayer.PlaybackState != NAudio.Wave.PlaybackState.Stopped)
                {
                    this.wavePlayer.Stop();

                    this.EnsureState(AudioPlayerState.Stopped);

                    this.isLoaded = false;
                }
            }
        }

        private static WaveChannel32 OpenMp3Stream(Stream stream)
        {
            WaveStream mp3Stream = new Mp3FileReader(stream);

            return new WaveChannel32(mp3Stream);
        }
         
        private void CreateInputStream(Song song)
        {
            Stream stream = File.OpenRead(song.FilePath);
             
            this.inputStream = OpenMp3Stream(stream);
                    
            this.inputStream.Volume = this.Volume;
        }

        private void EnsureState(AudioPlayerState state)
        {
            while (this.PlaybackState != state)
            {
                Thread.Sleep(200);
            }
        }

        private void UpdateSongState()
        {
            if (this.CurrentTime >= this.TotalTime)
            {
                this.Stop();
                this.OnSongFinished(EventArgs.Empty);
            }
        }
    }
}