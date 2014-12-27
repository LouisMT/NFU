using Nfu.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Nfu
{
    public sealed class LineSeparator : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LineSeparator()
        {
            Paint += PaintLineSeparator;
            MaximumSize = new Size(2000, 2);
            MinimumSize = new Size(0, 2);
            TabStop = false;
            Width = 350;
        }

        /// <summary>
        /// A simple seperator control.
        /// </summary>
        private void PaintLineSeparator(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawLine(Pens.DarkGray, new Point(0, 0), new Point(Width, 0));
            g.DrawLine(Pens.White, new Point(0, 1), new Point(Width, 1));
        }
    }

    public static class Misc
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr handle);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr handle, int id, int mode, Keys key);

        [DllImport("user32")]
        public static extern UInt32 SendMessage(IntPtr handle, UInt32 msg, UInt32 wParam, UInt32 lParam);

        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string username, string domain, string password, int logonType, int logonProvider, ref IntPtr token);

        private delegate void HotKeyPass();

        private class HotKeyWndProc : NativeWindow
        {
            public HotKeyPass HotKeyPass;
            public int WParam = 10000;

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == 0x0312 && m.WParam.ToInt32() == WParam)
                {
                    if (HotKeyPass != null) HotKeyPass.Invoke();
                }

                base.WndProc(ref m);
            }
        }

        /// <summary>
        /// Bind a hotkey to a method.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="method">The method.</param>
        /// <param name="id">The bind ID.</param>
        /// <param name="handle">The handle.</param>
        public static bool RegisterHotKey(Keys key, Action method, int id, IntPtr handle)
        {
            HotKeyWndProc hotKeyWnd = new HotKeyWndProc();

            if (!RegisterHotKey(handle, id, 0, key))
            {
                HandleError(new Win32Exception(String.Format(Resources.RegisterHotKeyFailed, key)), Resources.RegisterHotKey);
                return false;
            }

            try
            {
                hotKeyWnd.HotKeyPass = new HotKeyPass(method);
                hotKeyWnd.WParam = id;
                hotKeyWnd.AssignHandle(handle);
                return true;
            }
            catch
            {
                hotKeyWnd.ReleaseHandle();
                return false;
            }
        }

        /// <summary>
        /// Handle an exception.
        /// </summary>
        /// <param name="err">The exception.</param>
        /// <param name="name">Name of the exception.</param>
        /// <param name="show">Whether the exception should be shown in the status bar.</param>
        /// <param name="type">The type of error, Error is considered fatal and will close the program.</param>
        public static void HandleError(Exception err, string name, bool show = true, EventLogEntryType type = EventLogEntryType.Warning)
        {
            try
            {
                if (Settings.Default.Debug)
                {
                    EventLog.WriteEntry(Resources.AppName, String.Format("[{0}]: {1}\n\n{2}", name, err.Message, err.StackTrace), type);
                }
            }
            catch
            {
                MessageBox.Show(Resources.LogNotWriteable, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Settings.Default.Debug = false;
            }

            if (type == EventLogEntryType.Error)
            {
                MessageBox.Show(Resources.FatalError, Resources.FatalErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else if (show)
            {
                Program.FormCore.toolStripStatus.Text = HandleErrorStatusText(name);
            }
        }


        /// <summary>
        /// Get the error text for the status bar.
        /// </summary>
        /// <param name="name">The name of the error.</param>
        /// <returns>The error text.</returns>
        public static string HandleErrorStatusText(string name)
        {
            return String.Format(Resources.Failed, name);
        }

        /// <summary>
        /// Get the remote file name.
        /// This will remove unsafe characters and increaste the upload count.
        /// </summary>
        /// <param name="filename">The file name.</param>
        /// <returns>The remote filename.</returns>
        public static string GetRemoteFileName(string filename)
        {
            switch (Settings.Default.Filename)
            {
                case 0:
                    filename = Regex.Replace(filename.Replace(' ', '_'), "[^a-zA-Z_\\.]*$", String.Empty).Trim();
                    break;

                case 1:
                    if (Settings.Default.Count == 99999)
                        Settings.Default.Count = 0;

                    filename = String.Format("{0}-{1}{2}", DateTime.Now.ToString("ddMMyyyy"), (++Settings.Default.Count).ToString("D5"), Path.GetExtension(filename));

                    break;
            }

            return filename;
        }

        /// <summary>
        /// Enable or disable all controls except progressUpload on CoreForm.
        /// </summary>
        /// <param name="status">True to enable, false to disable.</param>
        public static void SetControlStatus(bool status)
        {
            foreach (Control control in Program.FormCore.Controls)
                if (control.Name != "progressUpload" && control.Name != "updateButton") control.Enabled = status;

            // Only enable the update button if status is true and there is an update available
            Program.FormCore.buttonUpdate.Enabled = (status && Program.FormCore.UpdateAvailable);
        }

        /// <summary>
        /// Show a balloon with some info.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="text">The message.</param>
        /// <param name="icon">The icon.</param>
        public static void ShowInfo(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (Program.FormCore.WindowState != FormWindowState.Minimized)
                return;

            Program.FormCore.notifyIconNfu.ShowBalloonTip(1000, title, text, icon);
        }

        /// <summary>
        /// Encrypt a password.
        /// </summary>
        /// <param name="plainPassword">The password in plain text.</param>
        /// <returns>The encrypted password.</returns>
        public static string Encrypt(string plainPassword)
        {
            if (String.IsNullOrEmpty(plainPassword))
            {
                return String.Empty;
            }

            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(Settings.Default.IV);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(Settings.Default.SaltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Settings.Default.PassPhrase, saltValueBytes, Settings.Default.PasswordIterations);
                byte[] keyBytes = password.GetBytes(Settings.Default.KeySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();
                byte[] cipherTextBytes = memoryStream.ToArray();

                memoryStream.Close();
                cryptoStream.Close();

                return Convert.ToBase64String(cipherTextBytes);
            }
            catch (Exception e)
            {
                HandleError(e, Resources.Encrypt);
                return null;
            }
        }

        /// <summary>
        /// Decrypt a password.
        /// </summary>
        /// <param name="encryptedPassword">The encrypted password.</param>
        /// <returns>The password in plain text.</returns>
        public static string Decrypt(string encryptedPassword)
        {
            if (String.IsNullOrEmpty(encryptedPassword))
            {
                return String.Empty;
            }

            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(Settings.Default.IV);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(Settings.Default.SaltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Settings.Default.PassPhrase, saltValueBytes, Settings.Default.PasswordIterations);
                byte[] keyBytes = password.GetBytes(Settings.Default.KeySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                memoryStream.Close();
                cryptoStream.Close();

                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception e)
            {
                HandleError(e, Resources.Decrypt);
                return null;
            }
        }

        /// <summary>
        /// Check if a certificate is valid.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="certificate">The certificate.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="sslPolicyErrors">The errors.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            bool validatedCertificate = true;

            // Check if SSL errors occured and if certificate hash is not trusted
            if (sslPolicyErrors != SslPolicyErrors.None && Settings.Default.TrustedHash != certificate.GetCertHashString())
            {
                if (MessageBox.Show(String.Format(Resources.UntrustedCertificate, certificate.Subject, certificate.Issuer, certificate.GetCertHashString()), Resources.UntrustedCertificateTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Settings.Default.TrustedHash = certificate.GetCertHashString();
                }
                else
                {
                    validatedCertificate = false;
                }
            }

            return validatedCertificate;
        }
    }
}
