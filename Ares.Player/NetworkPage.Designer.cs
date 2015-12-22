/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
    partial class NetworkPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkPage));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.shieldBox = new System.Windows.Forms.PictureBox();
            this.udpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.customIpPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.enableCustomIpBox = new System.Windows.Forms.CheckBox();
            this.webPortUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.enableWebBox = new System.Windows.Forms.CheckBox();
            this.ipAddressBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shieldBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.customIpPortUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.webPortUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.shieldBox);
            this.groupBox1.Controls.Add(this.udpPortUpDown);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.customIpPortUpDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.enableCustomIpBox);
            this.groupBox1.Controls.Add(this.webPortUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.enableWebBox);
            this.groupBox1.Controls.Add(this.ipAddressBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // shieldBox
            // 
            resources.ApplyResources(this.shieldBox, "shieldBox");
            this.shieldBox.Name = "shieldBox";
            this.shieldBox.TabStop = false;
            // 
            // udpPortUpDown
            // 
            resources.ApplyResources(this.udpPortUpDown, "udpPortUpDown");
            this.udpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.udpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udpPortUpDown.Name = "udpPortUpDown";
            this.udpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // customIpPortUpDown
            // 
            resources.ApplyResources(this.customIpPortUpDown, "customIpPortUpDown");
            this.customIpPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.customIpPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.customIpPortUpDown.Name = "customIpPortUpDown";
            this.customIpPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // enableCustomIpBox
            // 
            resources.ApplyResources(this.enableCustomIpBox, "enableCustomIpBox");
            this.enableCustomIpBox.Name = "enableCustomIpBox";
            this.enableCustomIpBox.UseVisualStyleBackColor = true;
            this.enableCustomIpBox.CheckedChanged += new System.EventHandler(this.enableCustomIpBox_CheckedChanged);
            // 
            // webPortUpDown
            // 
            resources.ApplyResources(this.webPortUpDown, "webPortUpDown");
            this.webPortUpDown.Maximum = new decimal(new int[] {
            50000,
            0,
            0,
            0});
            this.webPortUpDown.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.webPortUpDown.Name = "webPortUpDown";
            this.webPortUpDown.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // enableWebBox
            // 
            resources.ApplyResources(this.enableWebBox, "enableWebBox");
            this.enableWebBox.Name = "enableWebBox";
            this.enableWebBox.UseVisualStyleBackColor = true;
            this.enableWebBox.CheckedChanged += new System.EventHandler(this.enableWebBox_CheckedChanged);
            // 
            // ipAddressBox
            // 
            this.ipAddressBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ipAddressBox.FormattingEnabled = true;
            resources.ApplyResources(this.ipAddressBox, "ipAddressBox");
            this.ipAddressBox.Name = "ipAddressBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NetworkPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "NetworkPage";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.shieldBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udpPortUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.customIpPortUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.webPortUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.NumericUpDown customIpPortUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox enableCustomIpBox;
        private System.Windows.Forms.NumericUpDown webPortUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox enableWebBox;
        private System.Windows.Forms.ComboBox ipAddressBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown udpPortUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox shieldBox;
    }
}
