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
using System.Xml;
using System.Text;

namespace Ares.Settings
{
    public class BasicSettings
    {
        public enum SettingsLocation
        {
            AppDataDir,
            AppDir,
            Custom
        }

        public SettingsLocation UserSettingsLocation { get; set; }

        public String CustomSettingsDirectory { get; set; }

        public BasicSettings()
        {
            UserSettingsLocation = SettingsLocation.AppDataDir;
            CustomSettingsDirectory = "";
        }

        private readonly String settingsFileName = "Ares.Editor.BasicSettings.xml";

        public void WriteToFile()
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
            String directory = String.Empty;
            // if settings shall be stored in app dir or if they shall be stored in a custom dir and storage in app dir is allowed,
            // store the basic settings in the app dir
            if (UserSettingsLocation == SettingsLocation.AppDir || (IsAppDirAllowed() && UserSettingsLocation == SettingsLocation.Custom))
            {
                directory = GetSettingsDir(SettingsLocation.AppDir);
            }
            // else store them in the app data dir
            else
            {
                directory = GetSettingsDir(SettingsLocation.AppDataDir);
                if (IsAppDirAllowed())
                {
                    // delete other file so it isn't used next time
                    String otherFileName = System.IO.Path.Combine(GetSettingsDir(SettingsLocation.AppDir), settingsFileName);
                    if (System.IO.File.Exists(otherFileName))
                    {
                        System.IO.File.Delete(otherFileName);
                    }
                }
            }
            String fileName = System.IO.Path.Combine(directory, settingsFileName);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            System.IO.File.Copy(tempFileName, fileName, true);
            System.IO.File.Delete(tempFileName);            
        }

		private static string AppPath 
		{ 
		    get 
		    { 
		        return System.IO.Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]); 
		     } 
		} 
		
		public static bool IsAppDirAllowed()
        {
            return !AppPath.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        }

        public bool ReadFromFile()
        {
            // first look in app dir, if that is allowed
            if (IsAppDirAllowed())
            {
                String fileName = System.IO.Path.Combine(GetSettingsDir(SettingsLocation.AppDir), settingsFileName);
                if (System.IO.File.Exists(fileName))
                {
                    return DoReadFromFile(fileName);
                }
            }
            // then look in app data dir
            String fileName2 = System.IO.Path.Combine(GetSettingsDir(SettingsLocation.AppDataDir), settingsFileName);
            if (System.IO.File.Exists(fileName2))
            {
                return DoReadFromFile(fileName2);
            }
            // else no basic settings file found
            return false;
        }

        public String GetSettingsDir()
        {
            return GetSettingsDir(UserSettingsLocation);
        }

        public String GetSettingsDir(SettingsLocation location)
        {
            String settingsDir = String.Empty;
            if (location == SettingsLocation.AppDataDir)
            {
                settingsDir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Ares");
            }
            else if (location == SettingsLocation.AppDir)
            {
                settingsDir = System.IO.Path.GetDirectoryName(AppPath);
            }
            else
            {
                settingsDir = CustomSettingsDirectory;
            }
            return settingsDir;
        }

        private bool DoReadFromFile(String fileName)
        {
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
                return false;
            }
            catch (System.IO.IOException)
            {
                return false;
            }
        }

        private void WriteSettings(XmlWriter writer)
        {
            writer.WriteStartElement("BasicSettings");
            writer.WriteElementString("SettingsLocation", Enum.GetName(typeof(SettingsLocation), UserSettingsLocation));
            writer.WriteElementString("CustomSettingsDirectory", CustomSettingsDirectory);
            writer.WriteEndElement();
        }

        private void ReadSettings(XmlReader reader)
        {
            if (!reader.IsStartElement("BasicSettings"))
            {
                throw new XmlException("Expected BasicSettings element");
            }
            reader.Read();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("SettingsLocation"))
                {
                    UserSettingsLocation = (SettingsLocation)Enum.Parse(typeof(SettingsLocation), reader.ReadElementString());
                }
                else if (reader.IsStartElement("CustomSettingsDirectory"))
                {
                    CustomSettingsDirectory = reader.ReadElementString();
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
