﻿namespace Ares.Editor.Controls
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
            this.allPitchButton = new System.Windows.Forms.Button();
            this.pitchButton = new System.Windows.Forms.Button();
            this.pitchBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.allPitchButton);
            this.groupBox1.Controls.Add(this.pitchButton);
            this.groupBox1.Controls.Add(this.pitchBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
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
    }
}
