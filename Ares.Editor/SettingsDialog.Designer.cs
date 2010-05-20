namespace Ares.Editor
{
    partial class SettingsDialog
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectOtherDirButton = new System.Windows.Forms.Button();
            this.otherDirButton = new System.Windows.Forms.RadioButton();
            this.appDirButton = new System.Windows.Forms.RadioButton();
            this.userDirButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.selectSoundButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.selectMusicButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.soundDirLabel = new Ares.Editor.PathLabel();
            this.musicDirLabel = new Ares.Editor.PathLabel();
            this.otherDirLabel = new Ares.Editor.PathLabel();
            this.appDirLabel = new Ares.Editor.PathLabel();
            this.userDirLabel = new Ares.Editor.PathLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.selectOtherDirButton);
            this.groupBox1.Controls.Add(this.otherDirLabel);
            this.groupBox1.Controls.Add(this.otherDirButton);
            this.groupBox1.Controls.Add(this.appDirLabel);
            this.groupBox1.Controls.Add(this.appDirButton);
            this.groupBox1.Controls.Add(this.userDirLabel);
            this.groupBox1.Controls.Add(this.userDirButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 113);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(410, 109);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings Storage";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // selectOtherDirButton
            // 
            this.selectOtherDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectOtherDirButton.Location = new System.Drawing.Point(372, 75);
            this.selectOtherDirButton.Name = "selectOtherDirButton";
            this.selectOtherDirButton.Size = new System.Drawing.Size(32, 23);
            this.selectOtherDirButton.TabIndex = 8;
            this.selectOtherDirButton.Text = "...";
            this.selectOtherDirButton.UseCompatibleTextRendering = true;
            this.selectOtherDirButton.UseVisualStyleBackColor = true;
            this.selectOtherDirButton.Click += new System.EventHandler(this.selectOtherDirButton_Click);
            // 
            // otherDirButton
            // 
            this.otherDirButton.AutoSize = true;
            this.otherDirButton.Location = new System.Drawing.Point(6, 77);
            this.otherDirButton.Name = "otherDirButton";
            this.otherDirButton.Size = new System.Drawing.Size(102, 18);
            this.otherDirButton.TabIndex = 4;
            this.otherDirButton.TabStop = true;
            this.otherDirButton.Text = "Other Directory:";
            this.otherDirButton.UseCompatibleTextRendering = true;
            this.otherDirButton.UseVisualStyleBackColor = true;
            // 
            // appDirButton
            // 
            this.appDirButton.AutoSize = true;
            this.appDirButton.Location = new System.Drawing.Point(6, 53);
            this.appDirButton.Name = "appDirButton";
            this.appDirButton.Size = new System.Drawing.Size(126, 18);
            this.appDirButton.TabIndex = 2;
            this.appDirButton.TabStop = true;
            this.appDirButton.Text = "Application Directory";
            this.appDirButton.UseCompatibleTextRendering = true;
            this.appDirButton.UseVisualStyleBackColor = true;
            // 
            // userDirButton
            // 
            this.userDirButton.AutoSize = true;
            this.userDirButton.Location = new System.Drawing.Point(6, 29);
            this.userDirButton.Name = "userDirButton";
            this.userDirButton.Size = new System.Drawing.Size(94, 18);
            this.userDirButton.TabIndex = 0;
            this.userDirButton.TabStop = true;
            this.userDirButton.Text = "User Directory";
            this.userDirButton.UseCompatibleTextRendering = true;
            this.userDirButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.soundDirLabel);
            this.groupBox2.Controls.Add(this.musicDirLabel);
            this.groupBox2.Controls.Add(this.selectSoundButton);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.selectMusicButton);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(410, 95);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Directories";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // selectSoundButton
            // 
            this.selectSoundButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectSoundButton.Location = new System.Drawing.Point(372, 54);
            this.selectSoundButton.Name = "selectSoundButton";
            this.selectSoundButton.Size = new System.Drawing.Size(32, 23);
            this.selectSoundButton.TabIndex = 5;
            this.selectSoundButton.Text = "...";
            this.selectSoundButton.UseCompatibleTextRendering = true;
            this.selectSoundButton.UseVisualStyleBackColor = true;
            this.selectSoundButton.Click += new System.EventHandler(this.selectSoundButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sound Files:";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // selectMusicButton
            // 
            this.selectMusicButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.selectMusicButton.Location = new System.Drawing.Point(372, 23);
            this.selectMusicButton.Name = "selectMusicButton";
            this.selectMusicButton.Size = new System.Drawing.Size(32, 23);
            this.selectMusicButton.TabIndex = 2;
            this.selectMusicButton.Text = "...";
            this.selectMusicButton.UseCompatibleTextRendering = true;
            this.selectMusicButton.UseVisualStyleBackColor = true;
            this.selectMusicButton.Click += new System.EventHandler(this.selectMusicButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Music Files:";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(347, 228);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(266, 228);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // soundDirLabel
            // 
            this.soundDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.soundDirLabel.Location = new System.Drawing.Point(77, 59);
            this.soundDirLabel.Name = "soundDirLabel";
            this.soundDirLabel.Size = new System.Drawing.Size(289, 17);
            this.soundDirLabel.TabIndex = 7;
            this.soundDirLabel.Text = "C:\\";
            this.soundDirLabel.UseCompatibleTextRendering = true;
            // 
            // musicDirLabel
            // 
            this.musicDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.musicDirLabel.Location = new System.Drawing.Point(77, 28);
            this.musicDirLabel.Name = "musicDirLabel";
            this.musicDirLabel.Size = new System.Drawing.Size(289, 17);
            this.musicDirLabel.TabIndex = 6;
            this.musicDirLabel.Text = "C:\\";
            this.musicDirLabel.UseCompatibleTextRendering = true;
            // 
            // otherDirLabel
            // 
            this.otherDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.otherDirLabel.Location = new System.Drawing.Point(139, 80);
            this.otherDirLabel.Name = "otherDirLabel";
            this.otherDirLabel.Size = new System.Drawing.Size(227, 18);
            this.otherDirLabel.TabIndex = 5;
            this.otherDirLabel.Text = "(C:\\)";
            this.otherDirLabel.UseCompatibleTextRendering = true;
            // 
            // appDirLabel
            // 
            this.appDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.appDirLabel.Location = new System.Drawing.Point(139, 56);
            this.appDirLabel.Name = "appDirLabel";
            this.appDirLabel.Size = new System.Drawing.Size(265, 18);
            this.appDirLabel.TabIndex = 3;
            this.appDirLabel.Text = "(C:\\)";
            this.appDirLabel.UseCompatibleTextRendering = true;
            // 
            // userDirLabel
            // 
            this.userDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userDirLabel.Location = new System.Drawing.Point(139, 32);
            this.userDirLabel.Name = "userDirLabel";
            this.userDirLabel.Size = new System.Drawing.Size(265, 18);
            this.userDirLabel.TabIndex = 1;
            this.userDirLabel.Text = "(C:\\)";
            this.userDirLabel.UseCompatibleTextRendering = true;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 259);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(300, 297);
            this.Name = "SettingsDialog";
            this.Text = "Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button selectSoundButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button selectMusicButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectOtherDirButton;
        private PathLabel otherDirLabel;
        private System.Windows.Forms.RadioButton otherDirButton;
        private PathLabel appDirLabel;
        private System.Windows.Forms.RadioButton appDirButton;
        private PathLabel userDirLabel;
        private System.Windows.Forms.RadioButton userDirButton;
        private PathLabel soundDirLabel;
        private PathLabel musicDirLabel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}