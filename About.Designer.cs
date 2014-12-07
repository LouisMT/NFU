namespace NFU
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listViewLibraries = new System.Windows.Forms.ListView();
            this.columnName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnURL = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnFor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.textBoxCopyright = new System.Windows.Forms.TextBox();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.textBoxVersion = new System.Windows.Forms.TextBox();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelLibraries = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.lineSeperatorAbout = new NFU.LineSeparator();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            this.lineSeparatorAbout2 = new NFU.LineSeparator();
            this.linkLabelSource = new System.Windows.Forms.LinkLabel();
            this.buttonChangelog = new System.Windows.Forms.Button();
            this.buttonIssues = new System.Windows.Forms.Button();
            this.buttonContributors = new System.Windows.Forms.Button();
            this.buttonLicense = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listViewLibraries
            // 
            this.listViewLibraries.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnURL,
            this.columnFor});
            this.listViewLibraries.FullRowSelect = true;
            this.listViewLibraries.GridLines = true;
            this.listViewLibraries.Location = new System.Drawing.Point(12, 140);
            this.listViewLibraries.Name = "listViewLibraries";
            this.listViewLibraries.Size = new System.Drawing.Size(410, 97);
            this.listViewLibraries.TabIndex = 12;
            this.listViewLibraries.UseCompatibleStateImageBehavior = false;
            this.listViewLibraries.View = System.Windows.Forms.View.Details;
            this.listViewLibraries.DoubleClick += new System.EventHandler(this.OpenLibraryUrl);
            // 
            // columnName
            // 
            this.columnName.Text = global::NFU.Properties.Resources.About_Name;
            // 
            // columnURL
            // 
            this.columnURL.Text = global::NFU.Properties.Resources.About_Url;
            // 
            // columnFor
            // 
            this.columnFor.Text = global::NFU.Properties.Resources.About_UsedFor;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(9, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = global::NFU.Properties.Resources.About_Name;
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(12, 25);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.ReadOnly = true;
            this.textBoxName.Size = new System.Drawing.Size(253, 20);
            this.textBoxName.TabIndex = 1;
            this.textBoxName.Text = global::NFU.Properties.Resources.About_Nfu;
            // 
            // textBoxCopyright
            // 
            this.textBoxCopyright.Location = new System.Drawing.Point(12, 64);
            this.textBoxCopyright.Name = "textBoxCopyright";
            this.textBoxCopyright.ReadOnly = true;
            this.textBoxCopyright.Size = new System.Drawing.Size(410, 20);
            this.textBoxCopyright.TabIndex = 5;
            this.textBoxCopyright.Text = global::NFU.Properties.Resources.About_CopyrightText;
            // 
            // labelCopyright
            // 
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(9, 48);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(54, 13);
            this.labelCopyright.TabIndex = 4;
            this.labelCopyright.Text = global::NFU.Properties.Resources.About_Copyright;
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Location = new System.Drawing.Point(271, 25);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.ReadOnly = true;
            this.textBoxVersion.Size = new System.Drawing.Size(151, 20);
            this.textBoxVersion.TabIndex = 3;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(268, 9);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(45, 13);
            this.labelVersion.TabIndex = 2;
            this.labelVersion.Text = global::NFU.Properties.Resources.About_Version;
            // 
            // labelLibraries
            // 
            this.labelLibraries.AutoSize = true;
            this.labelLibraries.Location = new System.Drawing.Point(9, 124);
            this.labelLibraries.Name = "labelLibraries";
            this.labelLibraries.Size = new System.Drawing.Size(316, 13);
            this.labelLibraries.TabIndex = 11;
            this.labelLibraries.Text = global::NFU.Properties.Resources.About_ThirdParty;
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(302, 251);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(120, 23);
            this.buttonClose.TabIndex = 16;
            this.buttonClose.Text = global::NFU.Properties.Resources.About_ButtonClose;
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // lineSeperatorAbout
            // 
            this.lineSeperatorAbout.Location = new System.Drawing.Point(0, 119);
            this.lineSeperatorAbout.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeperatorAbout.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeperatorAbout.Name = "lineSeperatorAbout";
            this.lineSeperatorAbout.Size = new System.Drawing.Size(434, 2);
            this.lineSeperatorAbout.TabIndex = 10;
            this.lineSeperatorAbout.TabStop = false;
            // 
            // buttonOpenLog
            // 
            this.buttonOpenLog.Location = new System.Drawing.Point(12, 251);
            this.buttonOpenLog.Name = "buttonOpenLog";
            this.buttonOpenLog.Size = new System.Drawing.Size(119, 23);
            this.buttonOpenLog.TabIndex = 14;
            this.buttonOpenLog.Text = global::NFU.Properties.Resources.About_ButtonOpenLog;
            this.buttonOpenLog.UseVisualStyleBackColor = true;
            this.buttonOpenLog.Click += new System.EventHandler(this.OpenNfuLog);
            // 
            // lineSeparatorAbout2
            // 
            this.lineSeparatorAbout2.Location = new System.Drawing.Point(0, 243);
            this.lineSeparatorAbout2.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparatorAbout2.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparatorAbout2.Name = "lineSeparatorAbout2";
            this.lineSeparatorAbout2.Size = new System.Drawing.Size(434, 2);
            this.lineSeparatorAbout2.TabIndex = 13;
            this.lineSeparatorAbout2.TabStop = false;
            // 
            // linkLabelSource
            // 
            this.linkLabelSource.AutoSize = true;
            this.linkLabelSource.Location = new System.Drawing.Point(171, 256);
            this.linkLabelSource.Name = "linkLabelSource";
            this.linkLabelSource.Size = new System.Drawing.Size(92, 13);
            this.linkLabelSource.TabIndex = 15;
            this.linkLabelSource.TabStop = true;
            this.linkLabelSource.Text = global::NFU.Properties.Resources.About_Source;
            this.linkLabelSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenSource);
            // 
            // buttonChangelog
            // 
            this.buttonChangelog.Location = new System.Drawing.Point(12, 90);
            this.buttonChangelog.Name = "buttonChangelog";
            this.buttonChangelog.Size = new System.Drawing.Size(98, 23);
            this.buttonChangelog.TabIndex = 6;
            this.buttonChangelog.Text = global::NFU.Properties.Resources.About_ButtonChangeLog;
            this.buttonChangelog.UseVisualStyleBackColor = true;
            this.buttonChangelog.Click += new System.EventHandler(this.OpenChangelog);
            // 
            // buttonIssues
            // 
            this.buttonIssues.Location = new System.Drawing.Point(220, 90);
            this.buttonIssues.Name = "buttonIssues";
            this.buttonIssues.Size = new System.Drawing.Size(98, 23);
            this.buttonIssues.TabIndex = 8;
            this.buttonIssues.Text = global::NFU.Properties.Resources.About_ButtonIssues;
            this.buttonIssues.UseVisualStyleBackColor = true;
            this.buttonIssues.Click += new System.EventHandler(this.OpenIssues);
            // 
            // buttonContributors
            // 
            this.buttonContributors.Location = new System.Drawing.Point(116, 90);
            this.buttonContributors.Name = "buttonContributors";
            this.buttonContributors.Size = new System.Drawing.Size(98, 23);
            this.buttonContributors.TabIndex = 7;
            this.buttonContributors.Text = global::NFU.Properties.Resources.About_ButtonContributors;
            this.buttonContributors.UseVisualStyleBackColor = true;
            this.buttonContributors.Click += new System.EventHandler(this.OpenContributors);
            // 
            // buttonLicense
            // 
            this.buttonLicense.Location = new System.Drawing.Point(324, 90);
            this.buttonLicense.Name = "buttonLicense";
            this.buttonLicense.Size = new System.Drawing.Size(98, 23);
            this.buttonLicense.TabIndex = 9;
            this.buttonLicense.Text = global::NFU.Properties.Resources.About_ButtonLicense;
            this.buttonLicense.UseVisualStyleBackColor = true;
            this.buttonLicense.Click += new System.EventHandler(this.OpenLicense);
            // 
            // About
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(434, 286);
            this.Controls.Add(this.buttonLicense);
            this.Controls.Add(this.buttonContributors);
            this.Controls.Add(this.buttonIssues);
            this.Controls.Add(this.buttonChangelog);
            this.Controls.Add(this.linkLabelSource);
            this.Controls.Add(this.lineSeparatorAbout2);
            this.Controls.Add(this.buttonOpenLog);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.lineSeperatorAbout);
            this.Controls.Add(this.labelLibraries);
            this.Controls.Add(this.textBoxVersion);
            this.Controls.Add(this.labelVersion);
            this.Controls.Add(this.textBoxCopyright);
            this.Controls.Add(this.labelCopyright);
            this.Controls.Add(this.textBoxName);
            this.Controls.Add(this.labelName);
            this.Controls.Add(this.listViewLibraries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = global::NFU.Properties.Resources.About_Title;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listViewLibraries;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.ColumnHeader columnURL;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.TextBox textBoxCopyright;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.TextBox textBoxVersion;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelLibraries;
        private LineSeparator lineSeperatorAbout;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOpenLog;
        private LineSeparator lineSeparatorAbout2;
        private System.Windows.Forms.ColumnHeader columnFor;
        private System.Windows.Forms.LinkLabel linkLabelSource;
        private System.Windows.Forms.Button buttonChangelog;
        private System.Windows.Forms.Button buttonIssues;
        private System.Windows.Forms.Button buttonContributors;
        private System.Windows.Forms.Button buttonLicense;

    }
}
