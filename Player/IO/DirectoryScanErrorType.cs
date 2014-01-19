using System;

namespace Player.Core
{
    /// <summary>
    /// Specifies which error occured during the directory scan.
    /// </summary>
    [Serializable]
    public enum DirectoryScanErrorType
    {
        /// <summary>
        /// Indicates a security exception during the directory scan.
        /// </summary>
        SecurityError,

        /// <summary>
        /// Indicates an access exception during the directory scan.
        /// </summary>
        AccessError,

        /// <summary>
        /// Indicates that a directory could not be found during the scan.
        /// </summary>
        DirectoryNotFoundError
    }

    [Serializable]
    public enum FileScanErrorType
    { 
        SecurityError,
         
        AccessError,
         
        FileNotFoundError
    }
}