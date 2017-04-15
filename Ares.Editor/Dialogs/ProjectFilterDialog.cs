using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares.Editor.Dialogs
{
    public partial class ProjectFilterDialog : Form
    {
        public ProjectFilterDialog()
        {
            InitializeComponent();
        }

        public ProjectUseFilterMode FilterMode
        {
            get
            {
                if (noFilterButton.Checked) return ProjectUseFilterMode.NoFilter;
                else if (onlyUsedFilesButton.Checked) return ProjectUseFilterMode.UsedFilter;
                else return ProjectUseFilterMode.UnusedFilter;
            }
            set
            {
                switch (value)
                {
                    case ProjectUseFilterMode.NoFilter:
                        noFilterButton.Checked = true;
                        break;
                    case ProjectUseFilterMode.UnusedFilter:
                        showNotUsedButton.Checked = true;
                        break;
                    case ProjectUseFilterMode.UsedFilter:
                    default:
                        onlyUsedFilesButton.Checked = true;
                        break;
                }
            }
        }

        public bool AutoUpdateTree
        {
            get { return autoUpdateBox.Checked; }
            set { autoUpdateBox.Checked = value; }
        }
    }
}
