using NFU.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NFU
{
    public partial class LineSeparator : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public LineSeparator()
        {
            Paint += new PaintEventHandler(PaintLineSeparator);
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
            HotKeyWndProc HotKeyWnd = new HotKeyWndProc();

            if (!Misc.RegisterHotKey(handle, id, 0, key))
            {
                Misc.HandleError(new Win32Exception(String.Format(Resources.Misc_RegisterHotKeyFailed, key)), Resources.Misc_RegisterHotKey);
                return false;
            }

            try
            {
                HotKeyWnd.HotKeyPass = new HotKeyPass(method);
                HotKeyWnd.WParam = id;
                HotKeyWnd.AssignHandle(handle);
                return true;
            }
            catch
            {
                HotKeyWnd.ReleaseHandle();
                return false;
            }
        }

        /// <summary>
        /// Handle an exception.
        /// </summary>
        /// <param name="err">The exception.</param>
        /// <param name="name">Name of the exception.</param>
        /// <param name="fatal">Whether the exception is fatal or not.</param>
        public static void HandleError(Exception err, string name, bool fatal = false)
        {
            try
            {
                if (Settings.Default.Debug)
                    File.AppendAllText(String.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.LogFileName),
                        String.Format("[{0}] [{1}] ({2}) -> {3}{4}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), name, fatal,
                        err != null ? err.Message : "", Environment.NewLine));
            }
            catch { }

            if (fatal)
            {
                MessageBox.Show(Resources.Misc_FatalError, Resources.Misc_FatalErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                Program.formCore.toolStripStatus.Text = String.Format(Resources.Misc_Failed, name);
            }
        }

        /// <summary>
        /// Get the remote filename.
        /// </summary>
        /// <param name="path">The path to the local file.</param>
        /// <returns>The remote filename.</returns>
        public static string GetFilename(string path)
        {
            path = Path.GetFileName(path);

            switch (Settings.Default.Filename)
            {
                case 0:
                    path = Regex.Replace(path.Replace(' ', '_'), "[^a-zA-Z_\\.]*$", String.Empty).Trim();
                    break;

                case 1:
                    if (Settings.Default.Count == 99999)
                        Settings.Default.Count = 0;

                    path = String.Format("{0}-{1}{2}", DateTime.Now.ToString("ddMMyyyy"), (++Settings.Default.Count).ToString("D5"), Path.GetExtension(path));

                    Settings.Default.Save();
                    break;
            }

            return path;
        }

        /// <summary>
        /// Get a path to a temporary file.
        /// </summary>
        /// <param name="extension">The extension of the temporary file.</param>
        /// <returns></returns>
        public static string GetTempFileName(string extension)
        {
            return String.Format("{0}{1}", Path.GetTempFileName(), extension);
        }

        /// <summary>
        /// Enable or disable all controls except progressUpload on CoreForm.
        /// </summary>
        /// <param name="status">True to enable, false to disable.</param>
        public static void SetControlStatus(bool status)
        {
            foreach (Control control in Program.formCore.Controls)
                if (control.Name != "progressUpload" && control.Name != "buttonUpdate") control.Enabled = status;
        }

        /// <summary>
        /// Show a balloon with some info.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="text">The message.</param>
        /// <param name="icon">The icon.</param>
        public static void ShowInfo(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            if (Program.formCore.WindowState != FormWindowState.Minimized)
                return;

            Program.formCore.notifyIconNFU.ShowBalloonTip(1000, title, text, icon);
        }

        /// <summary>
        /// Encrypt a password.
        /// </summary>
        /// <param name="plainPassword">The password in plain text.</param>
        /// <returns>The encrypted password.</returns>
        public static string Encrypt(string plainPassword)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(Settings.Default.IV);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(Settings.Default.SaltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Settings.Default.PassPhrase, saltValueBytes, Settings.Default.PasswordIterations);
                byte[] KeyBytes = password.GetBytes(Settings.Default.KeySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(KeyBytes, initVectorBytes);
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
                HandleError(e, Resources.Misc_Encrypt);
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
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(Settings.Default.IV);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(Settings.Default.SaltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(encryptedPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(Settings.Default.PassPhrase, saltValueBytes, Settings.Default.PasswordIterations);
                byte[] KeyBytes = password.GetBytes(Settings.Default.KeySize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(KeyBytes, initVectorBytes);
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
                HandleError(e, Resources.Misc_Decrypt);
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
                if (MessageBox.Show(String.Format(Resources.Misc_UntrustedCertificate, certificate.Subject, certificate.Issuer, certificate.GetCertHashString()), Resources.Misc_UntrustedCertificateTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    Settings.Default.TrustedHash = certificate.GetCertHashString();
                    Settings.Default.Save();
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
