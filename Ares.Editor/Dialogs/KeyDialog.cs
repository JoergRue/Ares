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
    public partial class KeyDialog : Form
    {
        private KeyDialog()
        {
            InitializeComponent();
        }

        private void KeyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                result = System.Windows.Forms.DialogResult.Cancel;
            }
            else
            {
                result = System.Windows.Forms.DialogResult.OK;
                keyCode = (int)e.KeyCode;
            }
            Close();
        }

        public static DialogResult Show(IWin32Window parent, out int keyCode)
        {
            KeyDialog dialog = new KeyDialog();
            dialog.ShowDialog(parent);
            keyCode = dialog.keyCode;
            return dialog.result;
        }

        private DialogResult result;
        private int keyCode;
    }
}
