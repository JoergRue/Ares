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
namespace Ares.Editor
{
    partial class ProjectExplorer
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
                RemoveChangeListener();
                Ares.Editor.Actions.ElementChanges.Instance.RemoveListener(-1, ElementChanged);
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectExplorer));
            this.projectContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortModesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortModesByNameAscMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortModesByNameDescMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortModesByKeyAscMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortModesByKeyDescMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectTree = new MultiSelectTreeview.MultiSelectTreeview();
            this.modeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem17 = new System.Windows.Forms.ToolStripMenuItem();
            this.sortElementsByNameAscMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortElementsByNameDescMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortElementsByKeyAscMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortElementsByKeyDescMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem15 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMusicByTagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRandomPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBackgroundSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webRadioMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem14 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addParallelElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChoiceListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.containerContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectContainerKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem16 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.musicByTagsToolStripItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRandomPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addBackgroundSoundsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.webRadioItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.addParallelElementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialElementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addChoiceListToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.elementContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectElementKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem10 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.exportToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.tagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bgSoundsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectBGSoundsKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSoundChoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem11 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem12 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem13 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.importToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.setKeyButton = new System.Windows.Forms.ToolStripButton();
            this.moveDownButton = new System.Windows.Forms.ToolStripButton();
            this.moveUpButton = new System.Windows.Forms.ToolStripButton();
            this.exportDialog = new System.Windows.Forms.SaveFileDialog();
            this.importDialog = new System.Windows.Forms.OpenFileDialog();
            this.projectContextMenu.SuspendLayout();
            this.modeContextMenu.SuspendLayout();
            this.containerContextMenu.SuspendLayout();
            this.elementContextMenu.SuspendLayout();
            this.bgSoundsContextMenu.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectContextMenu
            // 
            resources.ApplyResources(this.projectContextMenu, "projectContextMenu");
            this.projectContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.projectContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.sortModesToolStripMenuItem,
            this.toolStripMenuItem1,
            this.importToolStripMenuItem1,
            this.pasteToolStripMenuItem});
            this.projectContextMenu.Name = "projectContextMenu";
            this.projectContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.projectContextMenu_Opening);
            // 
            // renameToolStripMenuItem
            // 
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // sortModesToolStripMenuItem
            // 
            resources.ApplyResources(this.sortModesToolStripMenuItem, "sortModesToolStripMenuItem");
            this.sortModesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortModesByNameAscMenuItem,
            this.sortModesByNameDescMenuItem,
            this.sortModesByKeyAscMenuItem,
            this.sortModesByKeyDescMenuItem});
            this.sortModesToolStripMenuItem.Name = "sortModesToolStripMenuItem";
            // 
            // sortModesByNameAscMenuItem
            // 
            resources.ApplyResources(this.sortModesByNameAscMenuItem, "sortModesByNameAscMenuItem");
            this.sortModesByNameAscMenuItem.Name = "sortModesByNameAscMenuItem";
            this.sortModesByNameAscMenuItem.Click += new System.EventHandler(this.sortModesByNameAsc_Clicked);
            // 
            // sortModesByNameDescMenuItem
            // 
            resources.ApplyResources(this.sortModesByNameDescMenuItem, "sortModesByNameDescMenuItem");
            this.sortModesByNameDescMenuItem.Name = "sortModesByNameDescMenuItem";
            this.sortModesByNameDescMenuItem.Click += new System.EventHandler(this.sortModesByNameDescMenuItem_Click);
            // 
            // sortModesByKeyAscMenuItem
            // 
            resources.ApplyResources(this.sortModesByKeyAscMenuItem, "sortModesByKeyAscMenuItem");
            this.sortModesByKeyAscMenuItem.Name = "sortModesByKeyAscMenuItem";
            this.sortModesByKeyAscMenuItem.Click += new System.EventHandler(this.sortModesByKeyAscMenuItem_Click);
            // 
            // sortModesByKeyDescMenuItem
            // 
            resources.ApplyResources(this.sortModesByKeyDescMenuItem, "sortModesByKeyDescMenuItem");
            this.sortModesByKeyDescMenuItem.Name = "sortModesByKeyDescMenuItem";
            this.sortModesByKeyDescMenuItem.Click += new System.EventHandler(this.sortModesByKeyDescMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // importToolStripMenuItem1
            // 
            resources.ApplyResources(this.importToolStripMenuItem1, "importToolStripMenuItem1");
            this.importToolStripMenuItem1.Name = "importToolStripMenuItem1";
            this.importToolStripMenuItem1.Click += new System.EventHandler(this.importToolStripMenuItem1_Click);
            // 
            // pasteToolStripMenuItem
            // 
            resources.ApplyResources(this.pasteToolStripMenuItem, "pasteToolStripMenuItem");
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Tag = "Paste";
            this.pasteToolStripMenuItem.Click += new System.EventHandler(this.pasteToolStripMenuItem_Click);
            // 
            // projectTree
            // 
            resources.ApplyResources(this.projectTree, "projectTree");
            this.projectTree.AllowDrop = true;
            this.projectTree.HideSelection = false;
            this.projectTree.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.projectTree.Name = "projectTree";
            this.projectTree.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("projectTree.SelectedNodes")));
            this.projectTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.projectTree_AfterLabelEdit);
            this.projectTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeCollapse);
            this.projectTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeExpand);
            this.projectTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeSelect);
            this.projectTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectTree_AfterSelect);
            this.projectTree.DragDrop += new System.Windows.Forms.DragEventHandler(this.projectTree_DragDrop);
            this.projectTree.DragEnter += new System.Windows.Forms.DragEventHandler(this.projectTree_DragEnter);
            this.projectTree.DragOver += new System.Windows.Forms.DragEventHandler(this.projectTree_DragOver);
            this.projectTree.DoubleClick += new System.EventHandler(this.projectTree_DoubleClick);
            this.projectTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.projectTree_KeyDown);
            this.projectTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectTree_MouseDown);
            this.projectTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.projectTree_MouseMove);
            this.projectTree.MouseUp += new System.Windows.Forms.MouseEventHandler(this.projectTree_MouseUp);
            // 
            // modeContextMenu
            // 
            resources.ApplyResources(this.modeContextMenu, "modeContextMenu");
            this.modeContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.modeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem17,
            this.toolStripSeparator9,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem3,
            this.toolStripMenuItem15,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.addScenarioToolStripMenuItem,
            this.addMusicByTagsToolStripMenuItem,
            this.addRandomPlaylistToolStripMenuItem,
            this.addSequentialPlaylistToolStripMenuItem,
            this.addBackgroundSoundsToolStripMenuItem,
            this.webRadioMenuItem,
            this.toolStripMenuItem14,
            this.toolStripSeparator2,
            this.addParallelElementToolStripMenuItem,
            this.addSequentialElementToolStripMenuItem,
            this.addChoiceListToolStripMenuItem,
            this.toolStripSeparator5,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem});
            this.modeContextMenu.Name = "modeContextMenu";
            this.modeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.modeContextMenu_Opening);
            // 
            // renameToolStripMenuItem1
            // 
            resources.ApplyResources(this.renameToolStripMenuItem1, "renameToolStripMenuItem1");
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            this.renameToolStripMenuItem1.Click += new System.EventHandler(this.renameToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Tag = "DuringPlay";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem17
            // 
            resources.ApplyResources(this.toolStripMenuItem17, "toolStripMenuItem17");
            this.toolStripMenuItem17.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sortElementsByNameAscMenuItem,
            this.sortElementsByNameDescMenuItem,
            this.sortElementsByKeyAscMenuItem,
            this.sortElementsByKeyDescMenuItem});
            this.toolStripMenuItem17.Name = "toolStripMenuItem17";
            // 
            // sortElementsByNameAscMenuItem
            // 
            resources.ApplyResources(this.sortElementsByNameAscMenuItem, "sortElementsByNameAscMenuItem");
            this.sortElementsByNameAscMenuItem.Name = "sortElementsByNameAscMenuItem";
            this.sortElementsByNameAscMenuItem.Click += new System.EventHandler(this.sortElementsByNameAscMenuItem_Click);
            // 
            // sortElementsByNameDescMenuItem
            // 
            resources.ApplyResources(this.sortElementsByNameDescMenuItem, "sortElementsByNameDescMenuItem");
            this.sortElementsByNameDescMenuItem.Name = "sortElementsByNameDescMenuItem";
            this.sortElementsByNameDescMenuItem.Click += new System.EventHandler(this.sortElementsByNameDescMenuItem_Click);
            // 
            // sortElementsByKeyAscMenuItem
            // 
            resources.ApplyResources(this.sortElementsByKeyAscMenuItem, "sortElementsByKeyAscMenuItem");
            this.sortElementsByKeyAscMenuItem.Name = "sortElementsByKeyAscMenuItem";
            this.sortElementsByKeyAscMenuItem.Click += new System.EventHandler(this.sortElementsByKeyAscMenuItem_Click);
            // 
            // sortElementsByKeyDescMenuItem
            // 
            resources.ApplyResources(this.sortElementsByKeyDescMenuItem, "sortElementsByKeyDescMenuItem");
            this.sortElementsByKeyDescMenuItem.Name = "sortElementsByKeyDescMenuItem";
            this.sortElementsByKeyDescMenuItem.Click += new System.EventHandler(this.sortElementsByKeyDescMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            resources.ApplyResources(this.toolStripSeparator9, "toolStripSeparator9");
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            // 
            // toolStripMenuItem4
            // 
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click_1);
            // 
            // toolStripMenuItem5
            // 
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click_1);
            // 
            // toolStripMenuItem3
            // 
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Tag = "Paste";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click_1);
            // 
            // toolStripMenuItem15
            // 
            resources.ApplyResources(this.toolStripMenuItem15, "toolStripMenuItem15");
            this.toolStripMenuItem15.Name = "toolStripMenuItem15";
            this.toolStripMenuItem15.Tag = "PasteLink";
            this.toolStripMenuItem15.Click += new System.EventHandler(this.toolStripMenuItem15_Click);
            // 
            // deleteToolStripMenuItem
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Tag = "MultipleNodes";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // addScenarioToolStripMenuItem
            // 
            resources.ApplyResources(this.addScenarioToolStripMenuItem, "addScenarioToolStripMenuItem");
            this.addScenarioToolStripMenuItem.Name = "addScenarioToolStripMenuItem";
            this.addScenarioToolStripMenuItem.Click += new System.EventHandler(this.addScenarioToolStripMenuItem_Click);
            // 
            // addMusicByTagsToolStripMenuItem
            // 
            resources.ApplyResources(this.addMusicByTagsToolStripMenuItem, "addMusicByTagsToolStripMenuItem");
            this.addMusicByTagsToolStripMenuItem.Name = "addMusicByTagsToolStripMenuItem";
            this.addMusicByTagsToolStripMenuItem.Click += new System.EventHandler(this.addMusicByTagsToolStripMenuItem_Click);
            // 
            // addRandomPlaylistToolStripMenuItem
            // 
            resources.ApplyResources(this.addRandomPlaylistToolStripMenuItem, "addRandomPlaylistToolStripMenuItem");
            this.addRandomPlaylistToolStripMenuItem.Name = "addRandomPlaylistToolStripMenuItem";
            this.addRandomPlaylistToolStripMenuItem.Click += new System.EventHandler(this.addRandomPlaylistToolStripMenuItem_Click);
            // 
            // addSequentialPlaylistToolStripMenuItem
            // 
            resources.ApplyResources(this.addSequentialPlaylistToolStripMenuItem, "addSequentialPlaylistToolStripMenuItem");
            this.addSequentialPlaylistToolStripMenuItem.Name = "addSequentialPlaylistToolStripMenuItem";
            this.addSequentialPlaylistToolStripMenuItem.Click += new System.EventHandler(this.addSequentialPlaylistToolStripMenuItem_Click);
            // 
            // addBackgroundSoundsToolStripMenuItem
            // 
            resources.ApplyResources(this.addBackgroundSoundsToolStripMenuItem, "addBackgroundSoundsToolStripMenuItem");
            this.addBackgroundSoundsToolStripMenuItem.Name = "addBackgroundSoundsToolStripMenuItem";
            this.addBackgroundSoundsToolStripMenuItem.Click += new System.EventHandler(this.addBackgroundSoundsToolStripMenuItem_Click);
            // 
            // webRadioMenuItem
            // 
            resources.ApplyResources(this.webRadioMenuItem, "webRadioMenuItem");
            this.webRadioMenuItem.Name = "webRadioMenuItem";
            this.webRadioMenuItem.Click += new System.EventHandler(this.webRadioMenuItem_Click);
            // 
            // toolStripMenuItem14
            // 
            resources.ApplyResources(this.toolStripMenuItem14, "toolStripMenuItem14");
            this.toolStripMenuItem14.Name = "toolStripMenuItem14";
            this.toolStripMenuItem14.Click += new System.EventHandler(this.toolStripMenuItem14_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // addParallelElementToolStripMenuItem
            // 
            resources.ApplyResources(this.addParallelElementToolStripMenuItem, "addParallelElementToolStripMenuItem");
            this.addParallelElementToolStripMenuItem.Name = "addParallelElementToolStripMenuItem";
            this.addParallelElementToolStripMenuItem.Click += new System.EventHandler(this.addParallelElementToolStripMenuItem_Click);
            // 
            // addSequentialElementToolStripMenuItem
            // 
            resources.ApplyResources(this.addSequentialElementToolStripMenuItem, "addSequentialElementToolStripMenuItem");
            this.addSequentialElementToolStripMenuItem.Name = "addSequentialElementToolStripMenuItem";
            this.addSequentialElementToolStripMenuItem.Click += new System.EventHandler(this.addSequentialElementToolStripMenuItem_Click);
            // 
            // addChoiceListToolStripMenuItem
            // 
            resources.ApplyResources(this.addChoiceListToolStripMenuItem, "addChoiceListToolStripMenuItem");
            this.addChoiceListToolStripMenuItem.Name = "addChoiceListToolStripMenuItem";
            this.addChoiceListToolStripMenuItem.Click += new System.EventHandler(this.addChoiceListToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // importToolStripMenuItem
            // 
            resources.ApplyResources(this.importToolStripMenuItem, "importToolStripMenuItem");
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            resources.ApplyResources(this.exportToolStripMenuItem, "exportToolStripMenuItem");
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Tag = "MultipleNodes";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // containerContextMenu
            // 
            resources.ApplyResources(this.containerContextMenu, "containerContextMenu");
            this.containerContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.containerContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.renameToolStripMenuItem2,
            this.selectContainerKeyMenuItem,
            this.modeElementStartingToolStripMenuItem,
            this.toolStripSeparator10,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8,
            this.toolStripMenuItem16,
            this.deleteToolStripMenuItem1,
            this.toolStripSeparator3,
            this.musicByTagsToolStripItem,
            this.addRandomPlaylistToolStripMenuItem1,
            this.addSequentialPlaylistToolStripMenuItem1,
            this.addBackgroundSoundsToolStripMenuItem1,
            this.webRadioItem2,
            this.toolStripSeparator4,
            this.addParallelElementToolStripMenuItem1,
            this.addSequentialElementToolStripMenuItem1,
            this.addChoiceListToolStripMenuItem1,
            this.toolStripSeparator6,
            this.importToolStripMenuItem2,
            this.exportToolStripMenuItem1});
            this.containerContextMenu.Name = "containerContextMenu";
            this.containerContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.containerContextMenu_Opening);
            // 
            // editToolStripMenuItem
            // 
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Tag = "DuringPlay";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // renameToolStripMenuItem2
            // 
            resources.ApplyResources(this.renameToolStripMenuItem2, "renameToolStripMenuItem2");
            this.renameToolStripMenuItem2.Name = "renameToolStripMenuItem2";
            this.renameToolStripMenuItem2.Click += new System.EventHandler(this.renameToolStripMenuItem2_Click);
            // 
            // selectContainerKeyMenuItem
            // 
            resources.ApplyResources(this.selectContainerKeyMenuItem, "selectContainerKeyMenuItem");
            this.selectContainerKeyMenuItem.Name = "selectContainerKeyMenuItem";
            this.selectContainerKeyMenuItem.Tag = "OnlyMode DuringPlay";
            this.selectContainerKeyMenuItem.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // modeElementStartingToolStripMenuItem
            // 
            resources.ApplyResources(this.modeElementStartingToolStripMenuItem, "modeElementStartingToolStripMenuItem");
            this.modeElementStartingToolStripMenuItem.Name = "modeElementStartingToolStripMenuItem";
            this.modeElementStartingToolStripMenuItem.Tag = "OnlyMode DuringPlay";
            this.modeElementStartingToolStripMenuItem.Click += new System.EventHandler(this.selectKeyToolStripMenuItem_Click);
            // 
            // toolStripSeparator10
            // 
            resources.ApplyResources(this.toolStripSeparator10, "toolStripSeparator10");
            this.toolStripSeparator10.Name = "toolStripSeparator10";
            // 
            // toolStripMenuItem6
            // 
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // toolStripMenuItem7
            // 
            resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripMenuItem8
            // 
            resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Tag = "Paste";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // toolStripMenuItem16
            // 
            resources.ApplyResources(this.toolStripMenuItem16, "toolStripMenuItem16");
            this.toolStripMenuItem16.Name = "toolStripMenuItem16";
            this.toolStripMenuItem16.Tag = "PasteLink";
            this.toolStripMenuItem16.Click += new System.EventHandler(this.toolStripMenuItem16_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem1, "deleteToolStripMenuItem1");
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            this.deleteToolStripMenuItem1.Tag = "MultipleNodes";
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // musicByTagsToolStripItem
            // 
            resources.ApplyResources(this.musicByTagsToolStripItem, "musicByTagsToolStripItem");
            this.musicByTagsToolStripItem.Name = "musicByTagsToolStripItem";
            this.musicByTagsToolStripItem.Click += new System.EventHandler(this.addMusicByTagsToolStripItem_Click);
            // 
            // addRandomPlaylistToolStripMenuItem1
            // 
            resources.ApplyResources(this.addRandomPlaylistToolStripMenuItem1, "addRandomPlaylistToolStripMenuItem1");
            this.addRandomPlaylistToolStripMenuItem1.Name = "addRandomPlaylistToolStripMenuItem1";
            this.addRandomPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.addRandomPlaylistToolStripMenuItem1_Click);
            // 
            // addSequentialPlaylistToolStripMenuItem1
            // 
            resources.ApplyResources(this.addSequentialPlaylistToolStripMenuItem1, "addSequentialPlaylistToolStripMenuItem1");
            this.addSequentialPlaylistToolStripMenuItem1.Name = "addSequentialPlaylistToolStripMenuItem1";
            this.addSequentialPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.addSequentialPlaylistToolStripMenuItem1_Click);
            // 
            // addBackgroundSoundsToolStripMenuItem1
            // 
            resources.ApplyResources(this.addBackgroundSoundsToolStripMenuItem1, "addBackgroundSoundsToolStripMenuItem1");
            this.addBackgroundSoundsToolStripMenuItem1.Name = "addBackgroundSoundsToolStripMenuItem1";
            this.addBackgroundSoundsToolStripMenuItem1.Click += new System.EventHandler(this.addBackgroundSoundsToolStripMenuItem1_Click);
            // 
            // webRadioItem2
            // 
            resources.ApplyResources(this.webRadioItem2, "webRadioItem2");
            this.webRadioItem2.Name = "webRadioItem2";
            this.webRadioItem2.Click += new System.EventHandler(this.webRadioItem2_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // addParallelElementToolStripMenuItem1
            // 
            resources.ApplyResources(this.addParallelElementToolStripMenuItem1, "addParallelElementToolStripMenuItem1");
            this.addParallelElementToolStripMenuItem1.Name = "addParallelElementToolStripMenuItem1";
            this.addParallelElementToolStripMenuItem1.Click += new System.EventHandler(this.addParallelElementToolStripMenuItem1_Click);
            // 
            // addSequentialElementToolStripMenuItem1
            // 
            resources.ApplyResources(this.addSequentialElementToolStripMenuItem1, "addSequentialElementToolStripMenuItem1");
            this.addSequentialElementToolStripMenuItem1.Name = "addSequentialElementToolStripMenuItem1";
            this.addSequentialElementToolStripMenuItem1.Click += new System.EventHandler(this.addSequentialElementToolStripMenuItem1_Click);
            // 
            // addChoiceListToolStripMenuItem1
            // 
            resources.ApplyResources(this.addChoiceListToolStripMenuItem1, "addChoiceListToolStripMenuItem1");
            this.addChoiceListToolStripMenuItem1.Name = "addChoiceListToolStripMenuItem1";
            this.addChoiceListToolStripMenuItem1.Click += new System.EventHandler(this.addChoiceListToolStripMenuItem1_Click);
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // importToolStripMenuItem2
            // 
            resources.ApplyResources(this.importToolStripMenuItem2, "importToolStripMenuItem2");
            this.importToolStripMenuItem2.Name = "importToolStripMenuItem2";
            this.importToolStripMenuItem2.Click += new System.EventHandler(this.importToolStripMenuItem2_Click);
            // 
            // exportToolStripMenuItem1
            // 
            resources.ApplyResources(this.exportToolStripMenuItem1, "exportToolStripMenuItem1");
            this.exportToolStripMenuItem1.Name = "exportToolStripMenuItem1";
            this.exportToolStripMenuItem1.Tag = "MultipleNodes";
            this.exportToolStripMenuItem1.Click += new System.EventHandler(this.exportToolStripMenuItem1_Click);
            // 
            // elementContextMenu
            // 
            resources.ApplyResources(this.elementContextMenu, "elementContextMenu");
            this.elementContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.elementContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem1,
            this.renameToolStripMenuItem3,
            this.selectElementKeyMenuItem,
            this.modeElementStartingToolStripMenuItem1,
            this.toolStripSeparator11,
            this.toolStripMenuItem9,
            this.toolStripMenuItem10,
            this.deleteToolStripMenuItem2,
            this.toolStripSeparator7,
            this.exportToolStripMenuItem2,
            this.tagsToolStripMenuItem});
            this.elementContextMenu.Name = "elementContextMenu";
            this.elementContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.elementContextMenu_Opening);
            // 
            // editToolStripMenuItem1
            // 
            resources.ApplyResources(this.editToolStripMenuItem1, "editToolStripMenuItem1");
            this.editToolStripMenuItem1.Name = "editToolStripMenuItem1";
            this.editToolStripMenuItem1.Tag = "DuringPlay";
            this.editToolStripMenuItem1.Click += new System.EventHandler(this.editToolStripMenuItem1_Click);
            // 
            // renameToolStripMenuItem3
            // 
            resources.ApplyResources(this.renameToolStripMenuItem3, "renameToolStripMenuItem3");
            this.renameToolStripMenuItem3.Name = "renameToolStripMenuItem3";
            this.renameToolStripMenuItem3.Click += new System.EventHandler(this.renameToolStripMenuItem3_Click);
            // 
            // selectElementKeyMenuItem
            // 
            resources.ApplyResources(this.selectElementKeyMenuItem, "selectElementKeyMenuItem");
            this.selectElementKeyMenuItem.Name = "selectElementKeyMenuItem";
            this.selectElementKeyMenuItem.Tag = "OnlyMode DuringPlay";
            this.selectElementKeyMenuItem.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // modeElementStartingToolStripMenuItem1
            // 
            resources.ApplyResources(this.modeElementStartingToolStripMenuItem1, "modeElementStartingToolStripMenuItem1");
            this.modeElementStartingToolStripMenuItem1.Name = "modeElementStartingToolStripMenuItem1";
            this.modeElementStartingToolStripMenuItem1.Tag = "OnlyMode DuringPlay";
            this.modeElementStartingToolStripMenuItem1.Click += new System.EventHandler(this.selectKeyToolStripMenuItem1_Click);
            // 
            // toolStripSeparator11
            // 
            resources.ApplyResources(this.toolStripSeparator11, "toolStripSeparator11");
            this.toolStripSeparator11.Name = "toolStripSeparator11";
            // 
            // toolStripMenuItem9
            // 
            resources.ApplyResources(this.toolStripMenuItem9, "toolStripMenuItem9");
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Click += new System.EventHandler(this.toolStripMenuItem9_Click);
            // 
            // toolStripMenuItem10
            // 
            resources.ApplyResources(this.toolStripMenuItem10, "toolStripMenuItem10");
            this.toolStripMenuItem10.Name = "toolStripMenuItem10";
            this.toolStripMenuItem10.Click += new System.EventHandler(this.toolStripMenuItem10_Click);
            // 
            // deleteToolStripMenuItem2
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem2, "deleteToolStripMenuItem2");
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            this.deleteToolStripMenuItem2.Tag = "MultipleNodes";
            this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem2_Click);
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // exportToolStripMenuItem2
            // 
            resources.ApplyResources(this.exportToolStripMenuItem2, "exportToolStripMenuItem2");
            this.exportToolStripMenuItem2.Name = "exportToolStripMenuItem2";
            this.exportToolStripMenuItem2.Tag = "MultipleNodes";
            this.exportToolStripMenuItem2.Click += new System.EventHandler(this.exportToolStripMenuItem2_Click);
            // 
            // tagsToolStripMenuItem
            // 
            resources.ApplyResources(this.tagsToolStripMenuItem, "tagsToolStripMenuItem");
            this.tagsToolStripMenuItem.Name = "tagsToolStripMenuItem";
            this.tagsToolStripMenuItem.Tag = "OnlyMusicLists";
            this.tagsToolStripMenuItem.Click += new System.EventHandler(this.tagsToolStripMenuItem_Click);
            // 
            // bgSoundsContextMenu
            // 
            resources.ApplyResources(this.bgSoundsContextMenu, "bgSoundsContextMenu");
            this.bgSoundsContextMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.bgSoundsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem4,
            this.selectBGSoundsKeyMenuItem,
            this.modeElementStartingToolStripMenuItem2,
            this.addSoundChoiceToolStripMenuItem,
            this.toolStripSeparator12,
            this.toolStripMenuItem11,
            this.toolStripMenuItem12,
            this.toolStripMenuItem13,
            this.deleteToolStripMenuItem3,
            this.toolStripSeparator8,
            this.importToolStripMenuItem4,
            this.exportToolStripMenuItem3});
            this.bgSoundsContextMenu.Name = "bgSoundsContextMenu";
            this.bgSoundsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.bgSoundsContextMenu_Opening);
            // 
            // renameToolStripMenuItem4
            // 
            resources.ApplyResources(this.renameToolStripMenuItem4, "renameToolStripMenuItem4");
            this.renameToolStripMenuItem4.Name = "renameToolStripMenuItem4";
            this.renameToolStripMenuItem4.Click += new System.EventHandler(this.renameToolStripMenuItem4_Click);
            // 
            // selectBGSoundsKeyMenuItem
            // 
            resources.ApplyResources(this.selectBGSoundsKeyMenuItem, "selectBGSoundsKeyMenuItem");
            this.selectBGSoundsKeyMenuItem.Name = "selectBGSoundsKeyMenuItem";
            this.selectBGSoundsKeyMenuItem.Tag = "OnlyMode DuringPlay";
            this.selectBGSoundsKeyMenuItem.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // modeElementStartingToolStripMenuItem2
            // 
            resources.ApplyResources(this.modeElementStartingToolStripMenuItem2, "modeElementStartingToolStripMenuItem2");
            this.modeElementStartingToolStripMenuItem2.Name = "modeElementStartingToolStripMenuItem2";
            this.modeElementStartingToolStripMenuItem2.Tag = "OnlyMode DuringPlay";
            this.modeElementStartingToolStripMenuItem2.Click += new System.EventHandler(this.selectKeyToolStripMenuItem2_Click);
            // 
            // addSoundChoiceToolStripMenuItem
            // 
            resources.ApplyResources(this.addSoundChoiceToolStripMenuItem, "addSoundChoiceToolStripMenuItem");
            this.addSoundChoiceToolStripMenuItem.Name = "addSoundChoiceToolStripMenuItem";
            this.addSoundChoiceToolStripMenuItem.Click += new System.EventHandler(this.addSoundChoiceToolStripMenuItem_Click);
            // 
            // toolStripSeparator12
            // 
            resources.ApplyResources(this.toolStripSeparator12, "toolStripSeparator12");
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            // 
            // toolStripMenuItem11
            // 
            resources.ApplyResources(this.toolStripMenuItem11, "toolStripMenuItem11");
            this.toolStripMenuItem11.Name = "toolStripMenuItem11";
            this.toolStripMenuItem11.Click += new System.EventHandler(this.toolStripMenuItem11_Click_1);
            // 
            // toolStripMenuItem12
            // 
            resources.ApplyResources(this.toolStripMenuItem12, "toolStripMenuItem12");
            this.toolStripMenuItem12.Name = "toolStripMenuItem12";
            this.toolStripMenuItem12.Click += new System.EventHandler(this.toolStripMenuItem12_Click);
            // 
            // toolStripMenuItem13
            // 
            resources.ApplyResources(this.toolStripMenuItem13, "toolStripMenuItem13");
            this.toolStripMenuItem13.Name = "toolStripMenuItem13";
            this.toolStripMenuItem13.Tag = "Paste";
            this.toolStripMenuItem13.Click += new System.EventHandler(this.toolStripMenuItem13_Click);
            // 
            // deleteToolStripMenuItem3
            // 
            resources.ApplyResources(this.deleteToolStripMenuItem3, "deleteToolStripMenuItem3");
            this.deleteToolStripMenuItem3.Name = "deleteToolStripMenuItem3";
            this.deleteToolStripMenuItem3.Tag = "MultipleNodes";
            this.deleteToolStripMenuItem3.Click += new System.EventHandler(this.deleteToolStripMenuItem3_Click);
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // importToolStripMenuItem4
            // 
            resources.ApplyResources(this.importToolStripMenuItem4, "importToolStripMenuItem4");
            this.importToolStripMenuItem4.Name = "importToolStripMenuItem4";
            this.importToolStripMenuItem4.Click += new System.EventHandler(this.importToolStripMenuItem4_Click);
            // 
            // exportToolStripMenuItem3
            // 
            resources.ApplyResources(this.exportToolStripMenuItem3, "exportToolStripMenuItem3");
            this.exportToolStripMenuItem3.Name = "exportToolStripMenuItem3";
            this.exportToolStripMenuItem3.Tag = "MultipleNodes";
            this.exportToolStripMenuItem3.Click += new System.EventHandler(this.exportToolStripMenuItem3_Click);
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.stopButton,
            this.setKeyButton,
            this.moveDownButton,
            this.moveUpButton});
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
            // setKeyButton
            // 
            resources.ApplyResources(this.setKeyButton, "setKeyButton");
            this.setKeyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.setKeyButton.Image = global::Ares.Editor.ImageResources.interrogation;
            this.setKeyButton.Name = "setKeyButton";
            this.setKeyButton.Click += new System.EventHandler(this.setKeyButton_Click);
            // 
            // moveDownButton
            // 
            resources.ApplyResources(this.moveDownButton, "moveDownButton");
            this.moveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // moveUpButton
            // 
            resources.ApplyResources(this.moveUpButton, "moveUpButton");
            this.moveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // exportDialog
            // 
            this.exportDialog.DefaultExt = "ares";
            resources.ApplyResources(this.exportDialog, "exportDialog");
            // 
            // importDialog
            // 
            this.importDialog.DefaultExt = "ares";
            resources.ApplyResources(this.importDialog, "importDialog");
            // 
            // ProjectExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.projectTree);
            this.Controls.Add(this.toolStrip1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Name = "ProjectExplorer";
            this.projectContextMenu.ResumeLayout(false);
            this.modeContextMenu.ResumeLayout(false);
            this.containerContextMenu.ResumeLayout(false);
            this.elementContextMenu.ResumeLayout(false);
            this.bgSoundsContextMenu.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip projectContextMenu;
        private MultiSelectTreeview.MultiSelectTreeview projectTree;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip modeContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem addScenarioToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRandomPlaylistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSequentialPlaylistToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addBackgroundSoundsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem addParallelElementToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addSequentialElementToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip containerContextMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem modeElementStartingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem addRandomPlaylistToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addSequentialPlaylistToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addBackgroundSoundsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem addParallelElementToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem addSequentialElementToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip elementContextMenu;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem modeElementStartingToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem addChoiceListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addChoiceListToolStripMenuItem1;
        private System.Windows.Forms.ContextMenuStrip bgSoundsContextMenu;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem addSoundChoiceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem modeElementStartingToolStripMenuItem2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton playButton;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripMenuItem selectContainerKeyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectElementKeyMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectBGSoundsKeyMenuItem;
        private System.Windows.Forms.ToolStripButton setKeyButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog exportDialog;
        private System.Windows.Forms.OpenFileDialog importDialog;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem9;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem10;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem11;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem12;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem13;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem14;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem15;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem16;
        private System.Windows.Forms.ToolStripMenuItem tagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem musicByTagsToolStripItem;
        private System.Windows.Forms.ToolStripMenuItem addMusicByTagsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton moveUpButton;
        private System.Windows.Forms.ToolStripButton moveDownButton;
        private System.Windows.Forms.ToolStripMenuItem webRadioMenuItem;
        private System.Windows.Forms.ToolStripMenuItem webRadioItem2;
        private System.Windows.Forms.ToolStripMenuItem sortModesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortModesByNameAscMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortModesByNameDescMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem17;
        private System.Windows.Forms.ToolStripMenuItem sortElementsByNameAscMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortElementsByNameDescMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortModesByKeyAscMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortModesByKeyDescMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortElementsByKeyAscMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortElementsByKeyDescMenuItem;
    }
}
