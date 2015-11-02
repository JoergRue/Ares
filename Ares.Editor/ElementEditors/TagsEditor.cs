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

namespace Ares.Editor.ElementEditors
{

    public partial class TagsEditor : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public TagsEditor()
        {
            HideOnClose = true;
            InitializeComponent();
            Ares.Editor.Actions.TagChanges.Instance.TagsDBChanged += new EventHandler<EventArgs>(TagsDBChanged);
        }

        void TagsDBChanged(object sender, EventArgs e)
        {
            if (sender != this)
            {
                UpdateAll();
            }
        }

        protected override string GetPersistString()
        {
            return "TagsEditor";
        }

        private Ares.Data.IProject m_Project;

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
            UpdateAll();
        }

        private void UpdateAll()
        {
            m_Listen = false;
            UpdateLanguageSelectionBox();
            UpdateLanguages();
            UpdateCategories();
            UpdateTags();
            UpdateTranslations();
            m_Listen = true;
        }

        private bool m_Listen = true;

        private enum TranslationType { Category, Tag, Language };
        private TranslationType m_LastTranslationType = TranslationType.Category;

        private int m_LanguageId = -1;

        private void UpdateSelectedTagLanguage()
        {
            if (languageSelectionBox.SelectedIndex < m_LanguagesForGUI.Count)
            {
                m_LanguageId = m_LanguagesForGUI[languageSelectionBox.SelectedIndex].Id;
                if (m_Project != null)
                {
                    m_Project.TagLanguageId = m_LanguageId;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                }
                UpdateAll();
            }
        }

        private IList<Ares.Tags.LanguageForLanguage> m_LanguagesForGUI;

        private void UpdateLanguageSelectionBox()
        {
            m_LanguageId = m_Project != null ? m_Project.TagLanguageId : -1;
            if (m_LanguageId == -1)
            {
                try
                {
                    m_LanguageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    m_LanguageId = -1;
                }
            }
            m_LanguagesForGUI = new List<Ares.Tags.LanguageForLanguage>();
            if (m_LanguageId != -1)
            {
                try
                {
                    languageSelectionBox.Items.Clear();
                    m_LanguagesForGUI = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllLanguages();
                    int selIndex = -1;
                    foreach (var language in m_LanguagesForGUI)
                    {
                        languageSelectionBox.Items.Add(language.Name);
                        if (language.Id == m_LanguageId)
                            selIndex = languageSelectionBox.Items.Count - 1;
                    }
                    if (selIndex == -1 && languageSelectionBox.Items.Count > 0)
                    {
                        selIndex = 0;
                    }
                    if (selIndex != -1)
                    {
                        languageSelectionBox.SelectedIndex = selIndex;
                        if (m_LanguagesForGUI[selIndex].Id != m_LanguageId)
                        {
                            m_LanguageId = m_LanguagesForGUI[selIndex].Id;
                            if (m_Project != null)
                            {
                                m_Project.TagLanguageId = m_LanguageId;
                            }
                        }
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private int m_LastLanguageId = -1;
        private IList<Ares.Tags.LanguageForLanguage> m_Languages;

        private void LanguageSelected()
        {
            if (languagesList.SelectedIndex < m_Languages.Count)
            {
                m_LastLanguageId = m_Languages[languagesList.SelectedIndex].Id;
                deleteLanguageButton.Enabled = m_Languages.Count > 1 && m_LastLanguageId != m_LanguageId;
                m_LastTranslationType = TranslationType.Language;
                m_Listen = false;
                UpdateTranslations();
                m_Listen = true;
            }
        }

        private void UpdateLanguages()
        {
            languagesList.BeginUpdate();
            languagesList.Items.Clear();
            m_Languages = new List<Ares.Tags.LanguageForLanguage>();
            if (m_LanguageId == -1)
            {
                deleteLanguageButton.Enabled = false;
                renameLanguageButton.Enabled = false;
                m_LastLanguageId = -1;
                languagesList.EndUpdate();
                return;
            }
            try
            {
                m_Languages = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllLanguages();
                int selIndex = -1;
                foreach (var language in m_Languages)
                {
                    languagesList.Items.Add(language.Name);
                    if (language.Id == m_LastLanguageId)
                        selIndex = languagesList.Items.Count - 1;
                }
                if (selIndex == -1 && m_Languages.Count > 0)
                    selIndex = 0;
                if (selIndex != -1)
                {
                    m_LastLanguageId = m_Languages[selIndex].Id;
                    languagesList.SelectedIndex = selIndex;
                }
                deleteLanguageButton.Enabled = m_Languages.Count > 1 && m_LastLanguageId != m_LanguageId;
                renameLanguageButton.Enabled = m_Languages.Count > 0;
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                deleteLanguageButton.Enabled = false;
                renameLanguageButton.Enabled = false;
            }
            languagesList.EndUpdate();
        }

        private int m_LastCategoryId = -1;
        private IList<Ares.Tags.CategoryForLanguage> m_Categories;

        private void CategorySelected()
        {
            if (categoriesList.SelectedIndex < m_Categories.Count)
            {
                m_LastCategoryId = m_Categories[categoriesList.SelectedIndex].Id;
                m_LastTranslationType = TranslationType.Category;
                m_Listen = false;
                UpdateTags();
                UpdateTranslations();
                categoryHiddenBox.Checked = m_Project != null && m_Project.GetHiddenTagCategories().Contains(m_LastCategoryId);
                m_Listen = true;
            }
        }

        private void UpdateCategories()
        {
            categoriesList.BeginUpdate();
            categoriesList.Items.Clear();
            m_Categories = new List<Ares.Tags.CategoryForLanguage>();
            if (m_LanguageId == -1)
            {
                deleteCategoryButton.Enabled = false;
                renameCategoryButton.Enabled = false;
                categoryHiddenBox.Enabled = false;
                m_LastCategoryId = -1;
                categoriesList.EndUpdate();
                return;
            }
            try
            {
                m_Categories = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllCategories();
                int selIndex = -1;
                foreach (var category in m_Categories)
                {
                    categoriesList.Items.Add(category.Name);
                    if (category.Id == m_LastCategoryId)
                    {
                        selIndex = categoriesList.Items.Count - 1;
                    }
                }
                if (selIndex == -1 && m_Categories.Count > 0)
                {
                    selIndex = 0;
                }
                if (selIndex != -1)
                {
                    m_LastCategoryId = m_Categories[selIndex].Id;
                    categoriesList.SelectedIndex = selIndex;
                    categoryHiddenBox.Checked = m_Project != null && m_Project.GetHiddenTagCategories().Contains(m_LastCategoryId);
                }
                else
                {
                    categoryHiddenBox.Checked = false;
                }
                deleteCategoryButton.Enabled = m_Categories.Count > 0;
                renameCategoryButton.Enabled = m_Categories.Count > 0;
                categoryHiddenBox.Enabled = m_Project != null && m_Categories.Count > 0;
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                deleteCategoryButton.Enabled = false;
                renameCategoryButton.Enabled = false;
                categoryHiddenBox.Enabled = false;
            }
            categoriesList.EndUpdate();
        }

        private int m_LastTagId = -1;
        private IList<Ares.Tags.TagForLanguage> m_Tags;

        private void TagSelected()
        {
            if (tagsList.SelectedIndex < m_Tags.Count)
            {
                m_LastTagId = m_Tags[tagsList.SelectedIndex].Id;
                m_LastTranslationType = TranslationType.Tag;
                m_Listen = false;
                UpdateTranslations();
                tagHiddenBox.Checked = m_Project != null && m_Project.GetHiddenTags().Contains(m_LastTagId);
                m_Listen = true;
            }
        }


        private void UpdateTags()
        {
            tagsList.BeginUpdate();
            tagsList.Items.Clear();
            m_Tags = new List<Ares.Tags.TagForLanguage>();
            if (m_LanguageId == -1 || m_LastCategoryId == -1)
            {
                deleteTagButton.Enabled = false;
                renameTagButton.Enabled = false;
                tagHiddenBox.Enabled = false;
                m_LastTagId = -1;
                tagsList.EndUpdate();
                return;
            }
            try
            {
                m_Tags = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllTags(m_LastCategoryId);
                int selIndex = -1;
                foreach (var tag in m_Tags)
                {
                    tagsList.Items.Add(tag.Name);
                    if (tag.Id == m_LastTagId)
                        selIndex = tagsList.Items.Count - 1;
                }
                if (selIndex == -1 && m_Tags.Count > 0)
                    selIndex = 0;
                if (selIndex != -1)
                {
                    m_LastTagId = m_Tags[selIndex].Id;
                    tagsList.SelectedIndex = selIndex;
                    tagHiddenBox.Checked = m_Project != null && m_Project.GetHiddenTags().Contains(m_LastTagId);
                }
                else
                {
                    tagHiddenBox.Checked = false;
                }
                deleteTagButton.Enabled = m_Tags.Count > 0;
                renameTagButton.Enabled = m_Tags.Count > 0;
                tagHiddenBox.Enabled = m_Project != null && m_Tags.Count > 0;
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                deleteTagButton.Enabled = false;
                renameTagButton.Enabled = false;
            }
            tagsList.EndUpdate();
        }

        private int m_LastTranslationId = -1;
        private IDictionary<int, string> m_Translations;
        private IDictionary<int, string> m_LanguageTranslations;

        private void UpdateTranslations()
        {
            translationsList.BeginUpdate();
            translationsList.Items.Clear();
            m_Translations = new Dictionary<int, string>();
            m_LanguageTranslations = new Dictionary<int, string>();
            translationsLabel.Text = StringResources.NoTranslationsLabelText;
            bool hasItem = false;
            if (m_LanguageId != -1)
            {
                try 
                {
                    var dbTranslations = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface;
                    switch (m_LastTranslationType)
                    {
                        case TranslationType.Category:
                            if (m_LastCategoryId != -1)
                            {
                                m_Translations = dbTranslations.GetCategoryTranslations(m_LastCategoryId);
                                translationsLabel.Text = String.Format(StringResources.TranslationsLabelText, StringResources.Category,
                                    categoriesList.SelectedItem);
                                hasItem = true;
                            }
                            break;
                        case TranslationType.Tag:
                            if (m_LastTagId != -1)
                            {
                                m_Translations = dbTranslations.GetTagTranslations(m_LastTagId);
                                translationsLabel.Text = String.Format(StringResources.TranslationsLabelText, StringResources.Tag,
                                    tagsList.SelectedItem);
                                hasItem = true;
                            }
                            break;
                        case TranslationType.Language:
                            if (m_LastLanguageId != -1)
                            {
                                m_Translations = dbTranslations.GetLanguageTranslations(m_LastLanguageId);
                                translationsLabel.Text = String.Format(StringResources.TranslationsLabelText, StringResources.Language,
                                    languagesList.SelectedItem);
                                hasItem = true;
                            }
                            break;
                        default:
                            break;
                    }
                    var languageTranslations = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId).GetAllLanguages();
                    foreach (var language in languageTranslations)
                    {
                        m_LanguageTranslations[language.Id] = language.Name;
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            int selIndex = -1;
            foreach (var translation in m_Translations)
            {
                if (translation.Key == m_LanguageId)
                    continue;
                if (!m_LanguageTranslations.ContainsKey(translation.Key))
                    continue;
                translationsList.Items.Add(m_LanguageTranslations[translation.Key]);
                translationsList.Items[translationsList.Items.Count - 1].SubItems.Add(translation.Value);
                translationsList.Items[translationsList.Items.Count - 1].Tag = translation.Key;
                if (translation.Key == m_LastTranslationId)
                    selIndex = translationsList.Items.Count - 1;
            }
            if (selIndex == -1 && translationsList.Items.Count > 0)
                selIndex = 0;
            if (selIndex != -1)
            {
                m_LastTranslationId = (int)translationsList.Items[selIndex].Tag;
                translationsList.SelectedIndices.Clear();
                translationsList.SelectedIndices.Add(selIndex);
            }
            if (translationsList.Items.Count == 0)
            {
                addTranslationButton.Enabled = hasItem && m_Languages.Count > 1;
                deleteTranslationButton.Enabled = false;
                renameTranslationButton.Enabled = false;
                translationsList.Columns[0].Width = (translationsList.Width - 20) / 2;
                translationsList.Columns[1].Width = (translationsList.Width - 20) / 2;
            }
            else
            {
                addTranslationButton.Enabled = m_Languages.Count > translationsList.Items.Count + 1;
                deleteTranslationButton.Enabled = m_LastTranslationType != TranslationType.Language ||  m_LastTranslationId != m_LastLanguageId;
                renameTranslationButton.Enabled = true;
                if (translationsList != null && translationsList.Columns.Count > 0)
                {
                    translationsList.Columns[0].Width = -1;
                    translationsList.Columns[1].Width = -1;
                }
            }
            translationsList.EndUpdate();
        }

        private void languageSelectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            UpdateSelectedTagLanguage();
        }

        private void categoriesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            CategorySelected();
        }

        private void tagsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            TagSelected();
        }

        private void languagesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            LanguageSelected();
        }

        private void addCategoryButton_Click(object sender, EventArgs e)
        {
            AddCategory();
        }

        private void AddCategory()
        {
            if (m_LanguageId == -1)
                return;

            String newName;
            if (Dialogs.InputDialogs.ShowInputDialog(this, StringResources.AddCategory, StringResources.NewCategoryName, out newName) == System.Windows.Forms.DialogResult.OK)
            {
                if (String.IsNullOrEmpty(newName))
                    return;
                if (m_Categories.FirstOrDefault((Ares.Tags.CategoryForLanguage item) => { return item.Name.Equals(newName, StringComparison.OrdinalIgnoreCase); }) != null)
                {
                    // category exists
                    MessageBox.Show(this, StringResources.CategoryExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    int newId = Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId).AddCategory(newName);
                    m_LastCategoryId = newId;
                    m_LastTranslationType = TranslationType.Category;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateCategories();
                    UpdateTags();
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void renameCategoryButton_Click(object sender, EventArgs e)
        {
            RenameCategory();
        }

        private void RenameCategory()
        {
            if (m_LanguageId == -1)
                return;
            if (m_LastCategoryId == -1)
                return;

            String newName;
            String oldName = categoriesList.SelectedItem != null ? categoriesList.SelectedItem.ToString() : String.Empty;
            if (Dialogs.InputDialogs.ShowInputDialog(this, StringResources.RenameCategory, StringResources.NewCategoryName2, oldName, out newName) == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(newName))
                    return;
                if (oldName.Equals(newName, StringComparison.Ordinal))
                    return;
                var existing = m_Categories.FirstOrDefault((Ares.Tags.CategoryForLanguage item) => { return item.Name.Equals(newName, StringComparison.OrdinalIgnoreCase); });
                if (existing != null && existing.Id != m_LastCategoryId)
                {
                    // category exists
                    MessageBox.Show(this, StringResources.CategoryExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId).SetCategoryName(m_LastCategoryId, newName);
                    m_LastTranslationType = TranslationType.Category;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateCategories();
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteCategoryButton_Click(object sender, EventArgs e)
        {
            DeleteCategory();
        }

        private void DeleteCategory()
        {
            if (m_LastCategoryId == -1)
                return;

            if (MessageBox.Show(this, StringResources.ReallyDeleteCategory, StringResources.Ares, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().WriteInterface.RemoveCategory(m_LastCategoryId);
                    m_LastCategoryId = -1;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateCategories();
                    UpdateTags();
                    UpdateTranslations();
                    m_Listen = true;
                    if (m_Project != null)
                    {
                        Ares.ModelInfo.ModelChecks.Instance.AdaptHiddenTags(m_Project);
                        Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addTagButton_Click(object sender, EventArgs e)
        {
            AddTag();
        }

        private void AddTag()
        {
            if (m_LanguageId == -1)
                return;

            String[] categories = new String[m_Categories.Count];
            for (int i = 0; i < categories.Length; ++i)
            {
                categories[i] = m_Categories[i].Name;
            }
            String selCat = categoriesList.SelectedIndex != -1 && categoriesList.Items.Count > 0 ? categoriesList.Items[categoriesList.SelectedIndex].ToString() : String.Empty;

            Dialogs.AddTagDialog dialog = new Dialogs.AddTagDialog();
            dialog.SetCategories(categories, selCat);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                String category = dialog.Category;
                String tagName = dialog.TagName;
                var existingCategory = m_Categories.FirstOrDefault((Ares.Tags.CategoryForLanguage item) => { return item.Name.Equals(category, StringComparison.OrdinalIgnoreCase); });
                int catId;
                try
                {
                    var dbWrite = Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId);
                    var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);
                    if (existingCategory != null)
                    {
                        catId = existingCategory.Id;
                    }
                    else
                    {
                        catId = dbWrite.AddCategory(category);
                    }
                    var allTags = dbRead.GetAllTags(catId);
                    var existingTag = allTags.FirstOrDefault((tagForLanguage) => { return tagForLanguage.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase); });
                    if (existingTag != null)
                    {
                        MessageBox.Show(this, StringResources.TagExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        m_LastTagId = existingTag.Id;
                    }
                    else
                    {
                        m_LastTagId = dbWrite.AddTag(catId, tagName);
                    }
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    if (m_LastCategoryId != catId)
                    {
                        m_LastCategoryId = catId;
                        UpdateCategories();
                    }
                    UpdateTags();
                    m_LastTranslationType = TranslationType.Tag;
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void renameTagButton_Click(object sender, EventArgs e)
        {
            RenameTag();
        }

        private void RenameTag()
        {
            if (m_LanguageId == -1)
                return;
            if (m_LastTagId == -1)
                return;

            String newName;
            String oldName = tagsList.SelectedItem != null ? tagsList.SelectedItem.ToString() : String.Empty;
            if (Dialogs.InputDialogs.ShowInputDialog(this, StringResources.RenameTag, StringResources.NewTagName, oldName, out newName) == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(newName))
                    return;
                if (oldName.Equals(newName, StringComparison.Ordinal))
                    return;
                var existing = m_Tags.FirstOrDefault((Ares.Tags.TagForLanguage item) => { return item.Name.Equals(newName, StringComparison.OrdinalIgnoreCase); });
                if (existing != null && existing.Id != m_LastCategoryId)
                {
                    // tag exists
                    MessageBox.Show(this, StringResources.TagExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LanguageId).SetTagName(m_LastTagId, newName);
                    m_LastTranslationType = TranslationType.Tag;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateTags();
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteTagButton_Click(object sender, EventArgs e)
        {
            DeleteTag();
        }

        private void DeleteTag()
        {
            if (m_LastTagId == -1)
                return;

            if (MessageBox.Show(this, StringResources.ReallyDeleteTag, StringResources.Ares, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().WriteInterface.RemoveTag(m_LastTagId);
                    m_LastTagId = -1;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateTags();
                    UpdateTranslations();
                    m_Listen = true;
                    if (m_Project != null)
                    {
                        Ares.ModelInfo.ModelChecks.Instance.AdaptHiddenTags(m_Project);
                        Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addTranslationButton_Click(object sender, EventArgs e)
        {
            AddOrRenameTranslation(-1, String.Empty);
        }

        private void AddOrRenameTranslation(int defaultLanguageId, String defaultName)
        {
            if (m_LanguageId == -1)
                return;
            if (m_Languages.Count < 2)
                return;

            String currentName = String.Empty;
            switch (m_LastTranslationType)
            {
                case TranslationType.Tag:
                    currentName = tagsList.SelectedItem.ToString();
                    break;
                case TranslationType.Category:
                    currentName = categoriesList.SelectedItem.ToString();
                    break;
                case TranslationType.Language:
                    currentName = languagesList.SelectedItem.ToString();
                    break;
                default:
                    return;
            }

            Dialogs.AddTranslationDialog dialog = new Dialogs.AddTranslationDialog();
            dialog.SetData(m_Languages, m_LanguageId, currentName, defaultName, defaultLanguageId);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                int langId = dialog.LanguageId;
                String name = dialog.Translation;
                try
                {
                    var dbWrite = Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(langId);
                    switch (m_LastTranslationType)
                    {
                        case TranslationType.Tag:
                            dbWrite.SetTagName(m_LastTagId, name);
                            break;
                        case TranslationType.Category:
                            dbWrite.SetCategoryName(m_LastCategoryId, name);
                            break;
                        case TranslationType.Language:
                            Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.SetLanguageName(langId, m_LastLanguageId, name);
                            break;
                        default:
                            break;
                    }
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void translationsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (translationsList.SelectedIndices.Count == 0)
            {
                renameTranslationButton.Enabled = false;
                deleteTranslationButton.Enabled = false;
            }
            else
            {
                m_LastTranslationId = (int)translationsList.Items[translationsList.SelectedIndices[0]].Tag;
                renameTranslationButton.Enabled = true;
                deleteTranslationButton.Enabled = m_LastTranslationType != TranslationType.Language ||  m_LastTranslationId != m_LastLanguageId;
            }
        }

        private void renameTranslationButton_Click(object sender, EventArgs e)
        {
            AddOrRenameTranslation(m_LastTranslationId, m_Translations[m_LastTranslationId]);
        }

        private void deleteTranslationButton_Click(object sender, EventArgs e)
        {
            DeleteTranslation();
        }

        private void DeleteTranslation()
        {
            if (m_LastTranslationId == -1)
                return;

            if (MessageBox.Show(this, StringResources.ReallyDeleteTranslation, StringResources.Ares, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    switch (m_LastTranslationType)
                    {
                        case TranslationType.Tag:
                            if (m_LastTagId == -1)
                                return;
                            Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LastTranslationId).RemoveTag(m_LastTagId);
                            break;
                        case TranslationType.Category:
                            if (m_LastCategoryId == -1)
                                return;
                            Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(m_LastTranslationId).RemoveCategory(m_LastCategoryId);
                            break;
                        case TranslationType.Language:
                            break;
                        default:
                            return;
                    }
                    m_LastTranslationId = -1;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateTranslations();
                    m_Listen = true;
                    if (m_Project != null)
                    {
                        Ares.ModelInfo.ModelChecks.Instance.AdaptHiddenTags(m_Project);
                        Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void addLanguageButton_Click(object sender, EventArgs e)
        {
            AddLanguage();
        }

        private void AddLanguage()
        {
            if (m_LanguageId == -1)
                return;
            String currentLanguageName = languageSelectionBox.SelectedItem.ToString();
            Dialogs.AddLanguageDialog dialog = new Dialogs.AddLanguageDialog();
            dialog.SetCurrentLanguageName(currentLanguageName);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                String name = dialog.NameInCurrentLanguage;
                String nameInNewLanguage = dialog.NameInNewLanguage;
                String langCode = dialog.LanguageCode;
                
                var existing = m_Languages.FirstOrDefault((Ares.Tags.LanguageForLanguage item) => { return item.Name.Equals(name, StringComparison.OrdinalIgnoreCase); });
                if (existing != null)
                {
                    MessageBox.Show(this, StringResources.LanguageExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    var dbIf = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface;
                    if (dbIf.HasLanguageForCode(langCode))
                    {
                        MessageBox.Show(this, String.Format(StringResources.LanguageExists2, currentLanguageName), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    int newId = dbIf.AddLanguage(langCode, nameInNewLanguage);
                    dbIf.SetLanguageName(m_LanguageId, newId, name);
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    m_LastTranslationType = TranslationType.Language;
                    m_LastLanguageId = newId;
                    UpdateLanguageSelectionBox();
                    UpdateLanguages();
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void renameLanguageButton_Click(object sender, EventArgs e)
        {
            RenameLanguage();
        }

        private void RenameLanguage()
        {
            if (m_LanguageId == -1)
                return;
            if (m_LastLanguageId == -1)
                return;

            String newName;
            String oldName = languagesList.SelectedItem != null ? languagesList.SelectedItem.ToString() : String.Empty;
            if (Dialogs.InputDialogs.ShowInputDialog(this, StringResources.RenameLanguage, StringResources.NewLanguageName, oldName, out newName) == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(newName))
                    return;
                if (oldName.Equals(newName, StringComparison.Ordinal))
                    return;
                var existing = m_Languages.FirstOrDefault((Ares.Tags.LanguageForLanguage item) => { return item.Name.Equals(newName, StringComparison.OrdinalIgnoreCase); });
                if (existing != null && existing.Id != m_LastLanguageId)
                {
                    // tag exists
                    MessageBox.Show(this, StringResources.LanguageExists, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.SetLanguageName(m_LanguageId, m_LastLanguageId, newName);
                    m_LastTranslationType = TranslationType.Language;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateLanguageSelectionBox();
                    UpdateLanguages();
                    UpdateTranslations();
                    m_Listen = true;
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void deleteLanguageButton_Click(object sender, EventArgs e)
        {
            DeleteLanguage();
        }

        private void DeleteLanguage()
        {
            if (m_LanguageId == -1)
                return;
            if (m_LastLanguageId == m_LanguageId)
                return;
            if (MessageBox.Show(this, StringResources.ReallyDeleteLanguage, StringResources.Ares, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                try
                {
                    Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.RemoveLanguage(m_LastLanguageId);
                    m_LastLanguageId = -1;
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateLanguageSelectionBox();
                    UpdateLanguages();
                    UpdateTranslations();
                    m_Listen = true;
                    if (m_Project != null)
                    {
                        Ares.ModelInfo.ModelChecks.Instance.AdaptHiddenTags(m_Project);
                        Ares.ModelInfo.ModelChecks.Instance.CheckAll(m_Project);
                    }
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void categoryHiddenBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (m_Project != null)
            {
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.HideCategoryAction(m_LastCategoryId, categoryHiddenBox.Checked, m_Project), m_Project);
            }
        }

        private void tagHiddenBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_Listen)
                return;
            if (m_Project != null)
            {
                Ares.Editor.Actions.Actions.Instance.AddNew(new Ares.Editor.Actions.HideTagAction(m_LastTagId, tagHiddenBox.Checked, m_Project), m_Project);
            }
        }

        private void cleanupDBButton_Click(object sender, EventArgs e)
        {
            try
            {
                String tempFileName = System.IO.Path.GetTempFileName() + ".txt";
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(tempFileName))
                {
                    Ares.Tags.TagsModule.GetTagsDB().WriteInterface.CleanupDB(writer, Ares.Settings.Settings.Instance.MusicDirectory, m_LanguageId);
                    Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                    m_Listen = false;
                    UpdateAll();
                    m_Listen = true;
                    if (MessageBox.Show(this, StringResources.TagsCleanedUp, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.None) == System.Windows.Forms.DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(tempFileName);
                    }
                }
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}
