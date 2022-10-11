using System; 
using System.IO;
using System.Linq;
using System.Security; 
using SharpCompress.Archives;
using SharpCompress.Common;

namespace Player.IO
{
    public class FileScanner : DirectoryScanner
    {  
        private string[] supportedExtensions =
        {
            ".rar",
            ".zip", 
            // ".7z" weird behavior
        };

        public FileScanner(string path) : base ()
        {
            if (path == null)
                throw new ArgumentNullException("path");
             
            this.FilePath = path; 
        }
         
        public event EventHandler<FileScanErrorEventArgs> FileScanError;
        public event EventHandler FileProceeded; 
         
        public string FilePath { get; private set; }
           
        public override void Start()
        { 
            this.ChooseHowToScan(this.FilePath); 
            this.OnFinished(EventArgs.Empty);
        }

        private void ChooseHowToScan(string file)
        { 
            var i = new FileInfo(file);
            if (supportedExtensions.Contains(i.Extension))
            {
                this.DirectoryPath = string.Format(@"C:\temp\{0}", Guid.NewGuid());

                if (Directory.Exists(DirectoryPath) == false)
                {
                    Directory.CreateDirectory(DirectoryPath);
                }

                var archive = ArchiveFactory.Open(file);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(DirectoryPath, new ExtractionOptions{ ExtractFullPath = true, Overwrite = true} );

                        var thispath = Path.Combine(DirectoryPath, entry.Key);
                        this.ScanFile(thispath);
                    }
                }
            }
            else
            {
                this.ScanFile(this.FilePath); 
            }
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

                    this.FilesFound.Add(fileInfo); 
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