using System;
using System.IO;
using System.Security;
using Player.Core;

namespace Player.IO
{
    public class FileScanner : IBaseScanner
    {
        private FileInfo foundFile;
        private volatile bool isStopped;

        public FileScanner(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            this.DirectoryPath = path; 
        }

        public event EventHandler<FileEventArgs> FileFound;
        public event EventHandler<FileScanErrorEventArgs> FileScanError;
        public event EventHandler FileProceeded;
        public event EventHandler Finished;
         
        public string DirectoryPath { get; private set; }
         
        public FileInfo FoundFile
        {
            get { return this.foundFile; }
        }

        public bool IsStopped
        {
            get { return this.isStopped; }
            private set { this.isStopped = value; }
        }

        public void Start()
        {
            this.ScanFile(this.DirectoryPath);

            this.OnFinished(EventArgs.Empty);
        }

        public void Stop()
        {
            this.IsStopped = true;
        }

        protected virtual void OnFileFound(FileEventArgs e)
        {
            if (this.FileFound != null)
                this.FileFound(this, e);
        }

        protected virtual void OnFinished(EventArgs e)
        {
            if (this.Finished != null)
                this.Finished(this, e);
        }

        protected virtual void OnFileScanError(FileScanErrorEventArgs e)
        {
            if (this.FileScanError != null)
                this.FileScanError(this, e);
        }

        protected virtual void OnFileProceeded(EventArgs e)
        {
            if (this.FileProceeded != null)
                this.FileProceeded(this, e);
        }

        private void ScanFile(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (this.IsStopped) { return; }

            var fileInfo = new FileInfo(path);

            try
            {
                if (fileInfo.Exists)
                {  
                    if (this.IsStopped) { return; }

                    this.foundFile = fileInfo;
                    this.OnFileFound(new FileEventArgs(fileInfo)); 
                }

                else
                {
                    this.OnFileScanError(new FileScanErrorEventArgs(fileInfo.FullName, FileScanErrorType.FileNotFoundError));
                }
            }

            catch (UnauthorizedAccessException)
            {
                this.OnFileScanError(new FileScanErrorEventArgs(path, FileScanErrorType.AccessError));
            }

            catch (SecurityException)
            {
                this.OnFileScanError(new FileScanErrorEventArgs(path, FileScanErrorType.SecurityError));
            }

            this.OnFileProceeded(EventArgs.Empty);
        }
    }
}