namespace Ares.Editor.Dialogs
{
    partial class ToolsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ToolsDialog));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.selectFileEditorButton = new System.Windows.Forms.Button();
            this.audioFileEditorBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.selectFileEditorButton);
            this.groupBox1.Controls.Add(this.audioFileEditorBox);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // selectFileEditorButton
            // 
            resources.ApplyResources(this.selectFileEditorButton, "selectFileEditorButton");
            this.selectFileEditorButton.Name = "selectFileEditorButton";
            this.selectFileEditorButton.UseCompatibleTextRendering = true;
            this.selectFileEditorButton.UseVisualStyleBackColor = true;
            this.selectFileEditorButton.Click += new System.EventHandler(this.selectFileEditorButton_Click);
            // 
            // audioFileEditorBox
            // 
            resources.ApplyResources(this.audioFileEditorBox, "audioFileEditorBox");
            this.audioFileEditorBox.Name = "audioFileEditorBox";
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
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            // 
            // ToolsDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolsDialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button selectFileEditorButton;
        private System.Windows.Forms.TextBox audioFileEditorBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}