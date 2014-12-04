using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFU
{
    public class UploadFile
    {
        private string path;
        private string fileName;
        private bool isDirectory;
        private bool isTemporary;

        public enum Type
        {
            Normal,
            Temporary
        }

        public UploadFile(Type uploadFileType = Type.Normal, string extension = null)
        {
            if (uploadFileType == Type.Temporary)
            {
                if (extension == null)
                {
                    Path = System.IO.Path.GetTempFileName();
                }
                else
                {
                    Path = String.Format("{0}.{1}", System.IO.Path.GetTempFileName(), extension);
                }

                isTemporary = true;
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
                fileName = Misc.GetFilename(path);
                isDirectory = Directory.Exists(path);
            }
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
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
            get
            {
                return isTemporary;
            }
            set
            {
                isTemporary = value;
            }
        }

        public void DeleteIfTemporary()
        {
            if (isTemporary)
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
