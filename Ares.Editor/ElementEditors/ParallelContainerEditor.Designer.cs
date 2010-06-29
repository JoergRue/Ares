/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
namespace Ares.Editor.ElementEditors
{
    partial class ParallelContainerEditor
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
                Actions.ElementChanges.Instance.RemoveListener(-1, Update);
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.volumeControl = new Ares.Editor.Controls.VolumeControl();
            this.parallelContainerControl = new Ares.Editor.Controls.ParallelContainerControl();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.repeatableControl = new Ares.Editor.ElementEditors.RepeatableControl();
            this.delayableControl = new Ares.Editor.ElementEditors.DelayableControl();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.playButton);
            this.groupBox1.Location = new System.Drawing.Point(12, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 75);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Test";
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            this.stopButton.Enabled = false;
            this.stopButton.Image = global::Ares.Editor.ImageResources.StopSmall;
            this.stopButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.stopButton.Location = new System.Drawing.Point(43, 19);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(31, 23);
            this.stopButton.TabIndex = 1;
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            this.playButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.playButton.Location = new System.Drawing.Point(6, 19);
            this.playButton.Name = "playButton";
            this.playButton.Size = new System.Drawing.Size(31, 23);
            this.playButton.TabIndex = 0;
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // volumeControl
            // 
            this.volumeControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.volumeControl.Location = new System.Drawing.Point(12, 94);
            this.volumeControl.Name = "volumeControl";
            this.volumeControl.Size = new System.Drawing.Size(294, 110);
            this.volumeControl.TabIndex = 10;
            // 
            // parallelContainerControl
            // 
            this.parallelContainerControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.parallelContainerControl.Location = new System.Drawing.Point(12, 210);
            this.parallelContainerControl.Name = "parallelContainerControl";
            this.parallelContainerControl.Size = new System.Drawing.Size(664, 323);
            this.parallelContainerControl.TabIndex = 11;
            this.parallelContainerControl.ActiveRowChanged += new System.EventHandler(this.parallelContainerControl_ActiveRowChanged);
            this.parallelContainerControl.ElementDoubleClick += new System.EventHandler<Ares.Editor.Controls.ElementDoubleClickEventArgs>(this.parallelContainerControl_ElementDoubleClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.repeatableControl);
            this.groupBox2.Controls.Add(this.delayableControl);
            this.groupBox2.Location = new System.Drawing.Point(312, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(364, 192);
            this.groupBox2.TabIndex = 12;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Element";
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // repeatableControl
            // 
            this.repeatableControl.Location = new System.Drawing.Point(6, 82);
            this.repeatableControl.Name = "repeatableControl";
            this.repeatableControl.Size = new System.Drawing.Size(346, 96);
            this.repeatableControl.TabIndex = 1;
            // 
            // delayableControl
            // 
            this.delayableControl.BackColor = System.Drawing.SystemColors.Control;
            this.delayableControl.Location = new System.Drawing.Point(6, 19);
            this.delayableControl.Name = "delayableControl";
            this.delayableControl.Size = new System.Drawing.Size(346, 57);
            this.delayableControl.TabIndex = 0;
            // 
            // ParallelContainerEditor
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 545);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.parallelContainerControl);
            this.Controls.Add(this.volumeControl);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ParallelContainerEditor";
            this.Text = "ParallelContainerEditor";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.ParallelContainerEditor_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.ParallelContainerEditor_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.ParallelContainerEditor_DragOver);
            this.DragLeave += new System.EventHandler(this.ParallelContainerEditor_DragLeave);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
        private Controls.VolumeControl volumeControl;
        private Controls.ParallelContainerControl parallelContainerControl;
        private System.Windows.Forms.GroupBox groupBox2;
        private RepeatableControl repeatableControl;
        private DelayableControl delayableControl;
    }
}