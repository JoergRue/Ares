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
    public partial class FileVolumeDialog : Form
    {
        public ElementVolumeEffectsChangeAction Action { get; set; }

        public FileVolumeDialog(IList<IFileElement> elements, IProject project)
        {
            InitializeComponent();
            Element = elements[0];
            m_Project = project;
            Action = new ElementVolumeEffectsChangeAction(elements, Element.Effects.HasRandomVolume,
                Element.Effects.Volume, Element.Effects.MinRandomVolume, Element.Effects.MaxRandomVolume,
                Element.Effects.FadeInTime, Element.Effects.FadeOutTime, Element.Effects.CrossFading);
            randomButton.Checked = Element.Effects.HasRandomVolume;
            fixedVolumeButton.Checked = !Element.Effects.HasRandomVolume;
            volumeBar.Value = Element.Effects.Volume;
            minRandomUpDown.Value = Element.Effects.MinRandomVolume;
            maxRandomUpDown.Value = Element.Effects.MaxRandomVolume;
            fadeInUpDown.Value = Element.Effects.FadeInTime;
            fadeOutUpDown.Value = Element.Effects.FadeOutTime;
            fadeInUnitBox.SelectedIndex = 0;
            fadeOutUnitBox.SelectedIndex = 0;
            crossFadingBox.Checked = Element.Effects.CrossFading;
            crossFadingBox.Enabled = Element.Effects.FadeOutTime > 0;
        }

        private IFileElement Element { get; set; }
        private IProject m_Project;

        private void fixedVolumeButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFixed();
        }

        private void UpdateFixed()
        {
            bool fix = fixedVolumeButton.Checked;
            volumeBar.Enabled = fix;
            minRandomUpDown.Enabled = !fix;
            maxRandomUpDown.Enabled = !fix;
        }

        private void randomButton_CheckedChanged(object sender, EventArgs e)
        {
            UpdateFixed();
        }

        private void minRandomUpDown_ValueChanged(object sender, EventArgs e)
        {

        }

        private void maxRandomUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (minRandomUpDown.Value > maxRandomUpDown.Value)
            {
                minRandomUpDown.Value = maxRandomUpDown.Value;
            }
        }

        public void UpdateAction()
        {
            bool random = randomButton.Checked;
            int fixValue = volumeBar.Value;
            int minRandomValue = (int)minRandomUpDown.Value;
            int maxRandomValue = (int)maxRandomUpDown.Value;
            int fadeIn = TimeConversion.GetTimeInMillis(fadeInUpDown, fadeInUnitBox);
            int fadeOut = TimeConversion.GetTimeInMillis(fadeOutUpDown, fadeOutUnitBox);
            bool crossFading = crossFadingBox.Checked;
            Action.SetData(random, fixValue, minRandomValue, maxRandomValue, fadeIn, fadeOut, crossFading);
        }

        private bool m_InPlay = false;

        private void playButton_Click(object sender, EventArgs e)
        {
            m_InPlay = true;
            UpdateAction();
            UpdateControls();
            Action.Do(m_Project);
            Actions.Playing.Instance.PlayElement(Element, this, () =>
            {
                m_InPlay = false;
                Action.Undo(m_Project);
                UpdateControls();
            }
            );
        }

        private void UpdateControls()
        {
            playButton.Enabled = !m_InPlay;
            stopButton.Enabled = m_InPlay;
            fixedVolumeButton.Enabled = !m_InPlay;
            volumeBar.Enabled = !m_InPlay;
            randomButton.Enabled = !m_InPlay;
            minRandomUpDown.Enabled = !m_InPlay;
            maxRandomUpDown.Enabled = !m_InPlay;
        }

        private void FileVolumeDialog_FormClosing(object sender, FormClosingEventArgs e)
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

        private TimeUnit m_FadeInUnit = TimeUnit.Milliseconds;
        private TimeUnit m_FadeOutUnit = TimeUnit.Milliseconds;

        private void fadeInUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int oldValue = TimeConversion.GetTimeInMillis((int)fadeInUpDown.Value, m_FadeInUnit);
            fadeInUpDown.Value = TimeConversion.GetTimeInUnit(oldValue, fadeInUnitBox);
            m_FadeInUnit = (TimeUnit)fadeInUnitBox.SelectedIndex;
        }

        private void fadeOutUnitBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int oldValue = TimeConversion.GetTimeInMillis((int)fadeOutUpDown.Value, m_FadeOutUnit);
            fadeOutUpDown.Value = TimeConversion.GetTimeInUnit(oldValue, fadeOutUnitBox);
            m_FadeOutUnit = (TimeUnit)fadeOutUnitBox.SelectedIndex;
        }

        private void fadeOutUpDown_ValueChanged(object sender, EventArgs e)
        {
            crossFadingBox.Enabled = fadeOutUpDown.Value > 0;
        }
    }
}
