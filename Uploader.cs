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
            uploadWorker.ProgressChanged += UploadWorkerProgressHandler;
            uploadWorker.RunWorkerCompleted += UploadWorkerCompletedHandler;
        }

        /// <summary>
        /// Upload one or more files to the remote server.
        /// </summary>
        /// <param name="aPath">String array of paths to the local files.</param>
        /// <returns>True on success; false on failure.</returns>
        public static bool Upload(string[] aPath)
        {
            if (isBusy)
                return false;

            currentIndex = 1;
            isBusy = true;
            uploadSuccess = true;
            filesDictionary.Clear();
            Misc.SetControlStatus(false);

            foreach (string cPath in aPath)
                filesDictionary.Add(cPath, Misc.GetFilename(cPath));

            uploadWorker.RunWorkerAsync();

            return true;
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="PNG">The image.</param>
        /// <returns></returns>
        public static bool UploadImage(Image aPNG)
        {
            string Filename = Misc.GetTempFileName(".png");
            aPNG.Save(Filename);
            return Uploader.Upload(new[] { Filename });
        }

        /// <summary>
        /// Upload a textfile.
        /// </summary>
        /// <param name="Text">The text.</param>
        /// <returns></returns>
        public static bool UploadText(string aText)
        {
            string Filename = Misc.GetTempFileName(".txt");
            File.WriteAllText(Filename, aText);
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
        /// The BackgroundWorker for the upload
        /// </summary>
        static void UploadWorkerHandler(object sender, DoWorkEventArgs a)
        {
            MessageBox.Show(Settings.Default.Count.ToString());
            foreach (KeyValuePair<string, string> file in filesDictionary)
            {
                bool abort = false;

                string path = file.Key;
                string filename = file.Value;

                switch (Settings.Default.TransferType)
                {

                    case 0:
                    case 3:
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

                            if (Settings.Default.TransferType == 3)
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
                                    if (uploadWorker.CancellationPending) return;

                                    outputStream.Write(Buffer, 0, readBytesCount);
                                    totalReadBytesCount += readBytesCount;

                                    uploadWorker.ReportProgress((int)(totalReadBytesCount * 100 / inputStream.Length));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            abort = true;
                            uploadSuccess = false;
                            Misc.HandleError(e, "FTP(S)");
                        }

                        break;

                    case 1:
                        try
                        {
                            using (FileStream inputStream = new FileStream(path, FileMode.Open))
                            using (SftpClient outputStream = new SftpClient(Settings.Default.Host, Settings.Default.Port, Settings.Default.Username, Misc.Decrypt(Settings.Default.Password)))
                            {
                                outputStream.Connect();

                                if (!String.IsNullOrEmpty(Settings.Default.Directory)) outputStream.ChangeDirectory(Settings.Default.Directory);

                                IAsyncResult Async = outputStream.BeginUploadFile(inputStream, filename);
                                SftpUploadAsyncResult SFTPAsync = Async as SftpUploadAsyncResult;

                                while (!SFTPAsync.IsCompleted)
                                {
                                    uploadWorker.ReportProgress((int)(SFTPAsync.UploadedBytes * 100 / (ulong)inputStream.Length));
                                }

                                outputStream.EndUploadFile(Async);
                            }
                        }
                        catch (Exception e)
                        {
                            abort = true;
                            uploadSuccess = false;
                            Misc.HandleError(e, "SFTP");
                        }

                        break;

                    case 2:
                        try
                        {
                            byte[] Buffer = new byte[1024 * 10];

                            IntPtr Token = IntPtr.Zero;
                            Misc.LogonUser(Settings.Default.Username, "NFU", Misc.Decrypt(Settings.Default.Password), 9, 0, ref Token);
                            WindowsIdentity Identity = new WindowsIdentity(Token);

                            string DestPath = (Settings.Default.Directory != null) ? String.Format(@"\\{0}\{1}\{2}", Settings.Default.Host, Settings.Default.Directory, filename) : String.Format(@"\\{0}\{1}", Settings.Default.Host, filename);

                            using (Identity.Impersonate())
                            using (FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                            using (FileStream outputStream = new FileStream(DestPath, FileMode.CreateNew, FileAccess.Write))
                            {
                                long FileLength = inputStream.Length;
                                long TotalBytes = 0;
                                int CurrentBlockSize;

                                while ((CurrentBlockSize = inputStream.Read(Buffer, 0, Buffer.Length)) > 0)
                                {
                                    if (uploadWorker.CancellationPending) return;

                                    outputStream.Write(Buffer, 0, CurrentBlockSize);
                                    TotalBytes += CurrentBlockSize;

                                    uploadWorker.ReportProgress((int)(TotalBytes * 100 / FileLength));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            abort = true;
                            uploadSuccess = false;
                            Misc.HandleError(e, "CIFS");
                        }

                        break;
                }

                if (abort)
                    break;

                currentIndex++;
            }
        }

        /// <summary>
        /// The progress handler for the upload
        /// </summary>
        static void UploadWorkerProgressHandler(object sender, ProgressChangedEventArgs e)
        {
            Program.CoreForm.progressUpload.Value = e.ProgressPercentage;
            Program.CoreForm.toolStripStatus.Text = string.Format("Uploading... ({0}/{1})", currentIndex, filesDictionary.Count);
        }

        /// <summary>
        /// The handler for the completion of the upload
        /// </summary>
        static void UploadWorkerCompletedHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            Misc.SetControlStatus(true);
            Program.CoreForm.progressUpload.Value = 0;

            if (uploadSuccess)
            {
                Misc.ShowInfo("NFU upload successful", "The file has been successfully uploaded");
                Program.CoreForm.toolStripStatus.Text = "Upload successful";

                List<string> clipboard = new List<string>();

                foreach (KeyValuePair<string, string> file in filesDictionary)
                    clipboard.Add(Settings.Default.URL + file.Value);

                Clipboard.SetText(String.Join(Environment.NewLine, clipboard));
            }

            isBusy = false;
        }
    }
}