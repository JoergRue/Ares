using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    partial class InputDialog : Form
    {
        private InputDialog()
        {
            InitializeComponent();
        }

        public static DialogResult Show(IWin32Window parent, String message, out String input)
        {
            return Show(parent, message, String.Empty, out input);
        }

        public static DialogResult Show(IWin32Window parent, String message, String defaultValue, out String input)
        {
            InputDialog dialog = new InputDialog();
            dialog.descriptionLabel.Text = message;
            dialog.textBox.Text = defaultValue;
            dialog.Text = StringResources.Ares;
            DialogResult result = dialog.ShowDialog(parent);
            if (result == DialogResult.OK)
                input = dialog.textBox.Text;
            else
                input = String.Empty;
            return result;
        }
    }
}
