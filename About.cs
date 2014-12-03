using NFU.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace NFU
{
    partial class About : Form
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public About()
        {
            InitializeComponent();

            // 22 is the width of the scrollbar
            listViewLibraries.Columns[0].Width = listViewLibraries.Width / 3 - 22;
            listViewLibraries.Columns[1].Width = listViewLibraries.Width / 3;
            listViewLibraries.Columns[2].Width = listViewLibraries.Width / 3;

            textBoxVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            ListViewItem sshNet = new ListViewItem(new string[] {
                Resources.About_SshNetName,
                Resources.About_SshNetUrl,
                Resources.About_SshNetUsedFor
            });
            ListViewItem iconPack = new ListViewItem(new string[] {
                Resources.About_IcoMoonName,
                Resources.About_IcoMoonUrl,
                Resources.About_IcoMoonUsedFor
            });

            listViewLibraries.Items.AddRange(new ListViewItem[] {
                sshNet,
                iconPack
            });
        }

        /// <summary>
        /// Open the selected library page.
        /// </summary>
        private void OpenLibraryURL(object sender, EventArgs e)
        {
            if (listViewLibraries.SelectedItems.Count > 0)
                Process.Start(listViewLibraries.SelectedItems[0].SubItems[1].Text);
        }

        /// <summary>
        /// Open the changelog page.
        /// </summary>
        private void OpenChangelog(object sender, EventArgs e)
        {
            Process.Start(Resources.About_ChangeLogUrl);
        }

        /// <summary>
        /// Open the contributors page.
        /// </summary>
        private void OpenContributors(object sender, EventArgs e)
        {
            Process.Start(Resources.About_ContributorsUrl);
        }

        /// <summary>
        /// Open the issues page.
        /// </summary>
        private void OpenIssues(object sender, EventArgs e)
        {
            Process.Start(Resources.About_IssuesUrl);
        }

        /// <summary>
        /// Open the license page.
        /// </summary>
        private void OpenLicense(object sender, EventArgs e)
        {
            Process.Start(Resources.About_LicenseUrl);
        }

        /// <summary>
        /// Open the NFU log.
        /// </summary>
        private void OpenNFULog(object sender, EventArgs e)
        {
            string filename = String.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Settings.Default.LogFileName);

            if (File.Exists(filename)) Process.Start(filename); else MessageBox.Show(Resources.About_LogFileNotFound,
                Resources.About_LogFileNotFoundTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Open the source page.
        /// </summary>
        private void OpenSource(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.About_SourceUrl);
        }
    }
}
