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
    partial class AddID3TagsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddID3TagsDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.moodBox = new System.Windows.Forms.CheckBox();
            this.genreBox = new System.Windows.Forms.CheckBox();
            this.albumBox = new System.Windows.Forms.CheckBox();
            this.interpreterBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.moodBox);
            this.groupBox1.Controls.Add(this.genreBox);
            this.groupBox1.Controls.Add(this.albumBox);
            this.groupBox1.Controls.Add(this.interpreterBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // moodBox
            // 
            resources.ApplyResources(this.moodBox, "moodBox");
            this.moodBox.Checked = true;
            this.moodBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.moodBox.Name = "moodBox";
            this.moodBox.UseVisualStyleBackColor = true;
            // 
            // genreBox
            // 
            resources.ApplyResources(this.genreBox, "genreBox");
            this.genreBox.Checked = true;
            this.genreBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.genreBox.Name = "genreBox";
            this.genreBox.UseVisualStyleBackColor = true;
            // 
            // albumBox
            // 
            resources.ApplyResources(this.albumBox, "albumBox");
            this.albumBox.Checked = true;
            this.albumBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.albumBox.Name = "albumBox";
            this.albumBox.UseVisualStyleBackColor = true;
            // 
            // interpreterBox
            // 
            resources.ApplyResources(this.interpreterBox, "interpreterBox");
            this.interpreterBox.Checked = true;
            this.interpreterBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.interpreterBox.Name = "interpreterBox";
            this.interpreterBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // AddID3TagsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddID3TagsDialog";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox genreBox;
        private System.Windows.Forms.CheckBox albumBox;
        private System.Windows.Forms.CheckBox interpreterBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox moodBox;
    }
}