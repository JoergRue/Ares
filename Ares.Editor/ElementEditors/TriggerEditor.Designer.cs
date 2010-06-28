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
            this.stopMusicLabel = new System.Windows.Forms.Label();
            this.stopMusicBox = new System.Windows.Forms.CheckBox();
            this.stopSoundsBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.groupBox2.SuspendLayout();
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
    }
}