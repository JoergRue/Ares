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
using System.Text;

using System.Xml;

namespace Ares.Settings
{
    [Serializable]
    class SettingsData
    {
        public String MusicDirectory { get; set; }
        public String SoundDirectory { get; set; }

        public String WindowLayout { get; set; }

        public RecentFiles RecentFiles { get; set; }

        public int GlobalVolume { get; set; }
        public int MusicVolume { get; set; }
        public int SoundVolume { get; set; }

        public int UdpPort { get; set; }
        public int TcpPort { get; set; }
        public int WebTcpPort { get; set; }
        public bool UseLegacyNetwork { get; set; }
        public bool UseWebNetwork { get; set; }

        public String IPAddress { get; set; }

        public int Version { get; set; }

        public bool CheckForUpdate { get; set; }

        public String SoundFileEditor { get; set; }

        public String ExternalMusicPlayer { get; set; }

        public int MessageFilterLevel { get; set; }

        public bool UseStreaming { get; set; }

        public String StreamingServerAddress { get; set; }

        public int StreamingServerPort { get; set; }

        public String StreamingPassword { get; set; }

        public int StreamingEncoder { get; set; }

        public String StreamingStreamName { get; set; }

        public int StreamingBitrate { get; set; }

        public String StreamingUserName { get; set; }

        public bool ShowKeysInButtons { get; set; }
        public bool GlobalKeyHook { get; set; }

        public String OnlineDBUserId { get; set; }
        public bool ShowDialogAfterUpload { get; set; }
        public bool ShowDialogAfterDownload { get; set; }

        public int TagMusicFadeTime { get; set; }
        public bool TagMusicFadeOnlyOnChange { get; set; }
        public bool PlayMusicOnAllSpeakers { get; set; }

        public int ButtonMusicFadeMode { get; set; }
        public int ButtonMusicFadeTime { get; set; }

        public String LocalPlayerPath { get; set; } // for controllers

        public bool ShowTipOfTheDay { get; set; }
        public int LastTipOfTheDay { get; set; }

        public int OutputDeviceIndex { get; set; }

		#if ANDROID
		public String ProjectDirectory {get;set;}
		#endif
    }

    public class Settings
    {
        private SettingsData Data { get; set; }

		#if !ANDROID
        public String MusicDirectory { get { return Data.MusicDirectory; } set { Data.MusicDirectory = value; } }
        public String SoundDirectory { get { return Data.SoundDirectory; } set { Data.SoundDirectory = value; } }
		#else
		public String MusicDirectory { get { return Data.MusicDirectory; } private set { Data.MusicDirectory = value; } }
		public String SoundDirectory { get { return Data.SoundDirectory; } private set { Data.SoundDirectory = value; } }
		#endif

        public String WindowLayout { get { return Data.WindowLayout; } set { Data.WindowLayout = value; } }

        public RecentFiles RecentFiles { get { return Data.RecentFiles; } set { Data.RecentFiles = value; } }

        public int GlobalVolume { get { return Data.GlobalVolume; } set { Data.GlobalVolume = value; } }
        public int MusicVolume { get { return Data.MusicVolume; } set { Data.MusicVolume = value; } }
        public int SoundVolume { get { return Data.SoundVolume; } set { Data.SoundVolume = value; } }

        public int UdpPort { get { return Data.UdpPort; } set { Data.UdpPort = value; } }
        public int TcpPort { get { return Data.TcpPort; } set { Data.TcpPort = value; } }
        public int WebTcpPort { get { return Data.WebTcpPort; } set { Data.WebTcpPort = value; } }
        public bool UseLegacyNetwork { get { return Data.UseLegacyNetwork; } set { Data.UseLegacyNetwork = value; } }
        public bool UseWebNetwork { get { return Data.UseWebNetwork; } set { Data.UseWebNetwork = value; } }

        public String IPAddress { get { return Data.IPAddress; } set { Data.IPAddress = value; } }

        public bool CheckForUpdate { get { return Data.CheckForUpdate; } set { Data.CheckForUpdate = value; } }

        public int Version { get { return Data.Version; } set { Data.Version = value; } }

        public String SoundFileEditor { get { return Data.SoundFileEditor; } set { Data.SoundFileEditor = value; } }

        public String ExternalMusicPlayer { get { return Data.ExternalMusicPlayer; } set { Data.ExternalMusicPlayer = value; } }

