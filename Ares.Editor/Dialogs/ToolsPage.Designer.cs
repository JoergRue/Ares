/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
namespace Ares.Editor.Dialogs
{
    partial class ToolsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolsPage));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.selectMusicPlayerButton = new System.Windows.Forms.Button();
            this.musicPlayerBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectFileEditorButton = new System.Windows.Forms.Button();
            this.audioFileEditorBox = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.selectMusicPlayerButton);
            this.groupBox2.Controls.Add(this.musicPlayerBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // selectMusicPlayerButton
            // 
            resources.ApplyResources(this.selectMusicPlayerButton, "selectMusicPlayerButton");
            this.selectMusicPlayerButton.Name = "selectMusicPlayerButton";
            this.selectMusicPlayerButton.UseCompatibleTextRendering = true;
            this.selectMusicPlayerButton.UseVisualStyleBackColor = true;
            this.selectMusicPlayerButton.Click += new System.EventHandler(this.selectMusicPlayerButton_Click);
            // 
            // musicPlayerBox
            // 
            resources.ApplyResources(this.musicPlayerBox, "musicPlayerBox");
            this.musicPlayerBox.Name = "musicPlayerBox";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.selectFileEditorButton);
            this.groupBox1.Controls.Add(this.audioFileEditorBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // selectFileEditorButton
            // 
            resources.ApplyResources(this.selectFileEditorButton, "selectFileEditorButton");
            this.selectFileEditorButton.Name = "selectFileEditorButton";
            this.selectFileEditorButton.UseCompatibleTextRendering = true;
            this.selectFileEditorButton.UseVisualStyleBackColor = true;
            this.selectFileEditorButton.Click += new System.EventHandler(this.selectFileEditorButton_Click);
            // 
            // audioFileEditorBox
            // 
            resources.ApplyResources(this.audioFileEditorBox, "audioFileEditorBox");
            this.audioFileEditorBox.Name = "audioFileEditorBox";
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // ToolsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ToolsPage";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button selectMusicPlayerButton;
        private System.Windows.Forms.TextBox musicPlayerBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button selectFileEditorButton;
        private System.Windows.Forms.TextBox audioFileEditorBox;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}
