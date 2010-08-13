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
            }
            if (disposing)
            {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParallelContainerEditor));
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
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.stopButton);
            this.groupBox1.Controls.Add(this.playButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseCompatibleTextRendering = true;
            // 
            // stopButton
            // 
            resources.ApplyResources(this.stopButton, "stopButton");
            this.stopButton.Image = global::Ares.Editor.ImageResources.StopSmall;
            this.stopButton.Name = "stopButton";
            this.stopButton.UseCompatibleTextRendering = true;
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // playButton
            // 
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            this.playButton.Name = "playButton";
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // volumeControl
            // 
            resources.ApplyResources(this.volumeControl, "volumeControl");
            this.volumeControl.Name = "volumeControl";
            // 
            // parallelContainerControl
            // 
            resources.ApplyResources(this.parallelContainerControl, "parallelContainerControl");
            this.parallelContainerControl.Name = "parallelContainerControl";
            this.parallelContainerControl.ActiveRowChanged += new System.EventHandler(this.parallelContainerControl_ActiveRowChanged);
            this.parallelContainerControl.ElementDoubleClick += new System.EventHandler<Ares.Editor.Controls.ElementDoubleClickEventArgs>(this.parallelContainerControl_ElementDoubleClick);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.repeatableControl);
            this.groupBox2.Controls.Add(this.delayableControl);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseCompatibleTextRendering = true;
            // 
            // repeatableControl
            // 
            resources.ApplyResources(this.repeatableControl, "repeatableControl");
            this.repeatableControl.Name = "repeatableControl";
            // 
            // delayableControl
            // 
            resources.ApplyResources(this.delayableControl, "delayableControl");
            this.delayableControl.BackColor = System.Drawing.SystemColors.Control;
            this.delayableControl.Name = "delayableControl";
            // 
            // ParallelContainerEditor
            // 
            resources.ApplyResources(this, "$this");
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.parallelContainerControl);
            this.Controls.Add(this.volumeControl);
            this.Controls.Add(this.groupBox1);
            this.Name = "ParallelContainerEditor";
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