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

            buttonUpdate.FlatStyle = FlatStyle.System;
            Misc.SendMessage(buttonUpdate.Handle, 0x160C, 0, 0xFFFFFFFF);

            if (Settings.Default.HandlePause) Misc.RegisterHotKey(Keys.Pause, HotKeyPause, 10000, Handle);
            if (Settings.Default.HandlePrintScreen) Misc.RegisterHotKey(Keys.PrintScreen, HotKeyPrintScreen, 20000, Handle);
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
            if(e.CloseReason == CloseReason.UserClosing)
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

        private void buttonFileHandler(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Uploader.Upload(openFileDialog.FileName);
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
                Misc.HandleError(null, "Screenshot");
            }
        }

        private void buttonImportHandler(object sender, EventArgs e)
        {
            if (Clipboard.ContainsFileDropList())
            {
                if (Clipboard.GetFileDropList().Count > 1)
                {

                    System.Collections.Specialized.StringCollection jwz = Clipboard.GetFileDropList();
                        
                    Task.Factory.StartNew(() =>
                        {

                            string zipFile = Path.GetTempFileName() + ".zip";

                            this.Invoke(new MethodInvoker(delegate  {progressUpload.Style = ProgressBarStyle.Marquee;} ));
                            
                            using (ZipArchive zip = ZipFile.Open(zipFile, ZipArchiveMode.Create))
                            {
                                foreach (string file in jwz)
                                {
                                    zip.CreateEntryFromFile(file, Path.GetFileName(file));
                                }
                            }
                            this.Invoke(new MethodInvoker(delegate { progressUpload.Style = ProgressBarStyle.Continuous; }));
                            Process.Start(zipFile);

                            this.Invoke(new MethodInvoker(delegate { Uploader.Upload(zipFile); }));
                        }
                    );

                    
                }
                else
                {
                    Uploader.Upload(Clipboard.GetFileDropList()[0]);
                }
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
                Misc.HandleError(null, "Import");
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

        private void Core_Shown(object sender, EventArgs e)
        {
            try
            {
                string latestVersion = checkVersion.DownloadString("https://u5r.nl/nfu/latest");

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
