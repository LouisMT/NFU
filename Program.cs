using NFU.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace NFU
{
    public enum TransferType
    {
        FTP,
        FTPSExplicit,
        SFTP,
        SFTPKeys,
        CIFS
    }

    static class Program
    {
        public static Core formCore;
        public static About formAbout;
        public static CP formCP;

        private static Mutex mutex = new Mutex(true, "NFU {537e6f56-11cb-461a-9983-634307543f5b}");

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Uncomment the following lines to change the UI culture in order to test translations
            //Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("nl-NL");
            //Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("nl-NL");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                if (mutex.WaitOne(TimeSpan.Zero, true))
                {
                    if (Settings.Default.NeedsUpgrade)
                    {
                        Settings.Default.Upgrade();
                        Settings.Default.NeedsUpgrade = false;
                        Settings.Default.Save();
                    }

                    if (Screen.AllScreens.Length > Settings.Default.Screen - 1)
                        Settings.Default.Screen = 0;

                    Settings.Default.Save();

                    formCore = new Core();
                    formAbout = new About();
                    formCP = new CP();

                    formCore.Setup();

                    if (Settings.Default.FirstRun) formCore.Load += (sender, e) =>
                        {
                            Program.formCP.ShowDialog();
                        };

                    if (args.Length > 0)
                        if (args[0] == "minimized")
                            formCore.WindowState = FormWindowState.Minimized;

                    Application.Run(formCore);
                }
                else
                {
                    MessageBox.Show(Resources.Program_AlreadyRunning,
                        Resources.Program_AlreadyRunningTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
