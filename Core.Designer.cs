namespace NFU
{
    partial class Core
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.components = new System.ComponentModel.Container();
            this.statusStripCore = new System.Windows.Forms.StatusStrip();
            this.toolStripStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonFile = new System.Windows.Forms.Button();
            this.buttonImport = new System.Windows.Forms.Button();
            this.buttonScreenshot = new System.Windows.Forms.Button();
            this.progressUpload = new System.Windows.Forms.ProgressBar();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.notifyIconNFU = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuMain = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buttonUpdate = new System.Windows.Forms.Button();
            this.labelUpdate = new System.Windows.Forms.Label();
            this.pictureBoxAbout = new System.Windows.Forms.PictureBox();
            this.pictureBoxSettings = new System.Windows.Forms.PictureBox();
            this.statusStripCore.SuspendLayout();
            this.contextMenuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAbout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSettings)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStripCore
            // 
            this.statusStripCore.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatus});
            this.statusStripCore.Location = new System.Drawing.Point(0, 122);
            this.statusStripCore.Name = "statusStripCore";
            this.statusStripCore.Size = new System.Drawing.Size(400, 22);
            this.statusStripCore.SizingGrip = false;
            this.statusStripCore.TabIndex = 6;
            this.statusStripCore.Text = "statusStrip1";
            // 
            // toolStripStatus
            // 
            this.toolStripStatus.Name = "toolStripStatus";
            this.toolStripStatus.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatus.Text = "Ready";
            // 
            // buttonFile
            // 
            this.buttonFile.Location = new System.Drawing.Point(12, 12);
            this.buttonFile.Name = "buttonFile";
            this.buttonFile.Size = new System.Drawing.Size(120, 23);
            this.buttonFile.TabIndex = 0;
            this.buttonFile.Text = "&File(s)";
            this.buttonFile.Click += new System.EventHandler(this.ButtonFile);
            // 
            // buttonImport
            // 
            this.buttonImport.Location = new System.Drawing.Point(268, 12);
            this.buttonImport.Name = "buttonImport";
            this.buttonImport.Size = new System.Drawing.Size(120, 23);
            this.buttonImport.TabIndex = 2;
            this.buttonImport.Text = "&Import";
            this.buttonImport.Click += new System.EventHandler(this.ButtonImport);
            // 
            // buttonScreenshot
            // 
            this.buttonScreenshot.Location = new System.Drawing.Point(138, 12);
            this.buttonScreenshot.Name = "buttonScreenshot";
            this.buttonScreenshot.Size = new System.Drawing.Size(124, 23);
            this.buttonScreenshot.TabIndex = 1;
            this.buttonScreenshot.Text = "&Screenshot";
            this.buttonScreenshot.Click += new System.EventHandler(this.ButtonScreenshot);
            // 
            // progressUpload
            // 
            this.progressUpload.Location = new System.Drawing.Point(12, 41);
            this.progressUpload.Name = "progressUpload";
            this.progressUpload.Size = new System.Drawing.Size(376, 10);
            this.progressUpload.TabIndex = 3;
            // 
            // openFileDialog
            // 
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.Title = "Choose one or more files to upload";
            // 
            // notifyIconNFU
            // 
            this.notifyIconNFU.ContextMenuStrip = this.contextMenuMain;
            this.notifyIconNFU.Icon = global::NFU.Properties.Resources.NFU;
            this.notifyIconNFU.Text = "NFU";
            this.notifyIconNFU.Visible = true;
            this.notifyIconNFU.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.NotifyIconNFUDoubleClick);
            // 
            // contextMenuMain
            // 
            this.contextMenuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.contextMenuMain.Name = "contextMenuMain";
            this.contextMenuMain.Size = new System.Drawing.Size(93, 26);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Image = global::NFU.Properties.Resources.Exit;
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(92, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitNFU);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.Enabled = false;
            this.buttonUpdate.Location = new System.Drawing.Point(138, 83);
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(124, 23);
            this.buttonUpdate.TabIndex = 5;
            this.buttonUpdate.Text = "&Update";
            this.buttonUpdate.UseVisualStyleBackColor = true;
            this.buttonUpdate.Click += new System.EventHandler(this.StartUpdate);
            // 
            // labelUpdate
            // 
            this.labelUpdate.Location = new System.Drawing.Point(12, 67);
            this.labelUpdate.Name = "labelUpdate";
            this.labelUpdate.Size = new System.Drawing.Size(376, 13);
            this.labelUpdate.TabIndex = 4;
            this.labelUpdate.Text = "You already have the latest version of NFU";
            this.labelUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBoxAbout
            // 
            this.pictureBoxAbout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxAbout.Image = global::NFU.Properties.Resources.About;
            this.pictureBoxAbout.Location = new System.Drawing.Point(364, 126);
            this.pictureBoxAbout.Name = "pictureBoxAbout";
            this.pictureBoxAbout.Size = new System.Drawing.Size(14, 14);
            this.pictureBoxAbout.TabIndex = 9;
            this.pictureBoxAbout.TabStop = false;
            this.pictureBoxAbout.Click += new System.EventHandler(this.OpenAbout);
            // 
            // pictureBoxSettings
            // 
            this.pictureBoxSettings.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxSettings.Image = global::NFU.Properties.Resources.Settings;
            this.pictureBoxSettings.Location = new System.Drawing.Point(382, 126);
            this.pictureBoxSettings.Name = "pictureBoxSettings";
            this.pictureBoxSettings.Size = new System.Drawing.Size(14, 14);
            this.pictureBoxSettings.TabIndex = 10;
            this.pictureBoxSettings.TabStop = false;
            this.pictureBoxSettings.Click += new System.EventHandler(this.OpenSettings);
            // 
            // Core
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 144);
            this.Controls.Add(this.pictureBoxSettings);
            this.Controls.Add(this.pictureBoxAbout);
            this.Controls.Add(this.labelUpdate);
            this.Controls.Add(this.buttonUpdate);
            this.Controls.Add(this.progressUpload);
            this.Controls.Add(this.buttonScreenshot);
            this.Controls.Add(this.buttonImport);
            this.Controls.Add(this.buttonFile);
            this.Controls.Add(this.statusStripCore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = global::NFU.Properties.Resources.NFU;
            this.MaximizeBox = false;
            this.Name = "Core";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NFU";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CoreFormClosing);
            this.Shown += new System.EventHandler(this.CoreShown);
            this.Resize += new System.EventHandler(this.CoreResize);
            this.statusStripCore.ResumeLayout(false);
            this.statusStripCore.PerformLayout();
            this.contextMenuMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAbout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxSettings)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStripCore;
        private System.Windows.Forms.Button buttonFile;
        private System.Windows.Forms.Button buttonImport;
        private System.Windows.Forms.Button buttonScreenshot;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        public System.Windows.Forms.ToolStripStatusLabel toolStripStatus;
        public System.Windows.Forms.ProgressBar progressUpload;
        private System.Windows.Forms.ContextMenuStrip contextMenuMain;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Button buttonUpdate;
        private System.Windows.Forms.Label labelUpdate;
        public System.Windows.Forms.NotifyIcon notifyIconNFU;
        private System.Windows.Forms.PictureBox pictureBoxAbout;
        private System.Windows.Forms.PictureBox pictureBoxSettings;



    }
}

