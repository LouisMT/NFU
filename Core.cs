using NFU.Properties;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFU
{
    public partial class Core : Form
    {
        private WebClient updateClient = new WebClient();
        private Image screenshot;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Core()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Constructor extension.
        /// </summary>
        public void Setup()
        {
            buttonUpdate.FlatStyle = FlatStyle.System;
            Misc.SendMessage(buttonUpdate.Handle, 0x160C, 0, 0xFFFFFFFF);

            if (Settings.Default.HandlePause && !Misc.RegisterHotKey(Keys.Pause, HotKeyPause, 10000, Handle))
            {
                MessageBox.Show(Resources.Core_PauseHotKeyNotRegistered,
                    Resources.Core_HotKeyNotRegisteredTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (Settings.Default.HandlePrintScreen && !Misc.RegisterHotKey(Keys.PrintScreen, HotKeyPrintScreen, 20000, Handle))
            {
                MessageBox.Show(Resources.Core_PrintScreenHotKeyNotRegistered,
                    Resources.Core_HotKeyNotRegisteredTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Start the update check.
        /// </summary>
        private async void CoreShown(object sender, EventArgs e)
        {
            await Task.Run(() => CheckForUpdate());
        }

        /// <summary>
        /// Minimize to system tray.
        /// </summary>
        private void CoreResize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && Settings.Default.MinimizeSystemTray)
            {
                if (!Settings.Default.TooltipShown)
                {
                    Misc.ShowInfo(Resources.Core_StillActiveTitle, Resources.Core_StillActive);
                    Settings.Default.TooltipShown = true;
                    Settings.Default.Save();
                }

                this.Hide();
            }
        }

        /// <summary>
        /// Minimize instead of close.
        /// </summary>
        private void CoreFormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
        }

        /// <summary>
        /// Restore window using System Tray.
        /// </summary>
        private void NotifyIconNFUClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
            }
            else
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
            }
        }


        /// <summary>
        /// Handler for the PrintScreen key.
        /// </summary>
        private void HotKeyPrintScreen()
        {
            ButtonScreenshot(null, null);
        }

        /// <summary>
        /// Handler for the Pause key.
        /// </summary>
        private void HotKeyPause()
        {
            ButtonImport(null, null);
        }

        /// <summary>
        /// Select file(s) to upload.
        /// </summary>
        private void ButtonFile(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<UploadFile> uploadFiles = new List<UploadFile>();

                foreach (string fileName in openFileDialog.FileNames)
                {
                    uploadFiles.Add(new UploadFile
                    {
                        Path = fileName
                    });
                }

                Uploader.Upload(uploadFiles.ToArray());
            }
        }

        /// <summary>
        /// Take a screenshot to upload.
        /// </summary>
        private void ButtonScreenshot(object sender, EventArgs e)
        {
            if (Snipper.isActive)
                return;

            screenshot = Snipper.Snip();

            if (screenshot != null)
            {
                reuploadScreenshotToolStripMenuItem.Enabled = true;

                switch (Snipper.returnType)
                {
                    case Snipper.returnTypes.Default:
                        Uploader.UploadImage(screenshot);
                        break;

                    case Snipper.returnTypes.ToClipboard:
                        Clipboard.SetImage(screenshot);
                        Misc.ShowInfo(Resources.Core_ScreenShotCopiedTitle, Resources.Core_ScreenShotCopied);
                        break;
                }
            }
            else
            {
                Misc.HandleError(new ArgumentException(Resources.Core_NoFilesSelected), Resources.Core_ScreenShot);
            }
        }

        /// <summary>
        /// Reupload the last screenshot.
        /// </summary>
        private void ReuploadScreenshot(object sender, EventArgs e)
        {
            Uploader.UploadImage(screenshot);
        }

        /// <summary>
        /// Import data from the clipboard to upload.
        /// </summary>
        private void ButtonImport(object sender, EventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                StringCollection files = Clipboard.GetFileDropList();
                List<UploadFile> uploadFiles = new List<UploadFile>();

                foreach (string file in files)
                {
                    uploadFiles.Add(new UploadFile
                    {
                        Path = file
                    });
                }

                Uploader.Upload(uploadFiles.ToArray());
            }
            else if (Clipboard.ContainsImage())
            {
                Uploader.UploadImage(Clipboard.GetImage());
            }
            else if (Clipboard.ContainsText())
            {
                Uploader.UploadText(Clipboard.GetText());
            }
            else
            {
                Misc.HandleError(new ArgumentException(String.Format(Resources.Core_CannotHandleTypes,
                    String.Join(",", Clipboard.GetDataObject().GetFormats()))), Resources.Core_Import);
            }
        }

        /// <summary>
        /// Open the settings dialog.
        /// </summary>
        private void OpenSettings(object sender, EventArgs e)
        {
            Program.formCP.ShowDialog();
        }

        /// <summary>
        /// Open the about dialog.
        /// </summary>
        private void OpenAbout(object sender, EventArgs e)
        {
            Program.formAbout.ShowDialog();
        }

        /// <summary>
        /// Exit NFU.
        /// </summary>
        private void ExitNFU(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Check for NFU updates.
        /// </summary>
        private async Task CheckForUpdate()
        {
            try
            {
                string latestVersion = await updateClient.DownloadStringTaskAsync(new Uri(Settings.Default.VersionUrl));

                if (latestVersion != Application.ProductVersion)
                {
                    buttonUpdate.Enabled = true;
                    labelUpdate.Text = Resources.Core_NewVersion;
                    Misc.ShowInfo(Resources.Core_UpdateAvailableTitle, Resources.Core_UpdateAvailable);
                }
            }
            catch (Exception err)
            {
                Misc.HandleError(err, Resources.Core_UpdateCheck);
            }
        }

        /// <summary>
        /// Update NFU.
        /// </summary>
        private void StartUpdate(object sender, EventArgs e)
        {
            try
            {
                string tempNFU = Path.GetTempFileName();
                string tempCMD = Path.GetTempFileName() + ".cmd";

                updateClient.DownloadFile(Settings.Default.ExecutableUrl, tempNFU);
                File.WriteAllText(tempCMD, String.Format("@ECHO OFF{3}TITLE {0}{3}ECHO {1}{3}TIMEOUT /T 5{3}ECHO." +
                    "{3}ECHO {2}{3}COPY /B /Y \"{4}\" \"{5}\"{3}START \"\" \"{5}\"", Resources.Core_UpdateTitle, Resources.Core_WaitingToExit, Resources.Core_Updating, Environment.NewLine, tempNFU, Application.ExecutablePath));

                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.UseShellExecute = true;
                startInfo.FileName = tempCMD;
                startInfo.Verb = "runas";

                Process.Start(startInfo);

                Application.Exit();
            }
            catch (Exception err)
            {
                Misc.HandleError(err, Resources.Core_Update);
            }
        }
    }
}
