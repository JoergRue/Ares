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
using System;

namespace Ares.Editor
{
    partial class FileExplorer
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
                if (m_FileType == FileType.Music)
                {
                    Actions.FilesWatcher.Instance.MusicDirChanges -= new EventHandler<EventArgs>(DirChanged);
                }
                else
                {
                    Actions.FilesWatcher.Instance.SoundDirChanges -= new EventHandler<EventArgs>(DirChanged);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileExplorer));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.treeView1 = new MultiSelectTreeview.MultiSelectTreeview();
            this.fileNodeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1.SuspendLayout();
            this.fileNodeContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.stopButton,
            this.refreshButton});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // playButton
            // 
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            this.playButton.Name = "playButton";
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = global::Ares.Editor.ImageResources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // refreshButton
            // 
            resources.ApplyResources(this.refreshButton, "refreshButton");
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // treeView1
            // 
            resources.ApplyResources(this.treeView1, "treeView1");
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("treeView1.SelectedNodes")));
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DoubleClick += new System.EventHandler(this.treeView1_DoubleClick);
            this.treeView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.treeView1_KeyDown);
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            this.treeView1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseMove);
            this.treeView1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseUp);
            // 
            // fileNodeContextMenu
            // 
            resources.ApplyResources(this.fileNodeContextMenu, "fileNodeContextMenu");
            this.fileNodeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playToolStripMenuItem,
            this.stopToolStripMenuItem,
            this.editToolStripMenuItem});
            this.fileNodeContextMenu.Name = "contextMenuStrip1";
            this.fileNodeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.fileNodeContextMenu_Opening);
            // 
            // playToolStripMenuItem
            // 
            resources.ApplyResources(this.playToolStripMenuItem, "playToolStripMenuItem");
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // stopToolStripMenuItem
            // 
            resources.ApplyResources(this.stopToolStripMenuItem, "stopToolStripMenuItem");
            this.stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            this.stopToolStripMenuItem.Click += new System.EventHandler(this.stopToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // FileExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FileExplorer";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.fileNodeContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton playButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private MultiSelectTreeview.MultiSelectTreeview treeView1;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ContextMenuStrip fileNodeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
    }
}