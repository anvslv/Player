using System;

namespace Player.IO
{
    public interface IBaseScanner 
    { 
        event EventHandler<FileEventArgs> FileFound;
        void Start(); 
    }
}