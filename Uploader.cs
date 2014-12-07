using NFU.Models;
using NFU.Properties;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace NFU
{
    public static class Uploader
    {
        public static bool IsBusy;

        private static string _uploadStatus;
        private static string _currentStatus;
        private static UploadFile[] _uploadFiles;
        private static readonly BackgroundWorker UploadWorker = new BackgroundWorker();

        /// <summary>
        /// Constructor.
        /// </summary>
        static Uploader()
        {
            UploadWorker.WorkerReportsProgress = true;
            UploadWorker.WorkerSupportsCancellation = true;

            UploadWorker.DoWork += UploadWorkerHandler;
            UploadWorker.ProgressChanged += UploadWorkerProgress;
            UploadWorker.RunWorkerCompleted += UploadWorkerCompleted;
        }

        /// <summary>
        /// Upload one or more files to the remote server.
        /// </summary>
        /// <param name="paths">UploadFile array of files to upload.</param>
        /// <returns>True on success; false on failure.</returns>
        public static bool Upload(UploadFile[] paths)
        {
            if (IsBusy)
                return false;

            IsBusy = true;
            _uploadStatus = Resources.Uploader_UploadSuccessfulStatus;
            Misc.SetControlStatus(false);

            _uploadFiles = paths;

            UploadWorker.RunWorkerAsync();

            return true;
        }

        /// <summary>
        /// Upload an image.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <returns></returns>
        public static bool UploadImage(Image image)
        {
            UploadFile file = new UploadFile(UploadFile.Type.Temporary, "png");
            image.Save(file.Path);
            return Upload(new[] { file });
        }

        /// <summary>
        /// Upload a textfile.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static bool UploadText(string text)
        {
            UploadFile file = new UploadFile(UploadFile.Type.Temporary, "txt");
            File.WriteAllText(file.Path, text);
            return Upload(new[] { file });
        }

        /// <summary>
        /// Cancel the current upload.
        /// </summary>
        public static void Cancel()
        {
            UploadWorker.CancelAsync();
        }

        /// <summary>
        /// The BackgroundWorker for the upload.
        /// </summary>
        static void UploadWorkerHandler(object sender, DoWorkEventArgs a)
        {
            int currentIndex = 1;
            bool abort = false;

            foreach (UploadFile file in _uploadFiles)
            {
                if (file.IsDirectory)
                {
                    // This is a directory, zip it first
                    _currentStatus = Resources.Uploader_ZippingDirectory;
                    UploadWorker.ReportProgress(0);

                    string zipFileName = String.Format("{0}.zip", file.FileName);
                    UploadFile zipFile = new UploadFile(UploadFile.Type.Temporary, "zip");

                    // Delete the temporary file first, because ZipFile.CreateFromDirectory
                    // can't write to an existing file
                    File.Delete(zipFile.Path);
                    ZipFile.CreateFromDirectory(file.Path, zipFile.Path);

                    file.Path = zipFile.Path;
                    file.FileName = zipFileName;
                    file.IsTemporary = true;
                    file.IsDirectory = true;
                }

                _currentStatus = String.Format(Resources.Uploader_Uploading, currentIndex, _uploadFiles.Length);
                UploadWorker.ReportProgress(0);

                file.BeforeUpload();

                switch (Settings.Default.TransferType)
                {
                    case (int)TransferType.Ftp:
                    case (int)TransferType.FtpsExplicit:
                        abort = UploadFtp(file);
                        break;

                    case (int)TransferType.Sftp:
                    case (int)TransferType.SftpKeys:
                        try
                        {
                            abort = UploadSftp(file);
                        }
                        catch (Exception err)
                        {
                            MessageBox.Show(Resources.Uploader_SshNetMissing, Resources.Uploader_SshNetMissingTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

                            _uploadStatus = Misc.HandleErrorStatusText(Resources.Uploader_Sftp);
                            Misc.HandleError(err, Resources.Uploader_Sftp);
                            abort = true;
                        }
                        break;

                    case (int)TransferType.Cifs:
                        abort = UploadCifs(file);
                        break;
                }

                file.AfterUpload();

                if (abort)
                    break;

                currentIndex++;
            }

            SendWebHookPayload(!abort);
        }

        /// <summary>
        /// Send the WebHook payload if WebHook is enabled.
        /// </summary>
        /// <param name="success">True if all files were uploaded successfully, otherwise false.</param>
        static void SendWebHookPayload(bool success)
        {
            if (Settings.Default.EnableWebHook)
            {
                _currentStatus = Resources.Uploader_SendingWebHook;
                UploadWorker.ReportProgress(0);

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();

                WebHook webHook = new WebHook
                {
                    Success = success,
                    Directory = Settings.Default.Directory
                };

                foreach (UploadFile file in _uploadFiles)
                {
                    webHook.Files.Add(new WebHookFile
                    {
                        FileName = file.FileName,
                        IsDirectory = file.IsDirectory
                    });
                }

                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                        webClient.UploadString(new Uri(Settings.Default.WebHookUrl), "POST", javaScriptSerializer.Serialize(webHook));
                    }
                    catch (Exception e)
                    {
                        _uploadStatus = String.Format(Resources.Uploader_WebHookFailed, _uploadStatus);
                        Misc.HandleError(e, "WebHook");
                    }
                }
            }
        }

        /// <summary>
        /// Upload a file via FTP(s).
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadFtp(UploadFile file)
        {
            try
            {
                byte[] buffer = new byte[1024 * 10];

                FtpWebRequest ftpRequest;

                if (!String.IsNullOrEmpty(Settings.Default.Directory))
                {
                    ftpRequest = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}/{3}", Settings.Default.Host, Settings.Default.Port, Settings.Default.Directory, file.FileName));
                }
                else
                {
                    ftpRequest = (FtpWebRequest)WebRequest.Create(String.Format("ftp://{0}:{1}/{2}", Settings.Default.Host, Settings.Default.Port, file.FileName));
                }

                if (Settings.Default.TransferType == (int)TransferType.FtpsExplicit)
                {
                    ftpRequest.EnableSsl = true;
                    ServicePointManager.ServerCertificateValidationCallback = Misc.ValidateServerCertificate;
                }

                ftpRequest.KeepAlive = false;
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                ftpRequest.Credentials = new NetworkCredential(Settings.Default.Username, Misc.Decrypt(Settings.Default.Password));

                using (FileStream inputStream = File.OpenRead(file.Path))
                using (Stream outputStream = ftpRequest.GetRequestStream())
                {
                    long totalReadBytesCount = 0;
                    int readBytesCount;
                    while ((readBytesCount = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (UploadWorker.CancellationPending) return true;

                        outputStream.Write(buffer, 0, readBytesCount);
                        totalReadBytesCount += readBytesCount;

                        UploadWorker.ReportProgress((int)(totalReadBytesCount * 100 / inputStream.Length));
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                _uploadStatus = Misc.HandleErrorStatusText(Resources.Uploader_Ftps);
                Misc.HandleError(e, Resources.Uploader_Ftps);
                return true;
            }
        }

        /// <summary>
        /// Upload a file via SFTP.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadSftp(UploadFile file)
        {
            try
            {
                 SftpClient client;

                if (Settings.Default.TransferType == (int) TransferType.SftpKeys)
                {
                    client = new SftpClient(Settings.Default.Host, Settings.Default.Port, Settings.Default.Username, new PrivateKeyFile(Misc.Decrypt(Settings.Default.Password)));
                }
                else
                {
                    client = new SftpClient(Settings.Default.Host, Settings.Default.Port, Settings.Default.Username, Misc.Decrypt(Settings.Default.Password));
                }

                using (FileStream inputStream = new FileStream(file.Path, FileMode.Open))
                using (SftpClient outputStream = client)
                {
                    outputStream.Connect();

                    if (!String.IsNullOrEmpty(Settings.Default.Directory)) outputStream.ChangeDirectory(Settings.Default.Directory);

                    IAsyncResult async = outputStream.BeginUploadFile(inputStream, file.FileName);
                    SftpUploadAsyncResult sftpAsync = async as SftpUploadAsyncResult;

                    while (sftpAsync != null && !sftpAsync.IsCompleted)
                    {
                        if (UploadWorker.CancellationPending) return true;

                        UploadWorker.ReportProgress((int)(sftpAsync.UploadedBytes * 100 / (ulong)inputStream.Length));
                    }

                    outputStream.EndUploadFile(async);
                }

                return false;
            }
            catch (Exception e)
            {
                _uploadStatus = Misc.HandleErrorStatusText(Resources.Uploader_Sftp);
                Misc.HandleError(e, Resources.CP_Sftp);
                return true;
            }
        }


        /// <summary>
        /// Upload a file via CIFS.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <returns>True on failure, false on success.</returns>
        static bool UploadCifs(UploadFile file)
        {
            try
            {
                byte[] buffer = new byte[1024 * 10];

                IntPtr token = IntPtr.Zero;
                Misc.LogonUser(Settings.Default.Username, Resources.Uploader_Nfu, Misc.Decrypt(Settings.Default.Password), 9, 0, ref token);
                WindowsIdentity identity = new WindowsIdentity(token);

                string destPath = (!String.IsNullOrEmpty(Settings.Default.Directory)) ? String.Format(@"\\{0}\{1}\{2}", Settings.Default.Host, Settings.Default.Directory, file.FileName) : String.Format(@"\\{0}\{1}", Settings.Default.Host, file.FileName);

                using (identity.Impersonate())
                using (FileStream inputStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                using (FileStream outputStream = new FileStream(destPath, FileMode.CreateNew, FileAccess.Write))
                {
                    long fileLength = inputStream.Length;
                    long totalBytes = 0;
                    int currentBlockSize;

                    while ((currentBlockSize = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        if (UploadWorker.CancellationPending) return true;

                        outputStream.Write(buffer, 0, currentBlockSize);
                        totalBytes += currentBlockSize;

                        UploadWorker.ReportProgress((int)(totalBytes * 100 / fileLength));
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                _uploadStatus = Misc.HandleErrorStatusText(Resources.Uploader_Cifs);
                Misc.HandleError(e, Resources.Uploader_Cifs);
                return true;
            }
        }

        /// <summary>
        /// The progress handler for the upload
        /// </summary>
        static void UploadWorkerProgress(object sender, ProgressChangedEventArgs e)
        {
            // Set the progress bar style to marquee if the progress is 0 to indicate something is happening
            Program.FormCore.progressUpload.Style = (e.ProgressPercentage == 0) ?
                ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;

            Program.FormCore.progressUpload.Value = e.ProgressPercentage;
            Program.FormCore.toolStripStatus.Text = _currentStatus;
        }

        /// <summary>
        /// The handler for the completion of the upload
        /// </summary>
        static void UploadWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Misc.SetControlStatus(true);

            Program.FormCore.progressUpload.Style = ProgressBarStyle.Continuous;
            Program.FormCore.progressUpload.Value = 0;

            Program.FormCore.toolStripStatus.Text = _uploadStatus;

            if (_uploadStatus == Resources.Uploader_UploadSuccessfulStatus)
            {
                Misc.ShowInfo(Resources.Uploader_UploadSuccessfulTitle, Resources.Uploader_UploadSuccessful);

                List<string> clipboard = _uploadFiles.Select(file => Settings.Default.URL + file.FileName).ToList();

                Clipboard.SetText(String.Join(Environment.NewLine, clipboard));
            }
            else
            {
                Misc.ShowInfo(Resources.Uploader_UploadFailedTitle, Resources.Uploader_UploadFailed, ToolTipIcon.Error);
            }

            IsBusy = false;
        }
    }
}
