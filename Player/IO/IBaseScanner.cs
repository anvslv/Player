using System;

namespace Player.Core
{
    public interface IBaseScanner 
    { 
        event EventHandler<FileEventArgs> FileFound;
        void Start(); 
    }
}