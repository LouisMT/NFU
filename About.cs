using NFU.Properties;
using System;
using System.Diagnostics;
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

            ListViewItem sshNet = new ListViewItem(new[] {
                Resources.About_SshNetName,
                Resources.About_SshNetUrl,
                Resources.About_SshNetUsedFor
            });
            ListViewItem iconPack = new ListViewItem(new[] {
                Resources.About_IcoMoonName,
                Resources.About_IcoMoonUrl,
                Resources.About_IcoMoonUsedFor
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
            Process.Start(Resources.About_SourceUrl);
        }
    }
}
