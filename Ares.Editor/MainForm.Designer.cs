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
    partial class MainForm
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
                Shutdown();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            WeifenLuo.WinFormsUI.Docking.DockPanelSkin dockPanelSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPanelSkin();
            WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin autoHideStripSkin1 = new WeifenLuo.WinFormsUI.Docking.AutoHideStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient1 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin dockPaneStripSkin1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripSkin();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient dockPaneStripGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient2 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient2 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient3 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient dockPaneStripToolWindowGradient1 = new WeifenLuo.WinFormsUI.Docking.DockPaneStripToolWindowGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient4 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient5 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.DockPanelGradient dockPanelGradient3 = new WeifenLuo.WinFormsUI.Docking.DockPanelGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient6 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            WeifenLuo.WinFormsUI.Docking.TabGradient tabGradient7 = new WeifenLuo.WinFormsUI.Docking.TabGradient();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.recentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dummyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.soundFileExplorerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.volumesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectErrorsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startPlayerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.stopAllToolStipMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hilfeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.newButton = new System.Windows.Forms.ToolStripButton();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.undoButton = new System.Windows.Forms.ToolStripButton();
            this.redoButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editMenu,
            this.viewToolStripMenuItem,
            this.extrasToolStripMenuItem,
            this.hilfeToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator2,
            this.recentMenuItem,
            this.toolStripSeparator5,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            // 
            // newToolStripMenuItem
            // 
            resources.ApplyResources(this.newToolStripMenuItem, "newToolStripMenuItem");
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // saveToolStripMenuItem
            // 
            resources.ApplyResources(this.saveToolStripMenuItem, "saveToolStripMenuItem");
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            resources.ApplyResources(this.saveAsToolStripMenuItem, "saveAsToolStripMenuItem");
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // recentMenuItem
            // 
            resources.ApplyResources(this.recentMenuItem, "recentMenuItem");
            this.recentMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dummyToolStripMenuItem});
            this.recentMenuItem.Name = "recentMenuItem";
            this.recentMenuItem.DropDownOpening += new System.EventHandler(this.recentMenuItem_DropDownOpening);
            // 
            // dummyToolStripMenuItem
            // 
            resources.ApplyResources(this.dummyToolStripMenuItem, "dummyToolStripMenuItem");
            this.dummyToolStripMenuItem.Name = "dummyToolStripMenuItem";
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // editMenu
            // 
            resources.ApplyResources(this.editMenu, "editMenu");
            this.editMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem});
            this.editMenu.Name = "editMenu";
            this.editMenu.DropDownClosed += new System.EventHandler(this.editMenu_DropDownClosed);
            this.editMenu.DropDownOpening += new System.EventHandler(this.editMenu_DropDownOpening);
            // 
            // undoToolStripMenuItem
            // 
            resources.ApplyResources(this.undoToolStripMenuItem, "undoToolStripMenuItem");
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            resources.ApplyResources(this.redoToolStripMenuItem, "redoToolStripMenuItem");
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectExplorerToolStripMenuItem,
            this.fileExplorerToolStripMenuItem,
            this.soundFileExplorerToolStripMenuItem,
            this.volumesToolStripMenuItem,
            this.projectErrorsToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.DropDownOpened += new System.EventHandler(this.viewToolStripMenuItem_DropDownOpened);
            // 
            // projectExplorerToolStripMenuItem
            // 
            resources.ApplyResources(this.projectExplorerToolStripMenuItem, "projectExplorerToolStripMenuItem");
            this.projectExplorerToolStripMenuItem.Name = "projectExplorerToolStripMenuItem";
            this.projectExplorerToolStripMenuItem.Click += new System.EventHandler(this.projectExplorerToolStripMenuItem_Click);
            // 
            // fileExplorerToolStripMenuItem
            // 
            resources.ApplyResources(this.fileExplorerToolStripMenuItem, "fileExplorerToolStripMenuItem");
            this.fileExplorerToolStripMenuItem.Name = "fileExplorerToolStripMenuItem";
            this.fileExplorerToolStripMenuItem.Click += new System.EventHandler(this.fileExplorerToolStripMenuItem_Click);
            // 
            // soundFileExplorerToolStripMenuItem
            // 
            resources.ApplyResources(this.soundFileExplorerToolStripMenuItem, "soundFileExplorerToolStripMenuItem");
            this.soundFileExplorerToolStripMenuItem.Name = "soundFileExplorerToolStripMenuItem";
            this.soundFileExplorerToolStripMenuItem.Click += new System.EventHandler(this.soundFileExplorerToolStripMenuItem_Click);
            // 
            // volumesToolStripMenuItem
            // 
            resources.ApplyResources(this.volumesToolStripMenuItem, "volumesToolStripMenuItem");
            this.volumesToolStripMenuItem.Name = "volumesToolStripMenuItem";
            this.volumesToolStripMenuItem.Click += new System.EventHandler(this.volumesToolStripMenuItem_Click);
            // 
            // projectErrorsToolStripMenuItem
            // 
            resources.ApplyResources(this.projectErrorsToolStripMenuItem, "projectErrorsToolStripMenuItem");
            this.projectErrorsToolStripMenuItem.Name = "projectErrorsToolStripMenuItem";
            this.projectErrorsToolStripMenuItem.Click += new System.EventHandler(this.projectErrorsToolStripMenuItem_Click);
            // 
            // extrasToolStripMenuItem
            // 
            resources.ApplyResources(this.extrasToolStripMenuItem, "extrasToolStripMenuItem");
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startPlayerToolStripMenuItem,
            this.toolStripSeparator3,
            this.stopAllToolStipMenuItem,
            this.toolStripSeparator4,
            this.settingsToolStripMenuItem});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.DropDownOpening += new System.EventHandler(this.extrasToolStripMenuItem_DropDownOpening);
            // 
            // startPlayerToolStripMenuItem
            // 
            resources.ApplyResources(this.startPlayerToolStripMenuItem, "startPlayerToolStripMenuItem");
            this.startPlayerToolStripMenuItem.Name = "startPlayerToolStripMenuItem";
            this.startPlayerToolStripMenuItem.Click += new System.EventHandler(this.startPlayerToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // stopAllToolStipMenuItem
            // 
            resources.ApplyResources(this.stopAllToolStipMenuItem, "stopAllToolStipMenuItem");
            this.stopAllToolStipMenuItem.Name = "stopAllToolStipMenuItem";
            this.stopAllToolStipMenuItem.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // settingsToolStripMenuItem
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem, "settingsToolStripMenuItem");
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // hilfeToolStripMenuItem
            // 
            resources.ApplyResources(this.hilfeToolStripMenuItem, "hilfeToolStripMenuItem");
            this.hilfeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpOnlineToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem,
            this.toolStripSeparator8,
            this.aboutToolStripMenuItem});
            this.hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            // 
            // helpOnlineToolStripMenuItem
            // 
            resources.ApplyResources(this.helpOnlineToolStripMenuItem, "helpOnlineToolStripMenuItem");
            this.helpOnlineToolStripMenuItem.Name = "helpOnlineToolStripMenuItem";
            this.helpOnlineToolStripMenuItem.Click += new System.EventHandler(this.helpOnlineToolStripMenuItem_Click);
            // 
            // checkForUpdateToolStripMenuItem
            // 
            resources.ApplyResources(this.checkForUpdateToolStripMenuItem, "checkForUpdateToolStripMenuItem");
            this.checkForUpdateToolStripMenuItem.Name = "checkForUpdateToolStripMenuItem";
            this.checkForUpdateToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdateToolStripMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            resources.ApplyResources(this.toolStripSeparator8, "toolStripSeparator8");
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // dockPanel
            // 
            resources.ApplyResources(this.dockPanel, "dockPanel");
            this.dockPanel.ActiveAutoHideContent = null;
            this.dockPanel.DockBackColor = System.Drawing.SystemColors.Control;
            this.dockPanel.DockLeftPortion = 0.2D;
            this.dockPanel.Name = "dockPanel";
            dockPanelGradient1.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient1.StartColor = System.Drawing.SystemColors.ControlLight;
            autoHideStripSkin1.DockStripGradient = dockPanelGradient1;
            tabGradient1.EndColor = System.Drawing.SystemColors.Control;
            tabGradient1.StartColor = System.Drawing.SystemColors.Control;
            tabGradient1.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            autoHideStripSkin1.TabGradient = tabGradient1;
            dockPanelSkin1.AutoHideStripSkin = autoHideStripSkin1;
            tabGradient2.EndColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.StartColor = System.Drawing.SystemColors.ControlLightLight;
            tabGradient2.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.ActiveTabGradient = tabGradient2;
            dockPanelGradient2.EndColor = System.Drawing.SystemColors.Control;
            dockPanelGradient2.StartColor = System.Drawing.SystemColors.Control;
            dockPaneStripGradient1.DockStripGradient = dockPanelGradient2;
            tabGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            tabGradient3.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripGradient1.InactiveTabGradient = tabGradient3;
            dockPaneStripSkin1.DocumentGradient = dockPaneStripGradient1;
            tabGradient4.EndColor = System.Drawing.SystemColors.ActiveCaption;
            tabGradient4.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient4.StartColor = System.Drawing.SystemColors.GradientActiveCaption;
            tabGradient4.TextColor = System.Drawing.SystemColors.ActiveCaptionText;
            dockPaneStripToolWindowGradient1.ActiveCaptionGradient = tabGradient4;
            tabGradient5.EndColor = System.Drawing.SystemColors.Control;
            tabGradient5.StartColor = System.Drawing.SystemColors.Control;
            tabGradient5.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.ActiveTabGradient = tabGradient5;
            dockPanelGradient3.EndColor = System.Drawing.SystemColors.ControlLight;
            dockPanelGradient3.StartColor = System.Drawing.SystemColors.ControlLight;
            dockPaneStripToolWindowGradient1.DockStripGradient = dockPanelGradient3;
            tabGradient6.EndColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            tabGradient6.StartColor = System.Drawing.SystemColors.GradientInactiveCaption;
            tabGradient6.TextColor = System.Drawing.SystemColors.ControlText;
            dockPaneStripToolWindowGradient1.InactiveCaptionGradient = tabGradient6;
            tabGradient7.EndColor = System.Drawing.Color.Transparent;
            tabGradient7.StartColor = System.Drawing.Color.Transparent;
            tabGradient7.TextColor = System.Drawing.SystemColors.ControlDarkDark;
            dockPaneStripToolWindowGradient1.InactiveTabGradient = tabGradient7;
            dockPaneStripSkin1.ToolWindowGradient = dockPaneStripToolWindowGradient1;
            dockPanelSkin1.DockPaneStripSkin = dockPaneStripSkin1;
            this.dockPanel.Skin = dockPanelSkin1;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "ares";
            resources.ApplyResources(this.saveFileDialog, "saveFileDialog");
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "ares";
            resources.ApplyResources(this.openFileDialog, "openFileDialog");
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newButton,
            this.openButton,
            this.saveButton,
            this.toolStripSeparator6,
            this.undoButton,
            this.redoButton,
            this.toolStripSeparator7,
            this.stopButton});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // newButton
            // 
            resources.ApplyResources(this.newButton, "newButton");
            this.newButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.newButton.Name = "newButton";
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // openButton
            // 
            resources.ApplyResources(this.openButton, "openButton");
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Name = "openButton";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // saveButton
            // 
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Name = "saveButton";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // toolStripSeparator6
            // 
            resources.ApplyResources(this.toolStripSeparator6, "toolStripSeparator6");
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            // 
            // undoButton
            // 
            resources.ApplyResources(this.undoButton, "undoButton");
            this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.undoButton.Image = global::Ares.Editor.ImageResources.undo;
            this.undoButton.Name = "undoButton";
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // redoButton
            // 
            resources.ApplyResources(this.redoButton, "redoButton");
            this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.redoButton.Image = global::Ares.Editor.ImageResources.redo;
            this.redoButton.Name = "redoButton";
            this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
            // 
            // toolStripSeparator7
            // 
            resources.ApplyResources(this.toolStripSeparator7, "toolStripSeparator7");
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Name = "stopButton";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.Filter = "*.xml";
            this.fileSystemWatcher1.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // MainForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startPlayerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
#if !MONO
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
#endif
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem editMenu;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem stopAllToolStipMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem recentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dummyToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton newButton;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton undoButton;
        private System.Windows.Forms.ToolStripButton redoButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripMenuItem soundFileExplorerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hilfeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.ToolStripMenuItem volumesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem projectErrorsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpOnlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdateToolStripMenuItem;
    }
}