        public int MessageFilterLevel { get { return Data.MessageFilterLevel; } set { Data.MessageFilterLevel = value; } }

        public bool UseStreaming { get { return Data.UseStreaming; } set { Data.UseStreaming = value; } }

        public String StreamingServerAddress { get { return Data.StreamingServerAddress; } set { Data.StreamingServerAddress = value; } }

        public int StreamingServerPort { get { return Data.StreamingServerPort; } set { Data.StreamingServerPort = value; } }

        public String StreamingPassword { get { return Data.StreamingPassword; } set { Data.StreamingPassword = value; } }

        public int StreamingEncoder { get { return Data.StreamingEncoder; } set { Data.StreamingEncoder = value; } }

        public String StreamingStreamName { get { return Data.StreamingStreamName; } set { Data.StreamingStreamName = value; } }

        public int StreamingBitrate { get { return Data.StreamingBitrate; } set { Data.StreamingBitrate = value; } }

        public String StreamingUserName { get { return Data.StreamingUserName; } set { Data.StreamingUserName = value; } }

        public bool ShowKeysInButtons { get { return Data.ShowKeysInButtons; } set { Data.ShowKeysInButtons = value; } }

        public bool GlobalKeyHook { get { return Data.GlobalKeyHook; } set { Data.GlobalKeyHook = value; } }

        public String OnlineDBUserId { get { return Data.OnlineDBUserId; } set { Data.OnlineDBUserId = value; } }
        public bool ShowDialogAfterUpload { get { return Data.ShowDialogAfterUpload; }  set { Data.ShowDialogAfterUpload = value; } }
        public bool ShowDialogAfterDownload { get { return Data.ShowDialogAfterDownload; } set { Data.ShowDialogAfterDownload = value; } }

        public int TagMusicFadeTime { get { return Data.TagMusicFadeTime; } set { Data.TagMusicFadeTime = value; } }
        public bool TagMusicFadeOnlyOnChange { get { return Data.TagMusicFadeOnlyOnChange; } set { Data.TagMusicFadeOnlyOnChange = value; } }
        public bool PlayMusicOnAllSpeakers { get { return Data.PlayMusicOnAllSpeakers; } set { Data.PlayMusicOnAllSpeakers = value; } }

        public int ButtonMusicFadeMode { get { return Data.ButtonMusicFadeMode; } set { Data.ButtonMusicFadeMode = value; } }
        public int ButtonMusicFadeTime { get { return Data.ButtonMusicFadeTime; } set { Data.ButtonMusicFadeTime = value; } }

        public String LocalPlayerPath { get { return Data.LocalPlayerPath; } set { Data.LocalPlayerPath = value; } }

        public bool ShowTipOfTheDay { get { return Data.ShowTipOfTheDay; } set { Data.ShowTipOfTheDay = value; } }
        public int LastTipOfTheDay { get { return Data.LastTipOfTheDay; } set { Data.LastTipOfTheDay = value; } }

        public int OutputDeviceIndex { get { return Data.OutputDeviceIndex; } set { Data.OutputDeviceIndex = value; } }

		#if ANDROID
		public String ProjectDirectory { get { return Data.ProjectDirectory; } private set { Data.ProjectDirectory = value; } }

		private IFolder mMusicFolder;
		private IFolder mSoundFolder;
		private IFolder mProjectFolder;

		public IFolder MusicFolder {
			get { return mMusicFolder; }
			set {
				mMusicFolder = value;
				MusicDirectory = value.IOName;
			}
		}
		public IFolder SoundFolder {
			get { return mSoundFolder; }
			set {
				mSoundFolder = value;
				SoundDirectory = value.IOName;
			}
		}
		public IFolder ProjectFolder {
			get { return mProjectFolder; }
			set {
				mProjectFolder = value;
				ProjectDirectory = value.IOName;
			}
		}
		#endif

        public static Settings Instance
        {
            get
            {
                lock (syncObject)
                {
                    if (s_Instance == null)
                        s_Instance = new Settings();
                    return s_Instance;
                }
            }
        }

        private static Settings s_Instance;

        public class SettingsEventArgs : EventArgs
        {
            public SettingsEventArgs(bool fundamentalChange)
            {
                FundamentalChange = fundamentalChange;
            }

            public bool FundamentalChange { get; set; }
        }
		
#if !MONO
        public event EventHandler<SettingsEventArgs> SettingsChanged;
#endif

