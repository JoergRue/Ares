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

namespace Ares.Editor.ElementEditors
{
    public partial class FileTagsEditor
#if !MONO
 : WeifenLuo.WinFormsUI.Docking.DockContent
#else
        : Form
#endif
    {
        public FileTagsEditor()
        {
            HideOnClose = true;
            InitializeComponent();
            Ares.Editor.Actions.TagChanges.Instance.TagsDBChanged += new EventHandler<EventArgs>(TagsDBChanged);
        }

#if !MONO
        protected override string GetPersistString()
        {
            return "FileTagsEditor";
        }
#endif

        private void id3Button_Click(object sender, EventArgs e)
        {
            Dialogs.AddID3TagsDialog dialog = new Dialogs.AddID3TagsDialog();
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                bool interpret = dialog.Interpret;
                bool album = dialog.Album;
                bool genre = dialog.Genre;
                bool mood = dialog.Mood;
                if (!interpret && !album && !genre && !mood)
                    return;

                int languageId = m_Project != null ? m_Project.TagLanguageId : -1;
                if (languageId == -1)
                {
                    try
                    {
                        languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                    }
                    catch (Ares.Tags.TagsDbException ex)
                    {
                        MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                var tokenSource = new System.Threading.CancellationTokenSource();
                Ares.CommonGUI.ProgressMonitorBase monitor = new TaskProgressMonitor(this, StringResources.ExtractingTags, tokenSource);

                var task = Ares.TagsImport.TagExtractor.ExtractTags(monitor, m_Files, Ares.Settings.Settings.Instance.MusicDirectory, languageId, interpret, album, genre, mood, tokenSource);
                task.ContinueWith((task2) =>
                {
                    monitor.Close();
                    if (task.Exception != null)
                    {
                        TaskHelpers.HandleTaskException(this, task.Exception, StringResources.TagExtractionError);
                    }
                    else
                    {
                        Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                        UpdateAll();
                    }
                }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.None, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void confirmButton_Click(object sender, EventArgs e)
        {
            Ares.Tags.TagsModule.GetTagsDB().WriteInterface.ConfirmTags(m_Files);
        }

        private void shareButton_Click(object sender, EventArgs e)
        {
            String user = Settings.Settings.Instance.OnlineDBUserId;
            if (String.IsNullOrEmpty(user))
            {
                DialogResult res = MessageBox.Show(this, StringResources.OnlineDbUserIdRequired, StringResources.Ares, MessageBoxButtons.OKCancel);
                if (res == DialogResult.OK)
                {
                    m_Parent.SetOnlineUserId();
                    user = Settings.Settings.Instance.OnlineDBUserId;
                }
            }
            if (String.IsNullOrEmpty(user))
            {
                // cancelled or didn't set a user name
                return;
            }

            bool includeLog = Settings.Settings.Instance.ShowDialogAfterUpload;

            System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
            TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.ExtractingMusicIds, tokenSource);
            monitor.IncreaseProgress(0.0, StringResources.ExtractingMusicIds);
            List<String> usedFiles = new List<String>();
            var readIf = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(m_LanguageId);
            foreach (String file in m_Files)
            {
                if (readIf.GetTagsForFile(file).Count > 0)
                {
                    usedFiles.Add(file);
                }
            }
            var task = Ares.TagsImport.MusicIdentification.UpdateMusicIdentification(monitor, usedFiles, Ares.Settings.Settings.Instance.MusicDirectory, tokenSource);
            var task2 = task.ContinueWith((t) =>
            {
                return Ares.TagsImport.GlobalDbUpload.UploadTags(monitor, usedFiles, user, includeLog, tokenSource);
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.Default);
            
            task2.ContinueWith((t) =>
            {
                monitor.Close();
                if (includeLog)
                {
                    String log = task2.Result;
                    var dialog = new Dialogs.OnlineDbResultDialog();
                    dialog.Log = log;
                    dialog.SetIsUpload();
                    dialog.ShowDialog(this);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            
            task.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.MusicIdExtractionError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            task2.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.MusicIdExtractionError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            bool includeLog = Settings.Settings.Instance.ShowDialogAfterDownload;

            System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
            TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.ExtractingMusicIds, tokenSource);
            monitor.IncreaseProgress(0.0, StringResources.ExtractingMusicIds);
            var task = Ares.TagsImport.MusicIdentification.UpdateMusicIdentification(monitor, m_Files, Ares.Settings.Settings.Instance.MusicDirectory, tokenSource);
            int nrOfFoundFiles = 0;
            var task2 = task.ContinueWith((t) =>
            {
                return Ares.TagsImport.GlobalDbDownload.DownloadTags(monitor, m_Files, out nrOfFoundFiles, includeLog, tokenSource);
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.Default);

            task2.ContinueWith((t) =>
            {
                monitor.Close();
                Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
                UpdateAll();
                if (includeLog)
                {
                    String log = task2.Result;
                    var dialog = new Dialogs.OnlineDbResultDialog();
                    dialog.Log = log;
                    dialog.SetIsDownload(m_Files.Count, nrOfFoundFiles);
                    dialog.ShowDialog(this);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            task2.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.MusicIdExtractionError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
            task.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.MusicIdExtractionError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void addTagButton_Click(object sender, EventArgs e)
        {
            Dialogs.AddTagDialog dialog = new Dialogs.AddTagDialog();
            String[] categories = new String[m_Categories.Count];
            for (int i = 0; i < m_Categories.Count; ++i)
                categories[i] = m_Categories[i].Name;
            String selCat = ((categoriesBox.SelectedIndex != -1 && categoriesBox.SelectedIndex != categoriesBox.Items.Count - 1)
                ? categoriesBox.SelectedItem.ToString() : String.Empty);
            dialog.SetCategories(categories, selCat);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                String category = dialog.Category;
                String tagName = dialog.TagName;
                AddTag(category, tagName);
            }
        }

        void TagsDBChanged(object sender, EventArgs e)
        {
            if (sender != this)
            {
                UpdateAll();
            }
        }

        private Ares.Data.IProject m_Project;
        private List<String> m_Files;
        private Dictionary<int, int> m_FileTags = new Dictionary<int, int>();
        private IFileTagsEditorParent m_Parent;

        public void SetParent(IFileTagsEditorParent parent)
        {
            m_Parent = parent;
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
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
                    return;
                }
            }
            UpdateControls();
        }

        public void SetFiles(IList<String> files)
        {
            m_Files = files != null ? new List<String>(files) : new List<String>();
            UpdateAll();
        }

        private void UpdateAll()
        {
            m_FileTags.Clear();

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

                if (m_Files.Count == 1)
                {
                    titleLabel.Text = String.Format(StringResources.TagsForSingleFile, m_Files[0]);
                }
                else
                {
                    titleLabel.Text = String.Format(StringResources.TagsForMultipleFiles, m_Files.Count);
                }

                UpdateControls();
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int m_LanguageId;

        private bool m_Listen = true;

        private IList<CategoryForLanguage> m_Categories;
        private IList<TagForLanguage> m_Tags;
        private IList<LanguageForLanguage> m_Languages;
        private int m_SelectedCategoryId = -1;

        private void UpdateControls()
        {
            bool hasFiles = (m_Files != null && m_Files.Count > 0);
            tagsBox.Enabled = hasFiles;
            shareButton.Enabled = hasFiles;
            downloadButton.Enabled = hasFiles;
            id3Button.Enabled = hasFiles;
            addTagButton.Enabled = hasFiles;
            confirmButton.Enabled = hasFiles;

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
                if (m_SelectedCategoryId == -2)
                    selIndex = categoriesBox.Items.Count - 1;
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
                AddTagToFiles(tagId);
            }
            else if (e.NewValue == CheckState.Unchecked)
            {
                RemoveTagFromFiles(tagId);
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
                    AddTagToFiles(tagId);
                    UpdateControls();
                }
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddTagToFiles(int tagId)
        {
            m_FileTags[tagId] = m_Files.Count;
            List<IList<int>> newTags = new List<IList<int>>();
            for (int i = 0; i < m_Files.Count; ++i)
            {
                newTags.Add(new List<int>() { tagId });
            }
            Ares.Tags.TagsModule.GetTagsDB().WriteInterface.AddFileTags(m_Files, newTags);
            Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
        }

        private void RemoveTagFromFiles(int tagId)
        {
            m_FileTags.Remove(tagId);
            List<IList<int>> removedTags = new List<IList<int>>();
            for (int i = 0; i < m_Files.Count; ++i)
            {
                removedTags.Add(new List<int>() { tagId });
            }
            Ares.Tags.TagsModule.GetTagsDB().WriteInterface.RemoveFileTags(m_Files, removedTags);
            Ares.Editor.Actions.TagChanges.Instance.FireTagsDBChanged(this);
        }
    }

    public interface IFileTagsEditorParent
    {
        void SetOnlineUserId();
    }
}
