namespace Ares.Editor.Controls
{
    partial class BalanceDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BalanceDialog));
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.maxRandomUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.minRandomUpDown);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.randomButton);
            this.groupBox1.Controls.Add(this.fixedBar);
            this.groupBox1.Controls.Add(this.fixedButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // maxRandomUpDown
            // 
            resources.ApplyResources(this.maxRandomUpDown, "maxRandomUpDown");
            this.maxRandomUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.maxRandomUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.maxRandomUpDown.Name = "maxRandomUpDown";
            this.maxRandomUpDown.ValueChanged += new System.EventHandler(this.maxRandomUpDown_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // minRandomUpDown
            // 
            resources.ApplyResources(this.minRandomUpDown, "minRandomUpDown");
            this.minRandomUpDown.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.minRandomUpDown.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            -2147483648});
            this.minRandomUpDown.Name = "minRandomUpDown";
            this.minRandomUpDown.ValueChanged += new System.EventHandler(this.minRandomUpDown_ValueChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            // 
            // randomButton
            // 
            resources.ApplyResources(this.randomButton, "randomButton");
            this.randomButton.Name = "randomButton";
            this.randomButton.TabStop = true;
            this.randomButton.UseCompatibleTextRendering = true;
            this.randomButton.UseVisualStyleBackColor = true;
            this.randomButton.CheckedChanged += new System.EventHandler(this.randomButton_CheckedChanged);
            // 
            // fixedBar
            // 
            resources.ApplyResources(this.fixedBar, "fixedBar");
            this.fixedBar.Maximum = 100;
            this.fixedBar.Minimum = -100;
            this.fixedBar.Name = "fixedBar";
            this.fixedBar.SmallChange = 5;
            this.fixedBar.TickFrequency = 5;
            // 
            // fixedButton
            // 
            resources.ApplyResources(this.fixedButton, "fixedButton");
            this.fixedButton.Name = "fixedButton";
            this.fixedButton.TabStop = true;
            this.fixedButton.UseCompatibleTextRendering = true;
            this.fixedButton.UseVisualStyleBackColor = true;
            this.fixedButton.CheckedChanged += new System.EventHandler(this.fixedButton_CheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.playButton);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Image = global::Ares.Editor.Controls.Properties.Resources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Image = global::Ares.Editor.Controls.Properties.Resources.RunSmall;
            this.playButton.Name = "playButton";
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseCompatibleTextRendering = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseCompatibleTextRendering = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // PitchDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PitchDialog";
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
        private System.Windows.Forms.Label label3;

    }
}