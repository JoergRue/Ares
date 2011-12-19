/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
using Ares.Data;

namespace Ares.ModelInfo
{
    public class FileLists : IElementVisitor
    {
        private Dictionary<String, IFileElement> m_Files = new Dictionary<string, IFileElement>();

        public IList<IFileElement> GetAllFiles(Object parents)
        {
            IProject project = parents as IProject;
            if (project != null)
            {
                return GetAllFiles(project);
            }
            IList<IXmlWritable> roots = parents as IList<IXmlWritable>;
            if (roots != null)
            {
                return GetAllFiles(roots);
            }
            return new List<IFileElement>();
        }

        public IList<IFileElement> GetAllFiles(IProject project)
        {
            foreach (IMode mode in project.GetModes())
            {
                foreach (IModeElement element in mode.GetElements())
                {
                    element.Visit(this);
                }
            }
            return new List<IFileElement>(m_Files.Values);
        }

        public IList<IFileElement> GetAllFiles(IList<IXmlWritable> roots)
        {
            foreach (IXmlWritable writable in roots)
            {
                IElement element = writable as IElement;
                if (element != null)
                {
                    element.Visit(this);
                }
            }
            return new List<IFileElement>(m_Files.Values);
        }

        public IList<String> GetAllFilePaths(IProject project)
        {
            List<String> paths = new List<string>();
            foreach (IFileElement element in GetAllFiles(project))
            {
                String path = element.SoundFileType == SoundFileType.Music ?
                    Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
                path = System.IO.Path.Combine(path, element.FilePath);
                path = System.IO.Path.GetFullPath(path);
                paths.Add(path);
            }
            return paths;
        }

        public void VisitFileElement(IFileElement fileElement)
        {
            String path;
            if (fileElement.SoundFileType == SoundFileType.Music)
            {
                path = Ares.Settings.Settings.Instance.MusicDirectory;
            }
            else
            {
                path = Ares.Settings.Settings.Instance.SoundDirectory;
            }
            path = System.IO.Path.Combine(path, fileElement.FilePath);
            path = System.IO.Path.GetFullPath(path).ToUpperInvariant();
            if (!m_Files.ContainsKey(path))
            {
                m_Files.Add(path, fileElement);
            }
        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IChoiceElement element in choiceContainer.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            foreach (ISequentialElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IChoiceElement element in musicList.GetElements())
            {
                element.Visit(this);
            }
        }
    }
}
