using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Player.Core;
using Player.IO;

namespace Player.Model
{
    internal sealed class SongFinder 
    {
        private static readonly string[] AllowedExtensions = new[] { ".mp3" };
        private readonly List<string> corruptFiles;
        private readonly Queue<string> pathQueue;
        private readonly IBaseScanner scanner;
        private readonly object songListLock;
        private readonly object tagLock;
        private volatile bool abort; 
        private volatile bool isSearching;
        private volatile bool isTagging;

        private readonly List<Song> songsFound;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongFinder"/> class.
        /// </summary>
        /// <param name="path">The path of the directory where the recursive search should start.</param>
        public SongFinder(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.tagLock = new object();
            this.songListLock = new object();

            this.pathQueue = new Queue<string>();
            this.corruptFiles = new List<string>();

            this.songsFound = new List<Song>();

            if (Directory.Exists(path))
            { 
                this.scanner = new DirectoryScanner(path);
            }
            else if (File.Exists(path))
            {
                this.scanner = new FileScanner(path);
            }
            this.scanner.FileFound += ScannerFileFound; 
        }

        /// <summary>
        /// Occurs when the song crawler has finished.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Occurs when a song has been found.
        /// </summary>
        public event EventHandler<SongEventArgs> SongFound;

        /// <summary>
        /// Gets the songs that have been found.
        /// </summary>
        /// <value>The songs that have been found.</value>
        public IEnumerable<Song> SongsFound
        {
            get { return this.songsFound; }
        }

        /// <summary>
        /// Gets the files that are corrupt and could not be read.
        /// </summary>
        public IEnumerable<string> CorruptFiles
        {
            get { return this.corruptFiles; }
        }

        /// <summary>
        /// Gets the total number of songs that are counted yet.
        /// </summary>
        public int CurrentTotalSongs
        {
            get
            {
                int pathCount;

                lock (this.tagLock)
                {
                    pathCount = this.pathQueue.Count;
                }

                lock (this.songListLock)
                {
                    pathCount += this.InternSongsFound.Count;
                }

                return pathCount;
            }
        }

        private ICollection<Song> InternSongsFound
        {
            get { return this.songsFound; }
        }
         
        /// <summary>
        /// Gets the number of tags that are processed yet.
        /// </summary>
        public int TagsProcessed
        {
            get
            {
                int songCount;

                lock (this.songListLock)
                {
                    songCount = this.InternSongsFound.Count;
                }

                return songCount;
            }
        }

        public void Abort()
        {
            this.abort = true;
        }
         
        /// <summary>
        /// Starts the <see cref="SongFinder"/>.
        /// </summary>
        public void Start()
        {
            var fileScanTask = Task.Factory.StartNew(this.StartFileScan);

            this.isSearching = true;

            var tagScanTask = Task.Factory.StartNew(this.StartTagScan);

            Task.WaitAll(fileScanTask, tagScanTask);

            this.OnFinished(EventArgs.Empty);
        }

        private static Song CreateSong(TagLib.Tag tag, TimeSpan duration, string filePath)
        {
            return new LocalSong(filePath, duration)
            {
                Album = PrepareTag(tag.Album, String.Empty),
                Artist = PrepareTag(tag.FirstPerformer, "Unknown Artist"), //HACK: In the future retrieve the string for an unkown artist from the view if we want to localize it 
                Title = PrepareTag(tag.Title, Path.GetFileNameWithoutExtension(filePath)),
                TrackNumber = (int)tag.Track, 
            };
        }

        private static string PrepareTag(string tag, string replacementIfNull)
        {
            return tag == null ? replacementIfNull : TagSanitizer.Sanitize(tag);
        }

        private void AddSong(TagLib.File file)
        { 
            var song = CreateSong(file.Tag, file.Properties.Duration, file.Name);

            lock (this.songListLock)
            {
                this.InternSongsFound.Add(song);
            }

            this.OnSongFound(new SongEventArgs(song));
        }

        private void ProcessFile(string filePath)
        {
            try
            { 
                TagLib.File file = null;
                 
                file = new TagLib.Mpeg.AudioFile(filePath);
                        
                if (file != null)
                {
                    if (file.Tag != null)
                    {
                        this.AddSong(file);
                    }

                    file.Dispose();
                }
            }

            catch (TagLib.CorruptFileException)
            {
                this.corruptFiles.Add(filePath);
            }

            catch (IOException)
            {
                this.corruptFiles.Add(filePath);
            }
        }

        private void ScannerFileFound(object sender, FileEventArgs e)
        {
            if (this.abort || !AllowedExtensions.Contains(e.File.Extension))
                return;

            lock (this.tagLock)
            {
                this.pathQueue.Enqueue(e.File.FullName);
            }
        }

        private void StartFileScan()
        {
            this.scanner.Start();
            this.isSearching = false;
        }

        private void StartTagScan()
        {
            this.isTagging = true;

            while ((this.isSearching || this.isTagging) && !this.abort)
            {
                string filePath = null;

                lock (this.tagLock)
                {
                    if (this.pathQueue.Any())
                    {
                        filePath = this.pathQueue.Dequeue();
                    }

                    else if (!this.isSearching)
                    {
                        this.isTagging = false;
                    }
                }

                if (filePath != null)
                {
                    this.ProcessFile(filePath);
                }
            }
        }
          
        private void OnFinished(EventArgs e)
        {
            if (this.Finished != null)
                this.Finished(this, e);
        }

        private void OnSongFound(SongEventArgs e)
        {
            if (this.SongFound != null)
                this.SongFound(this, e);
        }
    }
        
    /// <summary>
    /// Provides data for the <see cref="SongFinder.SongFound"/> event.
    /// </summary>
    public sealed class SongEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SongEventArgs"/> class.
        /// </summary>
        /// <param name="song">The song that has been found.</param>
        /// <exception cref="ArgumentNullException"><c>song</c> is null.</exception>
        public SongEventArgs(Song song)
        {
            if (song == null)
                throw new ArgumentNullException("song");

            this.Song = song;
        }

        /// <summary>
        /// Gets the song that has been found.
        /// </summary>
        /// <value>The song that has been found.</value>
        public Song Song { get; private set; }
    }
}