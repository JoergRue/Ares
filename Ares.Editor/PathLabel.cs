using System;
using System.Text;
using System.Windows.Forms;

namespace Ares.Editor
{
    class PathLabel : Label
    {
        [System.ComponentModel.Browsable(false)]
        public override bool AutoSize
        {
            get { return base.AutoSize; }
            set { base.AutoSize = false; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.PathEllipsis;
            TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.ForeColor, flags);
        }

    }
}
