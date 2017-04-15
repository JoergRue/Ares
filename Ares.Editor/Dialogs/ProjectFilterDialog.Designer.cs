namespace Ares.Editor.Dialogs
{
    partial class ProjectFilterDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectFilterDialog));
            this.autoUpdateBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.noFilterButton = new System.Windows.Forms.RadioButton();
            this.onlyUsedFilesButton = new System.Windows.Forms.RadioButton();
            this.showNotUsedButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // autoUpdateBox
            // 
            resources.ApplyResources(this.autoUpdateBox, "autoUpdateBox");
            this.autoUpdateBox.Name = "autoUpdateBox";
            this.autoUpdateBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // noFilterButton
            // 
            resources.ApplyResources(this.noFilterButton, "noFilterButton");
            this.noFilterButton.Name = "noFilterButton";
            this.noFilterButton.TabStop = true;
            this.noFilterButton.UseVisualStyleBackColor = true;
            // 
            // onlyUsedFilesButton
            // 
            resources.ApplyResources(this.onlyUsedFilesButton, "onlyUsedFilesButton");
            this.onlyUsedFilesButton.Name = "onlyUsedFilesButton";
            this.onlyUsedFilesButton.TabStop = true;
            this.onlyUsedFilesButton.UseVisualStyleBackColor = true;
            // 
            // showNotUsedButton
            // 
            resources.ApplyResources(this.showNotUsedButton, "showNotUsedButton");
            this.showNotUsedButton.Name = "showNotUsedButton";
            this.showNotUsedButton.TabStop = true;
            this.showNotUsedButton.UseVisualStyleBackColor = true;
            // 
            // ProjectFilterDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.showNotUsedButton);
            this.Controls.Add(this.onlyUsedFilesButton);
            this.Controls.Add(this.noFilterButton);
            this.Controls.Add(this.autoUpdateBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Name = "ProjectFilterDialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autoUpdateBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.RadioButton noFilterButton;
        private System.Windows.Forms.RadioButton onlyUsedFilesButton;
        private System.Windows.Forms.RadioButton showNotUsedButton;
    }
}