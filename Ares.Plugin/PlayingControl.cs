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
using System.Linq;
using System.Text;

namespace Ares.MediaPortalPlugin
{
    public class PlayingControl
    {
        public PlayingControl()
        {
        }

        public void Dispose()
        {
        }

        private Object syncObject = new Int16();

        private Controllers.Configuration m_CurrentProject;

        public Controllers.Configuration CurrentProject
        {
            get
            {
                lock (syncObject)
                {
                    return m_CurrentProject;
                }
            }
            set
            {
                lock (syncObject)
                {
                    m_CurrentProject = value;
                    elementsToTitles.Clear();
                    if (m_CurrentProject != null)
                    {
                        foreach (Controllers.Mode mode in m_CurrentProject.Modes)
                        {
                            foreach (Controllers.ModeElement element in mode.Elements)
                            {
                                elementsToTitles[element.Id] = element.Title;
                            }
                        }
                    }
                }
            }
        }

        private List<Ares.Controllers.MusicListItem> m_CurrentMusicList;

        public List<Ares.Controllers.MusicListItem> GetCurrentMusicList()
        {
            lock (syncObject)
            {
                return m_CurrentMusicList;
            }
        }

        public void MusicListChanged(List<Ares.Controllers.MusicListItem> newList)
        {
            lock (syncObject)
            {
                m_CurrentMusicList = newList;
            }
        }

        private String m_CurrentMode;

        private Dictionary<int, String> elementsToTitles = new Dictionary<int, string>();

        public String CurrentMode
        {
            get
            {
                lock (syncObject)
                {
                    return m_CurrentMode;
                }
            }
        }

        private List<String> m_ModeElements = new List<String>();
        private List<int> m_ModeElementIds = new List<int>();

        public IList<String> CurrentModeElements
        {
            get
            {
                lock (syncObject)
                {
                    List<String> copy = new List<String>(m_ModeElements);
                    return copy;
                }
            }
        }

        public List<int> CurrentModeElementIds
        {
            get
            {
                lock (syncObject)
                {
                    List<int> copy = new List<int>(m_ModeElementIds);
                    return copy;
                }
            }
        }


        private String m_MusicElementShort = String.Empty;
        private String m_MusicElementLong = String.Empty;

        public String CurrentMusicElementShort
        {
            get
            {
                lock (syncObject)
                {
                    return m_MusicElementShort;
                }
            }
        }

        public String CurrentMusicElementLong
        {
            get
            {
                lock (syncObject)
                {
                    return m_MusicElementLong;
                }
            }
        }

        private int m_GlobalVolume = 100;

