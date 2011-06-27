namespace Ares.Editor.Controls
{
    partial class FileEffectsControl
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
            if (disposing)
            {
                if (m_Element != null)
                {
                    Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
                }
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileEffectsControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.allSpeakerButton = new System.Windows.Forms.Button();
            this.speakerButton = new System.Windows.Forms.Button();
            this.speakerBox = new System.Windows.Forms.CheckBox();
            this.allVolumeButton = new System.Windows.Forms.Button();
            this.volumeButton = new System.Windows.Forms.Button();
            this.volumeBox = new System.Windows.Forms.CheckBox();
            this.allBalanceButton = new System.Windows.Forms.Button();
            this.balanceButton = new System.Windows.Forms.Button();
            this.balanceBox = new System.Windows.Forms.CheckBox();
            this.allPitchButton = new System.Windows.Forms.Button();
            this.pitchButton = new System.Windows.Forms.Button();
            this.pitchBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.allSpeakerButton);
            this.groupBox1.Controls.Add(this.speakerButton);
            this.groupBox1.Controls.Add(this.speakerBox);
            this.groupBox1.Controls.Add(this.allVolumeButton);
            this.groupBox1.Controls.Add(this.volumeButton);
            this.groupBox1.Controls.Add(this.volumeBox);
            this.groupBox1.Controls.Add(this.allBalanceButton);
            this.groupBox1.Controls.Add(this.balanceButton);
            this.groupBox1.Controls.Add(this.balanceBox);
            this.groupBox1.Controls.Add(this.allPitchButton);
            this.groupBox1.Controls.Add(this.pitchButton);
            this.groupBox1.Controls.Add(this.pitchBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // allSpeakerButton
            // 
            resources.ApplyResources(this.allSpeakerButton, "allSpeakerButton");
            this.allSpeakerButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allSpeakerButton.Name = "allSpeakerButton";
            this.allSpeakerButton.UseCompatibleTextRendering = true;
            this.allSpeakerButton.UseVisualStyleBackColor = true;
            this.allSpeakerButton.Click += new System.EventHandler(this.allSpeakerButton_Click);
            // 
            // speakerButton
            // 
            resources.ApplyResources(this.speakerButton, "speakerButton");
            this.speakerButton.Name = "speakerButton";
            this.speakerButton.UseCompatibleTextRendering = true;
            this.speakerButton.UseVisualStyleBackColor = true;
            this.speakerButton.Click += new System.EventHandler(this.speakerButton_Click);
            // 
            // speakerBox
            // 
            resources.ApplyResources(this.speakerBox, "speakerBox");
            this.speakerBox.Name = "speakerBox";
            this.speakerBox.UseCompatibleTextRendering = true;
            this.speakerBox.UseVisualStyleBackColor = true;
            this.speakerBox.CheckedChanged += new System.EventHandler(this.speakerBox_CheckedChanged);
            // 
            // allVolumeButton
            // 
            resources.ApplyResources(this.allVolumeButton, "allVolumeButton");
            this.allVolumeButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allVolumeButton.Name = "allVolumeButton";
            this.allVolumeButton.UseCompatibleTextRendering = true;
            this.allVolumeButton.UseVisualStyleBackColor = true;
            this.allVolumeButton.Click += new System.EventHandler(this.allVolumeButton_Click);
            // 
            // volumeButton
            // 
            resources.ApplyResources(this.volumeButton, "volumeButton");
            this.volumeButton.Name = "volumeButton";
            this.volumeButton.UseCompatibleTextRendering = true;
            this.volumeButton.UseVisualStyleBackColor = true;
            this.volumeButton.Click += new System.EventHandler(this.volumeButton_Click);
            // 
            // volumeBox
            // 
            resources.ApplyResources(this.volumeBox, "volumeBox");
            this.volumeBox.Name = "volumeBox";
            this.volumeBox.UseCompatibleTextRendering = true;
            this.volumeBox.UseVisualStyleBackColor = true;
            this.volumeBox.CheckedChanged += new System.EventHandler(this.volumeBox_CheckedChanged);
            // 
            // allBalanceButton
            // 
            resources.ApplyResources(this.allBalanceButton, "allBalanceButton");
            this.allBalanceButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allBalanceButton.Name = "allBalanceButton";
            this.allBalanceButton.UseCompatibleTextRendering = true;
            this.allBalanceButton.UseVisualStyleBackColor = true;
            this.allBalanceButton.Click += new System.EventHandler(this.allBalanceButton_Click);
            // 
            // balanceButton
            // 
            resources.ApplyResources(this.balanceButton, "balanceButton");
            this.balanceButton.Name = "balanceButton";
            this.balanceButton.UseCompatibleTextRendering = true;
            this.balanceButton.UseVisualStyleBackColor = true;
            this.balanceButton.Click += new System.EventHandler(this.balanceButton_Click);
            // 
            // balanceBox
            // 
            resources.ApplyResources(this.balanceBox, "balanceBox");
            this.balanceBox.Name = "balanceBox";
            this.balanceBox.UseCompatibleTextRendering = true;
            this.balanceBox.UseVisualStyleBackColor = true;
            this.balanceBox.CheckedChanged += new System.EventHandler(this.balanceBox_CheckedChanged);
            // 
            // allPitchButton
            // 
            resources.ApplyResources(this.allPitchButton, "allPitchButton");
            this.allPitchButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allPitchButton.Name = "allPitchButton";
            this.allPitchButton.UseCompatibleTextRendering = true;
            this.allPitchButton.UseVisualStyleBackColor = true;
            this.allPitchButton.Click += new System.EventHandler(this.allPitchButton_Click);
            // 
            // pitchButton
            // 
            resources.ApplyResources(this.pitchButton, "pitchButton");
            this.pitchButton.Name = "pitchButton";
            this.pitchButton.UseCompatibleTextRendering = true;
            this.pitchButton.UseVisualStyleBackColor = true;
            this.pitchButton.Click += new System.EventHandler(this.pitchButton_Click);
            // 
            // pitchBox
            // 
            resources.ApplyResources(this.pitchBox, "pitchBox");
            this.pitchBox.Name = "pitchBox";
            this.pitchBox.UseCompatibleTextRendering = true;
            this.pitchBox.UseVisualStyleBackColor = true;
            this.pitchBox.CheckedChanged += new System.EventHandler(this.pitchBox_CheckedChanged);
            // 
            // FileEffectsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FileEffectsControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox pitchBox;
        private System.Windows.Forms.Button pitchButton;
        private System.Windows.Forms.Button allPitchButton;
        private System.Windows.Forms.Button allBalanceButton;
        private System.Windows.Forms.Button balanceButton;
        private System.Windows.Forms.CheckBox balanceBox;
        private System.Windows.Forms.Button allVolumeButton;
        private System.Windows.Forms.Button volumeButton;
        private System.Windows.Forms.CheckBox volumeBox;
        private System.Windows.Forms.Button allSpeakerButton;
        private System.Windows.Forms.Button speakerButton;
        private System.Windows.Forms.CheckBox speakerBox;
    }
}
