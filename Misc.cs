using NFU.Properties;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NFU
{
    public partial class LineSeparator : UserControl
    {
        public LineSeparator()
        {
            Paint += new PaintEventHandler(PaintLineSeparator);
            MaximumSize = new Size(2000, 2);
            MinimumSize = new Size(0, 2);
            Width = 350;
        }

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
        public static extern int SetForegroundWindow(IntPtr aHandle);

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr aHandle, int aID, int aMode, Keys aKey);

        [DllImport("user32")]
        public static extern UInt32 SendMessage (IntPtr aHandle, UInt32 aMSG, UInt32 aWParam, UInt32 aLParam);

        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string aUsername, string aDomain, string aPassword, int aLogonType, int aLogonProvider, ref IntPtr aToken);

        private delegate void HotKeyPass();

        /// <summary>
        /// Bind a hotkey to a method.
        /// </summary>
        /// <param name="aKey">The key.</param>
        /// <param name="aMethod">The method.</param>
        /// <param name="aID">The bind ID.</param>
        /// <param name="aHandle">The handle.</param>
        public static void RegisterHotKey(Keys aKey, Action aMethod, int aID, IntPtr aHandle)
        {
            HotKeyWndProc HotKeyWnd = new HotKeyWndProc();

            if (!Misc.RegisterHotKey(aHandle, aID, 0, aKey)) return;

            try
            {
                HotKeyWnd.HotKeyPass = new HotKeyPass(aMethod);
                HotKeyWnd.WParam = aID;
                HotKeyWnd.AssignHandle(aHandle);
            }
            catch
            {
                HotKeyWnd.ReleaseHandle();
            }
        }

        private class HotKeyWndProc : NativeWindow
        {
            public HotKeyPass HotKeyPass;
            public int WParam = 10000;

            protected override void WndProc(ref Message aM)
            {
                if (aM.Msg == 0x0312 && aM.WParam.ToInt32() == WParam)
                {
                    if (HotKeyPass != null) HotKeyPass.Invoke();
                }

                base.WndProc(ref aM);
            }
        }

        /// <summary>
        /// Handle an exception.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="Name">Name of the exception.</param>
        /// <param name="Fatal">Whether the exception is fatal or not.</param>
        public static void HandleError(Exception e, string Name, bool Fatal = false)
        {
            try
            {
                if (Settings.Default.Debug) File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NFU.log", String.Format("[{0}] [{1}] ({2}) -> {3}{4}", DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), Name, Fatal, e.Message, Environment.NewLine));
            }
            catch { }

            if (Fatal)
            {
                MessageBox.Show("A fatal error occured. NFU will exit.", "NFU Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            else
            {
                Program.CoreForm.toolStripStatus.Text = String.Format("{0} failed", Name);
            }
        }

        /// <summary>
        /// Get the remote filename.
        /// </summary>
        /// <param name="aPath">The path to the local file.</param>
        /// <returns>The remote filename.</returns>
        public static string GetFilename(string aPath)
        {
            aPath = Path.GetFileName(aPath);

            switch (Settings.Default.Filename)
            {
                case 0:
                    aPath = Regex.Replace(aPath.Replace(' ', '_'), "^[a-zA-Z_.]*$", String.Empty).Trim();
                    break;

                case 1:
                    if (Settings.Default.Count == 99999) Settings.Default.Count = 0;
                    aPath = String.Format("{0}-{1}{2}", DateTime.Now.ToString("ddMMyyyy"), (++Settings.Default.Count).ToString("D5"), Path.GetExtension(aPath));
                    break;
            }

            return aPath;
        }

        /// <summary>
        /// Get a path to a temporary file.
        /// </summary>
        /// <param name="aExtension">The extension of the temporary file.</param>
        /// <returns></returns>
        public static string GetTempFileName(string aExtension)
        {
            return String.Format("{0}{1}", Path.GetTempFileName(), aExtension);
        }

        /// <summary>
        /// Enable or disable all controls except progressUpload on CoreForm.
        /// </summary>
        /// <param name="status">True = enable, false = disable.</param>
        public static void SetControlStatus(bool aStatus)
        {
            foreach (Control CTRL in Program.CoreForm.Controls)
            {
                if (CTRL.Name != "progressUpload" && CTRL.Name != "buttonUpdate") CTRL.Enabled = aStatus;
            }
        }

        public static void ShowInfo(string title, string text)
        {
            if (Program.CoreForm.WindowState != FormWindowState.Minimized) return;

            Program.CoreForm.notifyIconNFU.ShowBalloonTip(1000, title, text, ToolTipIcon.Info);
        }

        private const int keySize = 128;
        private const int passwordIterations = 2;
        private const string initVector = "*lzk3&HMv7(uC&aH";
        private const string saltValue = "[1uT@|:+k3dXmOf}2!(-Rc}*6g5eUMi9qO1{`4jgtx=5V_c-g :,S.Ica.77,_V$";
        private const string passPhrase = "ZS9|:c3)Xkov%.Pp}1MYxX 0FphF),#5bUir6kt R_Q 8?**(b,zW{pxq$N+Khgh";

        /// <summary>
        /// Encrypt a password.
        /// </summary>
        /// <param name="aPlainPassword">The password in plain text.</param>
        /// <returns>The encrypted password.</returns>
        public static string Encrypt(string aPlainPassword)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(aPlainPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);
                byte[] KeyBytes = password.GetBytes(keySize / 8);
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
                HandleError(e, "Encrypt");
                return null;
            }
        }

        /// <summary>
        /// Decrypt a password.
        /// </summary>
        /// <param name="aEncryptedPassword">The encrypted password.</param>
        /// <returns>The password in plain text.</returns>
        public static string Decrypt(string aEncryptedPassword)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
                byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
                byte[] cipherTextBytes = Convert.FromBase64String(aEncryptedPassword);
                Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltValueBytes, passwordIterations);
                byte[] KeyBytes = password.GetBytes(keySize / 8);
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
                HandleError(e, "Decrypt");
                return null;
            }
        }
    }
}
