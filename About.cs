using Nfu.Properties;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace Nfu
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

            var sshNet = new ListViewItem(new[] {
                Resources.SshNetName,
                Resources.SshNetUrl,
                Resources.SshNetUsedFor
            });
            var iconPack = new ListViewItem(new[] {
                Resources.IcoMoonName,
                Resources.IcoMoonUrl,
                Resources.IcoMoonUsedFor
            });

            listViewLibraries.Items.AddRange(new[] {
                sshNet,
                iconPack
            });
        }

        /// <summary>
        /// Open the selected library page.
        /// </summary>
        private void OpenLibraryUrl(object sender, EventArgs e)
        {
            if (listViewLibraries.SelectedItems.Count > 0)
                Process.Start(listViewLibraries.SelectedItems[0].SubItems[1].Text);
        }

        /// <summary>
        /// Open the changelog page.
        /// </summary>
        private void OpenChangelog(object sender, EventArgs e)
        {
            Process.Start(Resources.ChangeLogUrl);
        }

        /// <summary>
        /// Open the contributors page.
        /// </summary>
        private void OpenContributors(object sender, EventArgs e)
        {
            Process.Start(Resources.ContributorsUrl);
        }

        /// <summary>
        /// Open the issues page.
        /// </summary>
        private void OpenIssues(object sender, EventArgs e)
        {
            Process.Start(Resources.IssuesUrl);
        }

        /// <summary>
        /// Open the license page.
        /// </summary>
        private void OpenLicense(object sender, EventArgs e)
        {
            Process.Start(Resources.LicenseUrl);
        }

        /// <summary>
        /// Open the NFU log using the Event Viewer with a filter.
        /// </summary>
        private void OpenNfuLog(object sender, EventArgs e)
        {
            Process.Start("eventvwr", "/f:\"<QueryList><Query Id='0' Path='Application'><Select Path='Application'>*[System[Provider[@Name='NFU']]]</Select></Query></QueryList>\"");
        }

        /// <summary>
        /// Open the source page.
        /// </summary>
        private void OpenSource(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(Resources.SourceUrl);
        }
    }
}
