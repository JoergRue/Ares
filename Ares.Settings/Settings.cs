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
                else
                {
                    reader.ReadOuterXml();
                }
            }
            reader.ReadEndElement();
        }
    }
}
