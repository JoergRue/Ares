using System;
using System.Windows.Forms;

namespace Ares.MGPlugin
{
    class TransparentLabel : Label
    {
        public TransparentLabel()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Transparent;
        }
    }

    class TransparentLinkLabel : LinkLabel
    {
        public TransparentLinkLabel()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Transparent;
        }
    }

    class TransparentFlowLayoutPanel : FlowLayoutPanel
    {
        public TransparentFlowLayoutPanel()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = System.Drawing.Color.Transparent;
        }
    }
}
