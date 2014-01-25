using System;

namespace Player.IO
{
    public class FileScanErrorEventArgs : EventArgs
    { 
        public FileScanErrorEventArgs(string filePath, FileScanErrorType errorType)
        {
            this.FilePath = filePath;
            this.ErrorType = errorType;
        }
         
        public string FilePath { get; private set; }
         
        public FileScanErrorType ErrorType { get; private set; }
    }
}