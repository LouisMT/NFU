using Microsoft.Win32;
using NFU.Properties;
using System;
using System.Windows.Forms;

namespace NFU
{
    public partial class CP : Form
    {
        private RegistryKey autoStartKey = Registry.CurrentUser.OpenSubKey(Settings.Default.RunSubKey, true);

        /// <summary>
        /// Read all the settings back into the GUI.
        /// </summary>
        public CP()
        {
            InitializeComponent();

            comboBoxScreen.Items.Add(Resources.CP_MergeScreens);

            for (int i = 0; i < Screen.AllScreens.Length; i++)
                comboBoxScreen.Items.Add(String.Format(Resources.CP_Screen, i + 1));

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
                checkBoxStartWindows.Checked = autoStartKey.GetValue(Settings.Default.RunKey) != null;
            }
            catch { }

            comboBoxFilename.SelectedIndex = Settings.Default.Filename;
        }

        /// <summary>
        /// Reload the file counter.
        /// </summary>
        private void CPShown(object sender, EventArgs e)
        {
            labelCounter.Text = String.Format(Resources.CP_Counter, Settings.Default.Count.ToString("D5"));
        }

        private void ButtonSave(object sender, EventArgs e)
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
                if (checkBoxStartWindows.Checked)
                {
                    autoStartKey.SetValue(Settings.Default.RunKey, String.Format("\"{0}\" {1}", Application.ExecutablePath, "minimized"));
                }
                else
                {
                    autoStartKey.DeleteValue(Settings.Default.RunKey);
                }
            }
            catch { }

            Settings.Default.Filename = comboBoxFilename.SelectedIndex;

            if (Settings.Default.FirstRun) Settings.Default.FirstRun = false;

            Settings.Default.Save();

            Close();
        }

        /// <summary>
        /// Show or hide the password.
        /// </summary>
        private void CheckBoxShowPassword(object sender, EventArgs e)
        {
            textBoxPassword.UseSystemPasswordChar = !checkBoxShowPassword.Checked;
        }

        /// <summary>
        /// Show help text if available.
        /// </summary>
        private void SettingsHelper(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            string infoTitle = "", infoText = "";

            switch (textBox.Name)
            {
                case "textBoxHost":
                    infoTitle = Resources.CP_HostFqdnFormat;
                    infoText = Resources.CP_HostFqdnFormatExample;
                    break;

                case "textBoxUsername":
                    infoTitle = Resources.CP_ServerUserName;
                    infoText = Resources.CP_ServerUserNameExample;
                    break;

                case "textBoxPassword":
                    if (comboBoxType.SelectedIndex == (int)TransferType.SFTPKeys)
                    {
                        infoTitle = Resources.CP_ServerKey;
                        infoText = Resources.CP_ServerKeyExample;
                    }
                    else
                    {
                        infoTitle = Resources.CP_ServerPassword;
                        infoText = Resources.CP_ServerPasswordExample;
                    }
                    break;

                case "textBoxDirectory":
                    infoTitle = Resources.CP_ServerDirectory;
                    infoText = Resources.CP_ServerDirectoryExample;
                    break;

                case "textBoxURL":
                    infoTitle = Resources.CP_ServerPublicUrl;
                    infoText = Resources.CP_ServerPublicUrlExample;
                    break;
            }

            labelHelpTitle.Text = infoTitle;
            labelHelpText.Text = infoText;
        }

        /// <summary>
        /// Clear the help text.
        /// </summary>
        private void SettingsHelperClear(object sender, EventArgs e)
        {
            labelHelpTitle.Text = Resources.CP_Warning;
            labelHelpText.Text = Resources.CP_EffectAfterRestart;
        }

        /// <summary>
        /// Reset the file counter.
        /// </summary>
        private void ResetCounter(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.Count = 0;
            Settings.Default.Save();
            labelCounter.Text = String.Format(Resources.CP_Counter, "00000");
        }

        /// <summary>
        /// Set the default port for the selected protocol.
        /// </summary>
        private void TypeIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case (int)TransferType.FTP:
                case (int)TransferType.FTPSExplicit:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 21;
                    labelPassword.Text = Resources.CP_Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;

                case (int)TransferType.SFTP:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 22;
                    labelPassword.Text = Resources.CP_Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;

                case (int)TransferType.SFTPKeys:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 22;
                    labelPassword.Text = Resources.CP_KeyPath;
                    checkBoxShowPassword.Enabled = false;
                    textBoxPassword.UseSystemPasswordChar = false;
                    break;

                case (int)TransferType.CIFS:
                    numericUpDownPort.Enabled = false;
                    numericUpDownPort.Value = 0;
                    labelPassword.Text = Resources.CP_Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;
            }
        }
    }
}
