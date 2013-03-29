/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
    public class FileSearch : IElementVisitor
    {
        public List<KeyValuePair<IMode,  List<IModeElement>>> GetRootElements(IProject project, String relativePath, SoundFileType soundFileType)
        {
            List<KeyValuePair<IMode,  List<IModeElement>>> resultList = new List<KeyValuePair<IMode,  List<IModeElement>>>();
            if (project == null)
            {
                return resultList;
            }
            m_CurrentModeElement = null;
            m_SearchedPath = GetAbsolutePath(relativePath, soundFileType);
            foreach (IMode mode in project.GetModes())
            {
                m_CurrentList = new HashSet<IModeElement>();
                foreach (IModeElement element in mode.GetElements())
                {
                    m_CurrentModeElement = element;
                    element.Visit(this);
                }
                if (m_CurrentList.Count > 0)
                {
                    List<IModeElement> modeElements = new List<IModeElement>(m_CurrentList);
                    resultList.Add(new KeyValuePair<IMode, List<IModeElement>>(mode, modeElements));
                }
            }
            return resultList;
        }

        private String m_SearchedPath;
        private IModeElement m_CurrentModeElement;
        private HashSet<IModeElement> m_CurrentList;
        private HashSet<IElement> m_References = new HashSet<IElement>();

        private static String GetAbsolutePath(String relativePath, SoundFileType soundFileType)
        {
            if (soundFileType == SoundFileType.Music)
            {
                return System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, relativePath);
            }
            else
            {
                return System.IO.Path.Combine(Ares.Settings.Settings.Instance.SoundDirectory, relativePath);
            }
        }

        private static String GetAbsolutePath(IFileElement fileElement)
        {
            return GetAbsolutePath(fileElement.FilePath, fileElement.SoundFileType);
        }

        public void VisitFileElement(IFileElement fileElement)
        {
            if (m_CurrentModeElement != null)
            {
                if (m_SearchedPath.Equals(GetAbsolutePath(fileElement), StringComparison.CurrentCultureIgnoreCase))
                {
                    m_CurrentList.Add(m_CurrentModeElement);
                }
            }
        }

        public void VisitSequentialContainer(ISequentialContainer sequentialContainer)
        {
            foreach (ISequentialElement element in sequentialContainer.GetElements())
            {
                if (m_CurrentModeElement == null)
                    break;
                element.Visit(this);
            }
        }

        public void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer)
        {
            foreach (IParallelElement element in parallelContainer.GetElements())
            {
                if (m_CurrentModeElement == null)
                    break;
                element.Visit(this);
            }
        }

        public void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer)
        {
            foreach (IChoiceElement element in choiceContainer.GetElements())
            {
                if (m_CurrentModeElement == null)
                    break;
                element.Visit(this);
            }
        }

        public void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList)
        {
            foreach (ISequentialElement element in musicList.GetElements())
            {
                if (m_CurrentModeElement == null)
                    break;
                element.Visit(this);
            }
        }

        public void VisitRandomMusicList(IRandomBackgroundMusicList musicList)
        {
            foreach (IChoiceElement element in musicList.GetElements())
            {
                if (m_CurrentModeElement == null)
                    break;
                element.Visit(this);
            }
        }

        public void VisitMacro(IMacro macro)
        {
        }

        public void VisitMacroCommand(IMacroCommand macroCommand)
        {
        }

        public void VisitReference(IReferenceElement reference)
        {
            if (!m_References.Contains(reference))
            {
                m_References.Add(reference);
                IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement(reference.ReferencedId);
                if (referencedElement != null)
                {
                    referencedElement.Visit(this);
                }
                m_References.Remove(reference);
            }
        }

        public void VisitMusicByTags(IMusicByTags musicByTags)
        {
            if (m_CurrentModeElement != null)
            {
                foreach (String file in musicByTags.GetAllMatchingFiles())
                {
                    if (GetAbsolutePath(file, SoundFileType.Music).Equals(m_SearchedPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        m_CurrentList.Add(m_CurrentModeElement);
                        break;
                    }
                }
            }
        }
    }

    public static class MusicByTagsExtensions
    {
        public static IList<String> GetAllMatchingFiles(this IMusicByTags musicByTags)
        {
            try
            {
                var db = Ares.Tags.TagsModule.GetTagsDB().ReadInterface;
                if (musicByTags.IsOperatorAnd)
                {
                    return db.GetAllFilesWithAnyTagInEachCategory(musicByTags.GetTags());
                }
                else
                {
                    return db.GetAllFilesWithAnyTag(musicByTags.GetAllTags());
                }
            }
            catch (Ares.Tags.TagsDbException)
            {
                return new List<String>();
            }
        }
    }
}
