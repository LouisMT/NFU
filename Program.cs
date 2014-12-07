using NFU.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace NFU
{
    public enum TransferType
    {
        Ftp,
        FtpsExplicit,
        Sftp,
        SftpKeys,
        Cifs
    }

    static class Program
    {
        public static Core FormCore;
        public static About FormAbout;
        public static Cp FormCp;

        private static readonly Mutex Mutex = new Mutex(true, "NFU {537e6f56-11cb-461a-9983-634307543f5b}");

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
                if (Mutex.WaitOne(TimeSpan.Zero, true))
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

                    FormCore = new Core();
                    FormAbout = new About();
                    FormCp = new Cp();

                    FormCore.Setup();

                    if (Settings.Default.FirstRun) FormCore.Load += (sender, e) =>
                        {
                            FormCp.ShowDialog();
                        };

                    if (args.Length > 0)
                        if (args[0] == "minimized")
                            FormCore.WindowState = FormWindowState.Minimized;

                    Application.Run(FormCore);
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
