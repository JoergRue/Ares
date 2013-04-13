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
            this.label4 = new System.Windows.Forms.Label();
            this.allSoundsFadeButton = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.fadeOutSoundsUpDown = new System.Windows.Forms.NumericUpDown();
            this.fadeOutSoundsBox = new System.Windows.Forms.CheckBox();
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
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.hideInPlayerBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fadeOutSoundsUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.triggerDescLabel);
            this.groupBox1.Controls.Add(this.selectKeyButton);
            this.groupBox1.Controls.Add(this.label1);
            this.errorProvider.SetError(this.groupBox1, resources.GetString("groupBox1.Error"));
            this.errorProvider.SetIconAlignment(this.groupBox1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBox1, ((int)(resources.GetObject("groupBox1.IconPadding"))));
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // triggerDescLabel
            // 
            resources.ApplyResources(this.triggerDescLabel, "triggerDescLabel");
            this.errorProvider.SetError(this.triggerDescLabel, resources.GetString("triggerDescLabel.Error"));
            this.errorProvider.SetIconAlignment(this.triggerDescLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("triggerDescLabel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.triggerDescLabel, ((int)(resources.GetObject("triggerDescLabel.IconPadding"))));
            this.triggerDescLabel.Name = "triggerDescLabel";
            this.triggerDescLabel.UseCompatibleTextRendering = true;
            // 
            // selectKeyButton
            // 
            resources.ApplyResources(this.selectKeyButton, "selectKeyButton");
            this.errorProvider.SetError(this.selectKeyButton, resources.GetString("selectKeyButton.Error"));
            this.selectKeyButton.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.errorProvider.SetIconAlignment(this.selectKeyButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("selectKeyButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.selectKeyButton, ((int)(resources.GetObject("selectKeyButton.IconPadding"))));
            this.selectKeyButton.Name = "selectKeyButton";
            this.selectKeyButton.UseCompatibleTextRendering = true;
            this.selectKeyButton.UseVisualStyleBackColor = true;
            this.selectKeyButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.allSoundsFadeButton);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.fadeOutSoundsUpDown);
            this.groupBox2.Controls.Add(this.fadeOutSoundsBox);
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
            this.errorProvider.SetError(this.groupBox2, resources.GetString("groupBox2.Error"));
            this.errorProvider.SetIconAlignment(this.groupBox2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBox2, ((int)(resources.GetObject("groupBox2.IconPadding"))));
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.errorProvider.SetError(this.label4, resources.GetString("label4.Error"));
            this.errorProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.label4.Name = "label4";
            // 
            // allSoundsFadeButton
            // 
            resources.ApplyResources(this.allSoundsFadeButton, "allSoundsFadeButton");
            this.errorProvider.SetError(this.allSoundsFadeButton, resources.GetString("allSoundsFadeButton.Error"));
            this.errorProvider.SetIconAlignment(this.allSoundsFadeButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("allSoundsFadeButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.allSoundsFadeButton, ((int)(resources.GetObject("allSoundsFadeButton.IconPadding"))));
            this.allSoundsFadeButton.Name = "allSoundsFadeButton";
            this.allSoundsFadeButton.UseVisualStyleBackColor = true;
            this.allSoundsFadeButton.Click += new System.EventHandler(this.allSoundsFadeButton_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.errorProvider.SetError(this.label5, resources.GetString("label5.Error"));
            this.errorProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.label5.Name = "label5";
            // 
            // fadeOutSoundsUpDown
            // 
            resources.ApplyResources(this.fadeOutSoundsUpDown, "fadeOutSoundsUpDown");
            this.errorProvider.SetError(this.fadeOutSoundsUpDown, resources.GetString("fadeOutSoundsUpDown.Error"));
            this.errorProvider.SetIconAlignment(this.fadeOutSoundsUpDown, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fadeOutSoundsUpDown.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.fadeOutSoundsUpDown, ((int)(resources.GetObject("fadeOutSoundsUpDown.IconPadding"))));
            this.fadeOutSoundsUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.fadeOutSoundsUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.fadeOutSoundsUpDown.Name = "fadeOutSoundsUpDown";
            this.fadeOutSoundsUpDown.ValueChanged += new System.EventHandler(this.fadeOutSoundsUpDown_ValueChanged);
            // 
            // fadeOutSoundsBox
            // 
            resources.ApplyResources(this.fadeOutSoundsBox, "fadeOutSoundsBox");
            this.errorProvider.SetError(this.fadeOutSoundsBox, resources.GetString("fadeOutSoundsBox.Error"));
            this.errorProvider.SetIconAlignment(this.fadeOutSoundsBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fadeOutSoundsBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.fadeOutSoundsBox, ((int)(resources.GetObject("fadeOutSoundsBox.IconPadding"))));
            this.fadeOutSoundsBox.Name = "fadeOutSoundsBox";
            this.fadeOutSoundsBox.UseVisualStyleBackColor = true;
            this.fadeOutSoundsBox.CheckedChanged += new System.EventHandler(this.fadeOutSoundsBox_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // crossFadeButton
            // 
            resources.ApplyResources(this.crossFadeButton, "crossFadeButton");
            this.errorProvider.SetError(this.crossFadeButton, resources.GetString("crossFadeButton.Error"));
            this.errorProvider.SetIconAlignment(this.crossFadeButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("crossFadeButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.crossFadeButton, ((int)(resources.GetObject("crossFadeButton.IconPadding"))));
            this.crossFadeButton.Name = "crossFadeButton";
            this.crossFadeButton.TabStop = true;
            this.crossFadeButton.UseVisualStyleBackColor = true;
            this.crossFadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // fadeButton
            // 
            resources.ApplyResources(this.fadeButton, "fadeButton");
            this.errorProvider.SetError(this.fadeButton, resources.GetString("fadeButton.Error"));
            this.errorProvider.SetIconAlignment(this.fadeButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("fadeButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.fadeButton, ((int)(resources.GetObject("fadeButton.IconPadding"))));
            this.fadeButton.Name = "fadeButton";
            this.fadeButton.TabStop = true;
            this.fadeButton.UseVisualStyleBackColor = true;
            this.fadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // noFadeButton
            // 
            resources.ApplyResources(this.noFadeButton, "noFadeButton");
            this.errorProvider.SetError(this.noFadeButton, resources.GetString("noFadeButton.Error"));
            this.errorProvider.SetIconAlignment(this.noFadeButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("noFadeButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.noFadeButton, ((int)(resources.GetObject("noFadeButton.IconPadding"))));
            this.noFadeButton.Name = "noFadeButton";
            this.noFadeButton.TabStop = true;
            this.noFadeButton.UseVisualStyleBackColor = true;
            this.noFadeButton.CheckedChanged += new System.EventHandler(this.crossFadingBox_CheckedChanged);
            // 
            // allCrossFadeButton
            // 
            resources.ApplyResources(this.allCrossFadeButton, "allCrossFadeButton");
            this.errorProvider.SetError(this.allCrossFadeButton, resources.GetString("allCrossFadeButton.Error"));
            this.errorProvider.SetIconAlignment(this.allCrossFadeButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("allCrossFadeButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.allCrossFadeButton, ((int)(resources.GetObject("allCrossFadeButton.IconPadding"))));
            this.allCrossFadeButton.Name = "allCrossFadeButton";
            this.allCrossFadeButton.UseVisualStyleBackColor = true;
            this.allCrossFadeButton.Click += new System.EventHandler(this.allCrossFadeButton_Click);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // crossFadingUpDown
            // 
            resources.ApplyResources(this.crossFadingUpDown, "crossFadingUpDown");
            this.errorProvider.SetError(this.crossFadingUpDown, resources.GetString("crossFadingUpDown.Error"));
            this.errorProvider.SetIconAlignment(this.crossFadingUpDown, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("crossFadingUpDown.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.crossFadingUpDown, ((int)(resources.GetObject("crossFadingUpDown.IconPadding"))));
            this.crossFadingUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
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
            this.errorProvider.SetError(this.stopMusicLabel, resources.GetString("stopMusicLabel.Error"));
            this.errorProvider.SetIconAlignment(this.stopMusicLabel, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stopMusicLabel.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stopMusicLabel, ((int)(resources.GetObject("stopMusicLabel.IconPadding"))));
            this.stopMusicLabel.Name = "stopMusicLabel";
            this.stopMusicLabel.UseCompatibleTextRendering = true;
            // 
            // stopMusicBox
            // 
            resources.ApplyResources(this.stopMusicBox, "stopMusicBox");
            this.errorProvider.SetError(this.stopMusicBox, resources.GetString("stopMusicBox.Error"));
            this.errorProvider.SetIconAlignment(this.stopMusicBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stopMusicBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stopMusicBox, ((int)(resources.GetObject("stopMusicBox.IconPadding"))));
            this.stopMusicBox.Name = "stopMusicBox";
            this.stopMusicBox.UseCompatibleTextRendering = true;
            this.stopMusicBox.UseVisualStyleBackColor = true;
            this.stopMusicBox.CheckedChanged += new System.EventHandler(this.stopMusicBox_CheckedChanged);
            // 
            // stopSoundsBox
            // 
            resources.ApplyResources(this.stopSoundsBox, "stopSoundsBox");
            this.errorProvider.SetError(this.stopSoundsBox, resources.GetString("stopSoundsBox.Error"));
            this.errorProvider.SetIconAlignment(this.stopSoundsBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("stopSoundsBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.stopSoundsBox, ((int)(resources.GetObject("stopSoundsBox.IconPadding"))));
            this.stopSoundsBox.Name = "stopSoundsBox";
            this.stopSoundsBox.UseCompatibleTextRendering = true;
            this.stopSoundsBox.UseVisualStyleBackColor = true;
            this.stopSoundsBox.CheckedChanged += new System.EventHandler(this.stopSoundsBox_CheckedChanged);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.hideInPlayerBox);
            this.errorProvider.SetError(this.groupBox3, resources.GetString("groupBox3.Error"));
            this.errorProvider.SetIconAlignment(this.groupBox3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("groupBox3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.groupBox3, ((int)(resources.GetObject("groupBox3.IconPadding"))));
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.UseCompatibleTextRendering = true;
            // 
            // hideInPlayerBox
            // 
            resources.ApplyResources(this.hideInPlayerBox, "hideInPlayerBox");
            this.errorProvider.SetError(this.hideInPlayerBox, resources.GetString("hideInPlayerBox.Error"));
            this.errorProvider.SetIconAlignment(this.hideInPlayerBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("hideInPlayerBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.hideInPlayerBox, ((int)(resources.GetObject("hideInPlayerBox.IconPadding"))));
            this.hideInPlayerBox.Name = "hideInPlayerBox";
            this.hideInPlayerBox.UseVisualStyleBackColor = true;
            this.hideInPlayerBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // TriggerEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox3);
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
            ((System.ComponentModel.ISupportInitialize)(this.fadeOutSoundsUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox hideInPlayerBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button allSoundsFadeButton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown fadeOutSoundsUpDown;
        private System.Windows.Forms.CheckBox fadeOutSoundsBox;
    }
}