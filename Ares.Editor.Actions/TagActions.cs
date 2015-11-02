/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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
using System.Windows.Forms;

using Ares.Tags;

namespace Ares.Editor.Actions
{
    public class TagChanges
    {
        public event EventHandler<EventArgs> TagsDBChanged;

        public void FireTagsDBChanged(Object sender)
        {
            if (TagsDBChanged != null)
            {
                TagsDBChanged(sender, new EventArgs());
            }
        }

        public static TagChanges Instance
        {
            get
            {
                if (sInstance == null)
                    sInstance = new TagChanges();
                return sInstance;
            }
        }

        private static TagChanges sInstance;

        private TagChanges()
        {
        }
    }

    public class ChangeFileTagsAction : Action
    {
        private List<String> m_Files;
        private IList<IList<int>> m_AddedTags;
        private IList<IList<int>> m_RemovedTags;

        public ChangeFileTagsAction(List<String> files, HashSet<int> addedTags, HashSet<int> removedTags, int languageId)
        {
            m_Files = new List<string>(files);
            m_AddedTags = new List<IList<int>>();
            m_RemovedTags = new List<IList<int>>();

            foreach (String file in m_Files)
            {
                m_AddedTags.Add(new List<int>(addedTags));
                m_RemovedTags.Add(new List<int>(removedTags));
            }
        }

        public override void Do(Ares.Data.IProject project)
        {
            AddFileTags(m_AddedTags);
            RemoveFileTags(m_RemovedTags);
            Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
        }

        public override void Undo(Ares.Data.IProject project)
        {
            AddFileTags(m_RemovedTags);
            RemoveFileTags(m_AddedTags);
            Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
        }

        private void AddFileTags(IList<IList<int>> tags)
        {
            ITagsDBWrite dbWrite = Ares.Tags.TagsModule.GetTagsDB().WriteInterface;
            dbWrite.AddFileTags(m_Files, tags);
        }

        private void RemoveFileTags(IList<IList<int>> tags)
        {
            ITagsDBWrite dbWrite = Ares.Tags.TagsModule.GetTagsDB().WriteInterface;
            dbWrite.RemoveFileTags(m_Files, tags);
        }
    }

    public class HideCategoryAction : Action
    {
        private int m_CategoryId;
        private bool m_Hidden;
        private bool m_OldValue;

        public HideCategoryAction(int categoryId, bool isHidden, Ares.Data.IProject project)
        {
            m_CategoryId = categoryId;
            m_Hidden = isHidden;
            m_OldValue = project.GetHiddenTagCategories().Contains(categoryId);
        }

        public override void Do(Data.IProject project)
        {
            project.SetTagCategoryHidden(m_CategoryId, m_Hidden);
            TagChanges.Instance.FireTagsDBChanged(null);
        }

        public override void Undo(Data.IProject project)
        {
            project.SetTagCategoryHidden(m_CategoryId, m_OldValue);
            TagChanges.Instance.FireTagsDBChanged(null);
        }
    }

    public class HideTagAction : Action
    {
        private int m_TagId;
        private bool m_Hidden;
        private bool m_OldValue;

        public HideTagAction(int tagId, bool isHidden, Ares.Data.IProject project)
        {
            m_TagId = tagId;
            m_Hidden = isHidden;
            m_OldValue = project.GetHiddenTags().Contains(m_TagId);
        }

        public override void Do(Data.IProject project)
        {
            project.SetTagHidden(m_TagId, m_Hidden);
            TagChanges.Instance.FireTagsDBChanged(null);
        }

        public override void Undo(Data.IProject project)
        {
            project.SetTagHidden(m_TagId, m_OldValue);
            TagChanges.Instance.FireTagsDBChanged(null);
        }
    }
}
