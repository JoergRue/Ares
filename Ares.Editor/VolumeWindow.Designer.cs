namespace Ares.Editor
{
    partial class VolumeWindow
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
                Settings.Settings.Instance.SettingsChanged -= new System.EventHandler<Settings.Settings.SettingsEventArgs>(SettingsChanged);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VolumeWindow));
            this.soundVolumeBar = new System.Windows.Forms.TrackBar();
            this.label4 = new System.Windows.Forms.Label();
            this.musicVolumeBar = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.overallVolumeBar = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).BeginInit();
            this.SuspendLayout();
            // 
            // soundVolumeBar
            // 
            resources.ApplyResources(this.soundVolumeBar, "soundVolumeBar");
            this.soundVolumeBar.Maximum = 100;
            this.soundVolumeBar.Name = "soundVolumeBar";
            this.soundVolumeBar.SmallChange = 5;
            this.soundVolumeBar.TickFrequency = 5;
            this.soundVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.soundVolumeBar.Value = 100;
            this.soundVolumeBar.Scroll += new System.EventHandler(this.soundVolumeBar_Scroll);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            this.label4.UseCompatibleTextRendering = true;
            // 
            // musicVolumeBar
            // 
            resources.ApplyResources(this.musicVolumeBar, "musicVolumeBar");
            this.musicVolumeBar.Maximum = 100;
            this.musicVolumeBar.Name = "musicVolumeBar";
            this.musicVolumeBar.SmallChange = 5;
            this.musicVolumeBar.TickFrequency = 5;
            this.musicVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.musicVolumeBar.Value = 100;
            this.musicVolumeBar.Scroll += new System.EventHandler(this.musicVolumeBar_Scroll);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            this.label3.UseCompatibleTextRendering = true;
            // 
            // overallVolumeBar
            // 
            resources.ApplyResources(this.overallVolumeBar, "overallVolumeBar");
            this.overallVolumeBar.Maximum = 100;
            this.overallVolumeBar.Name = "overallVolumeBar";
            this.overallVolumeBar.SmallChange = 5;
            this.overallVolumeBar.TickFrequency = 5;
            this.overallVolumeBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.overallVolumeBar.Value = 100;
            this.overallVolumeBar.Scroll += new System.EventHandler(this.overallVolumeBar_Scroll);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.UseCompatibleTextRendering = true;
            // 
            // VolumeWindow
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.soundVolumeBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.musicVolumeBar);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.overallVolumeBar);
            this.Controls.Add(this.label2);
            this.Name = "VolumeWindow";
            this.Load += new System.EventHandler(this.VolumeWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.soundVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.musicVolumeBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overallVolumeBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar soundVolumeBar;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar musicVolumeBar;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TrackBar overallVolumeBar;
        private System.Windows.Forms.Label label2;
    }
}