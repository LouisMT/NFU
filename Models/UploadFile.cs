using Nfu.Properties;
using System;
using System.IO;
using IOPath = System.IO.Path;

namespace Nfu.Models
{
    public enum FileState
    {
        Normal,
        Temporary
    }

    public enum FileType
    {
        File,
        Directory,
        ZippedDirectory
    }

    public class UploadFile
    {
        private string _path;

        public UploadFile(FileState state = FileState.Normal, string extension = null)
        {
            State = state;

            if (state == FileState.Temporary)
            {
                Path = IOPath.GetTempFileName();
                FileName = IOPath.ChangeExtension(FileName, extension);
            }
        }

        /// <summary>
        /// The full path of this file.
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                _path = value;
                FileName = IOPath.GetFileName(_path);
                Type = Directory.Exists(_path) ? FileType.Directory : FileType.File;
            }
        }

        /// <summary>
        /// The file name of this file.
        /// </summary>
        public string FileName
        {
            get;
            set;
        }

        /// <summary>
        /// The state of this file.
        /// </summary>
        public FileState State
        {
            get;
            set;
        }

        /// <summary>
        /// The type of this file.
        /// </summary>
        public FileType Type
        {
            get;
            set;
        }

        /// <summary>
        /// Things that have to be done before uploading this file.
        /// </summary>
        public void BeforeUpload()
        {
            FileName = Misc.GetRemoteFileName(FileName);
        }

        /// <summary>
        /// Things that have to be done after uploading this file.
        /// </summary>
        public void AfterUpload()
        {
            // Delete this file if it is temporary
            if (State == FileState.Temporary)
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
                    Misc.HandleError(new Exception(String.Format(Resources.DeleteError, _path)), Resources.Title, false);
                }
            }
        }
    }
}
