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
    public partial class TipOfDayDialog : Form
    {
        public TipOfDayDialog()
        {
            InitializeComponent();
        }

        public void SetTips(List<String> tips, int firstTip)
        {
            mTips = tips;
            if (mTips == null)
                mTips = new List<string>();
            mCurrentTip = firstTip;
            if (mCurrentTip < 0 || mCurrentTip >= mTips.Count)
                mCurrentTip = 0;
            if (mCurrentTip < mTips.Count)
                SetTipText(mTips[mCurrentTip]);
            prevButton.Enabled = mCurrentTip > 0;
            nextButton.Enabled = mCurrentTip + 1 < mTips.Count;
        }

        private int mCurrentTip;
        private List<String> mTips = new List<string>();

        private void SetTipText(String text)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<html><body>");
            builder.Append(text);
            builder.Append("</body></html>");
            tipPanel.Text = builder.ToString();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            --mCurrentTip;
            if (mCurrentTip >= 0 && mCurrentTip < mTips.Count)
                SetTipText(mTips[mCurrentTip]);
            prevButton.Enabled = mCurrentTip > 0;
            nextButton.Enabled = mCurrentTip + 1 < mTips.Count;
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            ++mCurrentTip;
            if (mCurrentTip >= 0 && mCurrentTip < mTips.Count)
                SetTipText(mTips[mCurrentTip]);
            prevButton.Enabled = mCurrentTip > 0;
            nextButton.Enabled = mCurrentTip + 1 < mTips.Count;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var settings = Ares.Settings.Settings.Instance;
            settings.LastTipOfTheDay = mCurrentTip;
            settings.ShowTipOfTheDay = !dontShowAgainBox.Checked;
            settings.Commit();
        }


    }
}