        public int GlobalVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_GlobalVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_GlobalVolume = value;
                }
            }
        }

        private int m_MusicVolume = 100;

        public int MusicVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_MusicVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_MusicVolume = value;
                }
            }
        }

        private int m_SoundVolume = 100;

        public int SoundVolume
        {
            get
            {
                lock (syncObject)
                {
                    return m_SoundVolume;
                }
            }

            set
            {
                lock (syncObject)
                {
                    m_SoundVolume = value;
                }
            }
        }

        private bool m_MusicRepeat = false;
        private bool m_MusicOnAllSpeakers = false;

        private int m_MusicFadingOption = 0;
        private int m_MusicFadingTime = 0;

        public bool MusicRepeat
        {
            get
            {
                return m_MusicRepeat;
            }
        }

        public bool MusicOnAllSpeakers
        {
            get
            {
                return m_MusicOnAllSpeakers;
            }
        }

        public int MusicFadingOption
        {
            get
            {
                return m_MusicFadingOption;
            }
        }

        public int MusicFadingTime
        {
            get
            {
                return m_MusicFadingTime;
            }
        }

        public void ModeChanged(String newMode)
        {
            lock (syncObject)
            {
                m_CurrentMode = newMode;
            }
        }

        public void ModeElementStarted(int elementId)
        {
            lock (syncObject)
            {
                if (elementsToTitles.ContainsKey(elementId))
                {
                    m_ModeElements.Add(elementsToTitles[elementId]);
                }
                m_ModeElementIds.Add(elementId);
            }
        }

        public void ModeElementFinished(int elementId)
        {
            lock (syncObject)
            {
                if (elementsToTitles.ContainsKey(elementId))
                {
                    m_ModeElements.Remove(elementsToTitles[elementId]);
                }
                m_ModeElementIds.Remove(elementId);
            }
        }

        public void AllModeElementsStopped()
        {
            lock (syncObject)
            {
                m_ModeElements.Clear();
                m_ModeElementIds.Clear();
            }
        }

        public void MusicStarted(String shortTitle, String longTitle)
        {
            lock (syncObject)
            {
                m_MusicElementShort = shortTitle;
                m_MusicElementLong = longTitle;
            }
        }

        public void MusicFinished(String title)
        {
            lock (syncObject)
            {
                if (m_MusicElementShort == title)
                {
                    m_MusicElementShort = String.Empty;
                    m_MusicElementLong = String.Empty;
                }
            }
        }

        public void VolumeChanged(int index, int newValue)
        {
            lock (syncObject)
            {
                if (index == 1)
                {
                    m_MusicVolume = newValue;
                }
                else if (index == 0)
                {
                    m_SoundVolume = newValue;
                }
                else
                {
                    m_GlobalVolume = newValue;
                }
            }
        }

        public void MusicRepeatChanged(bool repeat)
        {
            lock (syncObject)
            {
                m_MusicRepeat = repeat;
            }
        }

        public void MusicOnAllSpeakersChanged(bool onAllSpeakers)
        {
            lock (syncObject)
            {
                m_MusicOnAllSpeakers = onAllSpeakers;
            }
        }

        public void MusicFadingChanged(int fadingOption, int fadingTime)
        {
            lock (syncObject)
            {
                m_MusicFadingOption = fadingOption;
                m_MusicFadingTime = fadingTime;
            }
        }

        private HashSet<int> m_CurrentMusicTags = new HashSet<int>();

        public HashSet<int> GetCurrentMusicTags()
        {
            lock (syncObject)
            {
                return new HashSet<int>(m_CurrentMusicTags);
            }
        }

        public void MusicTagAdded(int tagId)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Add(tagId);
            }
        }

        public void MusicTagRemoved(int tagId)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Remove(tagId);
            }
        }

        public void AllMusicTagsRemoved()
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Clear();
            }
        }

        public void MusicTagsChanged(ICollection<int> newTags, int categoryCombination, int fadeTime)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Clear();
                m_CurrentMusicTags.UnionWith(newTags);
                m_MusicTagCategoriesCombination = categoryCombination;
            }
        }

        private int m_MusicTagsFadeTime;
        private bool m_MusicTagsFadeOnlyOnChange;

        public void MusicTagsFadingChanged(int fadeTime, bool fadeOnlyOnChange)
        {
            lock (syncObject)
            {
                m_MusicTagsFadeTime = fadeTime;
                m_MusicTagsFadeOnlyOnChange = fadeOnlyOnChange;
            }
        }

        public void GetMusicTagsFading(out int fadeTime, out bool fadeOnlyOnChange)
        {
            lock (syncObject)
            {
                fadeTime = m_MusicTagsFadeTime;
                fadeOnlyOnChange = m_MusicTagsFadeOnlyOnChange;
            }
        }

        private int m_MusicTagCategoriesCombination = 0;

        public int GetMusicTagCategoriesCombination()
        {
            lock (syncObject)
            {
                return m_MusicTagCategoriesCombination;
            }
        }

        public void MusicTagCategoriesCombinationChanged(int categoryCombination)
        {
            lock (syncObject)
            {
                m_MusicTagCategoriesCombination = categoryCombination;
            }
        }

        private List<Controllers.MusicTagCategory> m_TagCategories;
        private Dictionary<int, List<Controllers.MusicTag>> m_TagsPerCategory;

        public List<Controllers.MusicTagCategory> TagCategories
        {
            get
            {
                lock (syncObject)
                {
                    return m_TagCategories;
                }
            }
        }

        public Dictionary<int, List<Controllers.MusicTag>> TagsPerCategory
        {
            get
            {
                lock (syncObject)
                {
                    return m_TagsPerCategory;
                }
            }
        }

        public void TagsChanged(List<Controllers.MusicTagCategory> categories, Dictionary<int, List<Controllers.MusicTag>> tagsPerCategory)
        {
            lock (syncObject)
            {
                m_TagCategories = categories;
                m_TagsPerCategory = tagsPerCategory;
            }
        }

        public void ActiveTagsChanged(List<int> activeTags)
        {
            lock (syncObject)
            {
                m_CurrentMusicTags.Clear();
                foreach (int tag in activeTags)
                    m_CurrentMusicTags.Add(tag);
            }
        }
    }
}
