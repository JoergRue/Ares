using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Online
{
    public partial class NewsDialog : Form
    {
        public NewsDialog()
        {
            InitializeComponent();
        }

        public void SetNews(String text)
        {
            newsPanel.Text = text;
        }
    }
}
