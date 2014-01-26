namespace Ares.MGPlugin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            this.selectPlayerButton = new System.Windows.Forms.Button();
            this.playerPathBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.askBeforePlayerStartBox = new System.Windows.Forms.CheckBox();
            this.startLocalPlayerBox = new System.Windows.Forms.CheckBox();
            this.udpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.playerPage = new System.Windows.Forms.TabPage();
            this.musicPage = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.crossFadeButton = new System.Windows.Forms.RadioButton();
            this.fadeButton = new System.Windows.Forms.RadioButton();
            this.noFadeButton = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.crossFadingUpDown = new System.Windows.Forms.NumericUpDown();
            this.allChannelsCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.playerPage.SuspendLayout();
            this.musicPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // selectPlayerButton
            // 
            this.selectPlayerButton.Location = new System.Drawing.Point(297, 104);
            this.selectPlayerButton.Name = "selectPlayerButton";
            this.selectPlayerButton.Size = new System.Drawing.Size(36, 23);
            this.selectPlayerButton.TabIndex = 6;
            this.selectPlayerButton.Text = "...";
            this.selectPlayerButton.UseVisualStyleBackColor = true;
            this.selectPlayerButton.Click += new System.EventHandler(this.selectPlayerButton_Click);
            // 
            // playerPathBox
            // 
            this.playerPathBox.Location = new System.Drawing.Point(16, 106);
            this.playerPathBox.Name = "playerPathBox";
            this.playerPathBox.ReadOnly = true;
            this.playerPathBox.Size = new System.Drawing.Size(268, 20);
            this.playerPathBox.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pfad zum lokalen Player:";
            // 
            // askBeforePlayerStartBox
            // 
            this.askBeforePlayerStartBox.AutoSize = true;
            this.askBeforePlayerStartBox.Location = new System.Drawing.Point(16, 70);
            this.askBeforePlayerStartBox.Name = "askBeforePlayerStartBox";
            this.askBeforePlayerStartBox.Size = new System.Drawing.Size(219, 17);
            this.askBeforePlayerStartBox.TabIndex = 3;
            this.askBeforePlayerStartBox.Text = "Nachfragen vor Start des lokalen Players";
            this.askBeforePlayerStartBox.UseVisualStyleBackColor = true;
            // 
            // startLocalPlayerBox
            // 
            this.startLocalPlayerBox.AutoSize = true;
            this.startLocalPlayerBox.Location = new System.Drawing.Point(16, 47);
            this.startLocalPlayerBox.Name = "startLocalPlayerBox";
            this.startLocalPlayerBox.Size = new System.Drawing.Size(303, 17);
            this.startLocalPlayerBox.TabIndex = 2;
            this.startLocalPlayerBox.Text = "Lokalen Player starten, wenn keiner im Netz gefunden wird";
            this.startLocalPlayerBox.UseVisualStyleBackColor = true;
            // 
            // udpPortUpDown
            // 
            this.udpPortUpDown.Location = new System.Drawing.Point(129, 15);
            this.udpPortUpDown.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.udpPortUpDown.Name = "udpPortUpDown";
            this.udpPortUpDown.Size = new System.Drawing.Size(83, 20);
            this.udpPortUpDown.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port für Player-Suche:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(290, 208);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Abbrechen";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(209, 208);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "Ares.Player.exe";
            this.openFileDialog1.Filter = "Ares Player|Ares.Player.exe";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.playerPage);
            this.tabControl1.Controls.Add(this.musicPage);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(354, 186);
            this.tabControl1.TabIndex = 3;
            // 
            // playerPage
            // 
            this.playerPage.Controls.Add(this.selectPlayerButton);
            this.playerPage.Controls.Add(this.startLocalPlayerBox);
            this.playerPage.Controls.Add(this.playerPathBox);
            this.playerPage.Controls.Add(this.label1);
            this.playerPage.Controls.Add(this.label2);
            this.playerPage.Controls.Add(this.udpPortUpDown);
            this.playerPage.Controls.Add(this.askBeforePlayerStartBox);
            this.playerPage.Location = new System.Drawing.Point(4, 22);
            this.playerPage.Name = "playerPage";
            this.playerPage.Padding = new System.Windows.Forms.Padding(3);
            this.playerPage.Size = new System.Drawing.Size(346, 160);
            this.playerPage.TabIndex = 0;
            this.playerPage.Text = "Player";
            // 
            // musicPage
            // 
            this.musicPage.Controls.Add(this.label3);
            this.musicPage.Controls.Add(this.label4);
            this.musicPage.Controls.Add(this.crossFadeButton);
            this.musicPage.Controls.Add(this.fadeButton);
            this.musicPage.Controls.Add(this.noFadeButton);
            this.musicPage.Controls.Add(this.label5);
            this.musicPage.Controls.Add(this.crossFadingUpDown);
            this.musicPage.Controls.Add(this.allChannelsCheckBox);
            this.musicPage.Location = new System.Drawing.Point(4, 22);
            this.musicPage.Name = "musicPage";
            this.musicPage.Padding = new System.Windows.Forms.Padding(3);
            this.musicPage.Size = new System.Drawing.Size(346, 160);
            this.musicPage.TabIndex = 1;
            this.musicPage.Text = "Musik";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(8, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(235, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Blenden, wenn die Musik manuell geändert wird:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(29, 127);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Blendzeit:";
            // 
            // crossFadeButton
            // 
            this.crossFadeButton.AutoSize = true;
            this.crossFadeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.crossFadeButton.Location = new System.Drawing.Point(28, 102);
            this.crossFadeButton.Name = "crossFadeButton";
            this.crossFadeButton.Size = new System.Drawing.Size(86, 17);
            this.crossFadeButton.TabIndex = 23;
            this.crossFadeButton.TabStop = true;
            this.crossFadeButton.Text = "Überblenden";
            this.crossFadeButton.UseVisualStyleBackColor = true;
            this.crossFadeButton.CheckedChanged += new System.EventHandler(this.crossFadeButton_CheckedChanged);
            // 
            // fadeButton
            // 
            this.fadeButton.AutoSize = true;
            this.fadeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.fadeButton.Location = new System.Drawing.Point(28, 79);
            this.fadeButton.Name = "fadeButton";
            this.fadeButton.Size = new System.Drawing.Size(167, 17);
            this.fadeButton.TabIndex = 22;
            this.fadeButton.TabStop = true;
            this.fadeButton.Text = "Ausblenden, dann Einblenden";
            this.fadeButton.UseVisualStyleBackColor = true;
            this.fadeButton.CheckedChanged += new System.EventHandler(this.fadeButton_CheckedChanged);
            // 
            // noFadeButton
            // 
            this.noFadeButton.AutoSize = true;
            this.noFadeButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.noFadeButton.Location = new System.Drawing.Point(28, 56);
            this.noFadeButton.Name = "noFadeButton";
            this.noFadeButton.Size = new System.Drawing.Size(88, 17);
            this.noFadeButton.TabIndex = 21;
            this.noFadeButton.TabStop = true;
            this.noFadeButton.Text = "Kein Blenden";
            this.noFadeButton.UseVisualStyleBackColor = true;
            this.noFadeButton.CheckedChanged += new System.EventHandler(this.noFadeButton_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(175, 127);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "ms";
            // 
            // crossFadingUpDown
            // 
            this.crossFadingUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.crossFadingUpDown.Location = new System.Drawing.Point(102, 125);
            this.crossFadingUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.crossFadingUpDown.Name = "crossFadingUpDown";
            this.crossFadingUpDown.Size = new System.Drawing.Size(67, 20);
            this.crossFadingUpDown.TabIndex = 19;
            // 
            // allChannelsCheckBox
            // 
            this.allChannelsCheckBox.AutoSize = true;
            this.allChannelsCheckBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.allChannelsCheckBox.Location = new System.Drawing.Point(8, 6);
            this.allChannelsCheckBox.Name = "allChannelsCheckBox";
            this.allChannelsCheckBox.Size = new System.Drawing.Size(216, 17);
            this.allChannelsCheckBox.TabIndex = 18;
            this.allChannelsCheckBox.Text = "Musik auf allen Lautsprechern abspielen";
            this.allChannelsCheckBox.UseVisualStyleBackColor = true;
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 243);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ares Controller Einstellungen";
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.playerPage.ResumeLayout(false);
            this.playerPage.PerformLayout();
            this.musicPage.ResumeLayout(false);
            this.musicPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox startLocalPlayerBox;
        private System.Windows.Forms.NumericUpDown udpPortUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox askBeforePlayerStartBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button selectPlayerButton;
        private System.Windows.Forms.TextBox playerPathBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage playerPage;
        private System.Windows.Forms.TabPage musicPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.RadioButton crossFadeButton;
        private System.Windows.Forms.RadioButton fadeButton;
        private System.Windows.Forms.RadioButton noFadeButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown crossFadingUpDown;
        private System.Windows.Forms.CheckBox allChannelsCheckBox;
    }
}