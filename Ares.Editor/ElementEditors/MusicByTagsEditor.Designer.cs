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
    partial class MusicByTagsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusicByTagsEditor));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tagsBox = new System.Windows.Forms.CheckedListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.categoriesBox = new System.Windows.Forms.ListBox();
            this.languageBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.clearButton = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.tagCategoriesAndButton = new System.Windows.Forms.RadioButton();
            this.tagCategoriesOrButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.fadeUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fadeUpDown)).BeginInit();
            this.SuspendLayout();
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
            resources.ApplyResources(this.tagsBox, "tagsBox");
            this.tagsBox.CheckOnClick = true;
            this.tagsBox.FormattingEnabled = true;
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
            // languageBox
            // 
            resources.ApplyResources(this.languageBox, "languageBox");
            this.languageBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageBox.FormattingEnabled = true;
            this.languageBox.Name = "languageBox";
            this.languageBox.SelectedIndexChanged += new System.EventHandler(this.languageBox_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // clearButton
            // 
            resources.ApplyResources(this.clearButton, "clearButton");
            this.clearButton.Name = "clearButton";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearTagsButton_Click);
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // tagCategoriesAndButton
            // 
            resources.ApplyResources(this.tagCategoriesAndButton, "tagCategoriesAndButton");
            this.tagCategoriesAndButton.Name = "tagCategoriesAndButton";
            this.tagCategoriesAndButton.TabStop = true;
            this.tagCategoriesAndButton.UseVisualStyleBackColor = true;
            this.tagCategoriesAndButton.CheckedChanged += new System.EventHandler(this.tagCategoriesAndButton_CheckedChanged);
            // 
            // tagCategoriesOrButton
            // 
            resources.ApplyResources(this.tagCategoriesOrButton, "tagCategoriesOrButton");
            this.tagCategoriesOrButton.Name = "tagCategoriesOrButton";
            this.tagCategoriesOrButton.TabStop = true;
            this.tagCategoriesOrButton.UseVisualStyleBackColor = true;
            this.tagCategoriesOrButton.CheckedChanged += new System.EventHandler(this.tagCategoriesOrButton_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // fadeUpDown
            // 
            resources.ApplyResources(this.fadeUpDown, "fadeUpDown");
            this.fadeUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.fadeUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.fadeUpDown.Name = "fadeUpDown";
            this.fadeUpDown.ValueChanged += new System.EventHandler(this.fadeUpDown_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // MusicByTagsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fadeUpDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.tagCategoriesAndButton);
            this.Controls.Add(this.tagCategoriesOrButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.languageBox);
            this.Controls.Add(this.label1);
            this.Name = "MusicByTagsEditor";
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.fadeUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckedListBox tagsBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox categoriesBox;
        private System.Windows.Forms.ComboBox languageBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.RadioButton tagCategoriesAndButton;
        private System.Windows.Forms.RadioButton tagCategoriesOrButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown fadeUpDown;
        private System.Windows.Forms.Label label3;
    }
}