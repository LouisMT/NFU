namespace NFU
{
    partial class CP
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
            this.tabControlCP = new System.Windows.Forms.TabControl();
            this.tabServer = new System.Windows.Forms.TabPage();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.labelURL = new System.Windows.Forms.Label();
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.checkBoxShowPassword = new System.Windows.Forms.CheckBox();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.seperatorServer = new NFU.LineSeparator();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxUsername = new System.Windows.Forms.TextBox();
            this.textBoxDirectory = new System.Windows.Forms.TextBox();
            this.textBoxHost = new System.Windows.Forms.TextBox();
            this.labelPassword = new System.Windows.Forms.Label();
            this.labelDirectory = new System.Windows.Forms.Label();
            this.labelUsername = new System.Windows.Forms.Label();
            this.labelHost = new System.Windows.Forms.Label();
            this.tabUpload = new System.Windows.Forms.TabPage();
            this.linkLabelReset = new System.Windows.Forms.LinkLabel();
            this.labelCounter = new System.Windows.Forms.Label();
            this.comboBoxFilename = new System.Windows.Forms.ComboBox();
            this.separatorNFU = new NFU.LineSeparator();
            this.tabNFU = new System.Windows.Forms.TabPage();
            this.checkBoxQuickScreenshots = new System.Windows.Forms.CheckBox();
            this.checkBoxStartWindows = new System.Windows.Forms.CheckBox();
            this.checkBoxSytemTray = new System.Windows.Forms.CheckBox();
            this.comboBoxScreen = new System.Windows.Forms.ComboBox();
            this.checkBoxPause = new System.Windows.Forms.CheckBox();
            this.checkBoxPrintScreen = new System.Windows.Forms.CheckBox();
            this.checkBoxDebug = new System.Windows.Forms.CheckBox();
            this.seperatorNFU = new NFU.LineSeparator();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelHelpTitle = new System.Windows.Forms.Label();
            this.labelHelpText = new System.Windows.Forms.Label();
            this.tabControlCP.SuspendLayout();
            this.tabServer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.tabUpload.SuspendLayout();
            this.tabNFU.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlCP
            // 
            this.tabControlCP.Controls.Add(this.tabServer);
            this.tabControlCP.Controls.Add(this.tabUpload);
            this.tabControlCP.Controls.Add(this.tabNFU);
            this.tabControlCP.Location = new System.Drawing.Point(12, 12);
            this.tabControlCP.Name = "tabControlCP";
            this.tabControlCP.SelectedIndex = 0;
            this.tabControlCP.Size = new System.Drawing.Size(460, 197);
            this.tabControlCP.TabIndex = 0;
            // 
            // tabServer
            // 
            this.tabServer.Controls.Add(this.numericUpDownPort);
            this.tabServer.Controls.Add(this.labelURL);
            this.tabServer.Controls.Add(this.textBoxURL);
            this.tabServer.Controls.Add(this.checkBoxShowPassword);
            this.tabServer.Controls.Add(this.comboBoxType);
            this.tabServer.Controls.Add(this.seperatorServer);
            this.tabServer.Controls.Add(this.textBoxPassword);
            this.tabServer.Controls.Add(this.textBoxUsername);
            this.tabServer.Controls.Add(this.textBoxDirectory);
            this.tabServer.Controls.Add(this.textBoxHost);
            this.tabServer.Controls.Add(this.labelPassword);
            this.tabServer.Controls.Add(this.labelDirectory);
            this.tabServer.Controls.Add(this.labelUsername);
            this.tabServer.Controls.Add(this.labelHost);
            this.tabServer.Location = new System.Drawing.Point(4, 22);
            this.tabServer.Name = "tabServer";
            this.tabServer.Padding = new System.Windows.Forms.Padding(3);
            this.tabServer.Size = new System.Drawing.Size(452, 171);
            this.tabServer.TabIndex = 0;
            this.tabServer.Text = "Server";
            this.tabServer.UseVisualStyleBackColor = true;
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(380, 41);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownPort.TabIndex = 4;
            this.numericUpDownPort.Value = new decimal(new int[] {
            21,
            0,
            0,
            0});
            // 
            // labelURL
            // 
            this.labelURL.AutoSize = true;
            this.labelURL.Location = new System.Drawing.Point(3, 148);
            this.labelURL.Name = "labelURL";
            this.labelURL.Size = new System.Drawing.Size(32, 13);
            this.labelURL.TabIndex = 12;
            this.labelURL.Text = "URL:";
            // 
            // textBoxURL
            // 
            this.textBoxURL.Location = new System.Drawing.Point(83, 145);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(363, 20);
            this.textBoxURL.TabIndex = 13;
            this.textBoxURL.Enter += new System.EventHandler(this.SettingsHelper);
            this.textBoxURL.Leave += new System.EventHandler(this.SettingsHelperClear);
            // 
            // checkBoxShowPassword
            // 
            this.checkBoxShowPassword.AutoSize = true;
            this.checkBoxShowPassword.Location = new System.Drawing.Point(348, 121);
            this.checkBoxShowPassword.Name = "checkBoxShowPassword";
            this.checkBoxShowPassword.Size = new System.Drawing.Size(101, 17);
            this.checkBoxShowPassword.TabIndex = 11;
            this.checkBoxShowPassword.Text = "Show password";
            this.checkBoxShowPassword.UseVisualStyleBackColor = true;
            this.checkBoxShowPassword.CheckedChanged += new System.EventHandler(this.CheckBoxShowPassword);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "FTP",
            "FTPS (Explicit)",
            "SFTP",
            "SFTP (SSH Keys)",
            "CIFS"});
            this.comboBoxType.Location = new System.Drawing.Point(6, 6);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(440, 21);
            this.comboBoxType.TabIndex = 0;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.TypeIndexChanged);
            // 
            // seperatorServer
            // 
            this.seperatorServer.Location = new System.Drawing.Point(0, 33);
            this.seperatorServer.MaximumSize = new System.Drawing.Size(2000, 2);
            this.seperatorServer.MinimumSize = new System.Drawing.Size(0, 2);
            this.seperatorServer.Name = "seperatorServer";
            this.seperatorServer.Size = new System.Drawing.Size(450, 2);
            this.seperatorServer.TabIndex = 1;
            this.seperatorServer.TabStop = false;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(83, 119);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(259, 20);
            this.textBoxPassword.TabIndex = 10;
            this.textBoxPassword.UseSystemPasswordChar = true;
            this.textBoxPassword.Enter += new System.EventHandler(this.SettingsHelper);
            this.textBoxPassword.Leave += new System.EventHandler(this.SettingsHelperClear);
            // 
            // textBoxUsername
            // 
            this.textBoxUsername.Location = new System.Drawing.Point(83, 93);
            this.textBoxUsername.Name = "textBoxUsername";
            this.textBoxUsername.Size = new System.Drawing.Size(363, 20);
            this.textBoxUsername.TabIndex = 8;
            this.textBoxUsername.Enter += new System.EventHandler(this.SettingsHelper);
            this.textBoxUsername.Leave += new System.EventHandler(this.SettingsHelperClear);
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.Location = new System.Drawing.Point(83, 67);
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.Size = new System.Drawing.Size(363, 20);
            this.textBoxDirectory.TabIndex = 6;
            this.textBoxDirectory.Enter += new System.EventHandler(this.SettingsHelper);
            this.textBoxDirectory.Leave += new System.EventHandler(this.SettingsHelperClear);
            // 
            // textBoxHost
            // 
            this.textBoxHost.Location = new System.Drawing.Point(83, 41);
            this.textBoxHost.Name = "textBoxHost";
            this.textBoxHost.Size = new System.Drawing.Size(291, 20);
            this.textBoxHost.TabIndex = 3;
            this.textBoxHost.Enter += new System.EventHandler(this.SettingsHelper);
            this.textBoxHost.Leave += new System.EventHandler(this.SettingsHelperClear);
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(3, 122);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(56, 13);
            this.labelPassword.TabIndex = 9;
            this.labelPassword.Text = "Password:";
            // 
            // labelDirectory
            // 
            this.labelDirectory.AutoSize = true;
            this.labelDirectory.Location = new System.Drawing.Point(3, 70);
            this.labelDirectory.Name = "labelDirectory";
            this.labelDirectory.Size = new System.Drawing.Size(52, 13);
            this.labelDirectory.TabIndex = 5;
            this.labelDirectory.Text = "Directory:";
            // 
            // labelUsername
            // 
            this.labelUsername.AutoSize = true;
            this.labelUsername.Location = new System.Drawing.Point(3, 96);
            this.labelUsername.Name = "labelUsername";
            this.labelUsername.Size = new System.Drawing.Size(58, 13);
            this.labelUsername.TabIndex = 7;
            this.labelUsername.Text = "Username:";
            // 
            // labelHost
            // 
            this.labelHost.AutoSize = true;
            this.labelHost.Location = new System.Drawing.Point(3, 44);
            this.labelHost.Name = "labelHost";
            this.labelHost.Size = new System.Drawing.Size(71, 13);
            this.labelHost.TabIndex = 2;
            this.labelHost.Text = "Host (FQDN):";
            // 
            // tabUpload
            // 
            this.tabUpload.Controls.Add(this.linkLabelReset);
            this.tabUpload.Controls.Add(this.labelCounter);
            this.tabUpload.Controls.Add(this.comboBoxFilename);
            this.tabUpload.Controls.Add(this.separatorNFU);
            this.tabUpload.Location = new System.Drawing.Point(4, 22);
            this.tabUpload.Name = "tabUpload";
            this.tabUpload.Size = new System.Drawing.Size(452, 171);
            this.tabUpload.TabIndex = 1;
            this.tabUpload.Text = "Upload";
            this.tabUpload.UseVisualStyleBackColor = true;
            // 
            // linkLabelReset
            // 
            this.linkLabelReset.AutoSize = true;
            this.linkLabelReset.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabelReset.Location = new System.Drawing.Point(412, 38);
            this.linkLabelReset.Name = "linkLabelReset";
            this.linkLabelReset.Size = new System.Drawing.Size(37, 13);
            this.linkLabelReset.TabIndex = 3;
            this.linkLabelReset.TabStop = true;
            this.linkLabelReset.Text = "Reset";
            this.linkLabelReset.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ResetCounter);
            // 
            // labelCounter
            // 
            this.labelCounter.AutoSize = true;
            this.labelCounter.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCounter.Location = new System.Drawing.Point(3, 38);
            this.labelCounter.Name = "labelCounter";
            this.labelCounter.Size = new System.Drawing.Size(91, 13);
            this.labelCounter.TabIndex = 2;
            this.labelCounter.Text = "Counter: 00000";
            // 
            // comboBoxFilename
            // 
            this.comboBoxFilename.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFilename.FormattingEnabled = true;
            this.comboBoxFilename.Items.AddRange(new object[] {
            "Use original filename",
            "Use generated filename"});
            this.comboBoxFilename.Location = new System.Drawing.Point(6, 6);
            this.comboBoxFilename.Name = "comboBoxFilename";
            this.comboBoxFilename.Size = new System.Drawing.Size(440, 21);
            this.comboBoxFilename.TabIndex = 0;
            // 
            // separatorNFU
            // 
            this.separatorNFU.Location = new System.Drawing.Point(0, 33);
            this.separatorNFU.MaximumSize = new System.Drawing.Size(2000, 2);
            this.separatorNFU.MinimumSize = new System.Drawing.Size(0, 2);
            this.separatorNFU.Name = "separatorNFU";
            this.separatorNFU.Size = new System.Drawing.Size(450, 2);
            this.separatorNFU.TabIndex = 1;
            this.separatorNFU.TabStop = false;
            // 
            // tabNFU
            // 
            this.tabNFU.Controls.Add(this.checkBoxQuickScreenshots);
            this.tabNFU.Controls.Add(this.checkBoxStartWindows);
            this.tabNFU.Controls.Add(this.checkBoxSytemTray);
            this.tabNFU.Controls.Add(this.comboBoxScreen);
            this.tabNFU.Controls.Add(this.checkBoxPause);
            this.tabNFU.Controls.Add(this.checkBoxPrintScreen);
            this.tabNFU.Controls.Add(this.checkBoxDebug);
            this.tabNFU.Controls.Add(this.seperatorNFU);
            this.tabNFU.Location = new System.Drawing.Point(4, 22);
            this.tabNFU.Name = "tabNFU";
            this.tabNFU.Padding = new System.Windows.Forms.Padding(3);
            this.tabNFU.Size = new System.Drawing.Size(452, 171);
            this.tabNFU.TabIndex = 2;
            this.tabNFU.Text = "NFU";
            this.tabNFU.UseVisualStyleBackColor = true;
            // 
            // checkBoxQuickScreenshots
            // 
            this.checkBoxQuickScreenshots.AutoSize = true;
            this.checkBoxQuickScreenshots.Location = new System.Drawing.Point(6, 87);
            this.checkBoxQuickScreenshots.Name = "checkBoxQuickScreenshots";
            this.checkBoxQuickScreenshots.Size = new System.Drawing.Size(114, 17);
            this.checkBoxQuickScreenshots.TabIndex = 4;
            this.checkBoxQuickScreenshots.Text = "Quick screenshots";
            this.checkBoxQuickScreenshots.UseVisualStyleBackColor = true;
            // 
            // checkBoxStartWindows
            // 
            this.checkBoxStartWindows.AutoSize = true;
            this.checkBoxStartWindows.Location = new System.Drawing.Point(6, 148);
            this.checkBoxStartWindows.Name = "checkBoxStartWindows";
            this.checkBoxStartWindows.Size = new System.Drawing.Size(117, 17);
            this.checkBoxStartWindows.TabIndex = 7;
            this.checkBoxStartWindows.Text = "Start with Windows";
            this.checkBoxStartWindows.UseVisualStyleBackColor = true;
            // 
            // checkBoxSytemTray
            // 
            this.checkBoxSytemTray.AutoSize = true;
            this.checkBoxSytemTray.Location = new System.Drawing.Point(313, 64);
            this.checkBoxSytemTray.Name = "checkBoxSytemTray";
            this.checkBoxSytemTray.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxSytemTray.Size = new System.Drawing.Size(133, 17);
            this.checkBoxSytemTray.TabIndex = 6;
            this.checkBoxSytemTray.Text = "Minimize to system tray";
            this.checkBoxSytemTray.UseVisualStyleBackColor = true;
            // 
            // comboBoxScreen
            // 
            this.comboBoxScreen.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxScreen.FormattingEnabled = true;
            this.comboBoxScreen.Location = new System.Drawing.Point(6, 6);
            this.comboBoxScreen.Name = "comboBoxScreen";
            this.comboBoxScreen.Size = new System.Drawing.Size(440, 21);
            this.comboBoxScreen.TabIndex = 0;
            // 
            // checkBoxPause
            // 
            this.checkBoxPause.AutoSize = true;
            this.checkBoxPause.Location = new System.Drawing.Point(6, 41);
            this.checkBoxPause.Name = "checkBoxPause";
            this.checkBoxPause.Size = new System.Drawing.Size(131, 17);
            this.checkBoxPause.TabIndex = 2;
            this.checkBoxPause.Text = "Handle the Pause key";
            this.checkBoxPause.UseVisualStyleBackColor = true;
            // 
            // checkBoxPrintScreen
            // 
            this.checkBoxPrintScreen.AutoSize = true;
            this.checkBoxPrintScreen.Location = new System.Drawing.Point(6, 64);
            this.checkBoxPrintScreen.Name = "checkBoxPrintScreen";
            this.checkBoxPrintScreen.Size = new System.Drawing.Size(159, 17);
            this.checkBoxPrintScreen.TabIndex = 3;
            this.checkBoxPrintScreen.Text = "Handle the Print Screen key";
            this.checkBoxPrintScreen.UseVisualStyleBackColor = true;
            // 
            // checkBoxDebug
            // 
            this.checkBoxDebug.AutoSize = true;
            this.checkBoxDebug.Location = new System.Drawing.Point(388, 41);
            this.checkBoxDebug.Name = "checkBoxDebug";
            this.checkBoxDebug.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.checkBoxDebug.Size = new System.Drawing.Size(58, 17);
            this.checkBoxDebug.TabIndex = 5;
            this.checkBoxDebug.Text = "Debug";
            this.checkBoxDebug.UseVisualStyleBackColor = true;
            // 
            // seperatorNFU
            // 
            this.seperatorNFU.Location = new System.Drawing.Point(0, 33);
            this.seperatorNFU.MaximumSize = new System.Drawing.Size(2000, 2);
            this.seperatorNFU.MinimumSize = new System.Drawing.Size(0, 2);
            this.seperatorNFU.Name = "seperatorNFU";
            this.seperatorNFU.Size = new System.Drawing.Size(450, 2);
            this.seperatorNFU.TabIndex = 1;
            this.seperatorNFU.TabStop = false;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(397, 215);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 3;
            this.buttonSave.Text = "&Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave);
            // 
            // labelHelpTitle
            // 
            this.labelHelpTitle.AutoSize = true;
            this.labelHelpTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHelpTitle.Location = new System.Drawing.Point(12, 212);
            this.labelHelpTitle.Name = "labelHelpTitle";
            this.labelHelpTitle.Size = new System.Drawing.Size(54, 13);
            this.labelHelpTitle.TabIndex = 1;
            this.labelHelpTitle.Text = "Warning";
            // 
            // labelHelpText
            // 
            this.labelHelpText.AutoSize = true;
            this.labelHelpText.Location = new System.Drawing.Point(12, 225);
            this.labelHelpText.Name = "labelHelpText";
            this.labelHelpText.Size = new System.Drawing.Size(236, 13);
            this.labelHelpText.TabIndex = 2;
            this.labelHelpText.Text = "Some settings may only take effect after a restart";
            // 
            // CP
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 250);
            this.Controls.Add(this.labelHelpText);
            this.Controls.Add(this.labelHelpTitle);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.tabControlCP);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CP";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Settings";
            this.Shown += new System.EventHandler(this.CPShown);
            this.tabControlCP.ResumeLayout(false);
            this.tabServer.ResumeLayout(false);
            this.tabServer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.tabUpload.ResumeLayout(false);
            this.tabUpload.PerformLayout();
            this.tabNFU.ResumeLayout(false);
            this.tabNFU.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlCP;
        private System.Windows.Forms.TabPage tabNFU;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkBoxDebug;
        private System.Windows.Forms.TabPage tabServer;
        private System.Windows.Forms.ComboBox comboBoxType;
        private LineSeparator seperatorServer;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxUsername;
        private System.Windows.Forms.TextBox textBoxHost;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.Label labelUsername;
        private System.Windows.Forms.Label labelHost;
        private System.Windows.Forms.TextBox textBoxDirectory;
        private System.Windows.Forms.Label labelDirectory;
        private System.Windows.Forms.CheckBox checkBoxShowPassword;
        private System.Windows.Forms.Label labelURL;
        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.CheckBox checkBoxPause;
        private System.Windows.Forms.CheckBox checkBoxPrintScreen;
        private System.Windows.Forms.Label labelHelpTitle;
        private System.Windows.Forms.ComboBox comboBoxScreen;
        private LineSeparator seperatorNFU;
        private System.Windows.Forms.Label labelHelpText;
        private System.Windows.Forms.TabPage tabUpload;
        private System.Windows.Forms.ComboBox comboBoxFilename;
        private LineSeparator separatorNFU;
        private System.Windows.Forms.CheckBox checkBoxSytemTray;
        private System.Windows.Forms.CheckBox checkBoxStartWindows;
        private System.Windows.Forms.LinkLabel linkLabelReset;
        private System.Windows.Forms.Label labelCounter;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.CheckBox checkBoxQuickScreenshots;

    }
}