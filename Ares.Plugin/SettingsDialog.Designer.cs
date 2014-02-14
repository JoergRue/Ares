namespace Ares.MediaPortalPlugin
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
            this.playerPathButton = new System.Windows.Forms.Button();
            this.playerPathBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.musicFilesButton = new System.Windows.Forms.Button();
            this.soundFilesButton = new System.Windows.Forms.Button();
            this.projectFileButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.musicFilesBox = new System.Windows.Forms.TextBox();
            this.soundFilesBox = new System.Windows.Forms.TextBox();
            this.projectFileBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.projectFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ipAddressBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tcpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.udpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.playerPathDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.playerPathButton);
            this.groupBox1.Controls.Add(this.playerPathBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.musicFilesButton);
            this.groupBox1.Controls.Add(this.soundFilesButton);
            this.groupBox1.Controls.Add(this.projectFileButton);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.musicFilesBox);
            this.groupBox1.Controls.Add(this.soundFilesBox);
            this.groupBox1.Controls.Add(this.projectFileBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // playerPathButton
            // 
            resources.ApplyResources(this.playerPathButton, "playerPathButton");
            this.playerPathButton.Name = "playerPathButton";
            this.playerPathButton.UseCompatibleTextRendering = true;
            this.playerPathButton.UseVisualStyleBackColor = true;
            this.playerPathButton.Click += new System.EventHandler(this.playerPathButton_Click);
            // 
            // playerPathBox
            // 
            resources.ApplyResources(this.playerPathBox, "playerPathBox");
            this.playerPathBox.Name = "playerPathBox";
            this.playerPathBox.ReadOnly = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.label7.UseCompatibleTextRendering = true;
            // 
            // musicFilesButton
            // 
            resources.ApplyResources(this.musicFilesButton, "musicFilesButton");
            this.musicFilesButton.Name = "musicFilesButton";
            this.musicFilesButton.UseCompatibleTextRendering = true;
            this.musicFilesButton.UseVisualStyleBackColor = true;
            this.musicFilesButton.Click += new System.EventHandler(this.musicFilesButton_Click);
            // 
            // soundFilesButton
            // 
            resources.ApplyResources(this.soundFilesButton, "soundFilesButton");
            this.soundFilesButton.Name = "soundFilesButton";
            this.soundFilesButton.UseCompatibleTextRendering = true;
            this.soundFilesButton.UseVisualStyleBackColor = true;
            this.soundFilesButton.Click += new System.EventHandler(this.soundFilesButton_Click);
            // 
            // projectFileButton
            // 
            resources.ApplyResources(this.projectFileButton, "projectFileButton");
            this.projectFileButton.Name = "projectFileButton";
            this.projectFileButton.UseCompatibleTextRendering = true;
            this.projectFileButton.UseVisualStyleBackColor = true;
            this.projectFileButton.Click += new System.EventHandler(this.projectFileButton_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // musicFilesBox
            // 
            resources.ApplyResources(this.musicFilesBox, "musicFilesBox");
            this.musicFilesBox.Name = "musicFilesBox";
            this.musicFilesBox.ReadOnly = true;
            // 
            // soundFilesBox
            // 
            resources.ApplyResources(this.soundFilesBox, "soundFilesBox");
            this.soundFilesBox.Name = "soundFilesBox";
            this.soundFilesBox.ReadOnly = true;
            // 
            // projectFileBox
            // 
            resources.ApplyResources(this.projectFileBox, "projectFileBox");
            this.projectFileBox.Name = "projectFileBox";
            this.projectFileBox.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // projectFileDialog
            // 
            this.projectFileDialog.DefaultExt = "ares";
            this.projectFileDialog.FileName = "openFileDialog1";
            resources.ApplyResources(this.projectFileDialog, "projectFileDialog");
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.ipAddressBox);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.tcpPortUpDown);
            this.groupBox2.Controls.Add(this.udpPortUpDown);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // ipAddressBox
            // 
            this.ipAddressBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ipAddressBox.FormattingEnabled = true;
            resources.ApplyResources(this.ipAddressBox, "ipAddressBox");
            this.ipAddressBox.Name = "ipAddressBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.UseCompatibleTextRendering = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // tcpPortUpDown
            // 
            resources.ApplyResources(this.tcpPortUpDown, "tcpPortUpDown");
            this.tcpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.tcpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.tcpPortUpDown.Name = "tcpPortUpDown";
            this.tcpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // udpPortUpDown
            // 
            resources.ApplyResources(this.udpPortUpDown, "udpPortUpDown");
            this.udpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.udpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udpPortUpDown.Name = "udpPortUpDown";
            this.udpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // playerPathDialog
            // 
            this.playerPathDialog.FileName = "Ares.CmdLinePlayer.exe";
            // 
            // SettingsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingsDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button musicFilesButton;
        private System.Windows.Forms.Button soundFilesButton;
        private System.Windows.Forms.Button projectFileButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox musicFilesBox;
        private System.Windows.Forms.TextBox soundFilesBox;
        private System.Windows.Forms.TextBox projectFileBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog projectFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.NumericUpDown udpPortUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown tcpPortUpDown;
        private System.Windows.Forms.ComboBox ipAddressBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button playerPathButton;
        private System.Windows.Forms.TextBox playerPathBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.OpenFileDialog playerPathDialog;
    }
}