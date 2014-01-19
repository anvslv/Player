using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Player.Core
{
    /// <summary>
    /// Provides a directoy scanner to scan a directory recursively for files and directories.
    /// </summary>
    public class DirectoryScanner : IBaseScanner
    {
        private readonly List<FileInfo> filesFound;
        private volatile bool isStopped;

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryScanner"/> class.
        /// </summary>
        /// <param name="path">The path of the directory to scan.</param>
        public DirectoryScanner(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.DirectoryPath = path;
            this.filesFound = new List<FileInfo>();
        }

        /// <summary>
        /// Occurs when a directory has been found.
        /// </summary>
        public event EventHandler<DirectoryEventArgs> DirectoryFound;

        /// <summary>
        /// Occurs when a directory has been proceeded.
        /// </summary>
        public event EventHandler DirectoryProceeded;

        /// <summary>
        /// Occurs when a directory couldn't be accessed.
        /// </summary>
        public event EventHandler<DirectoryScanErrorEventArgs> DirectoryScanError;

        /// <summary>
        /// Occurs when a file has been found.
        /// </summary>
        public event EventHandler<FileEventArgs> FileFound;
     
        /// <summary>
        /// Occurs when the search has finished.
        /// </summary>
        public event EventHandler Finished;

        /// <summary>
        /// Gets the directory path.
        /// </summary>
        public string DirectoryPath { get; private set; }

        /// <summary>
        /// Gets the found files.
        /// </summary>
        public IEnumerable<FileInfo> FilesFound
        {
            get { return this.filesFound; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the scanner is stopped.
        /// </summary>
        /// <value>
        /// true if the scanner is stopped; otherwise, false.
        /// </value>
        public bool IsStopped
        {
            get { return this.isStopped; }
            private set { this.isStopped = value; }
        }

        /// <summary>
        /// Starts the directory scanner.
        /// </summary>
        public void Start()
        {
            this.ScanDirectories(this.DirectoryPath);

            this.OnFinished(EventArgs.Empty);
        }

        /// <summary>
        /// Stops the directory scanner.
        /// </summary>
        public void Stop()
        {
            this.IsStopped = true;
        }

        /// <summary>
        /// Raises the <see cref="DirectoryFound"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DirectoryEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDirectoryFound(DirectoryEventArgs e)
        {
            if (this.DirectoryFound != null)
                this.DirectoryFound(this, e);
        }

        /// <summary>
        /// Raises the <see cref="DirectoryProceeded"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnDirectoryProceeded(EventArgs e)
        {
            if (this.DirectoryProceeded != null)
                this.DirectoryProceeded(this, e);
        }

        /// <summary>
        /// Raises the <see cref="DirectoryScanError"/> event.
        /// </summary>
        /// <param name="e">The <see cref="DirectoryScanErrorEventArgs"/> instance containing the event data.</param>
        protected virtual void OnDirectoryScanError(DirectoryScanErrorEventArgs e)
        {
            if (this.DirectoryScanError != null)
                this.DirectoryScanError(this, e);
        }

        /// <summary>
        /// Raises the <see cref="FileFound"/> event.
        /// </summary>
        /// <param name="e">The <see cref="FileEventArgs"/> instance containing the event data.</param>
        protected virtual void OnFileFound(FileEventArgs e)
        {
            if (this.FileFound != null)
                this.FileFound(this, e);
        }

        /// <summary>
        /// Raises the <see cref="Finished"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnFinished(EventArgs e)
        {
            if (this.Finished != null)
                this.Finished(this, e);
        }

        /// <summary>
        /// Scans a directory recursively.
        /// </summary>
        /// <param name="rootPath">The root path.</param>
        private void ScanDirectories(string rootPath)
        {
            if (rootPath == null)
                throw new ArgumentNullException("rootPath");

            if (this.IsStopped) { return; }

            var rootDirectory = new DirectoryInfo(rootPath);

            try
            {
                if (rootDirectory.Exists)
                {
                    FileInfo[] files = rootDirectory.GetFiles();

                    foreach (FileInfo file in files)
                    {
                        if (this.IsStopped) { return; }

                        this.filesFound.Add(file);
                        this.OnFileFound(new FileEventArgs(file));
                    }

                    DirectoryInfo[] directories = rootDirectory.GetDirectories();

                    foreach (DirectoryInfo directory in directories)
                    {
                        if (this.IsStopped) { return; }

                        this.OnDirectoryFound(new DirectoryEventArgs(directory));
                        this.ScanDirectories(directory.FullName);
                    }
                }

                else
                {
                    this.OnDirectoryScanError(new DirectoryScanErrorEventArgs(rootDirectory.FullName, DirectoryScanErrorType.DirectoryNotFoundError));
                }
            }

            catch (UnauthorizedAccessException)
            {
                this.OnDirectoryScanError(new DirectoryScanErrorEventArgs(rootPath, DirectoryScanErrorType.AccessError));
            }

            catch (SecurityException)
            {
                this.OnDirectoryScanError(new DirectoryScanErrorEventArgs(rootPath, DirectoryScanErrorType.SecurityError));
            }

            this.OnDirectoryProceeded(EventArgs.Empty);
        }
    }
}