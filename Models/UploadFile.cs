using System;
using System.IO;
using IOPath = System.IO.Path;

namespace NFU.Models
{
    public class UploadFile
    {
        private string path;
        private bool isDirectory;

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

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                path = value;
                FileName = Misc.GetFilename(path);
                isDirectory = Directory.Exists(path);
            }
        }

        public string FileName
        {
            get;
            set;
        }

        public bool IsDirectory
        {
            get
            {
                return isDirectory;
            }
        }

        /// <summary>
        /// Temporary files will be deleted after uploading.
        /// </summary>
        public bool IsTemporary
        {
            get;
            set;
        }

        public void DeleteIfTemporary()
        {
            if (IsTemporary)
            {
                try
                {
                    File.Delete(path);
                }
                catch
                { }
            }
        }
    }
}
