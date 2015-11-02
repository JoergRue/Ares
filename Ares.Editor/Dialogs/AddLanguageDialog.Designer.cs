/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
namespace Ares.Editor.Dialogs
{
    partial class AddLanguageDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddLanguageDialog));
            this.okbutton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.translationGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.langCodeBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.nameBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.nameLabel = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.translationGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // okbutton
            // 
            resources.ApplyResources(this.okbutton, "okbutton");
            this.okbutton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okbutton.Name = "okbutton";
            this.okbutton.UseVisualStyleBackColor = true;
            this.okbutton.Click += new System.EventHandler(this.okbutton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // translationGroupBox
            // 
            resources.ApplyResources(this.translationGroupBox, "translationGroupBox");
            this.translationGroupBox.Controls.Add(this.label1);
            this.translationGroupBox.Controls.Add(this.langCodeBox);
            this.translationGroupBox.Controls.Add(this.label4);
            this.translationGroupBox.Controls.Add(this.label3);
            this.translationGroupBox.Controls.Add(this.nameBox);
            this.translationGroupBox.Controls.Add(this.nameBox2);
            this.translationGroupBox.Controls.Add(this.label2);
            this.translationGroupBox.Controls.Add(this.nameLabel);
            this.translationGroupBox.Name = "translationGroupBox";
            this.translationGroupBox.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // langCodeBox
            // 
            resources.ApplyResources(this.langCodeBox, "langCodeBox");
            this.errorProvider1.SetIconPadding(this.langCodeBox, ((int)(resources.GetObject("langCodeBox.IconPadding"))));
            this.langCodeBox.Name = "langCodeBox";
            this.langCodeBox.Leave += new System.EventHandler(this.langCodeBox_Leave);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // nameBox
            // 
            resources.ApplyResources(this.nameBox, "nameBox");
            this.errorProvider1.SetIconPadding(this.nameBox, ((int)(resources.GetObject("nameBox.IconPadding"))));
            this.nameBox.Name = "nameBox";
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            // 
            // nameBox2
            // 
            resources.ApplyResources(this.nameBox2, "nameBox2");
            this.errorProvider1.SetIconPadding(this.nameBox2, ((int)(resources.GetObject("nameBox2.IconPadding"))));
            this.nameBox2.Name = "nameBox2";
            this.nameBox2.TextChanged += new System.EventHandler(this.nameBox2_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // nameLabel
            // 
            resources.ApplyResources(this.nameLabel, "nameLabel");
            this.nameLabel.Name = "nameLabel";
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider1.ContainerControl = this;
            // 
            // AddLanguageDialog
            // 
            this.AcceptButton = this.okbutton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.okbutton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.translationGroupBox);
            this.Name = "AddLanguageDialog";
            this.ShowInTaskbar = false;
            this.translationGroupBox.ResumeLayout(false);
            this.translationGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okbutton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox translationGroupBox;
        private System.Windows.Forms.TextBox nameBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox langCodeBox;
        private System.Windows.Forms.Label label4;
    }
}