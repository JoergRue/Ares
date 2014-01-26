namespace Ares.Player
{
    partial class MusicPage
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MusicPage));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.crossFadeButton = new System.Windows.Forms.RadioButton();
            this.fadeButton = new System.Windows.Forms.RadioButton();
            this.noFadeButton = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.crossFadingUpDown = new System.Windows.Forms.NumericUpDown();
            this.allChannelsCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.crossFadeButton);
            this.groupBox1.Controls.Add(this.fadeButton);
            this.groupBox1.Controls.Add(this.noFadeButton);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.crossFadingUpDown);
            this.groupBox1.Controls.Add(this.allChannelsCheckBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // crossFadeButton
            // 
            resources.ApplyResources(this.crossFadeButton, "crossFadeButton");
            this.crossFadeButton.Name = "crossFadeButton";
            this.crossFadeButton.TabStop = true;
            this.crossFadeButton.UseVisualStyleBackColor = true;
            this.crossFadeButton.CheckedChanged += new System.EventHandler(this.crossFadeButton_CheckedChanged);
            // 
            // fadeButton
            // 
            resources.ApplyResources(this.fadeButton, "fadeButton");
            this.fadeButton.Name = "fadeButton";
            this.fadeButton.TabStop = true;
            this.fadeButton.UseVisualStyleBackColor = true;
            this.fadeButton.CheckedChanged += new System.EventHandler(this.fadeButton_CheckedChanged);
            // 
            // noFadeButton
            // 
            resources.ApplyResources(this.noFadeButton, "noFadeButton");
            this.noFadeButton.Name = "noFadeButton";
            this.noFadeButton.TabStop = true;
            this.noFadeButton.UseVisualStyleBackColor = true;
            this.noFadeButton.CheckedChanged += new System.EventHandler(this.noFadeButton_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // crossFadingUpDown
            // 
            resources.ApplyResources(this.crossFadingUpDown, "crossFadingUpDown");
            this.crossFadingUpDown.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.crossFadingUpDown.Maximum = new decimal(new int[] {
            30000,
            0,
            0,
            0});
            this.crossFadingUpDown.Name = "crossFadingUpDown";
            // 
            // allChannelsCheckBox
            // 
            resources.ApplyResources(this.allChannelsCheckBox, "allChannelsCheckBox");
            this.allChannelsCheckBox.Name = "allChannelsCheckBox";
            this.allChannelsCheckBox.UseVisualStyleBackColor = true;
            // 
            // MusicPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "MusicPage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crossFadingUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox allChannelsCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton crossFadeButton;
        private System.Windows.Forms.RadioButton fadeButton;
        private System.Windows.Forms.RadioButton noFadeButton;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown crossFadingUpDown;
    }
}
