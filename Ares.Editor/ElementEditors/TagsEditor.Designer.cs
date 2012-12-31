/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
    partial class TagsEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagsEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.languageSelectionBox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.deleteCategoryButton = new System.Windows.Forms.Button();
            this.renameCategoryButton = new System.Windows.Forms.Button();
            this.addCategoryButton = new System.Windows.Forms.Button();
            this.categoriesList = new System.Windows.Forms.ListBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.translationsLabel = new System.Windows.Forms.Label();
            this.translationsList = new System.Windows.Forms.ListView();
            this.langColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.deleteTranslationButton = new System.Windows.Forms.Button();
            this.renameTranslationButton = new System.Windows.Forms.Button();
            this.addTranslationButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.deleteLanguageButton = new System.Windows.Forms.Button();
            this.renameLanguageButton = new System.Windows.Forms.Button();
            this.addLanguageButton = new System.Windows.Forms.Button();
            this.languagesList = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.deleteTagButton = new System.Windows.Forms.Button();
            this.renameTagButton = new System.Windows.Forms.Button();
            this.addTagButton = new System.Windows.Forms.Button();
            this.tagsList = new System.Windows.Forms.ListBox();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // languageSelectionBox
            // 
            resources.ApplyResources(this.languageSelectionBox, "languageSelectionBox");
            this.languageSelectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageSelectionBox.FormattingEnabled = true;
            this.languageSelectionBox.Name = "languageSelectionBox";
            this.languageSelectionBox.SelectedIndexChanged += new System.EventHandler(this.languageSelectionBox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.deleteCategoryButton);
            this.groupBox1.Controls.Add(this.renameCategoryButton);
            this.groupBox1.Controls.Add(this.addCategoryButton);
            this.groupBox1.Controls.Add(this.categoriesList);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // deleteCategoryButton
            // 
            resources.ApplyResources(this.deleteCategoryButton, "deleteCategoryButton");
            this.deleteCategoryButton.Name = "deleteCategoryButton";
            this.deleteCategoryButton.UseVisualStyleBackColor = true;
            this.deleteCategoryButton.Click += new System.EventHandler(this.deleteCategoryButton_Click);
            // 
            // renameCategoryButton
            // 
            resources.ApplyResources(this.renameCategoryButton, "renameCategoryButton");
            this.renameCategoryButton.Name = "renameCategoryButton";
            this.renameCategoryButton.UseVisualStyleBackColor = true;
            this.renameCategoryButton.Click += new System.EventHandler(this.renameCategoryButton_Click);
            // 
            // addCategoryButton
            // 
            resources.ApplyResources(this.addCategoryButton, "addCategoryButton");
            this.addCategoryButton.Name = "addCategoryButton";
            this.addCategoryButton.UseVisualStyleBackColor = true;
            this.addCategoryButton.Click += new System.EventHandler(this.addCategoryButton_Click);
            // 
            // categoriesList
            // 
            resources.ApplyResources(this.categoriesList, "categoriesList");
            this.categoriesList.FormattingEnabled = true;
            this.categoriesList.Name = "categoriesList";
            this.categoriesList.SelectedIndexChanged += new System.EventHandler(this.categoriesList_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.groupBox4, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // groupBox4
            // 
            resources.ApplyResources(this.groupBox4, "groupBox4");
            this.groupBox4.Controls.Add(this.translationsLabel);
            this.groupBox4.Controls.Add(this.translationsList);
            this.groupBox4.Controls.Add(this.deleteTranslationButton);
            this.groupBox4.Controls.Add(this.renameTranslationButton);
            this.groupBox4.Controls.Add(this.addTranslationButton);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.TabStop = false;
            // 
            // translationsLabel
            // 
            resources.ApplyResources(this.translationsLabel, "translationsLabel");
            this.translationsLabel.Name = "translationsLabel";
            // 
            // translationsList
            // 
            resources.ApplyResources(this.translationsList, "translationsList");
            this.translationsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.langColHeader,
            this.nameColHeader});
            this.translationsList.FullRowSelect = true;
            this.translationsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.translationsList.HideSelection = false;
            this.translationsList.MultiSelect = false;
            this.translationsList.Name = "translationsList";
            this.translationsList.UseCompatibleStateImageBehavior = false;
            this.translationsList.View = System.Windows.Forms.View.Details;
            this.translationsList.SelectedIndexChanged += new System.EventHandler(this.translationsList_SelectedIndexChanged);
            // 
            // langColHeader
            // 
            resources.ApplyResources(this.langColHeader, "langColHeader");
            // 
            // nameColHeader
            // 
            resources.ApplyResources(this.nameColHeader, "nameColHeader");
            // 
            // deleteTranslationButton
            // 
            resources.ApplyResources(this.deleteTranslationButton, "deleteTranslationButton");
            this.deleteTranslationButton.Name = "deleteTranslationButton";
            this.deleteTranslationButton.UseVisualStyleBackColor = true;
            this.deleteTranslationButton.Click += new System.EventHandler(this.deleteTranslationButton_Click);
            // 
            // renameTranslationButton
            // 
            resources.ApplyResources(this.renameTranslationButton, "renameTranslationButton");
            this.renameTranslationButton.Name = "renameTranslationButton";
            this.renameTranslationButton.UseVisualStyleBackColor = true;
            this.renameTranslationButton.Click += new System.EventHandler(this.renameTranslationButton_Click);
            // 
            // addTranslationButton
            // 
            resources.ApplyResources(this.addTranslationButton, "addTranslationButton");
            this.addTranslationButton.Name = "addTranslationButton";
            this.addTranslationButton.UseVisualStyleBackColor = true;
            this.addTranslationButton.Click += new System.EventHandler(this.addTranslationButton_Click);
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.deleteLanguageButton);
            this.groupBox3.Controls.Add(this.renameLanguageButton);
            this.groupBox3.Controls.Add(this.addLanguageButton);
            this.groupBox3.Controls.Add(this.languagesList);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // deleteLanguageButton
            // 
            resources.ApplyResources(this.deleteLanguageButton, "deleteLanguageButton");
            this.deleteLanguageButton.Name = "deleteLanguageButton";
            this.deleteLanguageButton.UseVisualStyleBackColor = true;
            this.deleteLanguageButton.Click += new System.EventHandler(this.deleteLanguageButton_Click);
            // 
            // renameLanguageButton
            // 
            resources.ApplyResources(this.renameLanguageButton, "renameLanguageButton");
            this.renameLanguageButton.Name = "renameLanguageButton";
            this.renameLanguageButton.UseVisualStyleBackColor = true;
            this.renameLanguageButton.Click += new System.EventHandler(this.renameLanguageButton_Click);
            // 
            // addLanguageButton
            // 
            resources.ApplyResources(this.addLanguageButton, "addLanguageButton");
            this.addLanguageButton.Name = "addLanguageButton";
            this.addLanguageButton.UseVisualStyleBackColor = true;
            this.addLanguageButton.Click += new System.EventHandler(this.addLanguageButton_Click);
            // 
            // languagesList
            // 
            resources.ApplyResources(this.languagesList, "languagesList");
            this.languagesList.FormattingEnabled = true;
            this.languagesList.Name = "languagesList";
            this.languagesList.SelectedIndexChanged += new System.EventHandler(this.languagesList_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.deleteTagButton);
            this.groupBox2.Controls.Add(this.renameTagButton);
            this.groupBox2.Controls.Add(this.addTagButton);
            this.groupBox2.Controls.Add(this.tagsList);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // deleteTagButton
            // 
            resources.ApplyResources(this.deleteTagButton, "deleteTagButton");
            this.deleteTagButton.Name = "deleteTagButton";
            this.deleteTagButton.UseVisualStyleBackColor = true;
            this.deleteTagButton.Click += new System.EventHandler(this.deleteTagButton_Click);
            // 
            // renameTagButton
            // 
            resources.ApplyResources(this.renameTagButton, "renameTagButton");
            this.renameTagButton.Name = "renameTagButton";
            this.renameTagButton.UseVisualStyleBackColor = true;
            this.renameTagButton.Click += new System.EventHandler(this.renameTagButton_Click);
            // 
            // addTagButton
            // 
            resources.ApplyResources(this.addTagButton, "addTagButton");
            this.addTagButton.Name = "addTagButton";
            this.addTagButton.UseVisualStyleBackColor = true;
            this.addTagButton.Click += new System.EventHandler(this.addTagButton_Click);
            // 
            // tagsList
            // 
            resources.ApplyResources(this.tagsList, "tagsList");
            this.tagsList.FormattingEnabled = true;
            this.tagsList.Name = "tagsList";
            this.tagsList.SelectedIndexChanged += new System.EventHandler(this.tagsList_SelectedIndexChanged);
            // 
            // TagsEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.languageSelectionBox);
            this.Controls.Add(this.label1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "TagsEditor";
            this.ShowInTaskbar = false;
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox languageSelectionBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button deleteCategoryButton;
        private System.Windows.Forms.Button renameCategoryButton;
        private System.Windows.Forms.Button addCategoryButton;
        private System.Windows.Forms.ListBox categoriesList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button deleteTagButton;
        private System.Windows.Forms.Button renameTagButton;
        private System.Windows.Forms.Button addTagButton;
        private System.Windows.Forms.ListBox tagsList;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button deleteTranslationButton;
        private System.Windows.Forms.Button renameTranslationButton;
        private System.Windows.Forms.Button addTranslationButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button deleteLanguageButton;
        private System.Windows.Forms.Button renameLanguageButton;
        private System.Windows.Forms.Button addLanguageButton;
        private System.Windows.Forms.ListBox languagesList;
        private System.Windows.Forms.ListView translationsList;
        private System.Windows.Forms.ColumnHeader langColHeader;
        private System.Windows.Forms.ColumnHeader nameColHeader;
        private System.Windows.Forms.Label translationsLabel;
    }
}