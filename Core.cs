using NFU.Properties;
using System;
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
                MessageBox.Show("Pause hotkey could not be registered. Is it already in use?",
                    "Failed to register hotkey", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (Settings.Default.HandlePrintScreen && !Misc.RegisterHotKey(Keys.PrintScreen, HotKeyPrintScreen, 20000, Handle))
            {
                MessageBox.Show("PrintScreen hotkey could not be registered. Is it already in use?",
                    "Failed to register hotkey", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    Misc.ShowInfo("NFU is still active", "Click on this icon to open NFU or right click to exit.");
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
                Uploader.Upload(openFileDialog.FileNames);
        }

        /// <summary>
        /// Take a screenshot to upload.
        /// </summary>
        private void ButtonScreenshot(object sender, EventArgs e)
        {
            if (Snipper.isActive)
                return;

            Image PNG = Snipper.Snip();

            if (PNG != null)
            {
                Uploader.UploadImage(PNG);
            }
            else
            {
                Misc.HandleError(new ArgumentException("No files selected"), "Screenshot");
            }
        }

        /// <summary>
        /// Import data from the clipboard to upload.
        /// </summary>
        private void ButtonImport(object sender, EventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                string[] files = new string[Clipboard.GetFileDropList().Count];
                Clipboard.GetFileDropList().CopyTo(files, 0);
                Uploader.Upload(files);
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
                Misc.HandleError(new ArgumentException("Cannot handle clipboard content of type(s) " +
                    string.Join(",", Clipboard.GetDataObject().GetFormats())), "Import");
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
                string latestVersion = await updateClient.DownloadStringTaskAsync(new Uri("https://u5r.nl/nfu/latest"));

                if (latestVersion != Application.ProductVersion)
                {
                    buttonUpdate.Enabled = true;
                    labelUpdate.Text = "A new version of NFU is available";
                    Misc.ShowInfo("NFU update available", "There is an update available for NFU");
                }
            }
            catch (Exception err)
            {
                Misc.HandleError(err, "Update Check");
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

                updateClient.DownloadFile("https://u5r.nl/nfu/NFU.exe", tempNFU);
                File.WriteAllText(tempCMD, String.Format("@ECHO OFF{0}TITLE NFU UPDATE{0}ECHO Waiting for NFU to exit...{0}TIMEOUT /T 5{0}ECHO." +
                    "{0}ECHO Updating NFU...{0}COPY /B /Y \"{1}\" \"{2}\"{0}START \"\" \"{2}\"", Environment.NewLine, tempNFU, Application.ExecutablePath));

                ProcessStartInfo startInfo = new ProcessStartInfo();

                startInfo.UseShellExecute = true;
                startInfo.FileName = tempCMD;
                startInfo.Verb = "runas";

                Process.Start(startInfo);

                Application.Exit();
            }
            catch (Exception err)
            {
                Misc.HandleError(err, "Update");
            }
        }
    }
}
