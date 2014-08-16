using Microsoft.Win32;
using NFU.Properties;
using System;
using System.Windows.Forms;

namespace NFU
{
    public partial class CP : Form
    {
        private readonly RegistryKey autoStartKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

        public CP()
        {
            InitializeComponent();

            comboBoxScreen.Items.Add("Merge screens");

            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                comboBoxScreen.Items.Add(String.Format("Screen {0}", i + 1));
            }

            comboBoxType.SelectedIndex = Settings.Default.TransferType;
            textBoxHost.Text = Settings.Default.Host;
            numericUpDownPort.Value = Settings.Default.Port;
            textBoxDirectory.Text = Settings.Default.Directory;
            textBoxUsername.Text = Settings.Default.Username;
            textBoxPassword.Text = Misc.Decrypt(Settings.Default.Password);
            textBoxURL.Text = Settings.Default.URL;

            comboBoxScreen.SelectedIndex = Settings.Default.Screen;
            checkBoxPause.Checked = Settings.Default.HandlePause;
            checkBoxPrintScreen.Checked = Settings.Default.HandlePrintScreen;
            checkBoxQuickScreenshots.Checked = Settings.Default.QuickScreenshots;
            checkBoxDebug.Checked = Settings.Default.Debug;
            checkBoxSytemTray.Checked = Settings.Default.MinimizeSystemTray;

            try
            {
                checkBoxStartWindows.Checked = autoStartKey.GetValue("NFU") != null;
            }
            catch { }

            comboBoxFilename.SelectedIndex = Settings.Default.Filename;
        }

        private void buttonSaveHandler(object sender, EventArgs e)
        {
            Settings.Default.TransferType = comboBoxType.SelectedIndex;
            Settings.Default.Host = textBoxHost.Text;
            Settings.Default.Port = (int)numericUpDownPort.Value;
            Settings.Default.Directory = textBoxDirectory.Text;
            Settings.Default.Username = textBoxUsername.Text;
            Settings.Default.Password = Misc.Encrypt(textBoxPassword.Text);
            Settings.Default.URL = textBoxURL.Text;

            Settings.Default.Screen = comboBoxScreen.SelectedIndex;
            Settings.Default.HandlePause = checkBoxPause.Checked;
            Settings.Default.HandlePrintScreen = checkBoxPrintScreen.Checked;
            Settings.Default.QuickScreenshots = checkBoxQuickScreenshots.Checked;
            Settings.Default.Debug = checkBoxDebug.Checked;
            Settings.Default.MinimizeSystemTray = checkBoxSytemTray.Checked;

            try
            {
                if (checkBoxStartWindows.Checked) autoStartKey.SetValue("NFU", String.Format("\"{0}\" {1}", Application.ExecutablePath, "minimized")); else autoStartKey.DeleteValue("NFU");
            }
            catch { }

            Settings.Default.Filename = comboBoxFilename.SelectedIndex;

            if (Settings.Default.FirstRun) Settings.Default.FirstRun = false;

            Settings.Default.Save();

            Close();
        }

        private void updateOnShown(object sender, EventArgs e)
        {
            labelCounter.Text = String.Format("Counter: {0}", Settings.Default.Count.ToString("D5"));
        }

        private void checkBoxShowPasswordHandler(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }

        private void settingsHelperHandler(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string infoTitle = "", infoText = "";

            switch (textBox.Name)
            {
                case "textBoxHost":
                    infoTitle = "Host in FQDN format";
                    infoText = "example.com";
                    break;

                case "textBoxUsername":
                    infoTitle = "Username for the server, empty for anonymous";
                    infoText = "root";
                    break;

                case "textBoxPassword":
                    infoTitle = "Password for the server, empty for anonymous";
                    infoText = "example123!";
                    break;

                case "textBoxDirectory":
                    infoTitle = "Absolute path for SFTP, relative path for FTP and CIFS";
                    infoText = "/home/example.com/files (absolute) example.com/files (relative)";
                    break;

                case "textBoxURL":
                    infoTitle = "Public URL pointing to your upload folder";
                    infoText = "http://example.com/files/";
                    break;
            }

            labelHelpTitle.Text = infoTitle;
            labelHelpText.Text = infoText;
        }

        private void settingsHelperClear(object sender, EventArgs e)
        {
            labelHelpTitle.Text = "Warning";
            labelHelpText.Text = "Some settings may only take effect after a restart";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.Count = 0;
            labelCounter.Text = "Counter: 00000";
        }

        private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case 0:
                case 3:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 21;
                    break;

                case 1:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 22;
                    break;

                case 2:
                    numericUpDownPort.Enabled = false;
                    numericUpDownPort.Value = 0;
                    break;
            }
        }
    }
}