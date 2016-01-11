/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System.Text;

namespace Ares.Playing
{
    [Serializable]
    public class BassInitException : Exception
    {
        public BassInitException(String message)
            : base(message)
        {
        }

        public BassInitException(String message, Exception inner)
            : base(message, inner)
        {
        }

        private BassInitException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class BassInit : IDisposable
    {
        private int bassPlugin1 = 0;
        private int bassPlugin2 = 0;
        private int bassPlugin3 = 0;

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

		private bool LoadBassPlugin(String pluginName, String exePath, out int pluginHandle)
		{
			#if ANDROID
			Java.Lang.JavaSystem.LoadLibrary(pluginName);
			String libName = "lib" + pluginName + ".so";
			#else
			String libName = IsLinux ? exePath + "/lib" + pluginName + ".so" : pluginName + ".dll";
			if (IsLinux && !System.IO.File.Exists(libName))
			{
				pluginHandle = 0;
				return false;
			}
			#endif
			pluginHandle = Un4seen.Bass.Bass.BASS_PluginLoad(libName);
			return true;
		}

        public BassInit(int deviceIndex, Action<String> warningHandler)
        {
            try 
            {
                BassRegistration.Registration.RegisterBass();
#if !MONO
                if (!Un4seen.Bass.Bass.LoadMe())
                {
                    throw new BassInitException(StringResources.BassLoadFail);
                }
#endif
                if (!Un4seen.Bass.Bass.BASS_Init(deviceIndex, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    throw new BassInitException(MakeBassInitErrorMessage());
                }

                Un4seen.Bass.Bass.BASS_SetConfig(Un4seen.Bass.BASSConfig.BASS_CONFIG_NET_PLAYLIST, 1);

#if !MONO
                if (!Un4seen.Bass.AddOn.Fx.BassFx.LoadMe())
                {
                    throw new BassInitException(StringResources.BassFxLoadFail);
                }
#endif
				#if ANDROID
				Java.Lang.JavaSystem.LoadLibrary("bass_fx");
				#endif
				string exepath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
				LoadBassPlugin("bassflac", exepath, out bassPlugin1);
                if (bassPlugin1 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    warningHandler(StringResources.BassFlacLoadFail);
                }

				bool pluginExists = LoadBassPlugin("bass_aac", exepath, out bassPlugin2);
                if (pluginExists && bassPlugin2 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    warningHandler(StringResources.BassAacLoadFail);
                }
				LoadBassPlugin("bassopus", exepath, out bassPlugin3);
                if (bassPlugin3 == 0 && Un4seen.Bass.Bass.BASS_ErrorGetCode() != Un4seen.Bass.BASSError.BASS_ERROR_ALREADY)
                {
                    warningHandler(StringResources.BassOpusLoadFail);
                }
            }
            catch (Exception ex)
            {
                throw new BassInitException(String.Format(StringResources.BassInitFail,
                                              ex.Message + "(" + ex.GetType().FullName + ")",
                                              ex.StackTrace));
            }
        }

        public class OutputDevice
        {
            public int Index { get; set; }
            public String Name { get; set; }
        }

        public static List<OutputDevice> GetDevices()
        {
            var result = new List<OutputDevice>();
            result.Add(new OutputDevice() { Index = -1, Name = StringResources.Default });
            var infos = Un4seen.Bass.Bass.BASS_GetDeviceInfos();
            for (int i = 1; i < infos.Length; ++i)
            {
                result.Add(new OutputDevice() { Index = i, Name = infos[i].name });
            }
            return result;
        }

        public void SwitchDevice(int newDevice)
        {
            try
            { 
                Un4seen.Bass.Bass.BASS_Free();
                if (!Un4seen.Bass.Bass.BASS_Init(newDevice, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
                {
                    throw new BassInitException(MakeBassInitErrorMessage());
                }
            }
            catch (Exception ex)
            {
                Un4seen.Bass.Bass.BASS_Init(-1, 44100, Un4seen.Bass.BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                throw new BassInitException(String.Format(StringResources.BassInitFail,
                                              ex.Message + "(" + ex.GetType().FullName + ")",
                                              ex.StackTrace));
            }
        }

        public void Dispose()
        {
            if (bassPlugin1 != 0)
                Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin1);
            if (bassPlugin2 != 0)
                Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin2);
            if (bassPlugin3 != 0)
                Un4seen.Bass.Bass.BASS_PluginFree(bassPlugin3);
            Un4seen.Bass.Bass.BASS_Free();
        }
    }
}
