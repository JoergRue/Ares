/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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
    partial class FileTagsEditor
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
                Ares.Editor.Actions.TagChanges.Instance.TagsDBChanged -= new System.EventHandler<System.EventArgs>(TagsDBChanged);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileTagsEditor));
            this.languageBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tagsBox = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.categoriesBox = new System.Windows.Forms.ListBox();
            this.addTagButton = new System.Windows.Forms.Button();
            this.downloadButton = new System.Windows.Forms.Button();
            this.shareButton = new System.Windows.Forms.Button();
            this.confirmButton = new System.Windows.Forms.Button();
            this.id3Button = new System.Windows.Forms.Button();
            this.titleLabel = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.musicBrainzButton = new System.Windows.Forms.Button();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // languageBox
            // 
            resources.ApplyResources(this.languageBox, "languageBox");
            this.languageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageBox.FormattingEnabled = true;
            this.languageBox.Name = "languageBox";
            this.toolTip1.SetToolTip(this.languageBox, resources.GetString("languageBox.ToolTip"));
            this.languageBox.SelectedIndexChanged += new System.EventHandler(this.languageBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.toolTip1.SetToolTip(this.label1, resources.GetString("label1.ToolTip"));
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.tagsBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox2, resources.GetString("groupBox2.ToolTip"));
            // 
            // tagsBox
            // 
            resources.ApplyResources(this.tagsBox, "tagsBox");
            this.tagsBox.CheckOnClick = true;
            this.tagsBox.FormattingEnabled = true;
            this.tagsBox.MultiColumn = true;
            this.tagsBox.Name = "tagsBox";
            this.toolTip1.SetToolTip(this.tagsBox, resources.GetString("tagsBox.ToolTip"));
            this.tagsBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.tagsBox_ItemCheck);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.categoriesBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.groupBox1, resources.GetString("groupBox1.ToolTip"));
            // 
            // categoriesBox
            // 
            resources.ApplyResources(this.categoriesBox, "categoriesBox");
            this.categoriesBox.FormattingEnabled = true;
            this.categoriesBox.Name = "categoriesBox";
            this.toolTip1.SetToolTip(this.categoriesBox, resources.GetString("categoriesBox.ToolTip"));
            this.categoriesBox.SelectedIndexChanged += new System.EventHandler(this.categoriesBox_SelectedIndexChanged);
            // 
            // addTagButton
            // 
            resources.ApplyResources(this.addTagButton, "addTagButton");
            this.addTagButton.Name = "addTagButton";
            this.toolTip1.SetToolTip(this.addTagButton, resources.GetString("addTagButton.ToolTip"));
            this.addTagButton.UseVisualStyleBackColor = true;
            this.addTagButton.Click += new System.EventHandler(this.addTagButton_Click);
            // 
            // downloadButton
            // 
            resources.ApplyResources(this.downloadButton, "downloadButton");
            this.downloadButton.Name = "downloadButton";
            this.toolTip1.SetToolTip(this.downloadButton, resources.GetString("downloadButton.ToolTip"));
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // shareButton
            // 
            resources.ApplyResources(this.shareButton, "shareButton");
            this.shareButton.Name = "shareButton";
            this.toolTip1.SetToolTip(this.shareButton, resources.GetString("shareButton.ToolTip"));
            this.shareButton.UseVisualStyleBackColor = true;
            this.shareButton.Click += new System.EventHandler(this.shareButton_Click);
            // 
            // confirmButton
            // 
            resources.ApplyResources(this.confirmButton, "confirmButton");
            this.confirmButton.Name = "confirmButton";
            this.toolTip1.SetToolTip(this.confirmButton, resources.GetString("confirmButton.ToolTip"));
            this.confirmButton.UseVisualStyleBackColor = true;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // id3Button
            // 
            resources.ApplyResources(this.id3Button, "id3Button");
            this.id3Button.Name = "id3Button";
            this.toolTip1.SetToolTip(this.id3Button, resources.GetString("id3Button.ToolTip"));
            this.id3Button.UseVisualStyleBackColor = true;
            this.id3Button.Click += new System.EventHandler(this.id3Button_Click);
            // 
            // titleLabel
            // 
            resources.ApplyResources(this.titleLabel, "titleLabel");
            this.titleLabel.Name = "titleLabel";
            this.toolTip1.SetToolTip(this.titleLabel, resources.GetString("titleLabel.ToolTip"));
            // 
            // musicBrainzButton
            // 
            resources.ApplyResources(this.musicBrainzButton, "musicBrainzButton");
            this.musicBrainzButton.Name = "musicBrainzButton";
            this.toolTip1.SetToolTip(this.musicBrainzButton, resources.GetString("musicBrainzButton.ToolTip"));
            this.musicBrainzButton.UseVisualStyleBackColor = true;
            this.musicBrainzButton.Click += new System.EventHandler(this.musicBrainzButton_Click);
            // 
            // FileTagsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.musicBrainzButton);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.id3Button);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.shareButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.addTagButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.languageBox);
            this.Controls.Add(this.label1);
            this.Name = "FileTagsEditor";
            this.toolTip1.SetToolTip(this, resources.GetString("$this.ToolTip"));
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox languageBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox tagsBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox categoriesBox;
        private System.Windows.Forms.Button addTagButton;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.Button shareButton;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button id3Button;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button musicBrainzButton;
    }
}