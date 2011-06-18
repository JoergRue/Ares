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
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.projectTree = new System.Windows.Forms.TreeView();
            this.modeContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addScenarioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRandomPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialPlaylistToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addBackgroundSoundsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.addParallelElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialElementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addChoiceListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.containerContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectContainerKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.addRandomPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialPlaylistToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addBackgroundSoundsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.addParallelElementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSequentialElementToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.addChoiceListToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.elementContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.editToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectElementKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.bgSoundsContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.selectBGSoundsKeyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.modeElementStartingToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSoundChoiceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.playButton = new System.Windows.Forms.ToolStripButton();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.setKeyButton = new System.Windows.Forms.ToolStripButton();
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
            this.projectContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.toolStripMenuItem1});
            this.projectContextMenu.Name = "projectContextMenu";
            resources.ApplyResources(this.projectContextMenu, "projectContextMenu");
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            resources.ApplyResources(this.renameToolStripMenuItem, "renameToolStripMenuItem");
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // projectTree
            // 
            resources.ApplyResources(this.projectTree, "projectTree");
            this.projectTree.HideSelection = false;
            this.projectTree.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.projectTree.Name = "projectTree";
            this.projectTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.projectTree_AfterLabelEdit);
            this.projectTree.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeCollapse);
            this.projectTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeExpand);
            this.projectTree.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.projectTree_BeforeSelect);
            this.projectTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.projectTree_AfterSelect);
            this.projectTree.DoubleClick += new System.EventHandler(this.projectTree_DoubleClick);
            this.projectTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.projectTree_KeyDown);
            this.projectTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this.projectTree_MouseDown);
            // 
            // modeContextMenu
            // 
            this.modeContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem1,
            this.toolStripMenuItem2,
            this.deleteToolStripMenuItem,
            this.toolStripSeparator1,
            this.addScenarioToolStripMenuItem,
            this.addRandomPlaylistToolStripMenuItem,
            this.addSequentialPlaylistToolStripMenuItem,
            this.addBackgroundSoundsToolStripMenuItem,
            this.toolStripSeparator2,
            this.addParallelElementToolStripMenuItem,
            this.addSequentialElementToolStripMenuItem,
            this.addChoiceListToolStripMenuItem});
            this.modeContextMenu.Name = "modeContextMenu";
            resources.ApplyResources(this.modeContextMenu, "modeContextMenu");
            this.modeContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.modeContextMenu_Opening);
            // 
            // renameToolStripMenuItem1
            // 
            this.renameToolStripMenuItem1.Name = "renameToolStripMenuItem1";
            resources.ApplyResources(this.renameToolStripMenuItem1, "renameToolStripMenuItem1");
            this.renameToolStripMenuItem1.Click += new System.EventHandler(this.renameToolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Tag = "DuringPlay";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            resources.ApplyResources(this.deleteToolStripMenuItem, "deleteToolStripMenuItem");
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // addScenarioToolStripMenuItem
            // 
            this.addScenarioToolStripMenuItem.Name = "addScenarioToolStripMenuItem";
            resources.ApplyResources(this.addScenarioToolStripMenuItem, "addScenarioToolStripMenuItem");
            this.addScenarioToolStripMenuItem.Click += new System.EventHandler(this.addScenarioToolStripMenuItem_Click);
            // 
            // addRandomPlaylistToolStripMenuItem
            // 
            this.addRandomPlaylistToolStripMenuItem.Name = "addRandomPlaylistToolStripMenuItem";
            resources.ApplyResources(this.addRandomPlaylistToolStripMenuItem, "addRandomPlaylistToolStripMenuItem");
            this.addRandomPlaylistToolStripMenuItem.Click += new System.EventHandler(this.addRandomPlaylistToolStripMenuItem_Click);
            // 
            // addSequentialPlaylistToolStripMenuItem
            // 
            this.addSequentialPlaylistToolStripMenuItem.Name = "addSequentialPlaylistToolStripMenuItem";
            resources.ApplyResources(this.addSequentialPlaylistToolStripMenuItem, "addSequentialPlaylistToolStripMenuItem");
            this.addSequentialPlaylistToolStripMenuItem.Click += new System.EventHandler(this.addSequentialPlaylistToolStripMenuItem_Click);
            // 
            // addBackgroundSoundsToolStripMenuItem
            // 
            this.addBackgroundSoundsToolStripMenuItem.Name = "addBackgroundSoundsToolStripMenuItem";
            resources.ApplyResources(this.addBackgroundSoundsToolStripMenuItem, "addBackgroundSoundsToolStripMenuItem");
            this.addBackgroundSoundsToolStripMenuItem.Click += new System.EventHandler(this.addBackgroundSoundsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // addParallelElementToolStripMenuItem
            // 
            this.addParallelElementToolStripMenuItem.Name = "addParallelElementToolStripMenuItem";
            resources.ApplyResources(this.addParallelElementToolStripMenuItem, "addParallelElementToolStripMenuItem");
            this.addParallelElementToolStripMenuItem.Click += new System.EventHandler(this.addParallelElementToolStripMenuItem_Click);
            // 
            // addSequentialElementToolStripMenuItem
            // 
            this.addSequentialElementToolStripMenuItem.Name = "addSequentialElementToolStripMenuItem";
            resources.ApplyResources(this.addSequentialElementToolStripMenuItem, "addSequentialElementToolStripMenuItem");
            this.addSequentialElementToolStripMenuItem.Click += new System.EventHandler(this.addSequentialElementToolStripMenuItem_Click);
            // 
            // addChoiceListToolStripMenuItem
            // 
            this.addChoiceListToolStripMenuItem.Name = "addChoiceListToolStripMenuItem";
            resources.ApplyResources(this.addChoiceListToolStripMenuItem, "addChoiceListToolStripMenuItem");
            this.addChoiceListToolStripMenuItem.Click += new System.EventHandler(this.addChoiceListToolStripMenuItem_Click);
            // 
            // containerContextMenu
            // 
            this.containerContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem,
            this.renameToolStripMenuItem2,
            this.selectContainerKeyMenuItem,
            this.modeElementStartingToolStripMenuItem,
            this.deleteToolStripMenuItem1,
            this.toolStripSeparator3,
            this.addRandomPlaylistToolStripMenuItem1,
            this.addSequentialPlaylistToolStripMenuItem1,
            this.addBackgroundSoundsToolStripMenuItem1,
            this.toolStripSeparator4,
            this.addParallelElementToolStripMenuItem1,
            this.addSequentialElementToolStripMenuItem1,
            this.addChoiceListToolStripMenuItem1});
            this.containerContextMenu.Name = "containerContextMenu";
            resources.ApplyResources(this.containerContextMenu, "containerContextMenu");
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
            this.renameToolStripMenuItem2.Name = "renameToolStripMenuItem2";
            resources.ApplyResources(this.renameToolStripMenuItem2, "renameToolStripMenuItem2");
            this.renameToolStripMenuItem2.Click += new System.EventHandler(this.renameToolStripMenuItem2_Click);
            // 
            // selectContainerKeyMenuItem
            // 
            this.selectContainerKeyMenuItem.Name = "selectContainerKeyMenuItem";
            resources.ApplyResources(this.selectContainerKeyMenuItem, "selectContainerKeyMenuItem");
            this.selectContainerKeyMenuItem.Tag = "OnlyMode DuringPlay";
            this.selectContainerKeyMenuItem.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // modeElementStartingToolStripMenuItem
            // 
            this.modeElementStartingToolStripMenuItem.Name = "modeElementStartingToolStripMenuItem";
            resources.ApplyResources(this.modeElementStartingToolStripMenuItem, "modeElementStartingToolStripMenuItem");
            this.modeElementStartingToolStripMenuItem.Tag = "OnlyMode DuringPlay";
            this.modeElementStartingToolStripMenuItem.Click += new System.EventHandler(this.selectKeyToolStripMenuItem_Click);
            // 
            // deleteToolStripMenuItem1
            // 
            this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
            resources.ApplyResources(this.deleteToolStripMenuItem1, "deleteToolStripMenuItem1");
            this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // addRandomPlaylistToolStripMenuItem1
            // 
            this.addRandomPlaylistToolStripMenuItem1.Name = "addRandomPlaylistToolStripMenuItem1";
            resources.ApplyResources(this.addRandomPlaylistToolStripMenuItem1, "addRandomPlaylistToolStripMenuItem1");
            this.addRandomPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.addRandomPlaylistToolStripMenuItem1_Click);
            // 
            // addSequentialPlaylistToolStripMenuItem1
            // 
            this.addSequentialPlaylistToolStripMenuItem1.Name = "addSequentialPlaylistToolStripMenuItem1";
            resources.ApplyResources(this.addSequentialPlaylistToolStripMenuItem1, "addSequentialPlaylistToolStripMenuItem1");
            this.addSequentialPlaylistToolStripMenuItem1.Click += new System.EventHandler(this.addSequentialPlaylistToolStripMenuItem1_Click);
            // 
            // addBackgroundSoundsToolStripMenuItem1
            // 
            this.addBackgroundSoundsToolStripMenuItem1.Name = "addBackgroundSoundsToolStripMenuItem1";
            resources.ApplyResources(this.addBackgroundSoundsToolStripMenuItem1, "addBackgroundSoundsToolStripMenuItem1");
            this.addBackgroundSoundsToolStripMenuItem1.Click += new System.EventHandler(this.addBackgroundSoundsToolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // addParallelElementToolStripMenuItem1
            // 
            this.addParallelElementToolStripMenuItem1.Name = "addParallelElementToolStripMenuItem1";
            resources.ApplyResources(this.addParallelElementToolStripMenuItem1, "addParallelElementToolStripMenuItem1");
            this.addParallelElementToolStripMenuItem1.Click += new System.EventHandler(this.addParallelElementToolStripMenuItem1_Click);
            // 
            // addSequentialElementToolStripMenuItem1
            // 
            this.addSequentialElementToolStripMenuItem1.Name = "addSequentialElementToolStripMenuItem1";
            resources.ApplyResources(this.addSequentialElementToolStripMenuItem1, "addSequentialElementToolStripMenuItem1");
            this.addSequentialElementToolStripMenuItem1.Click += new System.EventHandler(this.addSequentialElementToolStripMenuItem1_Click);
            // 
            // addChoiceListToolStripMenuItem1
            // 
            this.addChoiceListToolStripMenuItem1.Name = "addChoiceListToolStripMenuItem1";
            resources.ApplyResources(this.addChoiceListToolStripMenuItem1, "addChoiceListToolStripMenuItem1");
            this.addChoiceListToolStripMenuItem1.Click += new System.EventHandler(this.addChoiceListToolStripMenuItem1_Click);
            // 
            // elementContextMenu
            // 
            this.elementContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editToolStripMenuItem1,
            this.renameToolStripMenuItem3,
            this.selectElementKeyMenuItem,
            this.modeElementStartingToolStripMenuItem1,
            this.deleteToolStripMenuItem2});
            this.elementContextMenu.Name = "elementContextMenu";
            resources.ApplyResources(this.elementContextMenu, "elementContextMenu");
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
            this.renameToolStripMenuItem3.Name = "renameToolStripMenuItem3";
            resources.ApplyResources(this.renameToolStripMenuItem3, "renameToolStripMenuItem3");
            this.renameToolStripMenuItem3.Click += new System.EventHandler(this.renameToolStripMenuItem3_Click);
            // 
            // selectElementKeyMenuItem
            // 
            this.selectElementKeyMenuItem.Name = "selectElementKeyMenuItem";
            resources.ApplyResources(this.selectElementKeyMenuItem, "selectElementKeyMenuItem");
            this.selectElementKeyMenuItem.Tag = "OnlyMode DuringPlay";
            this.selectElementKeyMenuItem.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // modeElementStartingToolStripMenuItem1
            // 
            this.modeElementStartingToolStripMenuItem1.Name = "modeElementStartingToolStripMenuItem1";
            resources.ApplyResources(this.modeElementStartingToolStripMenuItem1, "modeElementStartingToolStripMenuItem1");
            this.modeElementStartingToolStripMenuItem1.Tag = "OnlyMode DuringPlay";
            this.modeElementStartingToolStripMenuItem1.Click += new System.EventHandler(this.selectKeyToolStripMenuItem1_Click);
            // 
            // deleteToolStripMenuItem2
            // 
            this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
            resources.ApplyResources(this.deleteToolStripMenuItem2, "deleteToolStripMenuItem2");
            this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem2_Click);
            // 
            // bgSoundsContextMenu
            // 
            this.bgSoundsContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem4,
            this.selectBGSoundsKeyMenuItem,
            this.modeElementStartingToolStripMenuItem2,
            this.deleteToolStripMenuItem3,
            this.addSoundChoiceToolStripMenuItem});
            this.bgSoundsContextMenu.Name = "bgSoundsContextMenu";
            resources.ApplyResources(this.bgSoundsContextMenu, "bgSoundsContextMenu");
            this.bgSoundsContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.bgSoundsContextMenu_Opening);
            // 
            // renameToolStripMenuItem4
            // 
            this.renameToolStripMenuItem4.Name = "renameToolStripMenuItem4";
            resources.ApplyResources(this.renameToolStripMenuItem4, "renameToolStripMenuItem4");
            this.renameToolStripMenuItem4.Click += new System.EventHandler(this.renameToolStripMenuItem4_Click);
            // 
            // selectBGSoundsKeyMenuItem
            // 
            this.selectBGSoundsKeyMenuItem.Name = "selectBGSoundsKeyMenuItem";
            resources.ApplyResources(this.selectBGSoundsKeyMenuItem, "selectBGSoundsKeyMenuItem");
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
            // deleteToolStripMenuItem3
            // 
            this.deleteToolStripMenuItem3.Name = "deleteToolStripMenuItem3";
            resources.ApplyResources(this.deleteToolStripMenuItem3, "deleteToolStripMenuItem3");
            this.deleteToolStripMenuItem3.Click += new System.EventHandler(this.deleteToolStripMenuItem3_Click);
            // 
            // addSoundChoiceToolStripMenuItem
            // 
            this.addSoundChoiceToolStripMenuItem.Name = "addSoundChoiceToolStripMenuItem";
            resources.ApplyResources(this.addSoundChoiceToolStripMenuItem, "addSoundChoiceToolStripMenuItem");
            this.addSoundChoiceToolStripMenuItem.Click += new System.EventHandler(this.addSoundChoiceToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playButton,
            this.stopButton,
            this.setKeyButton});
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
            // setKeyButton
            // 
            this.setKeyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            resources.ApplyResources(this.setKeyButton, "setKeyButton");
            this.setKeyButton.Image = global::Ares.Editor.ImageResources.interrogation;
            this.setKeyButton.Name = "setKeyButton";
            this.setKeyButton.Click += new System.EventHandler(this.setKeyButton_Click);
            // 
            // ProjectExplorer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.projectTree);
            this.Controls.Add(this.toolStrip1);
#if !MONO
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
                        | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
#endif
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
        private System.Windows.Forms.TreeView projectTree;
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
    }
}
