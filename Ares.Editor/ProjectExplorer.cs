using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    public partial class ProjectExplorer : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public ProjectExplorer()
        {
            InitializeComponent();
            projectTree.Nodes.Add(StringResources.NoOpenedProject);
        }

        void SetProject(Ares.Data.IProject project)
        {
        }

        private Ares.Data.IProject project;
    }
}