        public static readonly string PlayerID = "Player";
        public static readonly string EditorID = "Editor";

		#if !ANDROID
        public bool InitializeWithoutSharedMemory(String directory)
        {
            bool success = String.IsNullOrEmpty(directory) ? false : ReadFromFile(directory);
            if (!success)
            {
                InitDefaults();
            }
            return success;
        }
		#endif

		#if !ANDROID
        public bool Initialize(String id, String directory)
		#else
		public bool Initialize(Android.Content.Context context)
		#endif
        {
			#if !ANDROID
            m_ID = id;
			#endif
#if MONO
			bool success = false;
#else
            bool success = ReadFromSharedMemory();
#endif
			#if !ANDROID
            if (!success && !String.IsNullOrEmpty(directory))
            {
                success = ReadFromFile(directory);
            }
			if (!success)
			{
			InitDefaults();
			}
			#else
			InitDefaults();
			success = Read(context);
			#endif
            bool hasIP = !String.IsNullOrEmpty(IPAddress);
            if (hasIP)
            {
                try
                {
                    System.Net.IPAddress addr = System.Net.IPAddress.Parse(IPAddress);
                }
                catch (Exception)
                {
                    hasIP = false;
                }
            }
            if (!hasIP)
            {
                var addresses = System.Net.Dns.GetHostAddresses(String.Empty);
                if (addresses.Length > 0)
                {
                    IPAddress = addresses[0].ToString();
                    hasIP = true;
                }
            }
            if (!hasIP)
            {
                UseLegacyNetwork = false;
                UseWebNetwork = false;
            }
#if !MONO
            bool createdOwn;
            bool createdOther;
            m_OwnWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "AresSettingsEvent" + m_ID, out createdOwn);
            m_OtherWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "AresSettingsEvent" + (m_ID == PlayerID ? EditorID : PlayerID), out createdOther);
            m_SharedMemoryThread = new System.Threading.Thread(ListenForSMChanges);
            m_ContinueSMThread = true;
            m_SharedMemoryThread.Start();
#endif
            return success;
        }

        public void Shutdown()
        {
#if !MONO
            m_OwnWaitHandle.Close();
            m_OtherWaitHandle.Close();
            if (m_SharedMemory != null)
            {
                m_SharedMemory.Lock();
                m_SharedMemory.Unlock();
                m_SharedMemory.Dispose();
            }
            m_ContinueSMThread = false;
            m_SharedMemoryThread.Join();
#endif
        }

        public void Commit()
        {
#if !MONO
            WriteToSharedMemory();
#endif
        }

        private Settings()
        {
            Data = new SettingsData();
            InitDefaults();
        }
		
#if !MONO
        private void ListenForSMChanges()
        {
            bool continueThread = true;
            while (continueThread)
            {
                if (m_OtherWaitHandle.WaitOne(100))
                {
                    ReadFromSharedMemory();
                    m_OtherWaitHandle.Reset();
                }
                lock (syncObject)
                {
                    continueThread = m_ContinueSMThread;
                }
            }
        }
#endif

#if !ANDROID
		public readonly String settingsFileName = "Ares.Editor.Settings.xml";

        public void WriteToFile(String directory)
        {
            String tempFileName = System.IO.Path.GetTempFileName();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(tempFileName, settings))
            {
                writer.WriteStartDocument();
                WriteSettings(writer);
                writer.WriteEndDocument();
                writer.Flush();
            }
            String fileName = System.IO.Path.Combine(directory, settingsFileName);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            System.IO.File.Copy(tempFileName, fileName, true);
            System.IO.File.Delete(tempFileName);            
        }

        public bool ReadFromFile(String directory)
        {
            String fileName = System.IO.Path.Combine(directory, settingsFileName);
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreComments = true;
#if !MEDIAPORTAL
                settings.DtdProcessing = DtdProcessing.Ignore;
#endif
                using (System.IO.FileStream stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
                {
                    using (XmlReader reader = XmlReader.Create(stream, settings))
                    {
                        reader.Read();
                        reader.MoveToElement();
                        ReadSettings(reader);                       
                    }
                }
                return true;
            }
            catch (System.Xml.XmlException)
            {
                InitDefaults();
                return false;
            }
            catch (System.IO.IOException)
            {
                InitDefaults();
                return false;
            }
        }
