/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace Ares.Editor
{
    static class Program
    {
        private static String GetBassInitErrorMessage()
        {
            switch (Un4seen.Bass.Bass.BASS_ErrorGetCode())
            {
                case Un4seen.Bass.BASSError.BASS_ERROR_DEVICE:
                    return StringResources.BassDeviceInvalid;
                case Un4seen.Bass.BASSError.BASS_ERROR_ALREADY:
                    return StringResources.BassDeviceAlready;
                case Un4seen.Bass.BASSError.BASS_ERROR_DRIVER:
                    return StringResources.BassDeviceDriver;
                case Un4seen.Bass.BASSError.BASS_ERROR_FORMAT:
                    return StringResources.BassDeviceFormat;
                case Un4seen.Bass.BASSError.BASS_ERROR_MEM:
                    return StringResources.BassNoMem;
                case Un4seen.Bass.BASSError.BASS_ERROR_NO3D:
                    return StringResources.BassNo3D;
                default:
                    return StringResources.BassUnknown;
            }
        }

        private static String MakeBassInitErrorMessage()
        {
            int device = Un4seen.Bass.Bass.BASS_GetDevice();
            if (device != -1)
            {
                Un4seen.Bass.BASS_DEVICEINFO deviceInfo = Un4seen.Bass.Bass.BASS_GetDeviceInfo(device);
                if (deviceInfo != null)
                {
                    String deviceStr = String.Format(StringResources.BassDeviceInfo, deviceInfo.name,
                        deviceInfo.driver != null ? deviceInfo.driver : StringResources.NoDeviceDriver,
                        deviceInfo.IsEnabled ? StringResources.DeviceEnabled : StringResources.DeviceDisabled);
                    return String.Format(StringResources.BassInitFail, GetBassInitErrorMessage(), deviceStr);
                }
            }
            return String.Format(StringResources.BassInitFail, GetBassInitErrorMessage(), StringResources.NoDevice);
        }

        private static bool IsLinux
		{
		    get
		    {
		        int p = (int) Environment.OSVersion.Platform;
		        return (p == 4) || (p == 6) || (p == 128);
		    }
		}

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                string arg = Environment.GetCommandLineArgs()[1];
                string paramName = "Language=";
                if (arg.StartsWith(paramName))
                {
                    arg = arg.Substring(paramName.Length);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(arg);
                }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            int bassPlugin1, bassPlugin2, bassPlugin3;
            try
            {
                BassRegistration.Registration.RegisterBass();

#if !MONO
                if (!Un4seen.Bass.Bass.LoadMe())
                {
                    MessageBox.Show(StringResources.BassLoadFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
#endif
                if (!Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    MessageBox.Show(MakeBassInitErrorMessage(), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
#if !MONO
                if (!Un4seen.Bass.AddOn.Fx.BassFx.LoadMe())
                {
                    MessageBox.Show(StringResources.BassFxLoadFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
#endif
                String flacPlugin = IsLinux ? "libbassflag.so" : "bassflac.dll";
                bassPlugin1 = Un4seen.Bass.Bass.BASS_PluginLoad(flacPlugin);
                if (bassPlugin1 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    MessageBox.Show(StringResources.BassFlacLoadFail1, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                String aacPlugin = IsLinux ? "libbass_aac.so"  : "bass_aac.dll";
                bassPlugin2 = Un4seen.Bass.Bass.BASS_PluginLoad(aacPlugin);
                if (bassPlugin2 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    MessageBox.Show(StringResources.BassAacLoadFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                String opusPlugin = IsLinux ? "libbassopus.so" : "bassopus.dll";
                bassPlugin3 = Un4seen.Bass.Bass.BASS_PluginLoad(opusPlugin);
                if (bassPlugin3 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    MessageBox.Show(StringResources.BassOpusLoadFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(StringResources.BassInitFail, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                Application.Run(new MainForm());
            }
            catch (Ares.Ipc.ApplicationAlreadyStartedException)
            {
            }
            Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin1);
            Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin2);
            Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin3);
            Un4seen.Bass.Bass.BASS_Free();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            WriteExceptionTrace(ex);
        }

        static void WriteExceptionTrace(Exception ex)
        {
            try
            {
                String folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
                String path = System.IO.Path.Combine(folder, "Ares_Errors.log");
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, true))
                {
                    writer.WriteLine(System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    writer.WriteLine(ex.GetType().Name + ": " + ex.Message);
                    writer.WriteLine("Stack Trace:");
                    writer.WriteLine(ex.StackTrace);
                    writer.WriteLine("--------------------------------------------------");
                    writer.Flush();
                }
            }
            catch (Exception)
            {
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            WriteExceptionTrace(e.Exception);
            Dialogs.ExceptionDialog dialog = new Dialogs.ExceptionDialog();
            dialog.SetException(e.Exception);
            if (dialog.ShowDialog() == DialogResult.Abort)
            {
                Application.Exit();
            }
        }
    }
}
