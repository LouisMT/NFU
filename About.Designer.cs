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
            this.separatorAbout = new NFU.LineSeparator();
            this.buttonOpenLog = new System.Windows.Forms.Button();
            this.lineSeparator1 = new NFU.LineSeparator();
            this.linkLabelSource = new System.Windows.Forms.LinkLabel();
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
            this.listViewLibraries.Location = new System.Drawing.Point(12, 115);
            this.listViewLibraries.Name = "listViewLibraries";
            this.listViewLibraries.Size = new System.Drawing.Size(411, 97);
            this.listViewLibraries.TabIndex = 0;
            this.listViewLibraries.UseCompatibleStateImageBehavior = false;
            this.listViewLibraries.View = System.Windows.Forms.View.Details;
            this.listViewLibraries.DoubleClick += new System.EventHandler(this.OpenLibraryURL);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            // 
            // columnURL
            // 
            this.columnURL.Text = "URL";
            // 
            // columnFor
            // 
            this.columnFor.Text = "For";
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(12, 9);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(38, 13);
            this.labelName.TabIndex = 1;
            this.labelName.Text = "Name:";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(14, 25);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.ReadOnly = true;
            this.textBoxName.Size = new System.Drawing.Size(253, 20);
            this.textBoxName.TabIndex = 2;
            this.textBoxName.Text = "NFU";
            // 
            // textBoxCopyright
            // 
            this.textBoxCopyright.Location = new System.Drawing.Point(12, 64);
            this.textBoxCopyright.Name = "textBoxCopyright";
            this.textBoxCopyright.ReadOnly = true;
            this.textBoxCopyright.Size = new System.Drawing.Size(411, 20);
            this.textBoxCopyright.TabIndex = 4;
            this.textBoxCopyright.Text = "Louis Matthijssen 2014";
            // 
            // labelCopyright
            // 
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(12, 48);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(54, 13);
            this.labelCopyright.TabIndex = 3;
            this.labelCopyright.Text = "Copyright:";
            // 
            // textBoxVersion
            // 
            this.textBoxVersion.Location = new System.Drawing.Point(273, 25);
            this.textBoxVersion.Name = "textBoxVersion";
            this.textBoxVersion.ReadOnly = true;
            this.textBoxVersion.Size = new System.Drawing.Size(150, 20);
            this.textBoxVersion.TabIndex = 8;
            // 
            // labelVersion
            // 
            this.labelVersion.AutoSize = true;
            this.labelVersion.Location = new System.Drawing.Point(273, 9);
            this.labelVersion.Name = "labelVersion";
            this.labelVersion.Size = new System.Drawing.Size(45, 13);
            this.labelVersion.TabIndex = 7;
            this.labelVersion.Text = "Version:";
            // 
            // labelLibraries
            // 
            this.labelLibraries.AutoSize = true;
            this.labelLibraries.Location = new System.Drawing.Point(12, 99);
            this.labelLibraries.Name = "labelLibraries";
            this.labelLibraries.Size = new System.Drawing.Size(217, 13);
            this.labelLibraries.TabIndex = 9;
            this.labelLibraries.Text = "NFU uses the following third party resources:";
            // 
            // buttonClose
            // 
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(348, 226);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 11;
            this.buttonClose.Text = "&Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            // 
            // separatorAbout
            // 
            this.separatorAbout.Location = new System.Drawing.Point(0, 90);
            this.separatorAbout.MaximumSize = new System.Drawing.Size(2000, 2);
            this.separatorAbout.MinimumSize = new System.Drawing.Size(0, 2);
            this.separatorAbout.Name = "separatorAbout";
            this.separatorAbout.Size = new System.Drawing.Size(435, 2);
            this.separatorAbout.TabIndex = 10;
            // 
            // buttonOpenLog
            // 
            this.buttonOpenLog.Location = new System.Drawing.Point(12, 226);
            this.buttonOpenLog.Name = "buttonOpenLog";
            this.buttonOpenLog.Size = new System.Drawing.Size(100, 23);
            this.buttonOpenLog.TabIndex = 12;
            this.buttonOpenLog.Text = "Open log file";
            this.buttonOpenLog.UseVisualStyleBackColor = true;
            this.buttonOpenLog.Click += new System.EventHandler(this.OpenNFULog);
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Location = new System.Drawing.Point(0, 218);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(435, 2);
            this.lineSeparator1.TabIndex = 13;
            // 
            // linkLabelSource
            // 
            this.linkLabelSource.AutoSize = true;
            this.linkLabelSource.Location = new System.Drawing.Point(171, 231);
            this.linkLabelSource.Name = "linkLabelSource";
            this.linkLabelSource.Size = new System.Drawing.Size(92, 13);
            this.linkLabelSource.TabIndex = 14;
            this.linkLabelSource.TabStop = true;
            this.linkLabelSource.Text = "Source on GitHub";
            this.linkLabelSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OpenSource);
            // 
            // About
            // 
            this.AcceptButton = this.buttonClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(435, 261);
            this.Controls.Add(this.linkLabelSource);
            this.Controls.Add(this.lineSeparator1);
            this.Controls.Add(this.buttonOpenLog);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.separatorAbout);
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
            this.Text = "About";
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
        private LineSeparator separatorAbout;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.Button buttonOpenLog;
        private LineSeparator lineSeparator1;
        private System.Windows.Forms.ColumnHeader columnFor;
        private System.Windows.Forms.LinkLabel linkLabelSource;

    }
}
