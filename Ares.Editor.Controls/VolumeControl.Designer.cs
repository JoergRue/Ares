/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
namespace Ares.Editor.Controls
{
    partial class VolumeControl
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
                if (m_Element != null)
                    Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolumeControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.soundVolumeBar = new System.Windows.Forms.TrackBar();
            this.setsSoundBox = new System.Windows.Forms.CheckBox();
            this.musicVolumeBar = new System.Windows.Forms.TrackBar();
            this.setsMusicBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.soundVolumeBar);
            this.groupBox1.Controls.Add(this.setsSoundBox);
            this.groupBox1.Controls.Add(this.musicVolumeBar);
            this.groupBox1.Controls.Add(this.setsMusicBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // soundVolumeBar
            // 
            resources.ApplyResources(this.soundVolumeBar, "soundVolumeBar");
            this.soundVolumeBar.Maximum = 100;
            this.soundVolumeBar.Name = "soundVolumeBar";
            this.soundVolumeBar.SmallChange = 5;
            this.soundVolumeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // setsSoundBox
            // 
            resources.ApplyResources(this.setsSoundBox, "setsSoundBox");
            this.setsSoundBox.Name = "setsSoundBox";
            this.setsSoundBox.UseCompatibleTextRendering = true;
            this.setsSoundBox.UseVisualStyleBackColor = true;
            this.setsSoundBox.CheckedChanged += new System.EventHandler(this.setsSoundBox_CheckedChanged);
            // 
            // musicVolumeBar
            // 
            resources.ApplyResources(this.musicVolumeBar, "musicVolumeBar");
            this.musicVolumeBar.Maximum = 100;
            this.musicVolumeBar.Name = "musicVolumeBar";
            this.musicVolumeBar.SmallChange = 5;
            this.musicVolumeBar.TickFrequency = 0;
            this.musicVolumeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.musicVolumeBar.ValueChanged += new System.EventHandler(this.musicVolumeBar_ValueChanged);
            // 
            // setsMusicBox
            // 
            resources.ApplyResources(this.setsMusicBox, "setsMusicBox");
            this.setsMusicBox.Name = "setsMusicBox";
            this.setsMusicBox.UseCompatibleTextRendering = true;
            this.setsMusicBox.UseVisualStyleBackColor = true;
            this.setsMusicBox.CheckedChanged += new System.EventHandler(this.setsMusicBox_CheckedChanged);
            // 
            // VolumeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(297, 83);
            this.Name = "VolumeControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar soundVolumeBar;
        private System.Windows.Forms.CheckBox setsSoundBox;
        private System.Windows.Forms.TrackBar musicVolumeBar;
        private System.Windows.Forms.CheckBox setsMusicBox;
    }
}
