using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ares.Player
{
    public partial class MusicPage : UserControl
    {
        public MusicPage()
        {
            InitializeComponent();
            SetData();
        }

        private bool listen = true;

        private void SetData()
        {
            listen = false;
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            allChannelsCheckBox.Checked = settings.PlayMusicOnAllSpeakers;
            noFadeButton.Checked = settings.ButtonMusicFadeMode == 0;
            fadeButton.Checked = settings.ButtonMusicFadeMode == 1;
            crossFadeButton.Checked = settings.ButtonMusicFadeMode == 2;
            crossFadingUpDown.Value = settings.ButtonMusicFadeTime;
            crossFadingUpDown.Enabled = settings.ButtonMusicFadeMode != 0;
            listen = true;
        }

        private void SaveData()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            settings.PlayMusicOnAllSpeakers = allChannelsCheckBox.Checked;
            if (noFadeButton.Checked)
                settings.ButtonMusicFadeMode = 0;
            else if (fadeButton.Checked)
                settings.ButtonMusicFadeMode = 1;
            else
                settings.ButtonMusicFadeMode = 2;
            settings.ButtonMusicFadeTime = (int)crossFadingUpDown.Value;
        }

        public void OnConfirm()
        {
            SaveData();
        }

        private void noFadeButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }

        private void fadeButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }

        private void crossFadeButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!listen) return;
            crossFadingUpDown.Enabled = !noFadeButton.Checked;
        }
    }

    class MusicPageHost : Ares.CommonGUI.ISettingsDialogPage
    {
        private MusicPage m_Page = new MusicPage();

        public Control Page
        {
            get { return m_Page; }
        }

        public string PageTitle
        {
            get { return StringResources.Music; }
        }

        public void OnConfirm()
        {
            m_Page.OnConfirm();
        }
    }
}
