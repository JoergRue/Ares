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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Tags;

namespace Ares.Editor.Dialogs
{
    public partial class TagFilterDialog : Form
    {
        public TagFilterDialog()
        {
            InitializeComponent();
        }

        private Dictionary<int, HashSet<int>> m_TagsByCategory;
        private HashSet<int> m_SelectedTags;

        public Dictionary<int, HashSet<int>> TagsByCategory
        {
            get
            {
                Dictionary<int, HashSet<int>> result = new Dictionary<int, HashSet<int>>();
                foreach (var entry in m_TagsByCategory)
                {
                    result[entry.Key] = new HashSet<int>(entry.Value);
                }
                return result;
            }

            set
            {
                m_SelectedTags = new HashSet<int>();
                m_TagsByCategory = new Dictionary<int, HashSet<int>>();
                foreach (var entry in value)
                {
                    m_TagsByCategory[entry.Key] = new HashSet<int>(entry.Value);
                    m_SelectedTags.UnionWith(entry.Value);
                }
                UpdateControls();
            }
        }

        public Data.TagCategoryCombination TagCategoryCombination
        {
            get
            {
                return (Data.TagCategoryCombination)tagCategoryCombinationBox.SelectedIndex;
            }
            set
            {
                tagCategoryCombinationBox.SelectedIndex = (int)value;
            }
        }

        private int m_LanguageId;

        public int LanguageId
        {
            get { return m_LanguageId; }
            set
            {
                m_LanguageId = value;
                UpdateControls();
            }
        }

        private bool m_Listen = true;

        private IList<CategoryForLanguage> m_Categories;
        private IList<TagForLanguage> m_Tags;
        private int m_SelectedCategoryId = -1;
        private TagsFilterMode m_FilterMode = TagsFilterMode.NormalFilter;

        public TagsFilterMode FilterMode
        {
            get { return m_FilterMode; }
            set
            {
                m_Listen = false;
                switch (value)
                {
                    case TagsFilterMode.NoFilter:
                        allFilesButton.Checked = true;
                        normalFilterButton.Checked = false;
                        noTagsFilterButton.Checked = false;
                        categoriesBox.Enabled = false;
                        tagCategoryCombinationBox.Enabled = false;
                        tagsBox.Enabled = false;
                        break;
                    case TagsFilterMode.UntaggedFiles:
                        noTagsFilterButton.Checked = true;
                        normalFilterButton.Checked = false;
                        allFilesButton.Checked = false;
                        categoriesBox.Enabled = false;
                        tagCategoryCombinationBox.Enabled = false;
                        tagsBox.Enabled = false;
                        break;
                    case TagsFilterMode.NormalFilter:
                    default:
                        normalFilterButton.Checked = true;
                        allFilesButton.Checked = false;
                        noTagsFilterButton.Checked = false;
                        tagCategoryCombinationBox.Enabled = true;
                        categoriesBox.Enabled = true;
                        tagsBox.Enabled = true;
                        break;
                }
                m_FilterMode = value;
                m_Listen = true;
            }
        }

        private bool m_AutoUpdateTree = true;

        public bool AutoUpdateTree
        {
            get
            {
                return m_AutoUpdateTree;
            }
            set
            {
                m_AutoUpdateTree = value;
                m_Listen = false;
                autoUpdateBox.Checked = value;
                m_Listen = true;
            }

        }

        private void UpdateControls()
        {
            if (m_TagsByCategory == null)
            {
                return;
            }

            m_Listen = false;

            try
            {
                ITagsDBReadByLanguage tagsRead = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);

                m_Categories = tagsRead.GetAllCategories();
                categoriesBox.BeginUpdate();
                categoriesBox.Items.Clear();
                int selIndex = -1;
                foreach (CategoryForLanguage category in m_Categories)
                {
                    categoriesBox.Items.Add(category.Name);
                    if (category.Id == m_SelectedCategoryId)
                        selIndex = categoriesBox.Items.Count - 1;
                }
                categoriesBox.Items.Add(StringResources.All);
                if (selIndex == -1)
                {
                    selIndex = 0;
                    if (m_Categories.Count > 0)
                    {
                        m_SelectedCategoryId = m_Categories[0].Id;
                    }
                    else
                    {
                        m_SelectedCategoryId = -2;
                    }
                }
                categoriesBox.SelectedIndex = selIndex;
                categoriesBox.EndUpdate();

                UpdateTagsBox(tagsRead);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            m_Listen = true;
        }

