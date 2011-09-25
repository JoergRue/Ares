using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor.Dialogs
{
    public partial class ToolsDialog : Form
    {
        public ToolsDialog()
        {
            InitializeComponent();
            audioFileEditorBox.Text = Ares.Settings.Settings.Instance.SoundFileEditor;
        }

        private void selectFileEditorButton_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles);
            if (openFileDialog1.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                audioFileEditorBox.Text = openFileDialog1.FileName;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Ares.Settings.Settings.Instance.SoundFileEditor = audioFileEditorBox.Text;
        }
    }
}
