using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Settings
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();
            String assemblyVersion = (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString();
            versionLabel.Text = "Version " + assemblyVersion;
        }
    }
}
