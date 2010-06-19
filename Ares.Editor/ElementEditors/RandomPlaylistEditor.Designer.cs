namespace Ares.Editor.ElementEditors
{
    partial class RandomPlaylistEditor
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
                Actions.ElementChanges.Instance.RemoveListener(m_Playlist.Id, Update);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RandomPlaylistEditor));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.playButton = new System.Windows.Forms.Button();
            this.delayableControl = new Ares.Editor.ElementEditors.DelayableControl();
            this.repeatableControl = new Ares.Editor.ElementEditors.RepeatableControl();
            this.choiceContainerControl = new Ares.Editor.ElementEditorControls.ChoiceContainerControl();
            this.volumeControl = new Ares.Editor.Controls.VolumeControl();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AllowDrop = true;
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.InactiveCaption;
            this.label1.Name = "label1";
            this.label1.UseCompatibleTextRendering = true;
            this.label1.DragDrop += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragDrop);
            this.label1.DragEnter += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragEnter);
            this.label1.DragOver += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragOver);
            this.label1.DragLeave += new System.EventHandler(this.RandomPlaylistEditor_DragLeave);
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
            this.playButton.Image = global::Ares.Editor.ImageResources.RunSmall;
            resources.ApplyResources(this.playButton, "playButton");
            this.playButton.Name = "playButton";
            this.playButton.UseCompatibleTextRendering = true;
            this.playButton.UseVisualStyleBackColor = true;
            this.playButton.Click += new System.EventHandler(this.playButton_Click);
            // 
            // delayableControl
            // 
            this.delayableControl.BackColor = System.Drawing.SystemColors.Control;
            resources.ApplyResources(this.delayableControl, "delayableControl");
            this.delayableControl.MaximumSize = new System.Drawing.Size(346, 57);
            this.delayableControl.Name = "delayableControl";
            // 
            // repeatableControl
            // 
            resources.ApplyResources(this.repeatableControl, "repeatableControl");
            this.repeatableControl.MaximumSize = new System.Drawing.Size(346, 96);
            this.repeatableControl.Name = "repeatableControl";
            // 
            // choiceContainerControl
            // 
            resources.ApplyResources(this.choiceContainerControl, "choiceContainerControl");
            this.choiceContainerControl.Name = "choiceContainerControl";
            // 
            // volumeControl
            // 
            resources.ApplyResources(this.volumeControl, "volumeControl");
            this.volumeControl.MinimumSize = new System.Drawing.Size(297, 83);
            this.volumeControl.Name = "volumeControl";
            // 
            // RandomPlaylistEditor
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.volumeControl);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.delayableControl);
            this.Controls.Add(this.repeatableControl);
            this.Controls.Add(this.choiceContainerControl);
            this.Controls.Add(this.label1);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.Document)));
            this.Name = "RandomPlaylistEditor";
            this.SizeChanged += new System.EventHandler(this.RandomPlaylistEditor_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragOver);
            this.DragLeave += new System.EventHandler(this.RandomPlaylistEditor_DragLeave);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private ElementEditorControls.ChoiceContainerControl choiceContainerControl;
        private RepeatableControl repeatableControl;
        private DelayableControl delayableControl;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button playButton;
        private Controls.VolumeControl volumeControl;
    }
}