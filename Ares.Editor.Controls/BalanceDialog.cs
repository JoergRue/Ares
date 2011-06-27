using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Data;
using Ares.Editor.Actions;

namespace Ares.Editor.Controls
{
    public partial class BalanceDialog : Form
    {

        public BalanceChangeAction Action { get; set; }

        public BalanceDialog(IFileElement element)
        {
            InitializeComponent();
            Element = element;
            IIntEffect effect = element.Effects.Balance;
            Action = new BalanceChangeAction(element, true, effect.Random, effect.FixValue, effect.MinRandomValue, effect.MaxRandomValue);
            fixedButton.Checked = !effect.Random;
            randomButton.Checked = effect.Random;
            fixedBar.Value = effect.FixValue * 10;
            minRandomUpDown.Value = effect.MinRandomValue;
            maxRandomUpDown.Value = effect.MaxRandomValue;
        }

        private void fixedButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFixed();
        }

        private void UpdateFixed()
        {
            bool fix = fixedButton.Checked;
            fixedBar.Enabled = fix;
            minRandomUpDown.Enabled = !fix;
            maxRandomUpDown.Enabled = !fix;
        }

        private void randomButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFixed();
        }

        private void minRandomUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (maxRandomUpDown.Value < minRandomUpDown.Value)
            {
                maxRandomUpDown.Value = minRandomUpDown.Value;
            }
        }

        private void maxRandomUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (minRandomUpDown.Value > maxRandomUpDown.Value)
            {
                minRandomUpDown.Value = maxRandomUpDown.Value;
            }
        }

        private IFileElement Element { get; set; }

        public void UpdateAction()
        {
            bool random = randomButton.Checked;
            int fixValue = fixedBar.Value / 10;
            int minRandomValue = (int)minRandomUpDown.Value;
            int maxRandomValue = (int)maxRandomUpDown.Value;
            Action.SetData(true, random, fixValue, minRandomValue, maxRandomValue);
        }

        private bool m_InPlay = false;

        private void playButton_Click(object sender, EventArgs e)
        {
            m_InPlay = true;
            UpdateAction();
            UpdateControls();
            Action.Do();
            Actions.Playing.Instance.PlayElement(Element, this, () =>
                {
                    m_InPlay = false;
                    Action.Undo();
                    UpdateControls();
                }
            );
        }

        private void UpdateControls()
        {
            playButton.Enabled = !m_InPlay;
            stopButton.Enabled = m_InPlay;
            fixedButton.Enabled = !m_InPlay;
            fixedBar.Enabled = !m_InPlay;
            randomButton.Enabled = !m_InPlay;
            minRandomUpDown.Enabled = !m_InPlay;
            maxRandomUpDown.Enabled = !m_InPlay;
        }

        private void PitchDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_InPlay)
            {
                Actions.Playing.Instance.StopElement(Element);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(Element);
        }
    }
}