        private void categoriesBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (categoriesBox.SelectedIndex == categoriesBox.Items.Count - 1)
            {
                m_SelectedCategoryId = -2;
            }
            else
            {
                m_SelectedCategoryId = m_Categories[categoriesBox.SelectedIndex].Id;
            }
            m_Listen = false;
            try
            {
                ITagsDBReadByLanguage tagsRead = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);
                UpdateTagsBox(tagsRead);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            m_Listen = true;
        }

        private void UpdateTagsBox(ITagsDBReadByLanguage tagsRead)
        {
            tagsBox.BeginUpdate();
            tagsBox.Items.Clear();
            if (m_SelectedCategoryId >= 0)
            {
                m_Tags = tagsRead.GetAllTags(m_SelectedCategoryId);
            }
            else
            {
                IList<TagInfoForLanguage> allTags = tagsRead.GetAllTags();
                m_Tags = new List<TagForLanguage>(allTags.Count);
                foreach (TagInfoForLanguage tagInfo in allTags)
                {
                    m_Tags.Add(new TagForLanguage() { Name = tagInfo.Name, Id = tagInfo.Id });
                }
            }
            foreach (TagForLanguage tag in m_Tags)
            {
                tagsBox.Items.Add(tag.Name);
                if (m_SelectedTags.Contains(tag.Id))
                {
                    tagsBox.SetItemCheckState(tagsBox.Items.Count - 1, CheckState.Checked);
                }
            }
            tagsBox.EndUpdate();
        }

        private void tagsBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (!m_Listen)
                return;
            if (e.CurrentValue == e.NewValue)
                return;
            int tagId = m_Tags[e.Index].Id;
            int categoryId = m_SelectedCategoryId;
            if (categoryId < 0)
            {
                try
                {
                    List<int> tagIds = new List<int>();
                    tagIds.Add(tagId);
                    var tagInfo = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetTagInfos(tagIds);
                    if (tagInfo.Count > 0)
                    {
                        categoryId = tagInfo[0].CategoryId;
                    }
                    else
                    {
                        MessageBox.Show(this, String.Format(StringResources.TagsDbError, "No Info for tag Id!"), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (e.NewValue == CheckState.Checked)
            {
                m_SelectedTags.Add(tagId);
                if (!m_TagsByCategory.ContainsKey(categoryId))
                    m_TagsByCategory[categoryId] = new HashSet<int>();
                m_TagsByCategory[categoryId].Add(tagId);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                m_SelectedTags.Remove(tagId);
                if (m_TagsByCategory.ContainsKey(categoryId))
                {
                    m_TagsByCategory[categoryId].Remove(tagId);
                    if (m_TagsByCategory[categoryId].Count == 0)
                        m_TagsByCategory.Remove(categoryId);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_SelectedTags.Clear();
            m_TagsByCategory.Clear();
            m_Listen = false;
            try
            {
                ITagsDBReadByLanguage tagsRead = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);
                UpdateTagsBox(tagsRead);
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            m_Listen = true;
        }

        private void normalFilterButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            FilterMode = TagsFilterMode.NormalFilter;
        }

        private void noTagsFilterButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            FilterMode = TagsFilterMode.UntaggedFiles;
        }

        private void allFilesButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            FilterMode = TagsFilterMode.NoFilter;
        }

        private void autoUpdateBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            m_AutoUpdateTree = autoUpdateBox.Checked;
        }
    }
}
