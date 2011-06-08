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
namespace Ares.Player
{
    partial class Player
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
                m_Instance.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Player));
            this.projectNameLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.soundVolumeBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.musicVolumeBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.overallVolumeBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.soundsLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.musicLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.elementsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.modeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripButton();
            this.messagesButton = new System.Windows.Forms.ToolStripButton();
            this.editorButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.aboutButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.previousButton = new System.Windows.Forms.ToolStripButton();
            this.nextButton = new System.Windows.Forms.ToolStripButton();
            this.broadCastTimer = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ipAddressBox = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.tcpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label11 = new System.Windows.Forms.Label();
            this.udpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.clientStateLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dummyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousMusicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextMusicTitleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extrasToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpOnlineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectNameLabel
            // 
            resources.ApplyResources(this.projectNameLabel, "projectNameLabel");
            this.projectNameLabel.Name = "projectNameLabel";
            this.projectNameLabel.UseCompatibleTextRendering = true;
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.soundVolumeBar);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.musicVolumeBar);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.overallVolumeBar);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // soundVolumeBar
            // 
            resources.ApplyResources(this.soundVolumeBar, "soundVolumeBar");
            this.soundVolumeBar.Maximum = 100;
            this.soundVolumeBar.Name = "soundVolumeBar";
            this.soundVolumeBar.SmallChange = 5;
            this.soundVolumeBar.TickFrequency = 5;
            this.soundVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.soundVolumeBar.Value = 100;
            this.soundVolumeBar.Scroll += new System.EventHandler(this.soundVolumeBar_Scroll);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // musicVolumeBar
            // 
            resources.ApplyResources(this.musicVolumeBar, "musicVolumeBar");
            this.musicVolumeBar.Maximum = 100;
            this.musicVolumeBar.Name = "musicVolumeBar";
            this.musicVolumeBar.SmallChange = 5;
            this.musicVolumeBar.TickFrequency = 5;
            this.musicVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.musicVolumeBar.Value = 100;
            this.musicVolumeBar.Scroll += new System.EventHandler(this.musicVolumeBar_Scroll);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // overallVolumeBar
            // 
            resources.ApplyResources(this.overallVolumeBar, "overallVolumeBar");
            this.overallVolumeBar.Maximum = 100;
            this.overallVolumeBar.Name = "overallVolumeBar";
            this.overallVolumeBar.SmallChange = 5;
            this.overallVolumeBar.TickFrequency = 5;
            this.overallVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.overallVolumeBar.Value = 100;
            this.overallVolumeBar.Scroll += new System.EventHandler(this.overallVolumeBar_Scroll);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // groupBox3
            // 
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.soundsLabel);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.musicLabel);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.projectNameLabel);
            this.groupBox3.Controls.Add(this.elementsLabel);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.modeLabel);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            this.groupBox3.UseCompatibleTextRendering = true;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            this.label9.UseCompatibleTextRendering = true;
            // 
            // soundsLabel
            // 
            resources.ApplyResources(this.soundsLabel, "soundsLabel");
            this.soundsLabel.Name = "soundsLabel";
            this.soundsLabel.UseCompatibleTextRendering = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            this.label8.UseCompatibleTextRendering = true;
            // 
            // musicLabel
            // 
            resources.ApplyResources(this.musicLabel, "musicLabel");
            this.musicLabel.Name = "musicLabel";
            this.musicLabel.UseCompatibleTextRendering = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.label7.UseCompatibleTextRendering = true;
            // 
            // elementsLabel
            // 
            resources.ApplyResources(this.elementsLabel, "elementsLabel");
            this.elementsLabel.Name = "elementsLabel";
            this.elementsLabel.UseCompatibleTextRendering = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.UseCompatibleTextRendering = true;
            // 
            // modeLabel
            // 
            resources.ApplyResources(this.modeLabel, "modeLabel");
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.UseCompatibleTextRendering = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            this.label5.UseCompatibleTextRendering = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "ares";
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.Filter = "*.ares";
            this.fileSystemWatcher1.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.messagesButton,
            this.editorButton,
            this.settingsButton,
            this.aboutButton,
            this.toolStripSeparator1,
            this.stopButton,
            this.previousButton,
            this.nextButton});
            this.toolStrip1.Name = "toolStrip1";
            // 
            // openButton
            // 
            resources.ApplyResources(this.openButton, "openButton");
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Name = "openButton";
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // messagesButton
            // 
            resources.ApplyResources(this.messagesButton, "messagesButton");
            this.messagesButton.CheckOnClick = true;
            this.messagesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messagesButton.Name = "messagesButton";
            this.messagesButton.CheckStateChanged += new System.EventHandler(this.messagesButton_CheckedChanged);
            // 
            // editorButton
            // 
            resources.ApplyResources(this.editorButton, "editorButton");
            this.editorButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.editorButton.Name = "editorButton";
            this.editorButton.Click += new System.EventHandler(this.editorButton_Click);
            // 
            // settingsButton
            // 
            resources.ApplyResources(this.settingsButton, "settingsButton");
            this.settingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // aboutButton
            // 
            resources.ApplyResources(this.aboutButton, "aboutButton");
            this.aboutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // toolStripSeparator1
            // 
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Name = "stopButton";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // previousButton
            // 
            resources.ApplyResources(this.previousButton, "previousButton");
            this.previousButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.previousButton.Name = "previousButton";
            this.previousButton.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextButton
            // 
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextButton.Name = "nextButton";
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.ipAddressBox);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.tcpPortUpDown);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.udpPortUpDown);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.disconnectButton);
            this.groupBox1.Controls.Add(this.clientStateLabel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // ipAddressBox
            // 
            resources.ApplyResources(this.ipAddressBox, "ipAddressBox");
            this.ipAddressBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ipAddressBox.FormattingEnabled = true;
            this.ipAddressBox.Name = "ipAddressBox";
            this.ipAddressBox.SelectedIndexChanged += new System.EventHandler(this.ipAddressBox_SelectedIndexChanged);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            this.label12.UseCompatibleTextRendering = true;
            // 
            // tcpPortUpDown
            // 
            resources.ApplyResources(this.tcpPortUpDown, "tcpPortUpDown");
            this.tcpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.tcpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.tcpPortUpDown.Name = "tcpPortUpDown";
            this.tcpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.tcpPortUpDown.ValueChanged += new System.EventHandler(this.tcpPortUpDown_ValueChanged);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            this.label11.UseCompatibleTextRendering = true;
            // 
            // udpPortUpDown
            // 
            resources.ApplyResources(this.udpPortUpDown, "udpPortUpDown");
            this.udpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.udpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udpPortUpDown.Name = "udpPortUpDown";
            this.udpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udpPortUpDown.ValueChanged += new System.EventHandler(this.udpPortUpDown_ValueChanged);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            this.label10.UseCompatibleTextRendering = true;
            // 
            // disconnectButton
            // 
            resources.ApplyResources(this.disconnectButton, "disconnectButton");
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.UseCompatibleTextRendering = true;
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // clientStateLabel
            // 
            resources.ApplyResources(this.clientStateLabel, "clientStateLabel");
            this.clientStateLabel.Name = "clientStateLabel";
            this.clientStateLabel.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.playToolStripMenuItem,
            this.extrasToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            // 
            // projectToolStripMenuItem
            // 
            resources.ApplyResources(this.projectToolStripMenuItem, "projectToolStripMenuItem");
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            // 
            // openToolStripMenuItem
            // 
            resources.ApplyResources(this.openToolStripMenuItem, "openToolStripMenuItem");
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openButton_Click);
            // 
            // recentToolStripMenuItem
            // 
            resources.ApplyResources(this.recentToolStripMenuItem, "recentToolStripMenuItem");
            this.recentToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dummyToolStripMenuItem});
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.DropDownOpening += new System.EventHandler(this.recentToolStripMenuItem_DropDownOpening);
            // 
            // dummyToolStripMenuItem
            // 
            resources.ApplyResources(this.dummyToolStripMenuItem, "dummyToolStripMenuItem");
            this.dummyToolStripMenuItem.Name = "dummyToolStripMenuItem";
            // 
            // toolStripSeparator2
            // 
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            // 
            // exitToolStripMenuItem
            // 
            resources.ApplyResources(this.exitToolStripMenuItem, "exitToolStripMenuItem");
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            resources.ApplyResources(this.playToolStripMenuItem, "playToolStripMenuItem");
            this.playToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stopAllToolStripMenuItem,
            this.previousMusicToolStripMenuItem,
            this.nextMusicTitleToolStripMenuItem});
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            // 
            // stopAllToolStripMenuItem
            // 
            resources.ApplyResources(this.stopAllToolStripMenuItem, "stopAllToolStripMenuItem");
            this.stopAllToolStripMenuItem.Name = "stopAllToolStripMenuItem";
            this.stopAllToolStripMenuItem.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // previousMusicToolStripMenuItem
            // 
            resources.ApplyResources(this.previousMusicToolStripMenuItem, "previousMusicToolStripMenuItem");
            this.previousMusicToolStripMenuItem.Name = "previousMusicToolStripMenuItem";
            this.previousMusicToolStripMenuItem.Click += new System.EventHandler(this.previousButton_Click);
            // 
            // nextMusicTitleToolStripMenuItem
            // 
            resources.ApplyResources(this.nextMusicTitleToolStripMenuItem, "nextMusicTitleToolStripMenuItem");
            this.nextMusicTitleToolStripMenuItem.Name = "nextMusicTitleToolStripMenuItem";
            this.nextMusicTitleToolStripMenuItem.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // extrasToolStripMenuItem
            // 
            resources.ApplyResources(this.extrasToolStripMenuItem, "extrasToolStripMenuItem");
            this.extrasToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startEditorToolStripMenuItem,
            this.messagesToolStripMenuItem,
            this.toolStripSeparator3,
            this.settingsToolStripMenuItem1});
            this.extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            this.extrasToolStripMenuItem.DropDownOpening += new System.EventHandler(this.extrasToolStripMenuItem_DropDownOpening);
            // 
            // startEditorToolStripMenuItem
            // 
            resources.ApplyResources(this.startEditorToolStripMenuItem, "startEditorToolStripMenuItem");
            this.startEditorToolStripMenuItem.Name = "startEditorToolStripMenuItem";
            this.startEditorToolStripMenuItem.Click += new System.EventHandler(this.editorButton_Click);
            // 
            // messagesToolStripMenuItem
            // 
            resources.ApplyResources(this.messagesToolStripMenuItem, "messagesToolStripMenuItem");
            this.messagesToolStripMenuItem.Name = "messagesToolStripMenuItem";
            this.messagesToolStripMenuItem.Click += new System.EventHandler(this.messagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            // 
            // settingsToolStripMenuItem1
            // 
            resources.ApplyResources(this.settingsToolStripMenuItem1, "settingsToolStripMenuItem1");
            this.settingsToolStripMenuItem1.Name = "settingsToolStripMenuItem1";
            this.settingsToolStripMenuItem1.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // helpToolStripMenuItem
            // 
            resources.ApplyResources(this.helpToolStripMenuItem, "helpToolStripMenuItem");
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.helpOnlineToolStripMenuItem,
            this.checkForUpdateToolStripMenuItem,
            this.toolStripSeparator4,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
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
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // aboutToolStripMenuItem
            // 
            resources.ApplyResources(this.aboutToolStripMenuItem, "aboutToolStripMenuItem");
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // Player
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Player";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Player_FormClosing);
            this.Load += new System.EventHandler(this.Player_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tcpPortUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label projectNameLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TrackBar soundVolumeBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar musicVolumeBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar overallVolumeBar;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label soundsLabel;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label musicLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label elementsLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton openButton;
        private System.Windows.Forms.ToolStripButton messagesButton;
        private System.Windows.Forms.ToolStripButton editorButton;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton aboutButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton previousButton;
        private System.Windows.Forms.ToolStripButton nextButton;
        private System.Windows.Forms.Timer broadCastTimer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label clientStateLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.NumericUpDown tcpPortUpDown;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.NumericUpDown udpPortUpDown;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousMusicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextMusicTitleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extrasToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startEditorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem messagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dummyToolStripMenuItem;
        private System.Windows.Forms.ComboBox ipAddressBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.ToolStripMenuItem helpOnlineToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem checkForUpdateToolStripMenuItem;
    }
}

