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

        //private bool listen = true;

        private void SetData()
        {
            //listen = false;
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            allChannelsCheckBox.Checked = settings.PlayMusicOnAllSpeakers;
            //listen = true;
        }

        private void SaveData()
        {
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            settings.PlayMusicOnAllSpeakers = allChannelsCheckBox.Checked;
        }

        public void OnConfirm()
        {
            SaveData();
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
