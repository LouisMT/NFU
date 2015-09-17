using Microsoft.Win32;
using Nfu.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Nfu
{
    public partial class Cp : Form
    {
        private readonly RegistryKey _autoStartKey = Registry.CurrentUser.OpenSubKey(Settings.Default.RunSubKey, true);

        /// <summary>
        /// Read all the settings back into the GUI.
        /// </summary>
        public Cp()
        {
            InitializeComponent();

            SetDebugButtonState();

            comboBoxScreen.Items.Add(Resources.MergeScreens);

            for (var i = 0; i < Screen.AllScreens.Length; i++)
                comboBoxScreen.Items.Add(string.Format(Resources.Screen, i + 1));

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
            checkBoxSytemTray.Checked = Settings.Default.MinimizeSystemTray;

            try
            {
                checkBoxStartWindows.Checked = _autoStartKey.GetValue(Settings.Default.RunKey) != null;
            }
            catch
            {
                Misc.HandleError(new Exception(Resources.NoRunKey), Resources.Settings, false);
            }

            comboBoxFilename.SelectedIndex = Settings.Default.Filename;
            textBoxGeneratedFileNamePattern.Text = Settings.Default.GeneratedFileNamePattern;

            textBoxWebHookUrl.Text = Settings.Default.WebHookUrl;
            textBoxWebHookSecret.Text = Misc.Decrypt(Settings.Default.WebHookSecret);
            checkBoxEnableWebHook.Checked = Settings.Default.EnableWebHook;
        }

        /// <summary>
        /// Reload the file counter.
        /// </summary>
        private void CpShown(object sender, EventArgs e)
        {
            labelCounter.Text = string.Format(Resources.Counter, Settings.Default.Count.ToString("D5"));
        }

        private void ButtonSave(object sender, EventArgs e)
        {
            // Don't automatically save settings now
            Program.AutoSaveSettings = false;

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
            Settings.Default.MinimizeSystemTray = checkBoxSytemTray.Checked;

            try
            {
                if (checkBoxStartWindows.Checked)
                {
                    _autoStartKey.SetValue(Settings.Default.RunKey,
                        $"\"{Application.ExecutablePath}\" {"minimized"}");
                }
                else
                {
                    _autoStartKey.DeleteValue(Settings.Default.RunKey);
                }
            }
            catch
            {
                Misc.HandleError(new Exception(Resources.RunKeyChangeFailed), Resources.Settings, false);
            }

            Settings.Default.Filename = comboBoxFilename.SelectedIndex;
            Settings.Default.GeneratedFileNamePattern = textBoxGeneratedFileNamePattern.Text;

            // TODO: Validate URL
            Settings.Default.WebHookUrl = textBoxWebHookUrl.Text;
            Settings.Default.WebHookSecret = Misc.Encrypt(textBoxWebHookSecret.Text);
            Settings.Default.EnableWebHook = checkBoxEnableWebHook.Checked;

            if (Settings.Default.FirstRun) Settings.Default.FirstRun = false;

            Program.AutoSaveSettings = true;
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
            var textBox = (TextBox)sender;
            string infoTitle = "", infoText = "";

            switch (textBox.Name)
            {
                case "textBoxHost":
                    infoTitle = Resources.HostFqdnFormat;
                    infoText = Resources.HostFqdnFormatExample;
                    break;

                case "textBoxUsername":
                    infoTitle = Resources.ServerUserName;
                    infoText = Resources.ServerUserNameExample;
                    break;

                case "textBoxPassword":
                    if (comboBoxType.SelectedIndex == (int)TransferType.SftpKeys)
                    {
                        infoTitle = Resources.ServerKey;
                        infoText = Resources.ServerKeyExample;
                    }
                    else
                    {
                        infoTitle = Resources.ServerPassword;
                        infoText = Resources.ServerPasswordExample;
                    }
                    break;

                case "textBoxDirectory":
                    infoTitle = Resources.ServerDirectory;
                    infoText = Resources.ServerDirectoryExample;
                    break;

                case "textBoxURL":
                    infoTitle = Resources.ServerPublicUrl;
                    infoText = Resources.ServerPublicUrlExample;
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
            labelHelpTitle.Text = Resources.Warning;
            labelHelpText.Text = Resources.EffectAfterRestart;
        }

        /// <summary>
        /// Reset the file counter.
        /// </summary>
        private void ResetCounter(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Settings.Default.Count = 0;
            labelCounter.Text = string.Format(Resources.Counter, "00000");
        }

        /// <summary>
        /// Set the default port for the selected protocol.
        /// </summary>
        private void TypeIndexChanged(object sender, EventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case (int)TransferType.Ftp:
                case (int)TransferType.FtpsExplicit:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 21;
                    labelPassword.Text = Resources.Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;

                case (int)TransferType.Sftp:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 22;
                    labelPassword.Text = Resources.Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;

                case (int)TransferType.SftpKeys:
                    numericUpDownPort.Enabled = true;
                    numericUpDownPort.Value = 22;
                    labelPassword.Text = Resources.KeyPath;
                    checkBoxShowPassword.Enabled = false;
                    textBoxPassword.UseSystemPasswordChar = false;
                    break;

                case (int)TransferType.Cifs:
                    numericUpDownPort.Enabled = false;
                    numericUpDownPort.Value = 0;
                    labelPassword.Text = Resources.Password;
                    checkBoxShowPassword.Enabled = true;
                    textBoxPassword.UseSystemPasswordChar = true;
                    break;
            }
        }

        /// <summary>
        /// Set the state of the debug button.
        /// </summary>
        private void SetDebugButtonState()
        {
            buttonDebug.FlatStyle = FlatStyle.System;

            if (Settings.Default.Debug)
            {
                // Disable UAC shield
                Misc.SendMessage(buttonDebug.Handle, 0x160C, 0, 0x0);
                buttonDebug.Text = Resources.DisableDebug;
            }
            else
            {
                // Enable UAC shield
                Misc.SendMessage(buttonDebug.Handle, 0x160C, 0, 0xFFFFFFFF);
                buttonDebug.Text = Resources.EnableDebug;
            }
        }

        /// <summary>
        /// Enable or disable debug mode.
        /// </summary>
        private void EnableDisableDebug(object sender, EventArgs e)
        {
            if (Settings.Default.Debug)
            {
                Settings.Default.Debug = false;
            }
            else
            {
                var startInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = Application.ExecutablePath,
                    Verb = "runas",
                    Arguments = "debug"
                };

                var process = new Process
                {
                    StartInfo = startInfo,
                    EnableRaisingEvents = true
                };

                process.Start();
                process.WaitForExit();

                // Reload settings to check if debug
                // mode has been successfully enabled
                Settings.Default.Reload();
            }

            SetDebugButtonState();
        }

        private void OpenSettingsGoneUrl(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.SettingsGoneUrl);
        }
    }
}
