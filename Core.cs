using System.Linq;
using NFU.Properties;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NFU
{
    public partial class Core : Form
    {
        public Core()
        {
            InitializeComponent();
        }

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

        private void Core_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized && Settings.Default.MinimizeSystemTray)
            {
                if (!Settings.Default.TooltipShown)
                {
                    Misc.ShowInfo("NFU is still active", "Double click on this icon to open NFU");
                    Settings.Default.TooltipShown = true;
                }

                this.Hide();
            }
        }
        private void Core_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
            }
            else
            {
                Settings.Default.Save();
            }
        }

        private void notifyIconNFU_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();

            this.WindowState = FormWindowState.Normal;
        }

        private void HotKeyPrintScreen()
        {
            buttonScreenshotHandler(null, null);
        }

        private void HotKeyPause()
        {
            buttonImportHandler(null, null);
        }

        private async void buttonFileHandler(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                // copy this list because it may change while uploading
                var files = openFileDialog.FileNames.ToList();

                TaskCompletionSource<bool> tcs = null;
                EventHandler onComplete = (o, args) => tcs.TrySetResult(true);
                Uploader.UploadCompleted += onComplete;

                for (int index = 0; index < files.Count; index++)
                {
                    var file = files[index];
                    tcs = new TaskCompletionSource<bool>();
                    Uploader.Upload(file, index + 1, files.Count);
                    await tcs.Task;
                }

                Uploader.UploadCompleted -= onComplete;
            }
        }

        private void buttonScreenshotHandler(object sender, EventArgs e)
        {
            if (Snipper.isActive) return;

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

        private async void buttonImportHandler(object sender, EventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                // copy this list because it may change while uploading
                var files = Clipboard.GetFileDropList();

                TaskCompletionSource<bool> tcs = null;
                EventHandler onComplete = (o, args) => tcs.TrySetResult(true);
                Uploader.UploadCompleted += onComplete;

                for (int index = 0; index < files.Count; index++)
                {
                    var file = files[index];
                    tcs = new TaskCompletionSource<bool>();
                    Uploader.Upload(file, index + 1, files.Count);
                    await tcs.Task;
                }

                Uploader.UploadCompleted -= onComplete;
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
                Misc.HandleError(new ArgumentException("Cannot handle clipboard content of type(s) " + string.Join(",", Clipboard.GetDataObject().GetFormats())), "Import");
            }
        }

        private void openSettingsHandler(object sender, EventArgs e)
        {
            Program.ControlPanel.ShowDialog();
        }

        private void openAboutHandler(object sender, EventArgs e)
        {
            Program.AboutBox.ShowDialog();
        }

        private void exitNFUHandler(object sender, EventArgs e)
        {
            Application.Exit();
        }

        static readonly string currentVersion = "v1.2\n";
        static WebClient checkVersion = new WebClient();

        private async void Core_Shown(object sender, EventArgs e)
        {
            await Task.Run(() => CheckForUpdate());
        }

        private async Task CheckForUpdate()
        {
            try
            {
                string latestVersion = await checkVersion.DownloadStringTaskAsync(new Uri("https://u5r.nl/nfu/latest"));

                if (latestVersion != currentVersion)
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

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string tempNFU = Path.GetTempFileName();
                string tempCMD = Path.GetTempFileName() + ".cmd";

                checkVersion.DownloadFile("https://u5r.nl/NFU/NFU.exe", tempNFU);
                File.WriteAllText(tempCMD, String.Format("@ECHO OFF{0}TITLE NFU UPDATE{0}ECHO Waiting for NFU to exit...{0}TIMEOUT /T 5{0}ECHO.{0}ECHO Updating NFU...{0}COPY /B /Y \"{1}\" \"{2}\"{0}START \"\" \"{2}\"", Environment.NewLine, tempNFU, Application.ExecutablePath));

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
