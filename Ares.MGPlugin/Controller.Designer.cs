namespace Ares.MGPlugin
{
    partial class Controller
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
                DoDispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Controller));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.soundsBar = new System.Windows.Forms.TrackBar();
            this.musicBar = new System.Windows.Forms.TrackBar();
            this.overallVolumeBar = new System.Windows.Forms.TrackBar();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.musicLabel = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.elementsLabel = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.modeLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.projectLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.serverBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.modesList = new System.Windows.Forms.ListBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.elementsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.musicList = new System.Windows.Forms.ListBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.openButton = new System.Windows.Forms.ToolStripSplitButton();
            this.messagesButton = new System.Windows.Forms.ToolStripButton();
            this.settingsButton = new System.Windows.Forms.ToolStripButton();
            this.aboutButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.stopButton = new System.Windows.Forms.ToolStripButton();
            this.prevMusicButton = new System.Windows.Forms.ToolStripButton();
            this.nextMusicButton = new System.Windows.Forms.ToolStripButton();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.overLayPanel = new System.Windows.Forms.Panel();
            this.overlayOKButton = new System.Windows.Forms.Button();
            this.overlayCloseButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.transparentFlowLayoutPanel2 = new Ares.MGPlugin.TransparentFlowLayoutPanel();
            this.overlayLabel5 = new Ares.MGPlugin.TransparentLabel();
            this.settingsLabel = new Ares.MGPlugin.TransparentLinkLabel();
            this.overlayLabel6 = new Ares.MGPlugin.TransparentLabel();
            this.transparentFlowLayoutPanel1 = new Ares.MGPlugin.TransparentFlowLayoutPanel();
            this.overlayLabel3 = new Ares.MGPlugin.TransparentLabel();
            this.setupLabel = new Ares.MGPlugin.TransparentLinkLabel();
            this.overlayLabel4 = new Ares.MGPlugin.TransparentLabel();
            this.flowLayoutPanel1 = new Ares.MGPlugin.TransparentFlowLayoutPanel();
            this.overlayLabel2 = new Ares.MGPlugin.TransparentLabel();
            this.homepageLabel = new Ares.MGPlugin.TransparentLinkLabel();
            this.overlayLabel9 = new Ares.MGPlugin.TransparentLabel();
            this.overlayLabel7 = new Ares.MGPlugin.TransparentLabel();
            this.overlayLabel1 = new Ares.MGPlugin.TransparentLabel();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundsBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.overLayPanel.SuspendLayout();
            this.transparentFlowLayoutPanel2.SuspendLayout();
            this.transparentFlowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.soundsBar);
            this.groupBox1.Controls.Add(this.musicBar);
            this.groupBox1.Controls.Add(this.overallVolumeBar);
            this.groupBox1.Location = new System.Drawing.Point(3, 40);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 104);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lautstärke";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Geräusche: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Musik: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gesamt: ";
            // 
            // soundsBar
            // 
            this.soundsBar.AutoSize = false;
            this.soundsBar.Location = new System.Drawing.Point(65, 77);
            this.soundsBar.Maximum = 100;
            this.soundsBar.Name = "soundsBar";
            this.soundsBar.Size = new System.Drawing.Size(203, 25);
            this.soundsBar.TabIndex = 4;
            this.soundsBar.TickFrequency = 10;
            this.soundsBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.soundsBar.Value = 50;
            this.soundsBar.ValueChanged += new System.EventHandler(this.soundsBar_ValueChanged);
            // 
            // musicBar
            // 
            this.musicBar.AutoSize = false;
            this.musicBar.Location = new System.Drawing.Point(65, 47);
            this.musicBar.Maximum = 100;
            this.musicBar.Name = "musicBar";
            this.musicBar.Size = new System.Drawing.Size(203, 24);
            this.musicBar.TabIndex = 2;
            this.musicBar.TickFrequency = 10;
            this.musicBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.musicBar.Value = 50;
            this.musicBar.ValueChanged += new System.EventHandler(this.musicBar_ValueChanged);
            // 
            // overallVolumeBar
            // 
            this.overallVolumeBar.AutoSize = false;
            this.overallVolumeBar.Location = new System.Drawing.Point(65, 19);
            this.overallVolumeBar.Maximum = 100;
            this.overallVolumeBar.Name = "overallVolumeBar";
            this.overallVolumeBar.Size = new System.Drawing.Size(203, 22);
            this.overallVolumeBar.TabIndex = 1;
            this.overallVolumeBar.TickFrequency = 10;
            this.overallVolumeBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.overallVolumeBar.Value = 50;
            this.overallVolumeBar.ValueChanged += new System.EventHandler(this.overallVolumeBar_ValueChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.musicLabel);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.elementsLabel);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.modeLabel);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.projectLabel);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(3, 150);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(274, 130);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Status";
            // 
            // musicLabel
            // 
            this.musicLabel.Location = new System.Drawing.Point(58, 82);
            this.musicLabel.Name = "musicLabel";
            this.musicLabel.Size = new System.Drawing.Size(210, 37);
            this.musicLabel.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Musik: ";
            // 
            // elementsLabel
            // 
            this.elementsLabel.Location = new System.Drawing.Point(58, 60);
            this.elementsLabel.Name = "elementsLabel";
            this.elementsLabel.Size = new System.Drawing.Size(210, 13);
            this.elementsLabel.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 13);
            this.label6.TabIndex = 4;
            this.label6.Text = "Elemente: ";
            // 
            // modeLabel
            // 
            this.modeLabel.Location = new System.Drawing.Point(58, 38);
            this.modeLabel.Name = "modeLabel";
            this.modeLabel.Size = new System.Drawing.Size(210, 13);
            this.modeLabel.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Modus:  ";
            // 
            // projectLabel
            // 
            this.projectLabel.Location = new System.Drawing.Point(58, 16);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(210, 13);
            this.projectLabel.TabIndex = 1;
            this.projectLabel.Text = "- (bitte zuerst ein Projekt öffnen)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Projekt: ";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.connectButton);
            this.groupBox3.Controls.Add(this.serverBox);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(3, 286);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(274, 49);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Netzwerk";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(193, 14);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(75, 23);
            this.connectButton.TabIndex = 10;
            this.connectButton.Text = "Verbinden";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // serverBox
            // 
            this.serverBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Location = new System.Drawing.Point(61, 16);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(126, 21);
            this.serverBox.TabIndex = 9;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(44, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Server: ";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.modesList);
            this.groupBox4.Location = new System.Drawing.Point(283, 40);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(200, 256);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Modi";
            // 
            // modesList
            // 
            this.modesList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modesList.FormattingEnabled = true;
            this.modesList.Location = new System.Drawing.Point(6, 21);
            this.modesList.Name = "modesList";
            this.modesList.Size = new System.Drawing.Size(188, 225);
            this.modesList.TabIndex = 0;
            this.modesList.SelectedIndexChanged += new System.EventHandler(this.modesList_SelectedIndexChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.elementsPanel);
            this.groupBox5.Controls.Add(this.musicList);
            this.groupBox5.Location = new System.Drawing.Point(489, 40);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(230, 256);
            this.groupBox5.TabIndex = 4;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Elemente";
            // 
            // elementsPanel
            // 
            this.elementsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.elementsPanel.AutoScroll = true;
            this.elementsPanel.Location = new System.Drawing.Point(6, 21);
            this.elementsPanel.Name = "elementsPanel";
            this.elementsPanel.Size = new System.Drawing.Size(218, 232);
            this.elementsPanel.TabIndex = 2;
            // 
            // musicList
            // 
            this.musicList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.musicList.FormattingEnabled = true;
            this.musicList.Location = new System.Drawing.Point(6, 20);
            this.musicList.Name = "musicList";
            this.musicList.Size = new System.Drawing.Size(218, 225);
            this.musicList.TabIndex = 1;
            this.musicList.Visible = false;
            this.musicList.SelectedIndexChanged += new System.EventHandler(this.musicList_SelectedIndexChanged);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openButton,
            this.messagesButton,
            this.settingsButton,
            this.aboutButton,
            this.toolStripSeparator1,
            this.stopButton,
            this.prevMusicButton,
            this.nextMusicButton});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(188, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // openButton
            // 
            this.openButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.openButton.Image = ((System.Drawing.Image)(resources.GetObject("openButton.Image")));
            this.openButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(32, 22);
            this.openButton.Text = "Projekt öffnen";
            this.openButton.ButtonClick += new System.EventHandler(this.openButton_Click);
            this.openButton.DropDownOpening += new System.EventHandler(this.openButton_DropDownOpening);
            // 
            // messagesButton
            // 
            this.messagesButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messagesButton.Image = ((System.Drawing.Image)(resources.GetObject("messagesButton.Image")));
            this.messagesButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messagesButton.Name = "messagesButton";
            this.messagesButton.Size = new System.Drawing.Size(23, 22);
            this.messagesButton.Text = "Meldungen";
            this.messagesButton.Click += new System.EventHandler(this.messagesButton_Click);
            // 
            // settingsButton
            // 
            this.settingsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.settingsButton.Image = ((System.Drawing.Image)(resources.GetObject("settingsButton.Image")));
            this.settingsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.settingsButton.Name = "settingsButton";
            this.settingsButton.Size = new System.Drawing.Size(23, 22);
            this.settingsButton.Text = "Einstellungen";
            this.settingsButton.Click += new System.EventHandler(this.settingsButton_Click);
            // 
            // aboutButton
            // 
            this.aboutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.aboutButton.Image = ((System.Drawing.Image)(resources.GetObject("aboutButton.Image")));
            this.aboutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.aboutButton.Name = "aboutButton";
            this.aboutButton.Size = new System.Drawing.Size(23, 22);
            this.aboutButton.Text = "Über";
            this.aboutButton.Click += new System.EventHandler(this.aboutButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // stopButton
            // 
            this.stopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(23, 22);
            this.stopButton.Text = "Stoppe Alles";
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // prevMusicButton
            // 
            this.prevMusicButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevMusicButton.Image = ((System.Drawing.Image)(resources.GetObject("prevMusicButton.Image")));
            this.prevMusicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevMusicButton.Name = "prevMusicButton";
            this.prevMusicButton.Size = new System.Drawing.Size(23, 22);
            this.prevMusicButton.Text = "Letztes Musikstück";
            this.prevMusicButton.Click += new System.EventHandler(this.prevMusicButton_Click);
            // 
            // nextMusicButton
            // 
            this.nextMusicButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextMusicButton.Image = ((System.Drawing.Image)(resources.GetObject("nextMusicButton.Image")));
            this.nextMusicButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextMusicButton.Name = "nextMusicButton";
            this.nextMusicButton.Size = new System.Drawing.Size(23, 22);
            this.nextMusicButton.Text = "Nächstes Musikstück";
            this.nextMusicButton.Click += new System.EventHandler(this.nextMusicButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "ares";
            this.openFileDialog1.Filter = "Ares Projekte|*.ares";
            // 
            // overLayPanel
            // 
            this.overLayPanel.BackgroundImage = global::Ares.MGPlugin.Properties.Resources.Controller_Overlay;
            this.overLayPanel.Controls.Add(this.transparentFlowLayoutPanel2);
            this.overLayPanel.Controls.Add(this.transparentFlowLayoutPanel1);
            this.overLayPanel.Controls.Add(this.flowLayoutPanel1);
            this.overLayPanel.Controls.Add(this.overlayLabel9);
            this.overLayPanel.Controls.Add(this.overlayOKButton);
            this.overLayPanel.Controls.Add(this.overlayLabel7);
            this.overLayPanel.Controls.Add(this.overlayLabel1);
            this.overLayPanel.Controls.Add(this.overlayCloseButton);
            this.overLayPanel.Location = new System.Drawing.Point(3, 40);
            this.overLayPanel.Name = "overLayPanel";
            this.overLayPanel.Size = new System.Drawing.Size(853, 295);
            this.overLayPanel.TabIndex = 5;
            this.overLayPanel.Visible = false;
            // 
            // overlayOKButton
            // 
            this.overlayOKButton.Location = new System.Drawing.Point(735, 206);
            this.overlayOKButton.Name = "overlayOKButton";
            this.overlayOKButton.Size = new System.Drawing.Size(75, 23);
            this.overlayOKButton.TabIndex = 11;
            this.overlayOKButton.Text = "OK";
            this.overlayOKButton.UseVisualStyleBackColor = true;
            this.overlayOKButton.Click += new System.EventHandler(this.overlayOKButton_Click);
            // 
            // overlayCloseButton
            // 
            this.overlayCloseButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(177)))), ((int)(((byte)(177)))), ((int)(((byte)(177)))));
            this.overlayCloseButton.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.overlayCloseButton.FlatAppearance.BorderSize = 0;
            this.overlayCloseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.overlayCloseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Silver;
            this.overlayCloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.overlayCloseButton.Image = global::Ares.MGPlugin.Properties.Resources.eventlogError;
            this.overlayCloseButton.Location = new System.Drawing.Point(816, 0);
            this.overlayCloseButton.Name = "overlayCloseButton";
            this.overlayCloseButton.Size = new System.Drawing.Size(30, 34);
            this.overlayCloseButton.TabIndex = 0;
            this.overlayCloseButton.UseVisualStyleBackColor = false;
            this.overlayCloseButton.Click += new System.EventHandler(this.overlayCloseButton_Click);
            // 
            // transparentFlowLayoutPanel2
            // 
            this.transparentFlowLayoutPanel2.BackColor = System.Drawing.Color.Transparent;
            this.transparentFlowLayoutPanel2.Controls.Add(this.overlayLabel5);
            this.transparentFlowLayoutPanel2.Controls.Add(this.settingsLabel);
            this.transparentFlowLayoutPanel2.Controls.Add(this.overlayLabel6);
            this.transparentFlowLayoutPanel2.Location = new System.Drawing.Point(65, 155);
            this.transparentFlowLayoutPanel2.Name = "transparentFlowLayoutPanel2";
            this.transparentFlowLayoutPanel2.Size = new System.Drawing.Size(761, 18);
            this.transparentFlowLayoutPanel2.TabIndex = 15;
            // 
            // overlayLabel5
            // 
            this.overlayLabel5.AutoSize = true;
            this.overlayLabel5.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel5.Location = new System.Drawing.Point(3, 0);
            this.overlayLabel5.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.overlayLabel5.Name = "overlayLabel5";
            this.overlayLabel5.Size = new System.Drawing.Size(416, 16);
            this.overlayLabel5.TabIndex = 8;
            this.overlayLabel5.Text = "Falls der Ares Player doch installiert sein sollte, können Sie ihn in den";
            // 
            // settingsLabel
            // 
            this.settingsLabel.AutoSize = true;
            this.settingsLabel.BackColor = System.Drawing.Color.Transparent;
            this.settingsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.settingsLabel.Location = new System.Drawing.Point(419, 0);
            this.settingsLabel.Margin = new System.Windows.Forms.Padding(0);
            this.settingsLabel.Name = "settingsLabel";
            this.settingsLabel.Size = new System.Drawing.Size(88, 16);
            this.settingsLabel.TabIndex = 9;
            this.settingsLabel.TabStop = true;
            this.settingsLabel.Text = "Einstellungen";
            this.settingsLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.settingsLabel_LinkClicked);
            // 
            // overlayLabel6
            // 
            this.overlayLabel6.AutoSize = true;
            this.overlayLabel6.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel6.Location = new System.Drawing.Point(507, 0);
            this.overlayLabel6.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.overlayLabel6.Name = "overlayLabel6";
            this.overlayLabel6.Size = new System.Drawing.Size(207, 16);
            this.overlayLabel6.TabIndex = 10;
            this.overlayLabel6.Text = "des Plugins auswählen. Wenn ein";
            // 
            // transparentFlowLayoutPanel1
            // 
            this.transparentFlowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.transparentFlowLayoutPanel1.Controls.Add(this.overlayLabel3);
            this.transparentFlowLayoutPanel1.Controls.Add(this.setupLabel);
            this.transparentFlowLayoutPanel1.Controls.Add(this.overlayLabel4);
            this.transparentFlowLayoutPanel1.Location = new System.Drawing.Point(65, 131);
            this.transparentFlowLayoutPanel1.Name = "transparentFlowLayoutPanel1";
            this.transparentFlowLayoutPanel1.Size = new System.Drawing.Size(761, 23);
            this.transparentFlowLayoutPanel1.TabIndex = 14;
            // 
            // overlayLabel3
            // 
            this.overlayLabel3.AutoSize = true;
            this.overlayLabel3.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel3.Location = new System.Drawing.Point(3, 0);
            this.overlayLabel3.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.overlayLabel3.Name = "overlayLabel3";
            this.overlayLabel3.Size = new System.Drawing.Size(160, 16);
            this.overlayLabel3.TabIndex = 5;
            this.overlayLabel3.Text = "besuchen oder direkt das";
            // 
            // setupLabel
            // 
            this.setupLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.setupLabel.AutoSize = true;
            this.setupLabel.BackColor = System.Drawing.Color.Transparent;
            this.setupLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.setupLabel.Location = new System.Drawing.Point(163, 0);
            this.setupLabel.Margin = new System.Windows.Forms.Padding(0);
            this.setupLabel.Name = "setupLabel";
            this.setupLabel.Size = new System.Drawing.Size(74, 16);
            this.setupLabel.TabIndex = 6;
            this.setupLabel.TabStop = true;
            this.setupLabel.Text = "Ares Setup";
            this.setupLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.setupLabel_LinkClicked);
            // 
            // overlayLabel4
            // 
            this.overlayLabel4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.overlayLabel4.AutoSize = true;
            this.overlayLabel4.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel4.Location = new System.Drawing.Point(237, 0);
            this.overlayLabel4.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.overlayLabel4.Name = "overlayLabel4";
            this.overlayLabel4.Size = new System.Drawing.Size(93, 16);
            this.overlayLabel4.TabIndex = 7;
            this.overlayLabel4.Text = "herunterladen.";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.flowLayoutPanel1.Controls.Add(this.overlayLabel2);
            this.flowLayoutPanel1.Controls.Add(this.homepageLabel);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(65, 110);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(761, 20);
            this.flowLayoutPanel1.TabIndex = 13;
            // 
            // overlayLabel2
            // 
            this.overlayLabel2.AutoSize = true;
            this.overlayLabel2.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel2.Location = new System.Drawing.Point(3, 0);
            this.overlayLabel2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.overlayLabel2.Name = "overlayLabel2";
            this.overlayLabel2.Size = new System.Drawing.Size(629, 16);
            this.overlayLabel2.TabIndex = 3;
            this.overlayLabel2.Text = "Es wurde kein Ares Player im Netzwerk oder lokal installiert gefunden. Wenn Sie m" +
    "öchten, können Sie die";
            // 
            // homepageLabel
            // 
            this.homepageLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.homepageLabel.AutoSize = true;
            this.homepageLabel.BackColor = System.Drawing.Color.Transparent;
            this.homepageLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.homepageLabel.Location = new System.Drawing.Point(632, 0);
            this.homepageLabel.Margin = new System.Windows.Forms.Padding(0);
            this.homepageLabel.Name = "homepageLabel";
            this.homepageLabel.Size = new System.Drawing.Size(108, 16);
            this.homepageLabel.TabIndex = 4;
            this.homepageLabel.TabStop = true;
            this.homepageLabel.Text = "Ares Homepage";
            this.homepageLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.homepageLabel_LinkClicked);
            // 
            // overlayLabel9
            // 
            this.overlayLabel9.AutoSize = true;
            this.overlayLabel9.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel9.Location = new System.Drawing.Point(64, 24);
            this.overlayLabel9.Name = "overlayLabel9";
            this.overlayLabel9.Size = new System.Drawing.Size(183, 20);
            this.overlayLabel9.TabIndex = 12;
            this.overlayLabel9.Text = "Ares Controller Plugin";
            // 
            // overlayLabel7
            // 
            this.overlayLabel7.AutoSize = true;
            this.overlayLabel7.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel7.Location = new System.Drawing.Point(67, 176);
            this.overlayLabel7.Name = "overlayLabel7";
            this.overlayLabel7.Size = new System.Drawing.Size(651, 16);
            this.overlayLabel7.TabIndex = 10;
            this.overlayLabel7.Text = "Player im Netzwerk gestartet wurde, müssen Sie eventuell die Firewall-Einstellung" +
    "en der Rechner überprüfen.";
            // 
            // overlayLabel1
            // 
            this.overlayLabel1.BackColor = System.Drawing.Color.Transparent;
            this.overlayLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overlayLabel1.Location = new System.Drawing.Point(64, 61);
            this.overlayLabel1.Name = "overlayLabel1";
            this.overlayLabel1.Size = new System.Drawing.Size(746, 46);
            this.overlayLabel1.TabIndex = 1;
            this.overlayLabel1.Text = "Ares (Aural RPG Experience System) ist ein Programm, mit dem man während des Roll" +
    "enspiels passende Musik und Geräusche abspielen kann. Mit diesem Plugin lässt si" +
    "ch der Ares Player steuern.";
            // 
            // Controller
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.overLayPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Controller";
            this.Size = new System.Drawing.Size(859, 351);
            this.Load += new System.EventHandler(this.Controller_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.soundsBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.overLayPanel.ResumeLayout(false);
            this.overLayPanel.PerformLayout();
            this.transparentFlowLayoutPanel2.ResumeLayout(false);
            this.transparentFlowLayoutPanel2.PerformLayout();
            this.transparentFlowLayoutPanel1.ResumeLayout(false);
            this.transparentFlowLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TrackBar soundsBar;
        private System.Windows.Forms.TrackBar musicBar;
        private System.Windows.Forms.TrackBar overallVolumeBar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label musicLabel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label elementsLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label modeLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label projectLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ComboBox serverBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.ListBox modesList;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton messagesButton;
        private System.Windows.Forms.ToolStripButton settingsButton;
        private System.Windows.Forms.ToolStripButton aboutButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton stopButton;
        private System.Windows.Forms.ToolStripButton prevMusicButton;
        private System.Windows.Forms.ToolStripButton nextMusicButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripSplitButton openButton;
        private System.Windows.Forms.FlowLayoutPanel elementsPanel;
        private System.Windows.Forms.ListBox musicList;
        private System.Windows.Forms.Panel overLayPanel;
        private System.Windows.Forms.Button overlayOKButton;
        private TransparentLabel overlayLabel7;
        private TransparentLabel overlayLabel1;
        private System.Windows.Forms.Button overlayCloseButton;
        private TransparentLabel overlayLabel9;
        private System.Windows.Forms.ToolTip toolTip1;
        private TransparentFlowLayoutPanel flowLayoutPanel1;
        private TransparentLabel overlayLabel2;
        private TransparentLinkLabel homepageLabel;
        private TransparentFlowLayoutPanel transparentFlowLayoutPanel2;
        private TransparentLabel overlayLabel5;
        private TransparentLinkLabel settingsLabel;
        private TransparentLabel overlayLabel6;
        private TransparentFlowLayoutPanel transparentFlowLayoutPanel1;
        private TransparentLabel overlayLabel3;
        private TransparentLinkLabel setupLabel;
        private TransparentLabel overlayLabel4;

    }
}
