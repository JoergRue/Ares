/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System;

namespace Ares.Editor.AudioSourceSearch
{
    partial class AudioSourceSearchWindow
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
                // TODO: dispose
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AudioSourceSearchWindow));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.prevPageButton = new System.Windows.Forms.ToolStripButton();
            this.pageLabel = new System.Windows.Forms.ToolStripLabel();
            this.nextPageButton = new System.Windows.Forms.ToolStripButton();
            this.resultsListView = new System.Windows.Forms.ListView();
            this.colHeaderTitle = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderAuthor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colHeaderDuration = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.searchResultContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.downloadToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchSimilarItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.audioSourceComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.searchBox = new System.Windows.Forms.ToolStripTextBox();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.searchResultContextMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.toolStrip2);
            this.splitContainer1.Panel1.Controls.Add(this.resultsListView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.informationBox);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            // 
            // toolStrip2
            // 
            resources.ApplyResources(this.toolStrip2, "toolStrip2");
            this.toolStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.prevPageButton,
            this.pageLabel,
            this.nextPageButton});
            this.toolStrip2.Name = "toolStrip2";
            // 
            // prevPageButton
            // 
            resources.ApplyResources(this.prevPageButton, "prevPageButton");
            this.prevPageButton.Name = "prevPageButton";
            this.prevPageButton.Click += new System.EventHandler(this.prevPageButton_Click);
            // 
            // pageLabel
            // 
            this.pageLabel.Name = "pageLabel";
            resources.ApplyResources(this.pageLabel, "pageLabel");
            // 
            // nextPageButton
            // 
            resources.ApplyResources(this.nextPageButton, "nextPageButton");
            this.nextPageButton.Name = "nextPageButton";
            this.nextPageButton.Click += new System.EventHandler(this.nextPageButton_Click);
            // 
            // resultsListView
            // 
            resources.ApplyResources(this.resultsListView, "resultsListView");
            this.resultsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colHeaderTitle,
            this.colHeaderAuthor,
            this.colHeaderDuration});
            this.resultsListView.ContextMenuStrip = this.searchResultContextMenu;
            this.resultsListView.Name = "resultsListView";
            this.resultsListView.UseCompatibleStateImageBehavior = false;
            this.resultsListView.View = System.Windows.Forms.View.Details;
            this.resultsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.resultsListView_ItemDrag);
            this.resultsListView.SelectedIndexChanged += new System.EventHandler(this.resultsListView_SelectedIndexChanged);
            this.resultsListView.DoubleClick += new System.EventHandler(this.resultsListView_DoubleClick);
            // 
            // colHeaderTitle
            // 
            resources.ApplyResources(this.colHeaderTitle, "colHeaderTitle");
            // 
            // colHeaderAuthor
            // 
            resources.ApplyResources(this.colHeaderAuthor, "colHeaderAuthor");
            // 
            // colHeaderDuration
            // 
            resources.ApplyResources(this.colHeaderDuration, "colHeaderDuration");
            // 
            // searchResultContextMenu
            // 
            this.searchResultContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.searchResultContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.downloadMenuItem,
            this.downloadToMenuItem,
            this.searchSimilarItem});
            this.searchResultContextMenu.Name = "contextMenuStrip1";
            resources.ApplyResources(this.searchResultContextMenu, "searchResultContextMenu");
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            resources.ApplyResources(this.playToolStripMenuItem, "playToolStripMenuItem");
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // downloadMenuItem
            // 
            this.downloadMenuItem.Name = "downloadMenuItem";
            resources.ApplyResources(this.downloadMenuItem, "downloadMenuItem");
            this.downloadMenuItem.Click += new System.EventHandler(this.downloadMenuItem_Click);
            // 
            // downloadToMenuItem
            // 
            this.downloadToMenuItem.Name = "downloadToMenuItem";
            resources.ApplyResources(this.downloadToMenuItem, "downloadToMenuItem");
            this.downloadToMenuItem.Click += new System.EventHandler(this.downloadToMenuItem_Click);
            // 
            // searchSimilarItem
            // 
            this.searchSimilarItem.Name = "searchSimilarItem";
            resources.ApplyResources(this.searchSimilarItem, "searchSimilarItem");
            this.searchSimilarItem.Click += new System.EventHandler(this.searchSimilarItem_Click);
            // 
            // informationBox
            // 
            resources.ApplyResources(this.informationBox, "informationBox");
            this.informationBox.Name = "informationBox";
            this.informationBox.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.stopButton,
            this.audioSourceComboBox,
            this.searchBox,
            this.searchButton});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // playButton
            // 
            this.playButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            this.playButton.Name = "playButton";
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Image = global::Ares.Editor.ImageResources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // audioSourceComboBox
            // 
            this.audioSourceComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.audioSourceComboBox.Name = "audioSourceComboBox";
            resources.ApplyResources(this.audioSourceComboBox, "audioSourceComboBox");
            this.audioSourceComboBox.SelectedIndexChanged += new System.EventHandler(this.audioSourceComboBox_SelectedIndexChanged);
            // 
            // searchBox
            // 
            this.searchBox.Name = "searchBox";
            resources.ApplyResources(this.searchBox, "searchBox");
            this.searchBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.searchBox_KeyPress);
            // 
            // searchButton
            // 
            this.searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.searchButton, "searchButton");
            this.searchButton.Name = "searchButton";
            this.searchButton.Click += new System.EventHandler(this.searchButton_Click);
            // 
            // AudioSourceSearchWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "AudioSourceSearchWindow";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.searchResultContextMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton playButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ContextMenuStrip searchResultContextMenu;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem downloadMenuItem;
        private System.Windows.Forms.ToolStripTextBox searchBox;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.TextBox informationBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStripMenuItem downloadToMenuItem;
        private System.Windows.Forms.ListView resultsListView;
        private System.Windows.Forms.ColumnHeader colHeaderTitle;
        private System.Windows.Forms.ColumnHeader colHeaderAuthor;
        private System.Windows.Forms.ColumnHeader colHeaderDuration;
        private System.Windows.Forms.ToolStripComboBox audioSourceComboBox;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripButton prevPageButton;
        private System.Windows.Forms.ToolStripLabel pageLabel;
        private System.Windows.Forms.ToolStripButton nextPageButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolStripMenuItem searchSimilarItem;
    }
}