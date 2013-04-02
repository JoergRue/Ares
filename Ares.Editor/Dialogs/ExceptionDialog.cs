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
    public partial class ExceptionDialog : Form
    {
        public ExceptionDialog()
        {
            InitializeComponent();
        }

        public void SetException(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();
            builder.AppendFormat("Ares Editor Version {0}", assemblyVersion);
            builder.AppendLine();
            builder.AppendLine();
            builder.Append(ex.GetType().Name);
            builder.Append(": ");
            builder.AppendLine(ex.Message);
            builder.AppendLine();
            builder.Append(ex.StackTrace);
            infoBox.Text = builder.ToString();
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            String text = infoBox.Text;
            Clipboard.SetText(text);
        }

        private void mailLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String linkTarget = "mailto:aresrpg-bugs@lists.sourceforge.net";
            linkTarget += "?subject=Exception in Ares Editor ";
            linkTarget += (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();
            System.Diagnostics.Process.Start(linkTarget);
        }
    }
}
