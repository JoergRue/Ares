namespace Ares.Player
{
    partial class Player
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Player));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.aboutButton = new System.Windows.Forms.Button();
            this.settingsButton = new System.Windows.Forms.Button();
            this.openButton = new System.Windows.Forms.Button();
            this.projectNameLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.soundVolumeBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.musicVolumeBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.overallVolumeBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.soundsLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.musicLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.elementsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.modeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.settingsFileWatcher = new System.IO.FileSystemWatcher();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsFileWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.aboutButton);
            this.groupBox1.Controls.Add(this.settingsButton);
            this.groupBox1.Controls.Add(this.openButton);
            this.groupBox1.Controls.Add(this.projectNameLabel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(581, 44);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // aboutButton
            // 
            this.aboutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.aboutButton.Image = ((System.Drawing.Image)(resources.GetObject("aboutButton.Image")));
            this.aboutButton.Location = new System.Drawing.Point(541, 10);
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(32, 23);
            this.aboutButton.TabIndex = 4;
            this.aboutButton.UseCompatibleTextRendering = true;
            this.aboutButton.UseVisualStyleBackColor = true;
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.Location = new System.Drawing.Point(503, 11);
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(32, 23);
            this.settingsButton.TabIndex = 3;
            this.settingsButton.UseCompatibleTextRendering = true;
            this.settingsButton.UseVisualStyleBackColor = true;
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.Location = new System.Drawing.Point(465, 10);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(32, 23);
            this.openButton.TabIndex = 2;
            this.openButton.UseCompatibleTextRendering = true;
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // projectNameLabel
            // 
            this.projectNameLabel.Location = new System.Drawing.Point(57, 16);
            this.projectNameLabel.Name = "projectNameLabel";
            this.projectNameLabel.Size = new System.Drawing.Size(180, 17);
            this.projectNameLabel.TabIndex = 1;
            this.projectNameLabel.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Loaded:";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.soundVolumeBar);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.musicVolumeBar);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.overallVolumeBar);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Location = new System.Drawing.Point(12, 62);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(581, 175);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Volume";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // soundVolumeBar
            // 
            this.soundVolumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.soundVolumeBar.Location = new System.Drawing.Point(57, 118);
            this.soundVolumeBar.Maximum = 100;
            this.soundVolumeBar.Name = "soundVolumeBar";
            this.soundVolumeBar.Size = new System.Drawing.Size(516, 45);
            this.soundVolumeBar.SmallChange = 5;
            this.soundVolumeBar.TabIndex = 5;
            this.soundVolumeBar.TickFrequency = 5;
            this.soundVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.soundVolumeBar.Value = 100;
            this.soundVolumeBar.Scroll += new System.EventHandler(this.soundVolumeBar_Scroll);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 17);
            this.label4.TabIndex = 4;
            this.label4.Text = "Sounds:";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // musicVolumeBar
            // 
            this.musicVolumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.musicVolumeBar.Location = new System.Drawing.Point(57, 67);
            this.musicVolumeBar.Maximum = 100;
            this.musicVolumeBar.Name = "musicVolumeBar";
            this.musicVolumeBar.Size = new System.Drawing.Size(516, 45);
            this.musicVolumeBar.SmallChange = 5;
            this.musicVolumeBar.TabIndex = 3;
            this.musicVolumeBar.TickFrequency = 5;
            this.musicVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.musicVolumeBar.Value = 100;
            this.musicVolumeBar.Scroll += new System.EventHandler(this.musicVolumeBar_Scroll);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Music:";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // overallVolumeBar
            // 
            this.overallVolumeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.overallVolumeBar.Location = new System.Drawing.Point(57, 16);
            this.overallVolumeBar.Maximum = 100;
            this.overallVolumeBar.Name = "overallVolumeBar";
            this.overallVolumeBar.Size = new System.Drawing.Size(516, 45);
            this.overallVolumeBar.SmallChange = 5;
            this.overallVolumeBar.TabIndex = 1;
            this.overallVolumeBar.TickFrequency = 5;
            this.overallVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.overallVolumeBar.Value = 100;
            this.overallVolumeBar.Scroll += new System.EventHandler(this.overallVolumeBar_Scroll);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "Overall:";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.soundsLabel);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.musicLabel);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.elementsLabel);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.modeLabel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Location = new System.Drawing.Point(12, 243);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(581, 180);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Status";
            this.groupBox3.UseCompatibleTextRendering = true;
            // 
            // soundsLabel
            // 
            this.soundsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.soundsLabel.Location = new System.Drawing.Point(67, 131);
            this.soundsLabel.Name = "soundsLabel";
            this.soundsLabel.Size = new System.Drawing.Size(506, 36);
            this.soundsLabel.TabIndex = 7;
            this.soundsLabel.UseCompatibleTextRendering = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 131);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 17);
            this.label8.TabIndex = 6;
            this.label8.Text = "Sounds:";
            this.label8.UseCompatibleTextRendering = true;
            // 
            // musicLabel
            // 
            this.musicLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.musicLabel.Location = new System.Drawing.Point(67, 81);
            this.musicLabel.Name = "musicLabel";
            this.musicLabel.Size = new System.Drawing.Size(506, 40);
            this.musicLabel.TabIndex = 5;
            this.musicLabel.UseCompatibleTextRendering = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 81);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 17);
            this.label7.TabIndex = 4;
            this.label7.Text = "Music:";
            this.label7.UseCompatibleTextRendering = true;
            // 
            // elementsLabel
            // 
            this.elementsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.elementsLabel.Location = new System.Drawing.Point(67, 53);
            this.elementsLabel.Name = "elementsLabel";
            this.elementsLabel.Size = new System.Drawing.Size(506, 17);
            this.elementsLabel.TabIndex = 3;
            this.elementsLabel.UseCompatibleTextRendering = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 17);
            this.label6.TabIndex = 2;
            this.label6.Text = "Elements:";
            this.label6.UseCompatibleTextRendering = true;
            // 
            // modeLabel
            // 
            this.modeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.modeLabel.Location = new System.Drawing.Point(67, 26);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(506, 17);
            this.modeLabel.TabIndex = 1;
            this.modeLabel.UseCompatibleTextRendering = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(36, 17);
            this.label5.TabIndex = 0;
            this.label5.Text = "Mode:";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "ares";
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.Filter = "*.ares";
            this.fileSystemWatcher1.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // settingsFileWatcher
            // 
            this.settingsFileWatcher.EnableRaisingEvents = true;
            this.settingsFileWatcher.Filter = "*.xml";
            this.settingsFileWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.settingsFileWatcher.SynchronizingObject = this;
            this.settingsFileWatcher.Changed += new System.IO.FileSystemEventHandler(this.settingsFileWatcher_Changed);
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 435);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.KeyPreview = true;
            this.Name = "Player";
            this.Text = "Ares Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Player_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Player_KeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.settingsFileWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button settingsButton;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Label projectNameLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar soundVolumeBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar musicVolumeBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar overallVolumeBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label soundsLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label musicLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label elementsLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Button aboutButton;
        private System.IO.FileSystemWatcher settingsFileWatcher;
    }
}

