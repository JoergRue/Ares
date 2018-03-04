using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares.Editor.Dialogs
{
    public partial class LightEffectsPage : UserControl
    {
        public LightEffectsPage()
        {
            InitializeComponent();
            textHostIPAddress.Text = Ares.Settings.Settings.Instance.Tpm2NetTargetHost;
            textTpm2Port.Text = Ares.Settings.Settings.Instance.Tpm2NetTargetPort.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public void OnConfirm()
        {
            Ares.Settings.Settings.Instance.Tpm2NetTargetHost = textHostIPAddress.Text;
            try
            {
                Ares.Settings.Settings.Instance.Tpm2NetTargetPort = UInt16.Parse(textTpm2Port.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
    public class LightEffectsPageHost : Ares.CommonGUI.ISettingsDialogPage
    {
        private LightEffectsPage m_Page = new LightEffectsPage();

        public Control Page
        {
            get { return m_Page; }
        }

        public string PageTitle
        {
            get { return StringResources.LightEffects; }
        }

        public void OnConfirm()
        {
            m_Page.OnConfirm();
        }
    }

}
