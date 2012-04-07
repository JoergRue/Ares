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
namespace Ares.Editor.ElementEditors
{
    partial class TriggerEditor
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
                    Ares.Editor.Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
                    Ares.Editor.Actions.ElementChanges.Instance.RemoveListener(-1, Update);
                }
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriggerEditor));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.triggerDescLabel = new System.Windows.Forms.Label();
            this.selectKeyButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.crossFadeButton = new System.Windows.Forms.RadioButton();
            this.fadeButton = new System.Windows.Forms.RadioButton();
            this.noFadeButton = new System.Windows.Forms.RadioButton();
            this.allCrossFadeButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.crossFadingUpDown = new System.Windows.Forms.NumericUpDown();
            this.stopMusicLabel = new System.Windows.Forms.Label();
            this.stopMusicBox = new System.Windows.Forms.CheckBox();
            this.stopSoundsBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.triggerDescLabel);
            this.groupBox1.Controls.Add(this.selectKeyButton);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // triggerDescLabel
            // 
            resources.ApplyResources(this.triggerDescLabel, "triggerDescLabel");
            this.triggerDescLabel.Name = "triggerDescLabel";
            this.triggerDescLabel.UseCompatibleTextRendering = true;
            // 
            // selectKeyButton
            // 
            this.selectKeyButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            resources.ApplyResources(this.selectKeyButton, "selectKeyButton");
            this.selectKeyButton.Name = "selectKeyButton";
            this.selectKeyButton.UseCompatibleTextRendering = true;
            this.selectKeyButton.UseVisualStyleBackColor = true;
            this.selectKeyButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.crossFadeButton);
            this.groupBox2.Controls.Add(this.fadeButton);
            this.groupBox2.Controls.Add(this.noFadeButton);
            this.groupBox2.Controls.Add(this.allCrossFadeButton);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.crossFadingUpDown);
            this.groupBox2.Controls.Add(this.stopMusicLabel);
            this.groupBox2.Controls.Add(this.stopMusicBox);
            this.groupBox2.Controls.Add(this.stopSoundsBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // crossFadeButton
            // 
            resources.ApplyResources(this.crossFadeButton, "crossFadeButton");
            this.crossFadeButton.Name = "crossFadeButton";
            this.crossFadeButton.TabStop = true;
            this.crossFadeButton.UseVisualStyleBackColor = true;
            this.crossFadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // fadeButton
            // 
            resources.ApplyResources(this.fadeButton, "fadeButton");
            this.fadeButton.Name = "fadeButton";
            this.fadeButton.TabStop = true;
            this.fadeButton.UseVisualStyleBackColor = true;
            this.fadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // noFadeButton
            // 
            resources.ApplyResources(this.noFadeButton, "noFadeButton");
            this.noFadeButton.Name = "noFadeButton";
            this.noFadeButton.TabStop = true;
            this.noFadeButton.UseVisualStyleBackColor = true;
            this.noFadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // allCrossFadeButton
            // 
            resources.ApplyResources(this.allCrossFadeButton, "allCrossFadeButton");
            this.allCrossFadeButton.Name = "allCrossFadeButton";
            this.allCrossFadeButton.UseVisualStyleBackColor = true;
            this.allCrossFadeButton.Click += new System.EventHandler(this.allCrossFadeButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // crossFadingUpDown
            // 
            this.crossFadingUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.crossFadingUpDown, "crossFadingUpDown");
            this.crossFadingUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.crossFadingUpDown.Name = "crossFadingUpDown";
            this.crossFadingUpDown.ValueChanged += new System.EventHandler(this.crossFadingUpDown_ValueChanged);
            // 
            // stopMusicLabel
            // 
            resources.ApplyResources(this.stopMusicLabel, "stopMusicLabel");
            this.stopMusicLabel.Name = "stopMusicLabel";
            this.stopMusicLabel.UseCompatibleTextRendering = true;
            // 
            // stopMusicBox
            // 
            resources.ApplyResources(this.stopMusicBox, "stopMusicBox");
            this.stopMusicBox.Name = "stopMusicBox";
            this.stopMusicBox.UseCompatibleTextRendering = true;
            this.stopMusicBox.UseVisualStyleBackColor = true;
            this.stopMusicBox.CheckedChanged += new System.EventHandler(this.stopMusicBox_CheckedChanged);
            // 
            // stopSoundsBox
            // 
            resources.ApplyResources(this.stopSoundsBox, "stopSoundsBox");
            this.stopSoundsBox.Name = "stopSoundsBox";
            this.stopSoundsBox.UseCompatibleTextRendering = true;
            this.stopSoundsBox.UseVisualStyleBackColor = true;
            this.stopSoundsBox.CheckedChanged += new System.EventHandler(this.stopSoundsBox_CheckedChanged);
            // 
            // TriggerEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "TriggerEditor";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label triggerDescLabel;
        private System.Windows.Forms.Button selectKeyButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox stopMusicBox;
        private System.Windows.Forms.CheckBox stopSoundsBox;
        private System.Windows.Forms.Label stopMusicLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown crossFadingUpDown;
        private System.Windows.Forms.Button allCrossFadeButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton crossFadeButton;
        private System.Windows.Forms.RadioButton fadeButton;
        private System.Windows.Forms.RadioButton noFadeButton;
    }
}