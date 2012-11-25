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

namespace Ares.Editor.Controls
{
    public partial class SpeakersDialog : Form
    {
        private Data.IFileElement Element { get; set; }
        private bool m_InPlay = false;

        public Actions.SpeakerChangeAction Action { get; set; }
        
        public SpeakersDialog(IList<Data.IFileElement> elements)
        {
            InitializeComponent();
            Element = elements[0];
            Action = new Actions.SpeakerChangeAction(elements, true, Element.Effects.SpeakerAssignment.Random, Element.Effects.SpeakerAssignment.Assignment);
            if (Element.Effects.SpeakerAssignment.Random)
            {
                randomButton.Checked = true;
                defaultButton.Checked = false;
                oneSpeakerBox.Enabled = false;
                oneSpeakerBox.SelectedIndex = 0;
                oneSpeakerButton.Checked = false;
                twoSpeakersBox.Enabled = false;
                twoSpeakersBox.SelectedIndex = 0;
                twoSpeakersButton.Checked = false;
            }
            else
            {
                randomButton.Checked = false;
                switch (Element.Effects.SpeakerAssignment.Assignment)
                {
                    case Data.SpeakerAssignment.LeftFront:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.RightFront:
                        oneSpeakerBox.SelectedIndex = 1;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.Center:
                        oneSpeakerBox.SelectedIndex = 2;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.LeftBack:
                        oneSpeakerBox.SelectedIndex = 3;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.RightBack:
                        oneSpeakerBox.SelectedIndex = 4;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.LeftCenterBack:
                        oneSpeakerBox.SelectedIndex = 5;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.RightCenterBack:
                        oneSpeakerBox.SelectedIndex = 6;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.Subwoofer:
                        oneSpeakerBox.SelectedIndex = 7;
                        oneSpeakerButton.Checked = true;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersBox.Enabled = false;
                        twoSpeakersButton.Checked = false;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.BothFronts:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerBox.Enabled = false;
                        oneSpeakerButton.Checked = false;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersButton.Checked = true;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.BothRears:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerBox.Enabled = false;
                        oneSpeakerButton.Checked = false;
                        twoSpeakersBox.SelectedIndex = 1;
                        twoSpeakersButton.Checked = true;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.BothCenterRears:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerBox.Enabled = false;
                        oneSpeakerButton.Checked = false;
                        twoSpeakersBox.SelectedIndex = 2;
                        twoSpeakersButton.Checked = true;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.CenterAndSubwoofer:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerBox.Enabled = false;
                        oneSpeakerButton.Checked = false;
                        twoSpeakersBox.SelectedIndex = 3;
                        twoSpeakersButton.Checked = true;
                        defaultButton.Checked = false;
                        break;
                    case Data.SpeakerAssignment.Default:
                    default:
                        oneSpeakerBox.SelectedIndex = 0;
                        oneSpeakerBox.Enabled = false;
                        oneSpeakerButton.Checked = false;
                        twoSpeakersBox.SelectedIndex = 0;
                        twoSpeakersButton.Checked = false;
                        twoSpeakersBox.Enabled = false;
                        defaultButton.Checked = true;
                        break;
                }
            }
        }

        private void UpdateControls()
        {
            oneSpeakerBox.Enabled = oneSpeakerButton.Checked && !m_InPlay;
            twoSpeakersBox.Enabled = twoSpeakersButton.Checked && !m_InPlay;
            oneSpeakerButton.Enabled = !m_InPlay;
            twoSpeakersButton.Enabled = !m_InPlay;
            defaultButton.Enabled = !m_InPlay;
            randomButton.Enabled = !m_InPlay;
            playButton.Enabled = !m_InPlay;
            stopButton.Enabled = m_InPlay;
        }

        private void defaultButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        public void UpdateAction()
        {
            Data.SpeakerAssignment assignment = Data.SpeakerAssignment.Default;
            bool random = randomButton.Checked;
            if (!random && !defaultButton.Checked)
            {
                if (oneSpeakerButton.Checked)
                {
                    switch (oneSpeakerBox.SelectedIndex)
                    {
                        case 0:
                            assignment = Data.SpeakerAssignment.LeftFront;
                            break;
                        case 1:
                            assignment = Data.SpeakerAssignment.RightFront;
                            break;
                        case 2:
                            assignment = Data.SpeakerAssignment.Center;
                            break;
                        case 3:
                            assignment = Data.SpeakerAssignment.LeftBack;
                            break;
                        case 4:
                            assignment = Data.SpeakerAssignment.RightBack;
                            break;
                        case 5:
                            assignment = Data.SpeakerAssignment.LeftCenterBack;
                            break;
                        case 6:
                            assignment = Data.SpeakerAssignment.RightCenterBack;
                            break;
                        case 7:
                            assignment = Data.SpeakerAssignment.Subwoofer;
                            break;
                        default:
                            assignment = Data.SpeakerAssignment.Default;
                            break;
                    }
                }
                else if (twoSpeakersButton.Checked)
                {
                    switch (twoSpeakersBox.SelectedIndex)
                    {
                        case 0:
                            assignment = Data.SpeakerAssignment.BothFronts;
                            break;
                        case 1:
                            assignment = Data.SpeakerAssignment.BothRears;
                            break;
                        case 2:
                            assignment = Data.SpeakerAssignment.BothCenterRears;
                            break;
                        case 3:
                            assignment = Data.SpeakerAssignment.CenterAndSubwoofer;
                            break;
                        default:
                            assignment = Data.SpeakerAssignment.Default;
                            break;
                    }
                }
            }
            Action.SetData(true, random, assignment);
        }

        private void oneSpeakerButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void twoSpeakersButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void randomButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

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

        private void stopButton_Click(object sender, EventArgs e)
        {
            Actions.Playing.Instance.StopElement(Element);
        }

        private void SpeakersDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_InPlay)
            {
                Actions.Playing.Instance.StopElement(Element);
            }
        }


    }
}
