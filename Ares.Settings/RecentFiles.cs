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
using System.Text;
using System.Xml;

namespace Ares.Settings
{
    [Serializable]
    public class RecentFiles
    {
        public RecentFiles() { }

        [Serializable]
        public class ProjectEntry
        {
            public string FilePath { get; set; }
            public string ProjectName { get; set; }

            public ProjectEntry(String path, String name)
            {
                FilePath = path;
                ProjectName = name;
            }
        }

        private List<ProjectEntry> m_Files = new List<ProjectEntry>();

        public void AddFile(ProjectEntry file)
        {
            m_Files.Insert(0, file);
            for (int i = 1; i < m_Files.Count; ++i)
            {
                if (m_Files[i].FilePath == file.FilePath)
                {
                    m_Files.RemoveAt(i);
                    return;
                }
            }
            const int MAX_FILE_COUNT = 4;
            if (m_Files.Count > MAX_FILE_COUNT)
            {
                m_Files.RemoveRange(MAX_FILE_COUNT, m_Files.Count - MAX_FILE_COUNT);
            }
        }

        public IList<ProjectEntry> GetFiles()
        {
            return m_Files;
        }

        public void WriteFiles(XmlWriter writer)
        {
            writer.WriteStartElement("RecentFiles");
            foreach (ProjectEntry file in m_Files)
            {
                writer.WriteStartElement("RecentFile");
                writer.WriteAttributeString("Name", file.ProjectName);
                writer.WriteAttributeString("Path", file.FilePath);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void ReadFiles(XmlReader reader)
        {
            m_Files.Clear();
            if (!reader.IsStartElement("RecentFiles"))
                return;
            reader.Read();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("RecentFile"))
                {
                    String name = reader.GetAttribute("Name");
                    String path = reader.GetAttribute("Path");
                    if (!String.IsNullOrEmpty(name) && !String.IsNullOrEmpty(path))
                    {
                        m_Files.Add(new ProjectEntry(path, name));
                    }
                    if (reader.IsEmptyElement)
                    {
                        reader.Read();
                    }
                    else
                    {
                        reader.Read();
                        reader.ReadInnerXml();
                        reader.ReadEndElement();
                    }
                }
            }
            reader.ReadEndElement();
        }
    }
}
