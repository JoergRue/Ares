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
    partial class FileVolumeControl
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileVolumeControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.maxRandomUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.minRandomUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.randomButton = new System.Windows.Forms.RadioButton();
            this.fixedVolumeButton = new System.Windows.Forms.RadioButton();
            this.fadeOutUnitBox = new System.Windows.Forms.ComboBox();
            this.fadeInUnitBox = new System.Windows.Forms.ComboBox();
            this.allFadeOutButton = new System.Windows.Forms.Button();
            this.fadeOutUpDown = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.allFadeInButton = new System.Windows.Forms.Button();
            this.fadeInUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.allVolumeButton = new System.Windows.Forms.Button();
            this.volumeBar = new System.Windows.Forms.TrackBar();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRandomUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRandomUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fadeOutUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fadeInUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.maxRandomUpDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.minRandomUpDown);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.randomButton);
            this.groupBox1.Controls.Add(this.fixedVolumeButton);
            this.groupBox1.Controls.Add(this.fadeOutUnitBox);
            this.groupBox1.Controls.Add(this.fadeInUnitBox);
            this.groupBox1.Controls.Add(this.allFadeOutButton);
            this.groupBox1.Controls.Add(this.fadeOutUpDown);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.allFadeInButton);
            this.groupBox1.Controls.Add(this.fadeInUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.allVolumeButton);
            this.groupBox1.Controls.Add(this.volumeBar);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // maxRandomUpDown
            // 
            resources.ApplyResources(this.maxRandomUpDown, "maxRandomUpDown");
            this.maxRandomUpDown.Name = "maxRandomUpDown";
            this.maxRandomUpDown.ValueChanged += new System.EventHandler(this.maxRandomUpDown_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // minRandomUpDown
            // 
            resources.ApplyResources(this.minRandomUpDown, "minRandomUpDown");
            this.minRandomUpDown.Name = "minRandomUpDown";
            this.minRandomUpDown.ValueChanged += new System.EventHandler(this.minRandomUpDown_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
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
            // fixedVolumeButton
            // 
            resources.ApplyResources(this.fixedVolumeButton, "fixedVolumeButton");
            this.fixedVolumeButton.Name = "fixedVolumeButton";
            this.fixedVolumeButton.TabStop = true;
            this.fixedVolumeButton.UseCompatibleTextRendering = true;
            this.fixedVolumeButton.UseVisualStyleBackColor = true;
            this.fixedVolumeButton.CheckedChanged += new System.EventHandler(this.fixedVolumeButton_CheckedChanged);
            // 
            // fadeOutUnitBox
            // 
            resources.ApplyResources(this.fadeOutUnitBox, "fadeOutUnitBox");
            this.fadeOutUnitBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fadeOutUnitBox.FormattingEnabled = true;
            this.fadeOutUnitBox.Items.AddRange(new object[] {
            resources.GetString("fadeOutUnitBox.Items"),
            resources.GetString("fadeOutUnitBox.Items1"),
            resources.GetString("fadeOutUnitBox.Items2")});
            this.fadeOutUnitBox.Name = "fadeOutUnitBox";
            this.fadeOutUnitBox.SelectedIndexChanged += new System.EventHandler(this.fadeOutUnitBox_SelectedIndexChanged);
            // 
            // fadeInUnitBox
            // 
            resources.ApplyResources(this.fadeInUnitBox, "fadeInUnitBox");
            this.fadeInUnitBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fadeInUnitBox.FormattingEnabled = true;
            this.fadeInUnitBox.Items.AddRange(new object[] {
            resources.GetString("fadeInUnitBox.Items"),
            resources.GetString("fadeInUnitBox.Items1"),
            resources.GetString("fadeInUnitBox.Items2")});
            this.fadeInUnitBox.Name = "fadeInUnitBox";
            this.fadeInUnitBox.SelectedIndexChanged += new System.EventHandler(this.fadeInUnitBox_SelectedIndexChanged);
            // 
            // allFadeOutButton
            // 
            resources.ApplyResources(this.allFadeOutButton, "allFadeOutButton");
            this.allFadeOutButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allFadeOutButton.Name = "allFadeOutButton";
            this.allFadeOutButton.UseCompatibleTextRendering = true;
            this.allFadeOutButton.UseVisualStyleBackColor = true;
            this.allFadeOutButton.Click += new System.EventHandler(this.allFadeOutButton_Click);
            // 
            // fadeOutUpDown
            // 
            resources.ApplyResources(this.fadeOutUpDown, "fadeOutUpDown");
            this.fadeOutUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.fadeOutUpDown.Name = "fadeOutUpDown";
            this.fadeOutUpDown.ValueChanged += new System.EventHandler(this.fadeOutUpDown_ValueChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // allFadeInButton
            // 
            resources.ApplyResources(this.allFadeInButton, "allFadeInButton");
            this.allFadeInButton.Image = global::Ares.Editor.Controls.Properties.Resources.Multiple_Selection;
            this.allFadeInButton.Name = "allFadeInButton";
            this.allFadeInButton.UseCompatibleTextRendering = true;
            this.allFadeInButton.UseVisualStyleBackColor = true;
            this.allFadeInButton.Click += new System.EventHandler(this.allFadeInButton_Click);
            // 
            // fadeInUpDown
            // 
            resources.ApplyResources(this.fadeInUpDown, "fadeInUpDown");
            this.fadeInUpDown.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.fadeInUpDown.Name = "fadeInUpDown";
            this.fadeInUpDown.ValueChanged += new System.EventHandler(this.fadeInUpDown_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
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
            // volumeBar
            // 
            resources.ApplyResources(this.volumeBar, "volumeBar");
            this.volumeBar.LargeChange = 10;
            this.volumeBar.Maximum = 100;
            this.volumeBar.Name = "volumeBar";
            this.volumeBar.TickFrequency = 10;
            this.volumeBar.ValueChanged += new System.EventHandler(this.volumeBar_ValueChanged);
            // 
            // FileVolumeControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "FileVolumeControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRandomUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRandomUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fadeOutUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fadeInUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.volumeBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TrackBar volumeBar;
        private System.Windows.Forms.Button allVolumeButton;
        private System.Windows.Forms.Button allFadeOutButton;
        private System.Windows.Forms.NumericUpDown fadeOutUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button allFadeInButton;
        private System.Windows.Forms.NumericUpDown fadeInUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox fadeInUnitBox;
        private System.Windows.Forms.ComboBox fadeOutUnitBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown maxRandomUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown minRandomUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton randomButton;
        private System.Windows.Forms.RadioButton fixedVolumeButton;
    }
}
