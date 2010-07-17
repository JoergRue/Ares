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
    public class Settings
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

        public static Settings Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = new Settings();
                return s_Instance;
            }
        }

        private static Settings s_Instance;

        private Settings()
        {
            InitDefaults();
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
            WindowLayout = null;
            RecentFiles = new RecentFiles();
            MusicDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            SoundDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            GlobalVolume = MusicVolume = SoundVolume = 100;
            TcpPort = 11112;
            UdpPort = 8009;
        }

        private void WriteSettings(XmlWriter writer)
        {
            writer.WriteStartElement("Settings");
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
            writer.WriteEndElement();
            RecentFiles.WriteFiles(writer);
            writer.WriteEndElement();
        }

        private void ReadSettings(XmlReader reader)
        {
            RecentFiles = new RecentFiles();
            if (!reader.IsStartElement("Settings"))
            {
                throw new XmlException("Expected Settings element");
            }
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
    }
}
