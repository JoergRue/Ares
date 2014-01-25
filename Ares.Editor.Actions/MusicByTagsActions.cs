/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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

namespace Ares.Editor.Actions
{
    public class AddTagToMusicByTagsAction : Action
    {
        public AddTagToMusicByTagsAction(IMusicByTags element, int categoryId, int tagId)
        {
            m_Element = element;
            m_Category = categoryId;
            m_Tag = tagId;
        }

        public override void Do(IProject project)
        {
            m_Element.AddTag(m_Category, m_Tag);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            m_Element.RemoveTag(m_Category, m_Tag);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IMusicByTags m_Element;
        private int m_Category;
        private int m_Tag;
    }

    public class RemoveTagFromMusicByTagsAction : Action
    {
        public RemoveTagFromMusicByTagsAction(IMusicByTags element, int categoryId, int tagId)
        {
            m_Element = element;
            m_Category = categoryId;
            m_Tag = tagId;
        }

        public override void Do(IProject project)
        {
            m_Element.RemoveTag(m_Category, m_Tag);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            m_Element.AddTag(m_Category, m_Tag);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IMusicByTags m_Element;
        private int m_Category;
        private int m_Tag;
    }

    public class RemoveAllTagsFromMusicByTagsAction : Action
    {
        public RemoveAllTagsFromMusicByTagsAction(IMusicByTags element)
        {
            m_Element = element;
            m_OldTags = new Dictionary<int, HashSet<int>>();
            IDictionary<int, HashSet<int>> oldTags = element.GetTags();
            foreach (int category in oldTags.Keys)
            {
                HashSet<int> tags = new HashSet<int>();
                tags.UnionWith(oldTags[category]);
                m_OldTags[category] = tags;
            }
        }

        public override void Do(IProject project)
        {
            foreach (int category in m_OldTags.Keys)
            {
                foreach (int tag in m_OldTags[category])
                {
                    m_Element.RemoveTag(category, tag);
                }
            }
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            foreach (int category in m_OldTags.Keys)
            {
                foreach (int tag in m_OldTags[category])
                {
                    m_Element.AddTag(category, tag);
                }
            }
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private IMusicByTags m_Element;
        private Dictionary<int, HashSet<int>> m_OldTags;
    }

    public class SetOperatorInMusicByTagsAction : Action
    {
        public SetOperatorInMusicByTagsAction(IMusicByTags element, Ares.Data.TagCategoryCombination categoryCombination)
        {
            m_Element = element;
            m_OldValue = element.TagCategoryCombination;
            m_NewValue = categoryCombination;
        }

        public override void Do(IProject project)
        {
            m_Element.TagCategoryCombination = m_NewValue;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            m_Element.TagCategoryCombination = m_OldValue;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private Ares.Data.TagCategoryCombination m_OldValue;
        private Ares.Data.TagCategoryCombination m_NewValue;
        private IMusicByTags m_Element;
    }

    public class SetFadeTimeInMusicByTagsAction : Action
    {
        public SetFadeTimeInMusicByTagsAction(IMusicByTags element, int value)
        {
            m_Element = element;
            m_OldValue = element.FadeTime;
            m_NewValue = value;
        }

        public override void Do(IProject project)
        {
            m_Element.FadeTime = m_NewValue;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            m_Element.FadeTime = m_OldValue;
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        private int m_OldValue;
        private int m_NewValue;
        private IMusicByTags m_Element;
    }
}