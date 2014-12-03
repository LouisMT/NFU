using NFU.Properties;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Principal;
using System.Windows.Forms;

namespace NFU
{
    public static class Uploader
    {
        public static bool isBusy;

        private static int currentIndex;
        private static bool uploadSuccess;
        private static BackgroundWorker uploadWorker = new BackgroundWorker();
        private static Dictionary<string, string> filesDictionary = new Dictionary<string, string>();

        /// <summary>
        /// Constructor.
        /// </summary>
        static Uploader()
        {
            uploadWorker.WorkerReportsProgress = true;
            uploadWorker.WorkerSupportsCancellation = true;

            uploadWorker.DoWork += UploadWorkerHandler;
            uploadWorker.ProgressChanged += UploadWorkerProgress;
            uploadWorker.RunWorkerCompleted += UploadWorkerCompleted;
        }

        /// <summary>
        /// Upload one or more files to the remote server.
        /// </summary>
        /// <param name="paths">String array of paths to the local files.</param>
        /// <returns>True on success; false on failure.</returns>
        public static bool Upload(string[] paths)
        {
            if (isBusy)
                return false;

            isBusy = true;
            currentIndex = 1;
            uploadSuccess = true;
            filesDictionary.Clear();
            Misc.SetControlStatus(false);

            foreach (string path in paths)
                filesDictionary.Add(path, Misc.GetFilename(path));

            uploadWorker.RunWorkerAsync();

            return true;
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static bool UploadImage(Image image)
        {
            string Filename = Misc.GetTempFileName(".png");
            image.Save(Filename);
            return Uploader.Upload(new[] { Filename });
        }

        /// <summary>
        /// Upload a textfile.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <returns></returns>
        public static bool UploadText(string text)
        {
            string Filename = Misc.GetTempFileName(".txt");
            File.WriteAllText(Filename, text);
            return Uploader.Upload(new[] { Filename });
        }

        /// <summary>
        /// Cancel the current upload.
        /// </summary>
        public static void Cancel()
        {
            uploadWorker.CancelAsync();
        }

        /// <summary>
        /// The BackgroundWorker for the upload.
        /// </summary>
        static void UploadWorkerHandler(object sender, DoWorkEventArgs a)
        {
            foreach (KeyValuePair<string, string> file in filesDictionary)
            {
                bool abort = false;

                string path = file.Key;
                string filename = file.Value;

                switch (Settings.Default.TransferType)
                {
                    case (int)TransferType.FTP:
                    case (int)TransferType.FTPSExplicit:
                        abort = UploadFTP(path, filename);
                        break;

                    case (int)TransferType.SFTP:
                    case (int)TransferType.SFTPKeys:
                        try
                        {
                            abort = UploadSFTP(path, filename);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(Resources.Uploader_SshNetMissing, Resources.Uploader_SshNetMissingTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            uploadSuccess = false;
                            Misc.HandleError(err, Resources.Uploader_Sftp);
                            abort = true;
                        }
                        break;

                    case (int)TransferType.CIFS:
                        abort = UploadCIFS(path, filename);
                        break;
                }

                if (abort)
                    break;

                currentIndex++;
            }
        }

        /// <summary>
        /// Upload a file via FTP(s).
        /// </summary>
        /// <param name="path">The path of the local file.</param>
        /// <param name="filename">The filename of the remote file.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadFTP(string path, string filename)
        {
            try
            {
                byte[] Buffer = new byte[1024 * 10];

                FtpWebRequest FTPrequest;

                if (!String.IsNullOrEmpty(Settings.Default.Directory))
                {
                    FTPrequest = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}/{3}", Settings.Default.Host, Settings.Default.Port, Settings.Default.Directory, filename));
                }
                else
                {
                    FTPrequest = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}", Settings.Default.Host, Settings.Default.Port, filename));
                }

                if (Settings.Default.TransferType == (int)TransferType.FTPSExplicit)
                {
                    FTPrequest.EnableSsl = true;
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(Misc.ValidateServerCertificate);
                }

                FTPrequest.KeepAlive = false;
                FTPrequest.Method = WebRequestMethods.Ftp.UploadFile;

                FTPrequest.Credentials = new NetworkCredential(Settings.Default.Username, Misc.Decrypt(Settings.Default.Password));

                using (FileStream inputStream = File.OpenRead(path))
                using (Stream outputStream = FTPrequest.GetRequestStream())
                {
                    long totalReadBytesCount = 0;
                    int readBytesCount;
                    while ((readBytesCount = inputStream.Read(Buffer, 0, Buffer.Length)) > 0)
                    {
                        if (uploadWorker.CancellationPending) return true;

                        outputStream.Write(Buffer, 0, readBytesCount);
                        totalReadBytesCount += readBytesCount;

                        uploadWorker.ReportProgress((int)(totalReadBytesCount * 100 / inputStream.Length));
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                uploadSuccess = false;
                Misc.HandleError(e, Resources.Uploader_Ftps);
                return true;
            }
        }

        /// <summary>
        /// Upload a file via SFTP.
        /// </summary>
        /// <param name="path">The path of the local file.</param>
        /// <param name="filename">The filename of the remote file.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadSFTP(string path, string filename)
        {
            try
            {
                SftpClient client;

                if (Settings.Default.TransferType == (int)TransferType.SFTPKeys)
                {
                    client = new SftpClient(Settings.Default.Host, Settings.Default.Port, Settings.Default.Username, new PrivateKeyFile(Misc.Decrypt(Settings.Default.Password)));
                }
                else
                {
                    client = new SftpClient(Settings.Default.Host, Settings.Default.Port, Settings.Default.Username, Misc.Decrypt(Settings.Default.Password));
                }

                using (FileStream inputStream = new FileStream(path, FileMode.Open))
                using (SftpClient outputStream = client)
                {
                    outputStream.Connect();

                    if (!String.IsNullOrEmpty(Settings.Default.Directory)) outputStream.ChangeDirectory(Settings.Default.Directory);

                    IAsyncResult Async = outputStream.BeginUploadFile(inputStream, filename);
                    SftpUploadAsyncResult SFTPAsync = Async as SftpUploadAsyncResult;

                    while (!SFTPAsync.IsCompleted)
                    {
                        if (uploadWorker.CancellationPending) return true;

                        uploadWorker.ReportProgress((int)(SFTPAsync.UploadedBytes * 100 / (ulong)inputStream.Length));
                    }

                    outputStream.EndUploadFile(Async);
                }

                return false;
            }
            catch (Exception e)
            {
                uploadSuccess = false;
                Misc.HandleError(e, Resources.CP_Sftp);
                return true;
            }
        }


        /// <summary>
        /// Upload a file via CIFS.
        /// </summary>
        /// <param name="path">The path of the local file.</param>
        /// <param name="filename">The filename of the remote file.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadCIFS(string path, string filename)
        {
            try
            {
                byte[] Buffer = new byte[1024 * 10];

                IntPtr Token = IntPtr.Zero;
                Misc.LogonUser(Settings.Default.Username, Resources.Uploader_Nfu, Misc.Decrypt(Settings.Default.Password), 9, 0, ref Token);
                WindowsIdentity Identity = new WindowsIdentity(Token);

                string DestPath = (!String.IsNullOrEmpty(Settings.Default.Directory)) ? String.Format(@"\\{0}\{1}\{2}", Settings.Default.Host, Settings.Default.Directory, filename) : String.Format(@"\\{0}\{1}", Settings.Default.Host, filename);

                using (Identity.Impersonate())
                using (FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(DestPath, FileMode.CreateNew, FileAccess.Write))
                {
                    long FileLength = inputStream.Length;
                    long TotalBytes = 0;
                    int CurrentBlockSize;

                    while ((CurrentBlockSize = inputStream.Read(Buffer, 0, Buffer.Length)) > 0)
                    {
                        if (uploadWorker.CancellationPending) return true;

                        outputStream.Write(Buffer, 0, CurrentBlockSize);
                        TotalBytes += CurrentBlockSize;

                        uploadWorker.ReportProgress((int)(TotalBytes * 100 / FileLength));
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                uploadSuccess = false;
                Misc.HandleError(e, Resources.Uploader_Cifs);
                return true;
            }
        }

        /// <summary>
        /// The progress handler for the upload
        /// </summary>
        static void UploadWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            Program.formCore.progressUpload.Value = e.ProgressPercentage;
            Program.formCore.toolStripStatus.Text = String.Format(Resources.Uploader_Uploading, currentIndex, filesDictionary.Count);
        }

        /// <summary>
        /// The handler for the completion of the upload
        /// </summary>
        static void UploadWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Misc.SetControlStatus(true);
            Program.formCore.progressUpload.Value = 0;

            if (uploadSuccess)
            {
                Misc.ShowInfo(Resources.Uploader_UploadSuccessfulTitle, Resources.Uploader_UploadSuccessful);
                Program.formCore.toolStripStatus.Text = Resources.Uploader_UploadSuccessfulStatus;

                List<string> clipboard = new List<string>();

                foreach (KeyValuePair<string, string> file in filesDictionary)
                    clipboard.Add(Settings.Default.URL + file.Value);

                Clipboard.SetText(String.Join(Environment.NewLine, clipboard));
            }
            else
            {
                Misc.ShowInfo(Resources.Uploader_UploadFailedTitle, Resources.Uploader_UploadFailed, ToolTipIcon.Error);
            }

            isBusy = false;
        }
    }
}
