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
            }
            base.Dispose(disposing);
            Actions.ElementChanges.Instance.RemoveListener(m_Playlist.Id, Update);
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
            this.choiceContainerControl = new Ares.Editor.ElementEditorControls.ChoiceContainerControl();
            this.repeatableControl = new Ares.Editor.ElementEditors.RepeatableControl();
            this.delayableControl = new Ares.Editor.ElementEditors.DelayableControl();
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
            // choiceContainerControl
            // 
            resources.ApplyResources(this.choiceContainerControl, "choiceContainerControl");
            this.choiceContainerControl.Name = "choiceContainerControl";
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
            // RandomPlaylistEditor
            // 
            this.AllowDrop = true;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.delayableControl);
            this.Controls.Add(this.repeatableControl);
            this.Controls.Add(this.choiceContainerControl);
            this.Controls.Add(this.label1);
            this.Name = "RandomPlaylistEditor";
            this.SizeChanged += new System.EventHandler(this.RandomPlaylistEditor_SizeChanged);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.RandomPlaylistEditor_DragOver);
            this.DragLeave += new System.EventHandler(this.RandomPlaylistEditor_DragLeave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private ElementEditorControls.ChoiceContainerControl choiceContainerControl;
        private RepeatableControl repeatableControl;
        private DelayableControl delayableControl;
    }
}