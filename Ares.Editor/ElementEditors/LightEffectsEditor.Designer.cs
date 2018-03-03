namespace Ares.Editor.ElementEditors
{
    partial class LightEffectsEditor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.playButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonCenterLRMix = new System.Windows.Forms.Button();
            this.trackBarLRMix = new System.Windows.Forms.TrackBar();
            this.checkBoxChangeLRMix = new System.Windows.Forms.CheckBox();
            this.trackBarMaster = new System.Windows.Forms.TrackBar();
            this.checkBoxChangeMaster = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownLeftScene = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownRightScene = new System.Windows.Forms.NumericUpDown();
            this.checkBoxChangeRightScene = new System.Windows.Forms.CheckBox();
            this.checkBoxChangeLeftScene = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLRMix)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaster)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLeftScene)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRightScene)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.playButton);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(45, 57);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test";
            // 
            // playButton
            // 
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            this.playButton.Location = new System.Drawing.Point(6, 19);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(31, 23);
            this.playButton.TabIndex = 0;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonCenterLRMix);
            this.groupBox2.Controls.Add(this.trackBarLRMix);
            this.groupBox2.Controls.Add(this.checkBoxChangeLRMix);
            this.groupBox2.Controls.Add(this.trackBarMaster);
            this.groupBox2.Controls.Add(this.checkBoxChangeMaster);
            this.groupBox2.Location = new System.Drawing.Point(64, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(553, 103);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Slider";
            // 
            // buttonCenterLRMix
            // 
            this.buttonCenterLRMix.Location = new System.Drawing.Point(492, 48);
            this.buttonCenterLRMix.Name = "buttonCenterLRMix";
            this.buttonCenterLRMix.Size = new System.Drawing.Size(55, 23);
            this.buttonCenterLRMix.TabIndex = 4;
            this.buttonCenterLRMix.Text = "> | <";
            this.buttonCenterLRMix.UseVisualStyleBackColor = true;
            this.buttonCenterLRMix.Click += new System.EventHandler(this.buttonCenterLRMix_Click);
            // 
            // trackBarLRMix
            // 
            this.trackBarLRMix.Location = new System.Drawing.Point(122, 48);
            this.trackBarLRMix.Maximum = 255;
            this.trackBarLRMix.Name = "trackBarLRMix";
            this.trackBarLRMix.Size = new System.Drawing.Size(364, 45);
            this.trackBarLRMix.TabIndex = 3;
            this.trackBarLRMix.TickFrequency = 127;
            this.trackBarLRMix.ValueChanged += new System.EventHandler(this.trackBarLRMix_ValueChanged);
            // 
            // checkBoxChangeLRMix
            // 
            this.checkBoxChangeLRMix.AutoSize = true;
            this.checkBoxChangeLRMix.Location = new System.Drawing.Point(11, 48);
            this.checkBoxChangeLRMix.Name = "checkBoxChangeLRMix";
            this.checkBoxChangeLRMix.Size = new System.Drawing.Size(104, 17);
            this.checkBoxChangeLRMix.TabIndex = 2;
            this.checkBoxChangeLRMix.Text = "Change L/R Mix";
            this.checkBoxChangeLRMix.UseVisualStyleBackColor = true;
            this.checkBoxChangeLRMix.CheckedChanged += new System.EventHandler(this.checkBoxChangeLRMix_CheckedChanged);
            // 
            // trackBarMaster
            // 
            this.trackBarMaster.Location = new System.Drawing.Point(121, 19);
            this.trackBarMaster.Maximum = 255;
            this.trackBarMaster.Name = "trackBarMaster";
            this.trackBarMaster.Size = new System.Drawing.Size(365, 45);
            this.trackBarMaster.TabIndex = 1;
            this.trackBarMaster.TickFrequency = 16;
            this.trackBarMaster.ValueChanged += new System.EventHandler(this.trackBarMaster_ValueChanged);
            // 
            // checkBoxChangeMaster
            // 
            this.checkBoxChangeMaster.AutoSize = true;
            this.checkBoxChangeMaster.Location = new System.Drawing.Point(11, 24);
            this.checkBoxChangeMaster.Name = "checkBoxChangeMaster";
            this.checkBoxChangeMaster.Size = new System.Drawing.Size(98, 17);
            this.checkBoxChangeMaster.TabIndex = 0;
            this.checkBoxChangeMaster.Text = "Change Master";
            this.checkBoxChangeMaster.UseVisualStyleBackColor = true;
            this.checkBoxChangeMaster.CheckedChanged += new System.EventHandler(this.checkBoxChangeMaster_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.numericUpDownLeftScene);
            this.groupBox3.Controls.Add(this.numericUpDownRightScene);
            this.groupBox3.Controls.Add(this.checkBoxChangeRightScene);
            this.groupBox3.Controls.Add(this.checkBoxChangeLeftScene);
            this.groupBox3.Location = new System.Drawing.Point(13, 122);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(604, 76);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Scenes";
            // 
            // numericUpDownLeftScene
            // 
            this.numericUpDownLeftScene.Location = new System.Drawing.Point(138, 20);
            this.numericUpDownLeftScene.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownLeftScene.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLeftScene.Name = "numericUpDownLeftScene";
            this.numericUpDownLeftScene.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownLeftScene.TabIndex = 3;
            this.numericUpDownLeftScene.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLeftScene.ValueChanged += new System.EventHandler(this.numericUpDownLeftScene_ValueChanged);
            // 
            // numericUpDownRightScene
            // 
            this.numericUpDownRightScene.Location = new System.Drawing.Point(138, 41);
            this.numericUpDownRightScene.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownRightScene.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRightScene.Name = "numericUpDownRightScene";
            this.numericUpDownRightScene.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownRightScene.TabIndex = 2;
            this.numericUpDownRightScene.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownRightScene.ValueChanged += new System.EventHandler(this.numericUpDownRightScene_ValueChanged);
            // 
            // checkBoxChangeRightScene
            // 
            this.checkBoxChangeRightScene.AutoSize = true;
            this.checkBoxChangeRightScene.Location = new System.Drawing.Point(7, 44);
            this.checkBoxChangeRightScene.Name = "checkBoxChangeRightScene";
            this.checkBoxChangeRightScene.Size = new System.Drawing.Size(125, 17);
            this.checkBoxChangeRightScene.TabIndex = 1;
            this.checkBoxChangeRightScene.Text = "Change Right Scene";
            this.checkBoxChangeRightScene.UseVisualStyleBackColor = true;
            this.checkBoxChangeRightScene.CheckedChanged += new System.EventHandler(this.checkBoxChangeRightScene_CheckedChanged);
            // 
            // checkBoxChangeLeftScene
            // 
            this.checkBoxChangeLeftScene.AutoSize = true;
            this.checkBoxChangeLeftScene.Location = new System.Drawing.Point(7, 20);
            this.checkBoxChangeLeftScene.Name = "checkBoxChangeLeftScene";
            this.checkBoxChangeLeftScene.Size = new System.Drawing.Size(118, 17);
            this.checkBoxChangeLeftScene.TabIndex = 0;
            this.checkBoxChangeLeftScene.Text = "Change Left Scene";
            this.checkBoxChangeLeftScene.UseVisualStyleBackColor = true;
            this.checkBoxChangeLeftScene.CheckedChanged += new System.EventHandler(this.checkBoxChangeLeftScene_CheckedChanged);
            // 
            // LightEffectsEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(629, 423);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "LightEffectsEditor";
            this.Text = "LightEffectsEditor";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarLRMix)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaster)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLeftScene)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownRightScene)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox checkBoxChangeMaster;
        private System.Windows.Forms.Button buttonCenterLRMix;
        private System.Windows.Forms.TrackBar trackBarLRMix;
        private System.Windows.Forms.CheckBox checkBoxChangeLRMix;
        private System.Windows.Forms.TrackBar trackBarMaster;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.NumericUpDown numericUpDownLeftScene;
        private System.Windows.Forms.NumericUpDown numericUpDownRightScene;
        private System.Windows.Forms.CheckBox checkBoxChangeRightScene;
        private System.Windows.Forms.CheckBox checkBoxChangeLeftScene;
    }
}