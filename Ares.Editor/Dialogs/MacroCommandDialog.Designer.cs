namespace Ares.Editor.Dialogs
{
    partial class MacroCommandDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MacroCommandDialog));
            this.commandGroupBox = new System.Windows.Forms.GroupBox();
            this.waitUnitCombo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.waitTimeUpDown = new System.Windows.Forms.NumericUpDown();
            this.selectCommandElementButton = new System.Windows.Forms.Button();
            this.commandElementBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.commandTypeCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.conditionGroupBox = new System.Windows.Forms.GroupBox();
            this.selectConditionElementButton = new System.Windows.Forms.Button();
            this.conditionElementBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.conditionCombo = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.commandGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitTimeUpDown)).BeginInit();
            this.conditionGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // commandGroupBox
            // 
            resources.ApplyResources(this.commandGroupBox, "commandGroupBox");
            this.commandGroupBox.Controls.Add(this.waitUnitCombo);
            this.commandGroupBox.Controls.Add(this.label3);
            this.commandGroupBox.Controls.Add(this.waitTimeUpDown);
            this.commandGroupBox.Controls.Add(this.selectCommandElementButton);
            this.commandGroupBox.Controls.Add(this.commandElementBox);
            this.commandGroupBox.Controls.Add(this.label2);
            this.commandGroupBox.Controls.Add(this.commandTypeCombo);
            this.commandGroupBox.Controls.Add(this.label1);
            this.errorProvider.SetError(this.commandGroupBox, resources.GetString("commandGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.commandGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("commandGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.commandGroupBox, ((int)(resources.GetObject("commandGroupBox.IconPadding"))));
            this.commandGroupBox.Name = "commandGroupBox";
            this.commandGroupBox.TabStop = false;
            // 
            // waitUnitCombo
            // 
            resources.ApplyResources(this.waitUnitCombo, "waitUnitCombo");
            this.waitUnitCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.waitUnitCombo, resources.GetString("waitUnitCombo.Error"));
            this.waitUnitCombo.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.waitUnitCombo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("waitUnitCombo.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.waitUnitCombo, ((int)(resources.GetObject("waitUnitCombo.IconPadding"))));
            this.waitUnitCombo.Items.AddRange(new object[] {
            resources.GetString("waitUnitCombo.Items"),
            resources.GetString("waitUnitCombo.Items1"),
            resources.GetString("waitUnitCombo.Items2")});
            this.waitUnitCombo.Name = "waitUnitCombo";
            this.waitUnitCombo.SelectedIndexChanged += new System.EventHandler(this.waitUnitCombo_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.errorProvider.SetError(this.label3, resources.GetString("label3.Error"));
            this.errorProvider.SetIconAlignment(this.label3, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label3.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label3, ((int)(resources.GetObject("label3.IconPadding"))));
            this.label3.Name = "label3";
            // 
            // waitTimeUpDown
            // 
            resources.ApplyResources(this.waitTimeUpDown, "waitTimeUpDown");
            this.errorProvider.SetError(this.waitTimeUpDown, resources.GetString("waitTimeUpDown.Error"));
            this.errorProvider.SetIconAlignment(this.waitTimeUpDown, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("waitTimeUpDown.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.waitTimeUpDown, ((int)(resources.GetObject("waitTimeUpDown.IconPadding"))));
            this.waitTimeUpDown.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.waitTimeUpDown.Name = "waitTimeUpDown";
            // 
            // selectCommandElementButton
            // 
            resources.ApplyResources(this.selectCommandElementButton, "selectCommandElementButton");
            this.errorProvider.SetError(this.selectCommandElementButton, resources.GetString("selectCommandElementButton.Error"));
            this.errorProvider.SetIconAlignment(this.selectCommandElementButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("selectCommandElementButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.selectCommandElementButton, ((int)(resources.GetObject("selectCommandElementButton.IconPadding"))));
            this.selectCommandElementButton.Name = "selectCommandElementButton";
            this.selectCommandElementButton.UseVisualStyleBackColor = true;
            this.selectCommandElementButton.Click += new System.EventHandler(this.selectCommandElementButton_Click);
            // 
            // commandElementBox
            // 
            resources.ApplyResources(this.commandElementBox, "commandElementBox");
            this.errorProvider.SetError(this.commandElementBox, resources.GetString("commandElementBox.Error"));
            this.errorProvider.SetIconAlignment(this.commandElementBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("commandElementBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.commandElementBox, ((int)(resources.GetObject("commandElementBox.IconPadding"))));
            this.commandElementBox.Name = "commandElementBox";
            this.commandElementBox.ReadOnly = true;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.errorProvider.SetError(this.label2, resources.GetString("label2.Error"));
            this.errorProvider.SetIconAlignment(this.label2, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label2.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label2, ((int)(resources.GetObject("label2.IconPadding"))));
            this.label2.Name = "label2";
            // 
            // commandTypeCombo
            // 
            resources.ApplyResources(this.commandTypeCombo, "commandTypeCombo");
            this.commandTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.commandTypeCombo, resources.GetString("commandTypeCombo.Error"));
            this.commandTypeCombo.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.commandTypeCombo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("commandTypeCombo.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.commandTypeCombo, ((int)(resources.GetObject("commandTypeCombo.IconPadding"))));
            this.commandTypeCombo.Items.AddRange(new object[] {
            resources.GetString("commandTypeCombo.Items"),
            resources.GetString("commandTypeCombo.Items1"),
            resources.GetString("commandTypeCombo.Items2"),
            resources.GetString("commandTypeCombo.Items3"),
            resources.GetString("commandTypeCombo.Items4"),
            resources.GetString("commandTypeCombo.Items5"),
            resources.GetString("commandTypeCombo.Items6"),
            resources.GetString("commandTypeCombo.Items7")});
            this.commandTypeCombo.Name = "commandTypeCombo";
            this.commandTypeCombo.SelectedIndexChanged += new System.EventHandler(this.commandTypeCombo_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.errorProvider.SetError(this.label1, resources.GetString("label1.Error"));
            this.errorProvider.SetIconAlignment(this.label1, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label1.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label1, ((int)(resources.GetObject("label1.IconPadding"))));
            this.label1.Name = "label1";
            // 
            // conditionGroupBox
            // 
            resources.ApplyResources(this.conditionGroupBox, "conditionGroupBox");
            this.conditionGroupBox.Controls.Add(this.selectConditionElementButton);
            this.conditionGroupBox.Controls.Add(this.conditionElementBox);
            this.conditionGroupBox.Controls.Add(this.label5);
            this.conditionGroupBox.Controls.Add(this.label4);
            this.conditionGroupBox.Controls.Add(this.conditionCombo);
            this.errorProvider.SetError(this.conditionGroupBox, resources.GetString("conditionGroupBox.Error"));
            this.errorProvider.SetIconAlignment(this.conditionGroupBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("conditionGroupBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.conditionGroupBox, ((int)(resources.GetObject("conditionGroupBox.IconPadding"))));
            this.conditionGroupBox.Name = "conditionGroupBox";
            this.conditionGroupBox.TabStop = false;
            // 
            // selectConditionElementButton
            // 
            resources.ApplyResources(this.selectConditionElementButton, "selectConditionElementButton");
            this.errorProvider.SetError(this.selectConditionElementButton, resources.GetString("selectConditionElementButton.Error"));
            this.errorProvider.SetIconAlignment(this.selectConditionElementButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("selectConditionElementButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.selectConditionElementButton, ((int)(resources.GetObject("selectConditionElementButton.IconPadding"))));
            this.selectConditionElementButton.Name = "selectConditionElementButton";
            this.selectConditionElementButton.UseVisualStyleBackColor = true;
            this.selectConditionElementButton.Click += new System.EventHandler(this.selectConditionElementButton_Click);
            // 
            // conditionElementBox
            // 
            resources.ApplyResources(this.conditionElementBox, "conditionElementBox");
            this.errorProvider.SetError(this.conditionElementBox, resources.GetString("conditionElementBox.Error"));
            this.errorProvider.SetIconAlignment(this.conditionElementBox, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("conditionElementBox.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.conditionElementBox, ((int)(resources.GetObject("conditionElementBox.IconPadding"))));
            this.conditionElementBox.Name = "conditionElementBox";
            this.conditionElementBox.ReadOnly = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.errorProvider.SetError(this.label5, resources.GetString("label5.Error"));
            this.errorProvider.SetIconAlignment(this.label5, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label5.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label5, ((int)(resources.GetObject("label5.IconPadding"))));
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.errorProvider.SetError(this.label4, resources.GetString("label4.Error"));
            this.errorProvider.SetIconAlignment(this.label4, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("label4.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.label4, ((int)(resources.GetObject("label4.IconPadding"))));
            this.label4.Name = "label4";
            // 
            // conditionCombo
            // 
            resources.ApplyResources(this.conditionCombo, "conditionCombo");
            this.conditionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.errorProvider.SetError(this.conditionCombo, resources.GetString("conditionCombo.Error"));
            this.conditionCombo.FormattingEnabled = true;
            this.errorProvider.SetIconAlignment(this.conditionCombo, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("conditionCombo.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.conditionCombo, ((int)(resources.GetObject("conditionCombo.IconPadding"))));
            this.conditionCombo.Items.AddRange(new object[] {
            resources.GetString("conditionCombo.Items"),
            resources.GetString("conditionCombo.Items1"),
            resources.GetString("conditionCombo.Items2")});
            this.conditionCombo.Name = "conditionCombo";
            this.conditionCombo.SelectedIndexChanged += new System.EventHandler(this.conditionCombo_SelectedIndexChanged);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.errorProvider.SetError(this.cancelButton, resources.GetString("cancelButton.Error"));
            this.errorProvider.SetIconAlignment(this.cancelButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("cancelButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.cancelButton, ((int)(resources.GetObject("cancelButton.IconPadding"))));
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.errorProvider.SetError(this.okButton, resources.GetString("okButton.Error"));
            this.errorProvider.SetIconAlignment(this.okButton, ((System.Windows.Forms.ErrorIconAlignment)(resources.GetObject("okButton.IconAlignment"))));
            this.errorProvider.SetIconPadding(this.okButton, ((int)(resources.GetObject("okButton.IconPadding"))));
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            resources.ApplyResources(this.errorProvider, "errorProvider");
            // 
            // MacroCommandDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.conditionGroupBox);
            this.Controls.Add(this.commandGroupBox);
            this.Name = "MacroCommandDialog";
            this.commandGroupBox.ResumeLayout(false);
            this.commandGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.waitTimeUpDown)).EndInit();
            this.conditionGroupBox.ResumeLayout(false);
            this.conditionGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox commandGroupBox;
        private System.Windows.Forms.ComboBox waitUnitCombo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown waitTimeUpDown;
        private System.Windows.Forms.Button selectCommandElementButton;
        private System.Windows.Forms.TextBox commandElementBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox commandTypeCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox conditionGroupBox;
        private System.Windows.Forms.Button selectConditionElementButton;
        private System.Windows.Forms.TextBox conditionElementBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox conditionCombo;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}