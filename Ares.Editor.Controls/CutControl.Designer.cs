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
namespace Ares.Editor.Controls
{
    partial class CutControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CutControl));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cueOutActive = new System.Windows.Forms.CheckBox();
            this.cueOutTime = new System.Windows.Forms.MaskedTextBox();
            this.cueInActive = new System.Windows.Forms.CheckBox();
            this.cueInTime = new System.Windows.Forms.MaskedTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cueOutActive);
            this.groupBox1.Controls.Add(this.cueOutTime);
            this.groupBox1.Controls.Add(this.cueInActive);
            this.groupBox1.Controls.Add(this.cueInTime);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // cueOutActive
            // 
            resources.ApplyResources(this.cueOutActive, "cueOutActive");
            this.cueOutActive.Name = "cueOutActive";
            this.cueOutActive.UseVisualStyleBackColor = true;
            this.cueOutActive.CheckedChanged += new System.EventHandler(this.cueOutActive_CheckedChanged);
            // 
            // cueOutTime
            // 
            resources.ApplyResources(this.cueOutTime, "cueOutTime");
            this.cueOutTime.Name = "cueOutTime";
            this.cueOutTime.Leave += new System.EventHandler(this.cueOutTime_Leave);
            // 
            // cueInActive
            // 
            resources.ApplyResources(this.cueInActive, "cueInActive");
            this.cueInActive.Name = "cueInActive";
            this.cueInActive.UseVisualStyleBackColor = true;
            this.cueInActive.CheckedChanged += new System.EventHandler(this.cueInActive_CheckedChanged);
            // 
            // cueInTime
            // 
            resources.ApplyResources(this.cueInTime, "cueInTime");
            this.cueInTime.Name = "cueInTime";
            this.cueInTime.Leave += new System.EventHandler(this.cueInTime_Leave);
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            this.label6.UseCompatibleTextRendering = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            this.label7.UseCompatibleTextRendering = true;
            // 
            // CutControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "CutControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox cueOutActive;
        private System.Windows.Forms.MaskedTextBox cueOutTime;
        private System.Windows.Forms.CheckBox cueInActive;
        private System.Windows.Forms.MaskedTextBox cueInTime;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}
