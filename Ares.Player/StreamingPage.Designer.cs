/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
namespace Ares.Player
{
    partial class StreamingPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StreamingPage));
            this.streamingBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serverAddressBox = new System.Windows.Forms.TextBox();
            this.serverPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.encodingBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.streamNameBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.bitrateBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.urlLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.userNameBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.serverPortUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // streamingBox
            // 
            resources.ApplyResources(this.streamingBox, "streamingBox");
            this.streamingBox.Name = "streamingBox";
            this.streamingBox.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // serverAddressBox
            // 
            resources.ApplyResources(this.serverAddressBox, "serverAddressBox");
            this.serverAddressBox.Name = "serverAddressBox";
            this.serverAddressBox.TextChanged += new System.EventHandler(this.serverAddressBox_TextChanged);
            // 
            // serverPortUpDown
            // 
            resources.ApplyResources(this.serverPortUpDown, "serverPortUpDown");
            this.serverPortUpDown.Maximum = new decimal(new int[] {
            15000,
            0,
            0,
            0});
            this.serverPortUpDown.Name = "serverPortUpDown";
            this.serverPortUpDown.ValueChanged += new System.EventHandler(this.serverPortUpDown_ValueChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // passwordBox
            // 
            resources.ApplyResources(this.passwordBox, "passwordBox");
            this.passwordBox.Name = "passwordBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // encodingBox
            // 
            this.encodingBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.encodingBox.FormattingEnabled = true;
            this.encodingBox.Items.AddRange(new object[] {
            resources.GetString("encodingBox.Items"),
            resources.GetString("encodingBox.Items1")});
            resources.ApplyResources(this.encodingBox, "encodingBox");
            this.encodingBox.Name = "encodingBox";
            this.encodingBox.SelectedIndexChanged += new System.EventHandler(this.encodingBox_SelectedIndexChanged);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.userNameBox);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.streamNameBox);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.bitrateBox);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.urlLabel);
            this.groupBox1.Controls.Add(this.streamingBox);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.encodingBox);
            this.groupBox1.Controls.Add(this.serverAddressBox);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.serverPortUpDown);
            this.groupBox1.Controls.Add(this.passwordBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // streamNameBox
            // 
            resources.ApplyResources(this.streamNameBox, "streamNameBox");
            this.streamNameBox.Name = "streamNameBox";
            this.streamNameBox.TextChanged += new System.EventHandler(this.streamNameBox_TextChanged);
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // bitrateBox
            // 
            this.bitrateBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.bitrateBox.FormattingEnabled = true;
            this.bitrateBox.Items.AddRange(new object[] {
            resources.GetString("bitrateBox.Items"),
            resources.GetString("bitrateBox.Items1"),
            resources.GetString("bitrateBox.Items2"),
            resources.GetString("bitrateBox.Items3"),
            resources.GetString("bitrateBox.Items4"),
            resources.GetString("bitrateBox.Items5"),
            resources.GetString("bitrateBox.Items6")});
            resources.ApplyResources(this.bitrateBox, "bitrateBox");
            this.bitrateBox.Name = "bitrateBox";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // urlLabel
            // 
            resources.ApplyResources(this.urlLabel, "urlLabel");
            this.urlLabel.Name = "urlLabel";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // userNameBox
            // 
            resources.ApplyResources(this.userNameBox, "userNameBox");
            this.userNameBox.Name = "userNameBox";
            // 
            // StreamingPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "StreamingPage";
            ((System.ComponentModel.ISupportInitialize)(this.serverPortUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox streamingBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox serverAddressBox;
        private System.Windows.Forms.NumericUpDown serverPortUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox encodingBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox bitrateBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox streamNameBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox userNameBox;
        private System.Windows.Forms.Label label8;
    }
}