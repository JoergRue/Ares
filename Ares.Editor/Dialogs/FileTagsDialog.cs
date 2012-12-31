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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ares.Tags;

namespace Ares.Editor.Dialogs
{
    public partial class FileTagsDialog : Form
    {
        public FileTagsDialog()
        {
            InitializeComponent();
        }

        private List<String> m_Files;
        private Dictionary<int, int> m_FileTags = new Dictionary<int, int>();
        private Dictionary<int, int> m_OriginalFileTags = new Dictionary<int, int>();
        private HashSet<int> m_AddedTags = new HashSet<int>();
        private HashSet<int> m_RemovedTags = new HashSet<int>();

        public HashSet<int> AddedTags { get { return m_AddedTags; } }
        public HashSet<int> RemovedTags { get { return m_RemovedTags; } }

        public void SetFiles(List<String> files)
        {
            m_Files = files;
            
            m_FileTags.Clear();
            m_AddedTags.Clear();
            m_RemovedTags.Clear();

            try
            {
                ITagsDBReadByLanguage dbRead = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);
                foreach (String file in m_Files)
                {
                    IList<TagInfoForLanguage> tagsForFile = dbRead.GetTagsForFile(file);
                    foreach (TagInfoForLanguage tagInfo in tagsForFile)
                    {
                        if (m_FileTags.ContainsKey(tagInfo.Id))
                        {
                            m_FileTags[tagInfo.Id]++;
                        }
                        else
                        {
                            m_FileTags[tagInfo.Id] = 1;
                        }
                    }
                }
                m_OriginalFileTags = new Dictionary<int, int>(m_FileTags);

                UpdateControls();
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private IList<LanguageForLanguage> m_Languages;
        private int m_SelectedCategoryId = -1;

        private void UpdateControls()
        {
            if (m_Files == null || m_Files.Count == 0)
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

                languageBox.BeginUpdate();
                languageBox.Items.Clear();
                m_Languages = tagsRead.GetAllLanguages();
                selIndex = -1;
                foreach (LanguageForLanguage language in m_Languages)
                {
                    languageBox.Items.Add(language.Name);
                    if (language.Id == m_LanguageId)
                        selIndex = languageBox.Items.Count - 1;
                }
                if (selIndex != -1)
                {
                    languageBox.SelectedIndex = selIndex;
                }
                languageBox.EndUpdate();
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            m_Listen = true;
        }

        private void languageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            LanguageId = m_Languages[languageBox.SelectedIndex].Id;
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
                if (m_FileTags.ContainsKey(tag.Id))
                {
                    tagsBox.SetItemCheckState(tagsBox.Items.Count - 1, m_FileTags[tag.Id] == m_Files.Count ? CheckState.Checked : CheckState.Indeterminate);
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
            if (e.NewValue == CheckState.Checked)
            {
                m_FileTags[tagId] = m_Files.Count;
                m_AddedTags.Add(tagId);
                m_RemovedTags.Remove(tagId);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                m_FileTags.Remove(tagId);
                m_AddedTags.Remove(tagId);
                m_RemovedTags.Add(tagId);
            }
            else
            {
                // undetermined again
                m_FileTags[tagId] = m_OriginalFileTags[tagId];
                m_AddedTags.Remove(tagId);
                m_RemovedTags.Remove(tagId);
            }
        }

        private void addTagButton_Click(object sender, EventArgs e)
        {
            AddTagDialog dialog = new AddTagDialog();
            String[] categories = new String[m_Categories.Count];
            for (int i = 0; i < m_Categories.Count; ++i)
                categories[i] = m_Categories[i].Name;
            String selCat = ((categoriesBox.SelectedIndex != -1  && categoriesBox.SelectedIndex != categoriesBox.Items.Count - 1) 
                ? categoriesBox.SelectedItem.ToString() : String.Empty);
            dialog.SetCategories(categories, selCat);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                String category = dialog.Category;
                String tagName = dialog.TagName;
                AddTag(category, tagName);
            }
        }

        private void AddTag(String category, String tagName)
        {
            try
            {
                bool hasChanges = false;
                int categoryId = -1;
                CategoryForLanguage existing = m_Categories.FirstOrDefault((categoryForLanguage) => { return categoryForLanguage.Name.Equals(category, StringComparison.OrdinalIgnoreCase); });
                if (existing == null)
                {
                    // the category is new
                    hasChanges = true;
                    categoryId = TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId).AddCategory(category);
                }
                else
                {
                    categoryId = existing.Id;
                    IList<TagForLanguage> tags = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllTags(categoryId);
                    TagForLanguage existingTag = tags.FirstOrDefault((tagForLanguage) => { return tagForLanguage.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase); });
                    if (existingTag == null)
                    {
                        hasChanges = true;
                    }
                }
                if (hasChanges)
                {
                    // either category or tag inside the category is new
                    int tagId = TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId).AddTag(categoryId, tagName);
                    m_FileTags[tagId] = m_Files.Count;
                    m_AddedTags.Add(tagId);
                    UpdateControls();
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged();
                }
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
