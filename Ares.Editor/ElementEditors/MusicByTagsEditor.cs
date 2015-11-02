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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ares.Tags;
using Ares.Data;

namespace Ares.Editor.ElementEditors
{
    partial class MusicByTagsEditor : EditorBase
    {
        public MusicByTagsEditor()
        {
            InitializeComponent();
        }

        private IMusicByTags m_Element;
        private int m_LanguageId;

        public void SetElement(IMusicByTags element, IProject project)
        {
            m_Project = project;
            if (m_Element != null)
            {
                Ares.Editor.Actions.ElementChanges.Instance.RemoveListener(m_Element.Id, Update);
            }
            m_Element = element;
            if (m_Element != null)
            {
                Ares.Editor.Actions.ElementChanges.Instance.AddListener(m_Element.Id, Update);
                this.Text = m_Element.Title;
            }
            m_LanguageId = project.TagLanguageId;
            if (m_LanguageId == -1)
            {
                try
                {
                    m_LanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                }
                catch (Ares.Tags.TagsDbException)
                {
                }
            }
            UpdateControls();
        }

        private void Update(int elementID, Actions.ElementChanges.ChangeType changeType)
        {
            if (!m_Listen)
                return;
            UpdateControls();
        }

        private bool m_Listen = true;

        private IProject m_Project;
        private IList<LanguageForLanguage> m_Languages;
        private IList<CategoryForLanguage> m_Categories;
        private IList<TagForLanguage> m_Tags;
        private int m_SelectedCategoryId = -1;

        private void UpdateControls()
        {
            m_Listen = false;

            tagCategoryCombinationBox.Enabled = m_Element != null;
            tagCategoryCombinationBox.SelectedIndex = (m_Element != null) ? (int)m_Element.TagCategoryCombination : 0;

            fadeUpDown.Enabled = m_Element != null;
            fadeUpDown.Value = m_Element != null ? m_Element.FadeTime : 0;

            try
            {
                ITagsDBReadByLanguage tagsRead = TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);

                languageBox.BeginUpdate();
                languageBox.Items.Clear();
                m_Languages = tagsRead.GetAllLanguages();
                int selIndex = -1;
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

                m_Categories = tagsRead.GetAllCategories();
                categoriesBox.BeginUpdate();
                categoriesBox.Items.Clear();
                selIndex = -1;
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
            HashSet<int> selectedTags = new HashSet<int>();
            if (m_Element != null) 
                selectedTags.UnionWith(m_Element.GetAllTags());
            foreach (TagForLanguage tag in m_Tags)
            {
                tagsBox.Items.Add(tag.Name);
                
                if (selectedTags.Contains(tag.Id))
                {
                    tagsBox.SetItemCheckState(tagsBox.Items.Count - 1, CheckState.Checked);
                }
            }
            tagsBox.Enabled = (m_Element != null);
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
                m_Listen = false;
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.AddTagToMusicByTagsAction(m_Element, categoryId, tagId), m_Project);
                m_Listen = true;
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                m_Listen = false;
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.RemoveTagFromMusicByTagsAction(m_Element, categoryId, tagId), m_Project);
                m_Listen = true;
            }
        }

        private void clearTagsButton_Click(object sender, EventArgs e)
        {
            m_Listen = false;
            Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.RemoveAllTagsFromMusicByTagsAction(m_Element), m_Project);
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

        private void languageBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (languageBox.SelectedIndex < m_Languages.Count)
            {
                m_LanguageId = m_Languages[languageBox.SelectedIndex].Id;
                if (m_Project != null)
                {
                    m_Project.TagLanguageId = m_LanguageId;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                }
                UpdateControls();
            }
        }

        private void fadeUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            m_Listen = false;
            int fadeTime = (int)fadeUpDown.Value;
            Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.SetFadeTimeInMusicByTagsAction(m_Element, fadeTime), m_Project);
            m_Listen = true;
        }

        private void tagCategoryCombinationBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            m_Listen = false;
            Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.SetOperatorInMusicByTagsAction(m_Element, 
                (TagCategoryCombination)tagCategoryCombinationBox.SelectedIndex), m_Project);
            m_Listen = true;
        }

    }
}