#endif

        private void InitDefaults()
        {
            Version = 1;
            WindowLayout = null;
            RecentFiles = new RecentFiles();
			#if !ANDROID
            MusicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            SoundDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic), "Sounds");
			#else
			MusicFolder = GetDefaultMusicDirectory();
			SoundFolder = GetDefaultSoundDirectory();
			#endif
            GlobalVolume = MusicVolume = SoundVolume = 100;
            TcpPort = 11112;
            WebTcpPort = 11113;
            UseLegacyNetwork = true;
			#if !ANDROID
            UseWebNetwork = true;
			#else
			UseWebNetwork = false;
			#endif
            UdpPort = 8009;
            IPAddress = String.Empty;
			#if !ANDROID
            CheckForUpdate = true;
			#else
			CheckForUpdate = false;
			#endif
            SoundFileEditor = String.Empty;
            ExternalMusicPlayer = String.Empty;
            MessageFilterLevel = 2; // warning
            UseStreaming = false;
            StreamingServerAddress = "localhost";
            StreamingServerPort = 8000;
            StreamingPassword = "hackme";
            StreamingEncoder = 1;
            StreamingStreamName = "Ares";
            StreamingBitrate = 128;
            StreamingUserName = String.Empty;
            ShowKeysInButtons = false;
            GlobalKeyHook = false;
            OnlineDBUserId = String.Empty;
            ShowDialogAfterDownload = true;
            ShowDialogAfterUpload = true;
            TagMusicFadeTime = 0;
            TagMusicFadeOnlyOnChange = false;
            PlayMusicOnAllSpeakers = false;
            ButtonMusicFadeMode = 0;
            ButtonMusicFadeTime = 0;
            LocalPlayerPath = String.Empty;
            ShowTipOfTheDay = true;
            LastTipOfTheDay = -1;
            OutputDeviceIndex = -1;
			#if ANDROID
			ProjectFolder = GetDefaultProjectDirectory();
			#endif
        }

		#if !ANDROID
        private void WriteSettings(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
            writer.WriteAttributeString("Version", 1.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteElementString("MusicDirectory", MusicDirectory);
            writer.WriteElementString("SoundsDirectory", SoundDirectory);
            writer.WriteStartElement("WindowLayout");
            writer.WriteRaw(WindowLayout);
            writer.WriteEndElement();
            writer.WriteStartElement("Volumes");
            writer.WriteAttributeString("Overall", GlobalVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Music", MusicVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Sound", SoundVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteStartElement("Network");
            writer.WriteAttributeString("TcpPort", TcpPort.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("UdpPort", UdpPort.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("IPAddress", IPAddress);
            writer.WriteAttributeString("CheckForUpdate", CheckForUpdate ? "true" : "false");
            writer.WriteAttributeString("Enabled", UseLegacyNetwork ? "true" : "false");
            writer.WriteAttributeString("WebTcpPort", WebTcpPort.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("UseWebNetwork", UseWebNetwork ? "true" : "false");
            writer.WriteEndElement();
            RecentFiles.WriteFiles(writer);
            writer.WriteStartElement("Tools");
            writer.WriteElementString("SoundFileEditor", SoundFileEditor);
            writer.WriteElementString("ExternalMusicPlayer", ExternalMusicPlayer);
            writer.WriteEndElement();
            writer.WriteStartElement("Options");
            writer.WriteAttributeString("MessageFilterLevel", MessageFilterLevel.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ShowKeys", ShowKeysInButtons ? "true" : "false");
            writer.WriteAttributeString("GlobalKeyHook", GlobalKeyHook ? "true" : "false");
            writer.WriteAttributeString("OutputDevice", OutputDeviceIndex.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteStartElement("Streaming");
            writer.WriteAttributeString("Active", UseStreaming ? "true" : "false");
            writer.WriteAttributeString("Address", StreamingServerAddress);
            writer.WriteAttributeString("Port", StreamingServerPort.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Password", StreamingPassword);
            writer.WriteAttributeString("Encoding", StreamingEncoder.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("StreamName", StreamingStreamName);
            writer.WriteAttributeString("Bitrate", StreamingBitrate.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("UserName", StreamingUserName);
            writer.WriteEndElement();
            writer.WriteStartElement("OnlineDB");
            writer.WriteAttributeString("UserId", OnlineDBUserId);
            writer.WriteAttributeString("DialogAfterDownload", ShowDialogAfterDownload ? "true" : "false");
            writer.WriteAttributeString("DialogAfterUpload", ShowDialogAfterUpload ? "true" : "false");
            writer.WriteEndElement();
            writer.WriteStartElement("TagMusicFading");
            writer.WriteAttributeString("FadeTime", TagMusicFadeTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("FadeOnlyOnChange", TagMusicFadeOnlyOnChange ? "true" : "false");
            writer.WriteEndElement();
            writer.WriteStartElement("Music");
            writer.WriteAttributeString("PlayOnAllSpeakers", PlayMusicOnAllSpeakers ? "true" : "false");
            writer.WriteAttributeString("ButtonFadeMode", ButtonMusicFadeMode.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("ButtonFadeTime", ButtonMusicFadeTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteStartElement("Controllers");
            writer.WriteAttributeString("LocalPlayerPath", LocalPlayerPath);
            writer.WriteEndElement();
            writer.WriteStartElement("TipOfTheDay");
            writer.WriteAttributeString("ShowTip", ShowTipOfTheDay ? "true" : "false");
            writer.WriteAttributeString("LastTip", LastTipOfTheDay.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void ReadSettings(XmlReader reader)
        {
            RecentFiles = new RecentFiles();
            if (!reader.IsStartElement("Settings"))
            {
                throw new XmlException("Expected Settings element");
            }
            Version = reader.GetIntegerAttributeOrDefault("Version", 0);
            reader.Read();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("MusicDirectory"))
                {
                    MusicDirectory = reader.ReadElementString();
                }
                else if (reader.IsStartElement("SoundsDirectory"))
                {
                    SoundDirectory = reader.ReadElementString();
                }
                else if (reader.IsStartElement("WindowLayout"))
                {
                    if (!reader.IsEmptyElement)
                    {
                        WindowLayout = reader.ReadInnerXml();
                    }
                    else
                    {
                        reader.Read();
                    }
                }
                else if (reader.IsStartElement("RecentFiles"))
                {
                    RecentFiles.ReadFiles(reader);
                }
                else if (reader.IsStartElement("Volumes"))
                {
                    GlobalVolume = reader.GetIntegerAttribute("Overall");
                    MusicVolume = reader.GetIntegerAttribute("Music");
                    SoundVolume = reader.GetIntegerAttribute("Sound");
                    if (GlobalVolume > 100) GlobalVolume = 100;
                    if (GlobalVolume < 0) GlobalVolume = 0;
                    if (MusicVolume > 100) MusicVolume = 100;
                    if (MusicVolume < 0) MusicVolume = 0;
                    if (SoundVolume > 100) SoundVolume = 100;
                    if (SoundVolume < 0) SoundVolume = 0;
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("Network"))
                {
                    UdpPort = reader.GetIntegerAttribute("UdpPort");
                    TcpPort = reader.GetIntegerAttribute("TcpPort");
                    WebTcpPort = reader.GetIntegerAttributeOrDefault("WebTcpPort", TcpPort + 1);
                    IPAddress = reader.GetAttribute("IPAddress");
                    CheckForUpdate = reader.GetBooleanAttributeOrDefault("CheckForUpdate", true);
                    UseLegacyNetwork = reader.GetBooleanAttributeOrDefault("Enabled", true);
                    UseWebNetwork = reader.GetBooleanAttributeOrDefault("UseWebNetwork", true);
                    if (IPAddress == null) IPAddress = String.Empty;
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("Tools"))
                {
                    reader.Read();
                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement("SoundFileEditor"))
                        {
                            SoundFileEditor = reader.ReadElementString();
                        }
                        else if (reader.IsStartElement("ExternalMusicPlayer"))
                        {
                            ExternalMusicPlayer = reader.ReadElementString();
                        }
                        else
                        {
                            reader.ReadOuterXml();
                        }
                    }
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("Options"))
                {
                    MessageFilterLevel = reader.GetIntegerAttribute("MessageFilterLevel");
                    ShowKeysInButtons = reader.GetBooleanAttributeOrDefault("ShowKeys", false);
                    GlobalKeyHook = reader.GetBooleanAttributeOrDefault("GlobalKeyHook", false);
                    OutputDeviceIndex = reader.GetIntegerAttributeOrDefault("OutputDevice", -1);
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("Streaming"))
                {
                    UseStreaming = reader.GetBooleanAttribute("Active");
                    StreamingServerAddress = reader.GetAttribute("Address");
                    StreamingServerPort = reader.GetIntegerAttribute("Port");
                    StreamingPassword = reader.GetAttribute("Password");
                    StreamingEncoder = reader.GetIntegerAttribute("Encoding");
                    StreamingStreamName = reader.GetAttribute("StreamName");
                    if (String.IsNullOrEmpty(StreamingStreamName))
                        StreamingStreamName = "Ares";
                    StreamingBitrate = reader.GetIntegerAttributeOrDefault("Bitrate", 128);
                    StreamingUserName = reader.GetAttribute("UserName");
                    if (StreamingUserName == null)
                        StreamingUserName = String.Empty;
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("OnlineDB"))
                {
                    OnlineDBUserId = reader.GetAttribute("UserId");
                    ShowDialogAfterUpload = reader.GetBooleanAttributeOrDefault("DialogAfterUpload", true);
                    ShowDialogAfterDownload = reader.GetBooleanAttributeOrDefault("DialogAfterDownload", true);
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("TagMusicFading"))
                {
                    TagMusicFadeTime = reader.GetIntegerAttributeOrDefault("FadeTime", 0);
                    TagMusicFadeOnlyOnChange = reader.GetBooleanAttributeOrDefault("FadeOnlyOnChange", false);
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("Music"))
                {
                    PlayMusicOnAllSpeakers = reader.GetBooleanAttributeOrDefault("PlayOnAllSpeakers", false);
                    ButtonMusicFadeMode = reader.GetIntegerAttributeOrDefault("ButtonFadeMode", 0);
                    ButtonMusicFadeTime = reader.GetIntegerAttributeOrDefault("ButtonFadeTime", 0);
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("Controllers"))
                {
                    LocalPlayerPath = reader.GetAttribute("LocalPlayerPath");
                    if (LocalPlayerPath == null) LocalPlayerPath = String.Empty;
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("TipOfTheDay"))
                {
                    ShowTipOfTheDay = reader.GetBooleanAttributeOrDefault("ShowTip", true);
                    LastTipOfTheDay = reader.GetIntegerAttributeOrDefault("LastTip", 0);
                    if (reader.IsEmptyElement)
                        reader.Read();
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
                else
                {
                    reader.ReadOuterXml();
                }
            }
            reader.ReadEndElement();
        }
		#endif

        private bool CompareForFundamentalChange(SettingsData other)
        {
            if (other.SoundDirectory != this.SoundDirectory)
                return true;
            if (other.MusicDirectory != this.MusicDirectory)
                return true;
            if (other.TcpPort != this.TcpPort)
                return true;
            if (other.WebTcpPort != this.WebTcpPort)
                return true;
            if (other.UseLegacyNetwork != this.UseLegacyNetwork)
                return true;
            if (other.UseWebNetwork != this.UseWebNetwork)
                return true;
            if (other.UdpPort != this.UdpPort)
                return true;
            if (other.OutputDeviceIndex != this.OutputDeviceIndex)
                return true;
            return false;
        }
		
#if !MONO
        private bool ReadFromSharedMemory()
        {
            try
            {
                String id = "AresSettingsMemory" + (m_ID == PlayerID ? EditorID : PlayerID);
                using (Ares.Ipc.SharedMemory mem = new Ipc.SharedMemory(id))
                {
                    SettingsData s = null;
                    mem.Lock();
                    try
                    {
                        s = (SettingsData)mem.GetObject();
                    }
                    finally
                    {
                        mem.Unlock();
                    }
                    bool fundamentalChange = CompareForFundamentalChange(s);
                    lock (syncObject)
                    {
                        Data = s;
                    }
                    if (SettingsChanged != null)
                    {
                        SettingsChanged(this, new SettingsEventArgs(fundamentalChange));
                    }
                    return true;
                }
            }
            catch (Ipc.SharedMemoryException)
            {
                return false;
            }
        }

        private void WriteToSharedMemory()
        {
            try
            {
                if (m_SharedMemory == null)
                {
                    m_SharedMemory = new Ipc.SharedMemory("AresSettingsMemory" + m_ID, Data);
                }
                else
                {
                    m_SharedMemory.Lock();
                    try
                    {
                        m_SharedMemory.AddObject(Data, true);
                    }
                    finally
                    {
                        m_SharedMemory.Unlock();
                    }
                }
                m_OwnWaitHandle.Set();
            }
            catch (Ares.Ipc.SharedMemoryException)
            {
            }
        }

        private Ares.Ipc.SharedMemory m_SharedMemory;

        private System.Threading.EventWaitHandle m_OwnWaitHandle;

        private System.Threading.EventWaitHandle m_OtherWaitHandle;

        private System.Threading.Thread m_SharedMemoryThread;

        private bool m_ContinueSMThread;
#endif

		#if !ANDROID
        private String m_ID;
		#endif

#if ANDROID
		public void Write(Android.Content.Context context)
		{
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(context);
			var editor = prefs.Edit();
			editor.PutInt("version", Version);
			editor.PutInt("messageFilterLevel", MessageFilterLevel);
			editor.PutString("musicFolder", MusicFolder.Serialize());
			editor.PutString("soundFolder", SoundFolder.Serialize());
			editor.PutString("projectFolder", ProjectFolder.Serialize());
			editor.PutInt("overallVolume", GlobalVolume);
			editor.PutInt("musicVolume", MusicVolume);
			editor.PutInt("soundVolume", SoundVolume);
			editor.PutInt("udpPort", UdpPort);
			editor.PutInt("tcpPort", TcpPort);
			editor.PutString("lastProject", RecentFiles.GetFiles().Count > 0 ? RecentFiles.GetFiles()[0].FilePath : String.Empty);
			editor.PutInt("tagMusicFadeTime", TagMusicFadeTime);
			editor.PutBoolean("tagMusicFadeOnlyOnChange", TagMusicFadeOnlyOnChange);
			editor.PutInt("buttonMusicFadeMode", ButtonMusicFadeMode);
			editor.PutInt("buttonMusicFadeTime", ButtonMusicFadeTime);
			editor.Apply();
		}
	
		public void WriteMessageLevel(Android.Content.Context context)
		{
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(context);
			var editor = prefs.Edit();
			editor.PutInt("messageFilterLevel", MessageFilterLevel);
			editor.Apply();
		}

		public bool Read(Android.Content.Context context)
		{
			var prefs = Android.Preferences.PreferenceManager.GetDefaultSharedPreferences(context);
			bool hasPrefs = prefs.GetInt("version", 0) > 0;
			MessageFilterLevel = prefs.GetInt("messageFilterLevel", 2);
			String musicFolder = prefs.GetString("musicFolder", GetDefaultMusicDirectory().Serialize());
			MusicFolder = FolderFactory.CreateFromSerialization(musicFolder);
			String soundFolder = prefs.GetString("soundFolder", GetDefaultSoundDirectory().Serialize());
			SoundFolder = FolderFactory.CreateFromSerialization(soundFolder);
			String projectFolder = prefs.GetString("projectFolder", GetDefaultProjectDirectory().Serialize());
			ProjectFolder = FolderFactory.CreateFromSerialization(projectFolder);
			GlobalVolume = prefs.GetInt("overallVolume", 100);
			MusicVolume = prefs.GetInt("musicVolume", 100);
			SoundVolume = prefs.GetInt("soundVolume", 100);
			UdpPort = prefs.GetInt("udpPort", 8009);
			TcpPort = prefs.GetInt("tcpPort", 11112);
			String lastProject = prefs.GetString("lastProject", String.Empty);
			if (!String.IsNullOrEmpty(lastProject))
				RecentFiles.AddFile(new RecentFiles.ProjectEntry(lastProject, "Project0"));
			TagMusicFadeTime = prefs.GetInt("tagMusicFadeTime", 0);
			TagMusicFadeOnlyOnChange = prefs.GetBoolean("tagMusicFadeOnlyOnChange", false);
			ButtonMusicFadeMode = prefs.GetInt("buttonMusicFadeMode", 0);
			ButtonMusicFadeTime = prefs.GetInt("buttonMusicFadeTime", 0);
			return hasPrefs;
		}

		public static IFolder GetDefaultMusicDirectory()
		{
			return FolderFactory.CreateFileSystemFolder(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Java.IO.File.Separator + "Ares" + Java.IO.File.Separator + "Music");
		}

		public static IFolder GetDefaultSoundDirectory()
		{
			return FolderFactory.CreateFileSystemFolder(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Java.IO.File.Separator + "Ares" + Java.IO.File.Separator + "Sounds");
		}

		public static IFolder GetDefaultProjectDirectory()
		{
			return FolderFactory.CreateFileSystemFolder(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + Java.IO.File.Separator + "Ares" + Java.IO.File.Separator + "Projects");
		}
#endif

        private static Object syncObject = new Int16();
    }
}
