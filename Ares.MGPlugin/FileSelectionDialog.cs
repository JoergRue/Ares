using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    public partial class FileSelectionDialog : Form
    {
        public FileSelectionDialog()
        {
            InitializeComponent();
        }

        private List<String> m_Files;

        public void SetFiles(List<String> files)
        {
            foreach (String file in files)
            {
                fileList.Items.Add(file);
            }
            m_Files = files;
            if (m_Files.Count > 0)
                fileList.SelectedIndex = 0;
        }

        private String m_SelectedFile;

        public String SelectedFile { get { return m_SelectedFile; } }

        private void okButton_Click(object sender, EventArgs e)
        {
            int index = fileList.SelectedIndex;
            if (index >= 0 && index < m_Files.Count)
                m_SelectedFile = m_Files[index];
        }
    }
}
