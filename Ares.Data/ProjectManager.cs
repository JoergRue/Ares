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
using System.Xml;

namespace Ares.Data
{
    /// <summary>
    /// Management of ARES projects.
    /// </summary>
    public interface IProjectManager
    {
        /// <summary>
        /// Creates a new project.
        /// </summary>
        IProject CreateProject(String title);

        /// <summary>
        /// Loads a project from a file.
        /// </summary>
        IProject LoadProject(String fileName);

        /// <summary>
        /// Saves a project. The file name must already be set.
        /// </summary>
        void SaveProject(IProject project);

        /// <summary>
        /// Saves a project to a file.
        /// </summary>
        void SaveProject(IProject project, String fileName);

        /// <summary>
        /// Unloads a project.
        /// </summary>
        void UnloadProject(IProject project);

        /// <summary>
        /// Exports something to an XML file
        /// </summary>
        /// <param name="element">The element</param>
        /// <param name="fileName">the file</param>
        void ExportElements(IList<IXmlWritable> elements, String fileName);

        /// <summary>
        /// Reads something from an XML file
        /// </summary>
        /// <param name="fileName">the file</param>
        /// <returns>the read element, or null</returns>
        IList<IXmlWritable> ImportElements(String fileName);
    }

    class ProjectManager : IProjectManager
    {
        #region IProjectManager Members

        public IProject CreateProject(String title)
        {
            return new Project(title);
        }

        public IProject LoadProject(String fileName)
        {
            IList<IXmlWritable> elements = DoImportElements(fileName, true);
            if (elements.Count > 0 && elements[0] is IProject)
            {
                return (IProject)elements[0];
            }
            else
            {
                throw new XmlException(String.Format(StringResources.ExpectedElement, "Project"));
            }
        }

        public void SaveProject(IProject project)
        {
            if (String.IsNullOrEmpty(project.FileName))
            {
                throw new ArgumentException(StringResources.FileNameMustBeSet);
            }
            DoSaveProject(project, project.FileName);
            project.Changed = false;
        }

        public void SaveProject(IProject project, String fileName)
        {
            DoSaveProject(project, fileName);
            project.FileName = fileName;
            project.Changed = false;
        }

        private void DoSaveProject(IProject project, String fileName)
        {
            List<IXmlWritable> elements = new List<IXmlWritable>();
            elements.Add(project);
            DoExportElements(elements, fileName, true);
        }

        public void UnloadProject(IProject project)
        {
            DataModule.TheElementRepository.Clear();
        }

        public void ExportElements(IList<IXmlWritable> elements, String fileName)
        {
            DoExportElements(elements, fileName, false);
        }

        private void DoExportElements(IList<IXmlWritable> elements, String fileName, bool isProject)
        {
            String tempFileName = System.IO.Path.GetTempFileName();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Indent = true;
            using (XmlWriter writer = XmlWriter.Create(tempFileName, settings))
            {
                writer.WriteStartDocument();
                if (!isProject)
                {
                    writer.WriteStartElement("ExportedElements");
                }
                foreach (IXmlWritable element in elements)
                {
                    element.WriteToXml(writer);
                    if (element is IBackgroundSoundChoice)
                    {
                        BackgroundSounds.WriteAdditionalData(writer, element as IBackgroundSoundChoice);
                    }
                }
                if (!isProject)
                {
                    writer.WriteEndElement();
                }
                writer.WriteEndDocument();
                writer.Flush();
            }
            System.IO.File.Copy(tempFileName, fileName, true);
            System.IO.File.Delete(tempFileName);
        }

        public IList<IXmlWritable> ImportElements(String fileName)
        {
            return DoImportElements(fileName, false);
        }

        private IList<IXmlWritable> DoImportElements(String fileName, bool isProject)
        {
            List<IXmlWritable> elements = new List<IXmlWritable>();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.ProhibitDtd = false;
            using (System.IO.FileStream stream = new System.IO.FileStream(fileName, System.IO.FileMode.Open))
            {
                using (XmlReader reader = XmlReader.Create(stream, settings))
                {
                    reader.Read();
                    reader.MoveToElement();
                    if (!reader.IsStartElement("ExportedElements") && !reader.IsStartElement("Project"))
                    {
                        XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "ExportedElements"), reader);
                    }
                    if (reader.IsEmptyElement)
                    {
                        reader.Read();
                    }
                    else
                    {
                        if (!isProject)
                        {
                            reader.Read(); // ExportedElements or Project
                            while (reader.IsStartElement())
                            {
                                if (reader.IsStartElement("Modes")) // in project files
                                {
                                    if (!reader.IsEmptyElement)
                                    {
                                        reader.Read();
                                        ReadElements(elements, reader, fileName);
                                        reader.ReadEndElement();
                                    }
                                    else
                                        reader.Read();
                                }
                                else
                                {
                                    ReadElements(elements, reader, fileName);
                                }
                            }
                            reader.ReadEndElement();
                        }
                        else
                        {
                            ReadElements(elements, reader, fileName);
                        }
                    }
                }
            }

            return elements;
        }

        private void ReadElements(IList<IXmlWritable> elements, XmlReader reader, String fileName)
        {
            while (reader.IsStartElement())
            {
                IXmlWritable element = ReadElement(reader, fileName);
                if (element != null)
                {
                    elements.Add(element);
                }
            }
        }

        private IXmlWritable ReadElement(XmlReader reader, String fileName)
        {
            if (reader.IsStartElement("Project"))
            {
                return new Project(reader, fileName);
            }
            else if (reader.IsStartElement("Mode"))
            {
                return new Mode(reader);
            }
            else if (reader.IsStartElement("ModeElement"))
            {
                return new ModeElement(reader);
            }
            else
            {
                return DataModule.TheElementFactory.CreateElement(reader);
            }
        }

        #endregion
    }
}
