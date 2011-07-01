/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
 This file is part of Ares.

 Ares is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 Ares is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with Ares; if not, write to the Free Software
 Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
 */
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
            IBalanceEffect effect = element.Effects.Balance;
            Action = new BalanceChangeAction(element, true);
            fixedButton.Checked = !effect.Random && !effect.IsPanning;
            randomButton.Checked = effect.Random;
            movingButton.Checked = effect.IsPanning;
            fixedBar.Value = effect.FixValue * 10;
            minRandomUpDown.Value = effect.MinRandomValue;
            maxRandomUpDown.Value = effect.MaxRandomValue;
            moveFromUpDown.Value = effect.PanningStart;
            moveToUpDown.Value = effect.PanningEnd;
            UpdateState();
        }

        private void fixedButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            bool fix = fixedButton.Checked;
            bool move = movingButton.Checked;
            fixedBar.Enabled = fix;
            minRandomUpDown.Enabled = !fix && !move;
            maxRandomUpDown.Enabled = !fix && !move;
            moveFromUpDown.Enabled = move;
            moveToUpDown.Enabled = move;
        }

        private void randomButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
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
            bool panning = movingButton.Checked;
            int fixValue = fixedBar.Value / 10;
            int minRandomValue = (int)minRandomUpDown.Value;
            int maxRandomValue = (int)maxRandomUpDown.Value;
            int moveFromValue = (int)moveFromUpDown.Value;
            int moveToValue = (int)moveToUpDown.Value;
            Action.SetData(true, random, fixValue, minRandomValue, maxRandomValue, panning, moveFromValue, moveToValue);
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

        private void movingButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateState();
        }
    }
}
