using NFU.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace NFU
{
    static class Program
    {
        public static readonly Mutex Mtx = new Mutex(true, "NFU {537e6f56-11cb-461a-9983-634307543f5b}");

        public static Core CoreForm;
        public static About AboutBox;
        public static CP ControlPanel;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                if (Mtx.WaitOne(TimeSpan.Zero, true))
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    CoreForm = new Core();
                    AboutBox = new About();
                    ControlPanel = new CP();

	                CoreForm.Setup();

                    if (Screen.AllScreens.Length > Settings.Default.Screen - 1) Settings.Default.Screen = 0;

                    if (Settings.Default.FirstRun) CoreForm.Load += (sender, e) =>
                        {
                            Program.ControlPanel.ShowDialog();
                        };

                    if (args.Length > 0)
                    {
                        if (args[0] == "minimized") CoreForm.WindowState = FormWindowState.Minimized;
                    }

                    Application.Run(CoreForm);
                }
                else
                {
                    MessageBox.Show("NFU is already running. Please close the other instance of NFU.", "NFU Already Running", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
