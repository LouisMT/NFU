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
                "SSH.NET",
                "https://sshnet.codeplex.com",
                "SFTP uploads"
            });
            ListViewItem iconPack = new ListViewItem(new string[] {
                "IcoMoon",
                "https://icomoon.io",
                "All icons"
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
            Process.Start("https://github.com/Naxiz/NFU/commits/master");
        }

        /// <summary>
        /// Open the contributors page.
        /// </summary>
        private void OpenContributors(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Naxiz/NFU/graphs/contributors");
        }

        /// <summary>
        /// Open the issues page.
        /// </summary>
        private void OpenIssues(object sender, EventArgs e)
        {
            Process.Start("https://github.com/Naxiz/NFU/issues");
        }

        /// <summary>
        /// Open the license page.
        /// </summary>
        private void OpenLicense(object sender, EventArgs e)
        {
            Process.Start("https://github.com/naxiz/NFU/blob/master/LICENSE");
        }

        /// <summary>
        /// Open the NFU log.
        /// </summary>
        private void OpenNFULog(object sender, EventArgs e)
        {
            string filename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\NFU.log";

            if (File.Exists(filename)) Process.Start(filename); else MessageBox.Show("The log could not be found.\n\nDid you enable debug mode?",
                "NFU log not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Open the source page.
        /// </summary>
        private void OpenSource(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/Naxiz/NFU");
        }
    }
}
