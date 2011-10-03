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

        public String IPAddress { get; set; }

        public int Version { get; set; }

        public bool CheckForUpdate { get; set; }

        public String SoundFileEditor { get; set; }

        public int MessageFilterLevel { get; set; }

    }

    public class Settings
    {
        private SettingsData Data { get; set; }

        public String MusicDirectory { get { return Data.MusicDirectory; } set { Data.MusicDirectory = value; } }
        public String SoundDirectory { get { return Data.SoundDirectory; } set { Data.SoundDirectory = value; } }

        public String WindowLayout { get { return Data.WindowLayout; } set { Data.WindowLayout = value; } }

        public RecentFiles RecentFiles { get { return Data.RecentFiles; } set { Data.RecentFiles = value; } }

        public int GlobalVolume { get { return Data.GlobalVolume; } set { Data.GlobalVolume = value; } }
        public int MusicVolume { get { return Data.MusicVolume; } set { Data.MusicVolume = value; } }
        public int SoundVolume { get { return Data.SoundVolume; } set { Data.SoundVolume = value; } }

        public int UdpPort { get { return Data.UdpPort; } set { Data.UdpPort = value; } }
        public int TcpPort { get { return Data.TcpPort; } set { Data.TcpPort = value; } }

        public String IPAddress { get { return Data.IPAddress; } set { Data.IPAddress = value; } }

        public bool CheckForUpdate { get { return Data.CheckForUpdate; } set { Data.CheckForUpdate = value; } }

        public int Version { get { return Data.Version; } set { Data.Version = value; } }

        public String SoundFileEditor { get { return Data.SoundFileEditor; } set { Data.SoundFileEditor = value; } }

        public int MessageFilterLevel { get { return Data.MessageFilterLevel; } set { Data.MessageFilterLevel = value; } }

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

        public event EventHandler<SettingsEventArgs> SettingsChanged;

        public static readonly string PlayerID = "Player";
        public static readonly string EditorID = "Editor";

        public bool Initialize(String id, String directory)
        {
            m_ID = id;
            bool success = ReadFromSharedMemory();
            if (!success && !String.IsNullOrEmpty(directory))
            {
                success = ReadFromFile(directory);
            }
            if (!success)
            {
                InitDefaults();
            }
            bool createdOwn;
            bool createdOther;
            m_OwnWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "AresSettingsEvent" + m_ID, out createdOwn);
            m_OtherWaitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset, "AresSettingsEvent" + (m_ID == PlayerID ? EditorID : PlayerID), out createdOther);
            m_SharedMemoryThread = new System.Threading.Thread(ListenForSMChanges);
            m_ContinueSMThread = true;
            m_SharedMemoryThread.Start();
            return success;
        }

        public void Shutdown()
        {
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
        }

        public void Commit()
        {
            WriteToSharedMemory();
        }

        private Settings()
        {
            Data = new SettingsData();
            InitDefaults();
        }

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
                settings.ProhibitDtd = false;
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

        private void InitDefaults()
        {
            Version = 1;
            WindowLayout = null;
            RecentFiles = new RecentFiles();
            MusicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            SoundDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            GlobalVolume = MusicVolume = SoundVolume = 100;
            TcpPort = 11112;
            UdpPort = 8009;
            IPAddress = String.Empty;
            CheckForUpdate = true;
            SoundFileEditor = String.Empty;
            MessageFilterLevel = 2; // warning
        }

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
            writer.WriteEndElement();
            RecentFiles.WriteFiles(writer);
            writer.WriteStartElement("Tools");
            writer.WriteElementString("SoundFileEditor", SoundFileEditor);
            writer.WriteEndElement();
            writer.WriteStartElement("Options");
            writer.WriteAttributeString("MessageFilterLevel", MessageFilterLevel.ToString(System.Globalization.CultureInfo.InvariantCulture));
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
                    IPAddress = reader.GetAttribute("IPAddress");
                    CheckForUpdate = reader.GetBooleanAttributeOrDefault("CheckForUpdate", true);
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

        private bool CompareForFundamentalChange(SettingsData other)
        {
            if (other.SoundDirectory != this.SoundDirectory)
                return true;
            if (other.MusicDirectory != this.MusicDirectory)
                return true;
            if (other.TcpPort != this.TcpPort)
                return true;
            return false;
        }

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

        private String m_ID;

        private static Object syncObject = new Int16();
    }
}
