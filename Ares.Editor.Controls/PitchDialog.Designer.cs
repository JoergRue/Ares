namespace Ares.Editor.Controls
{
    partial class PitchDialog
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
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PitchDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.maxRandomUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.minRandomUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.randomButton = new System.Windows.Forms.RadioButton();
            this.fixedBar = new System.Windows.Forms.TrackBar();
            this.fixedButton = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRandomUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRandomUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fixedBar)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.maxRandomUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minRandomUpDown);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.randomButton);
            this.groupBox1.Controls.Add(this.fixedBar);
            this.groupBox1.Controls.Add(this.fixedButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 162);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(208, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 17);
            this.label3.TabIndex = 15;
            this.label3.Text = "semitones";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // maxRandomUpDown
            // 
            this.maxRandomUpDown.Location = new System.Drawing.Point(156, 123);
            this.maxRandomUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.maxRandomUpDown.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            -2147483648});
            this.maxRandomUpDown.Name = "maxRandomUpDown";
            this.maxRandomUpDown.Size = new System.Drawing.Size(46, 20);
            this.maxRandomUpDown.TabIndex = 14;
            this.maxRandomUpDown.ValueChanged += new System.EventHandler(this.maxRandomUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(127, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 17);
            this.label2.TabIndex = 13;
            this.label2.Text = "and";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // minRandomUpDown
            // 
            this.minRandomUpDown.Location = new System.Drawing.Point(75, 122);
            this.minRandomUpDown.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.minRandomUpDown.Minimum = new decimal(new int[] {
            60,
            0,
            0,
            -2147483648});
            this.minRandomUpDown.Name = "minRandomUpDown";
            this.minRandomUpDown.Size = new System.Drawing.Size(46, 20);
            this.minRandomUpDown.TabIndex = 12;
            this.minRandomUpDown.ValueChanged += new System.EventHandler(this.minRandomUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "Between";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // randomButton
            // 
            this.randomButton.AutoSize = true;
            this.randomButton.Location = new System.Drawing.Point(6, 94);
            this.randomButton.Name = "randomButton";
            this.randomButton.Size = new System.Drawing.Size(65, 18);
            this.randomButton.TabIndex = 10;
            this.randomButton.TabStop = true;
            this.randomButton.Text = "Random";
            this.randomButton.UseCompatibleTextRendering = true;
            this.randomButton.UseVisualStyleBackColor = true;
            this.randomButton.CheckedChanged += new System.EventHandler(this.randomButton_CheckedChanged);
            // 
            // fixedBar
            // 
            this.fixedBar.Location = new System.Drawing.Point(21, 43);
            this.fixedBar.Maximum = 200;
            this.fixedBar.Minimum = -200;
            this.fixedBar.Name = "fixedBar";
            this.fixedBar.Size = new System.Drawing.Size(230, 45);
            this.fixedBar.SmallChange = 10;
            this.fixedBar.TabIndex = 9;
            this.fixedBar.TickFrequency = 10;
            // 
            // fixedButton
            // 
            this.fixedButton.AutoSize = true;
            this.fixedButton.Location = new System.Drawing.Point(6, 19);
            this.fixedButton.Name = "fixedButton";
            this.fixedButton.Size = new System.Drawing.Size(50, 18);
            this.fixedButton.TabIndex = 8;
            this.fixedButton.TabStop = true;
            this.fixedButton.Text = "Fixed";
            this.fixedButton.UseCompatibleTextRendering = true;
            this.fixedButton.UseVisualStyleBackColor = true;
            this.fixedButton.CheckedChanged += new System.EventHandler(this.fixedButton_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.playButton);
            this.groupBox2.Location = new System.Drawing.Point(12, 180);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(269, 59);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Test";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = global::Ares.Editor.Controls.Properties.Resources.StopSmall;
            this.stopButton.Location = new System.Drawing.Point(50, 19);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(38, 23);
            this.stopButton.TabIndex = 1;
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Image = global::Ares.Editor.Controls.Properties.Resources.RunSmall;
            this.playButton.Location = new System.Drawing.Point(6, 19);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(38, 23);
            this.playButton.TabIndex = 0;
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(206, 245);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(125, 245);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // PitchDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(293, 282);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PitchDialog";
            this.Text = "Pitch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PitchDialog_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxRandomUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minRandomUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fixedBar)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown maxRandomUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown minRandomUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton randomButton;
        private System.Windows.Forms.TrackBar fixedBar;
        private System.Windows.Forms.RadioButton fixedButton;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button playButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;

    }
}