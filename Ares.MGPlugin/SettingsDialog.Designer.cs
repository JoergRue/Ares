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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.askBeforePlayerStartBox = new System.Windows.Forms.CheckBox();
            this.startLocalPlayerBox = new System.Windows.Forms.CheckBox();
            this.udpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.playerPathBox = new System.Windows.Forms.TextBox();
            this.selectPlayerButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.selectPlayerButton);
            this.groupBox1.Controls.Add(this.playerPathBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.askBeforePlayerStartBox);
            this.groupBox1.Controls.Add(this.startLocalPlayerBox);
            this.groupBox1.Controls.Add(this.udpPortUpDown);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(332, 145);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Player";
            // 
            // askBeforePlayerStartBox
            // 
            this.askBeforePlayerStartBox.AutoSize = true;
            this.askBeforePlayerStartBox.Location = new System.Drawing.Point(9, 80);
            this.askBeforePlayerStartBox.Name = "askBeforePlayerStartBox";
            this.askBeforePlayerStartBox.Size = new System.Drawing.Size(219, 17);
            this.askBeforePlayerStartBox.TabIndex = 3;
            this.askBeforePlayerStartBox.Text = "Nachfragen vor Start des lokalen Players";
            this.askBeforePlayerStartBox.UseVisualStyleBackColor = true;
            // 
            // startLocalPlayerBox
            // 
            this.startLocalPlayerBox.AutoSize = true;
            this.startLocalPlayerBox.Location = new System.Drawing.Point(9, 57);
            this.startLocalPlayerBox.Name = "startLocalPlayerBox";
            this.startLocalPlayerBox.Size = new System.Drawing.Size(303, 17);
            this.startLocalPlayerBox.TabIndex = 2;
            this.startLocalPlayerBox.Text = "Lokalen Player starten, wenn keiner im Netz gefunden wird";
            this.startLocalPlayerBox.UseVisualStyleBackColor = true;
            // 
            // udpPortUpDown
            // 
            this.udpPortUpDown.Location = new System.Drawing.Point(122, 25);
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
            this.label1.Location = new System.Drawing.Point(6, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Port für Player-Suche:";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(269, 163);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Abbrechen";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(188, 163);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Pfad zum lokalen Player:";
            // 
            // playerPathBox
            // 
            this.playerPathBox.Location = new System.Drawing.Point(9, 116);
            this.playerPathBox.Name = "playerPathBox";
            this.playerPathBox.ReadOnly = true;
            this.playerPathBox.Size = new System.Drawing.Size(268, 20);
            this.playerPathBox.TabIndex = 5;
            // 
            // selectPlayerButton
            // 
            this.selectPlayerButton.Location = new System.Drawing.Point(290, 114);
            this.selectPlayerButton.Name = "selectPlayerButton";
            this.selectPlayerButton.Size = new System.Drawing.Size(36, 23);
            this.selectPlayerButton.TabIndex = 6;
            this.selectPlayerButton.Text = "...";
            this.selectPlayerButton.UseVisualStyleBackColor = true;
            this.selectPlayerButton.Click += new System.EventHandler(this.selectPlayerButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "Ares.Player.exe";
            this.openFileDialog1.Filter = "Ares Player|Ares.Player.exe";
            // 
            // SettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 195);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Ares Controller Einstellungen";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
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
    }
}