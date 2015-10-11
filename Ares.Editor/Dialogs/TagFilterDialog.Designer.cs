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
namespace Ares.Editor.Dialogs
{
    partial class TagFilterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagFilterDialog));
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tagsBox = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.categoriesBox = new System.Windows.Forms.ListBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tagCategoryCombinationBox = new System.Windows.Forms.ComboBox();
            this.normalFilterButton = new System.Windows.Forms.RadioButton();
            this.noTagsFilterButton = new System.Windows.Forms.RadioButton();
            this.allFilesButton = new System.Windows.Forms.RadioButton();
            this.autoUpdateBox = new System.Windows.Forms.CheckBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.tagsBox);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // tagsBox
            // 
            this.tagsBox.CheckOnClick = true;
            resources.ApplyResources(this.tagsBox, "tagsBox");
            this.tagsBox.FormattingEnabled = true;
            this.tagsBox.MultiColumn = true;
            this.tagsBox.Name = "tagsBox";
            this.tagsBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.tagsBox_ItemCheck);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.categoriesBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // categoriesBox
            // 
            resources.ApplyResources(this.categoriesBox, "categoriesBox");
            this.categoriesBox.FormattingEnabled = true;
            this.categoriesBox.Name = "categoriesBox";
            this.categoriesBox.SelectedIndexChanged += new System.EventHandler(this.categoriesBox_SelectedIndexChanged);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // tagCategoryCombinationBox
            // 
            resources.ApplyResources(this.tagCategoryCombinationBox, "tagCategoryCombinationBox");
            this.tagCategoryCombinationBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagCategoryCombinationBox.FormattingEnabled = true;
            this.tagCategoryCombinationBox.Items.AddRange(new object[] {
            resources.GetString("tagCategoryCombinationBox.Items"),
            resources.GetString("tagCategoryCombinationBox.Items1"),
            resources.GetString("tagCategoryCombinationBox.Items2")});
            this.tagCategoryCombinationBox.Name = "tagCategoryCombinationBox";
            // 
            // normalFilterButton
            // 
            resources.ApplyResources(this.normalFilterButton, "normalFilterButton");
            this.normalFilterButton.Name = "normalFilterButton";
            this.normalFilterButton.TabStop = true;
            this.normalFilterButton.UseVisualStyleBackColor = true;
            this.normalFilterButton.CheckedChanged += new System.EventHandler(this.normalFilterButton_CheckedChanged);
            // 
            // noTagsFilterButton
            // 
            resources.ApplyResources(this.noTagsFilterButton, "noTagsFilterButton");
            this.noTagsFilterButton.Name = "noTagsFilterButton";
            this.noTagsFilterButton.TabStop = true;
            this.noTagsFilterButton.UseVisualStyleBackColor = true;
            this.noTagsFilterButton.CheckedChanged += new System.EventHandler(this.noTagsFilterButton_CheckedChanged);
            // 
            // allFilesButton
            // 
            resources.ApplyResources(this.allFilesButton, "allFilesButton");
            this.allFilesButton.Name = "allFilesButton";
            this.allFilesButton.TabStop = true;
            this.allFilesButton.UseVisualStyleBackColor = true;
            this.allFilesButton.CheckedChanged += new System.EventHandler(this.allFilesButton_CheckedChanged);
            // 
            // autoUpdateBox
            // 
            resources.ApplyResources(this.autoUpdateBox, "autoUpdateBox");
            this.autoUpdateBox.Name = "autoUpdateBox";
            this.autoUpdateBox.UseVisualStyleBackColor = true;
            this.autoUpdateBox.CheckedChanged += new System.EventHandler(this.autoUpdateBox_CheckedChanged);
            // 
            // TagFilterDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.autoUpdateBox);
            this.Controls.Add(this.allFilesButton);
            this.Controls.Add(this.noTagsFilterButton);
            this.Controls.Add(this.normalFilterButton);
            this.Controls.Add(this.tagCategoryCombinationBox);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "TagFilterDialog";
            this.ShowInTaskbar = false;
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox tagsBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox categoriesBox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.ComboBox tagCategoryCombinationBox;
        private System.Windows.Forms.RadioButton normalFilterButton;
        private System.Windows.Forms.RadioButton noTagsFilterButton;
        private System.Windows.Forms.RadioButton allFilesButton;
        private System.Windows.Forms.CheckBox autoUpdateBox;
    }
}