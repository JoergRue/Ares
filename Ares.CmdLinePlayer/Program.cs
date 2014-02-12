/*
 Copyright (c) 2014 [Joerg Ruedenauer]
 
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
 */using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.CmdLinePlayer
{
    class Program
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
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        static int Main(string[] args)
        {
            System.AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

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
			
            Console.WriteLine(String.Format(StringResources.AresPlayer, (new System.Reflection.AssemblyName(System.Reflection.Assembly.GetExecutingAssembly().FullName)).Version.ToString()));
            Console.WriteLine();

            int bassPlugin1, bassPlugin2, bassPlugin3;
            try
            {
                BassRegistration.Registration.RegisterBass();
#if !MONO
                if (!Un4seen.Bass.Bass.LoadMe())
                {
                    System.Console.Error.WriteLine(StringResources.BassLoadFail);
                    return 1;
                }
#endif
                if (!Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    System.Console.Error.WriteLine(MakeBassInitErrorMessage());
                    return 1;
                }
#if !MONO
                if (!Un4seen.Bass.AddOn.Fx.BassFx.LoadMe())
                {
                    System.Console.Error.WriteLine(StringResources.BassFxLoadFail);
                    return 1;
                }
#endif
                string exepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                String flacPlugin = IsLinux ? exepath + "/libbassflac.so" : "bassflac.dll";
                bassPlugin1 = Un4seen.Bass.Bass.BASS_PluginLoad(flacPlugin);
                if (bassPlugin1 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    System.Console.WriteLine(StringResources.BassFlacLoadFail);
                }

                String aacPlugin = IsLinux ? exepath + "/libbass_aac.so" : "bass_aac.dll";
                bassPlugin2 = Un4seen.Bass.Bass.BASS_PluginLoad(aacPlugin);
                if (bassPlugin2 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    System.Console.WriteLine(StringResources.BassAacLoadFail);
                }

                String opusPlugin = IsLinux ? exepath + "/libbassopus.so" : "bassopus.dll";
                bassPlugin3 = Un4seen.Bass.Bass.BASS_PluginLoad(opusPlugin);
                if (bassPlugin3 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    System.Console.WriteLine(StringResources.BassOpusLoadFail);
                }

            }
            catch (Exception ex)
            {
                System.Console.Error.WriteLine(String.Format(StringResources.BassInitFail,
                                              ex.Message + "(" + ex.GetType().FullName + ")",
                                              ex.StackTrace));
                return 1;
            }

            PlayerOptions options = new PlayerOptions();
            int res = 0;
            if (options.Parse(args))
            {
                Player player = new Player();
                res = player.Run(options);
            }
            else
            {
                res = 2;
            }

            Un4seen.Bass.Bass.BASS_Free();
            return res;
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
				Console.WriteLine("Unhandled Exception!");
                Console.WriteLine(ex.GetType() + ": " + ex.Message);
                Console.WriteLine(ex.StackTrace);
                WriteExceptionTrace(ex);
            }
			else
			{
				Console.WriteLine("Unhandled Exception!");
			}
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
    }
}
