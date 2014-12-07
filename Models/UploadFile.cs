using NFU.Properties;
using System;
using System.IO;
using IOPath = System.IO.Path;

namespace NFU.Models
{
    public class UploadFile
    {
        private string _path;

        public enum Type
        {
            Normal,
            Temporary
        }

        public UploadFile(Type type = Type.Normal, string extension = null)
        {
            if (type == Type.Temporary)
            {
                Path = IOPath.GetTempFileName();
                FileName = IOPath.ChangeExtension(FileName, extension);
                IsTemporary = true;
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
                FileName = Misc.GetFilename(_path);
                IsDirectory = Directory.Exists(_path);
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
        /// Indicates if this file is a directory or a zipped directory.
        /// </summary>
        public bool IsDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates if this file is temporary.
        /// Temporary files will be deleted after uploading.
        /// </summary>
        public bool IsTemporary
        {
            get;
            set;
        }

        /// <summary>
        /// Delete this file if it is temporary.
        /// </summary>
        public void DeleteIfTemporary()
        {
            if (IsTemporary)
            {
                try
                {
                    File.Delete(_path);
                }
                catch
                {
                    Misc.HandleError(new Exception(String.Format(Resources.UploadFile_DeleteError, _path)), Resources.UploadFile_Title, false, false);
                }
            }
        }
    }
}
