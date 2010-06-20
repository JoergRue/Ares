using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Ares.Player
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                BassRegistration.Registration.RegisterBass();
                if (!Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    MessageBox.Show(StringResources.BassInitFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                if (!Un4seen.Bass.AddOn.Fx.BassFx.LoadMe())
                {
                    MessageBox.Show(StringResources.BassInitFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show(StringResources.BassInitFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Application.Run(new Player());
            Un4seen.Bass.Bass.BASS_Free();
        }
    }
}
