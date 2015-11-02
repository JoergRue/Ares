/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
namespace Ares.Editor.ElementEditors
{
    partial class ChoiceContainerEditor
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
                Actions.ElementChanges.Instance.RemoveListener(-1, Update);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChoiceContainerEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.choiceContainerControl = new Ares.Editor.ElementEditorControls.ChoiceContainerControl();
            this.volumeControl = new Ares.Editor.Controls.VolumeControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.playButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Image = global::Ares.Editor.ImageResources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            // 
            // playButton
            // 
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Name = "playButton";
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // choiceContainerControl
            // 
            resources.ApplyResources(this.choiceContainerControl, "choiceContainerControl");
            this.choiceContainerControl.Name = "choiceContainerControl";
            // 
            // volumeControl
            // 
            resources.ApplyResources(this.volumeControl, "volumeControl");
            this.volumeControl.Name = "volumeControl";
            // 
            // ChoiceContainerEditor
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.choiceContainerControl);
            this.Controls.Add(this.volumeControl);
            this.Name = "ChoiceContainerEditor";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.VolumeControl volumeControl;
        private ElementEditorControls.ChoiceContainerControl choiceContainerControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Label label1;
    }
}