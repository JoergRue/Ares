namespace Ares.Editor.ElementEditors
{
    partial class DelayableControl
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
                if (m_Element != null)
                    Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DelayableControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lastLabel = new System.Windows.Forms.Label();
            this.maxDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fixedDelayUpDown = new System.Windows.Forms.NumericUpDown();
            this.fixedLabel = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxDelayUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fixedDelayUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lastLabel);
            this.groupBox1.Controls.Add(this.maxDelayUpDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.fixedDelayUpDown);
            this.groupBox1.Controls.Add(this.fixedLabel);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // lastLabel
            // 
            resources.ApplyResources(this.lastLabel, "lastLabel");
            this.lastLabel.Name = "lastLabel";
            this.lastLabel.UseCompatibleTextRendering = true;
            // 
            // maxDelayUpDown
            // 
            this.maxDelayUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.maxDelayUpDown, "maxDelayUpDown");
            this.maxDelayUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.maxDelayUpDown.Name = "maxDelayUpDown";
            this.maxDelayUpDown.ValueChanged += new System.EventHandler(this.maxDelayUpDown_ValueChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // fixedDelayUpDown
            // 
            this.fixedDelayUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            resources.ApplyResources(this.fixedDelayUpDown, "fixedDelayUpDown");
            this.fixedDelayUpDown.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.fixedDelayUpDown.Name = "fixedDelayUpDown";
            this.fixedDelayUpDown.ValueChanged += new System.EventHandler(this.fixedDelayUpDown_ValueChanged);
            // 
            // fixedLabel
            // 
            resources.ApplyResources(this.fixedLabel, "fixedLabel");
            this.fixedLabel.Name = "fixedLabel";
            this.fixedLabel.UseCompatibleTextRendering = true;
            // 
            // DelayableControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.groupBox1);
            this.Name = "DelayableControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.maxDelayUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fixedDelayUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lastLabel;
        private System.Windows.Forms.NumericUpDown maxDelayUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown fixedDelayUpDown;
        private System.Windows.Forms.Label fixedLabel;
    }
}
