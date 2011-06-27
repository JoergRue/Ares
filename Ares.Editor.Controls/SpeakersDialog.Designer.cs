namespace Ares.Editor.Controls
{
    partial class SpeakersDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpeakersDialog));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.oneSpeakerBox = new System.Windows.Forms.ComboBox();
            this.twoSpeakersBox = new System.Windows.Forms.ComboBox();
            this.randomButton = new System.Windows.Forms.RadioButton();
            this.twoSpeakersButton = new System.Windows.Forms.RadioButton();
            this.oneSpeakerButton = new System.Windows.Forms.RadioButton();
            this.defaultButton = new System.Windows.Forms.RadioButton();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.playButton);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Image = global::Ares.Editor.Controls.Properties.Resources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Image = global::Ares.Editor.Controls.Properties.Resources.RunSmall;
            this.playButton.Name = "playButton";
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.oneSpeakerBox);
            this.groupBox1.Controls.Add(this.twoSpeakersBox);
            this.groupBox1.Controls.Add(this.randomButton);
            this.groupBox1.Controls.Add(this.twoSpeakersButton);
            this.groupBox1.Controls.Add(this.oneSpeakerButton);
            this.groupBox1.Controls.Add(this.defaultButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // oneSpeakerBox
            // 
            resources.ApplyResources(this.oneSpeakerBox, "oneSpeakerBox");
            this.oneSpeakerBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.oneSpeakerBox.FormattingEnabled = true;
            this.oneSpeakerBox.Items.AddRange(new object[] {
            resources.GetString("oneSpeakerBox.Items"),
            resources.GetString("oneSpeakerBox.Items1"),
            resources.GetString("oneSpeakerBox.Items2"),
            resources.GetString("oneSpeakerBox.Items3"),
            resources.GetString("oneSpeakerBox.Items4"),
            resources.GetString("oneSpeakerBox.Items5"),
            resources.GetString("oneSpeakerBox.Items6"),
            resources.GetString("oneSpeakerBox.Items7")});
            this.oneSpeakerBox.Name = "oneSpeakerBox";
            // 
            // twoSpeakersBox
            // 
            resources.ApplyResources(this.twoSpeakersBox, "twoSpeakersBox");
            this.twoSpeakersBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.twoSpeakersBox.FormattingEnabled = true;
            this.twoSpeakersBox.Items.AddRange(new object[] {
            resources.GetString("twoSpeakersBox.Items"),
            resources.GetString("twoSpeakersBox.Items1"),
            resources.GetString("twoSpeakersBox.Items2"),
            resources.GetString("twoSpeakersBox.Items3")});
            this.twoSpeakersBox.Name = "twoSpeakersBox";
            // 
            // randomButton
            // 
            resources.ApplyResources(this.randomButton, "randomButton");
            this.randomButton.Name = "randomButton";
            this.randomButton.TabStop = true;
            this.randomButton.UseCompatibleTextRendering = true;
            this.randomButton.UseVisualStyleBackColor = true;
            this.randomButton.CheckedChanged += new System.EventHandler(this.randomButton_CheckedChanged);
            // 
            // twoSpeakersButton
            // 
            resources.ApplyResources(this.twoSpeakersButton, "twoSpeakersButton");
            this.twoSpeakersButton.Name = "twoSpeakersButton";
            this.twoSpeakersButton.TabStop = true;
            this.twoSpeakersButton.UseCompatibleTextRendering = true;
            this.twoSpeakersButton.UseVisualStyleBackColor = true;
            this.twoSpeakersButton.CheckedChanged += new System.EventHandler(this.twoSpeakersButton_CheckedChanged);
            // 
            // oneSpeakerButton
            // 
            resources.ApplyResources(this.oneSpeakerButton, "oneSpeakerButton");
            this.oneSpeakerButton.Name = "oneSpeakerButton";
            this.oneSpeakerButton.TabStop = true;
            this.oneSpeakerButton.UseCompatibleTextRendering = true;
            this.oneSpeakerButton.UseVisualStyleBackColor = true;
            this.oneSpeakerButton.CheckedChanged += new System.EventHandler(this.oneSpeakerButton_CheckedChanged);
            // 
            // defaultButton
            // 
            resources.ApplyResources(this.defaultButton, "defaultButton");
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.TabStop = true;
            this.defaultButton.UseCompatibleTextRendering = true;
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.CheckedChanged += new System.EventHandler(this.defaultButton_CheckedChanged);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SpeakersDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "SpeakersDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SpeakersDialog_FormClosing);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox oneSpeakerBox;
        private System.Windows.Forms.ComboBox twoSpeakersBox;
        private System.Windows.Forms.RadioButton randomButton;
        private System.Windows.Forms.RadioButton twoSpeakersButton;
        private System.Windows.Forms.RadioButton oneSpeakerButton;
        private System.Windows.Forms.RadioButton defaultButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}