﻿/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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

namespace Ares.Editor
{
    partial class FileExplorer : ToolWindow
    {
        private static ImageList sImageList = null;

        static FileExplorer()
        {
            sImageList = new ImageList();
            sImageList.Images.Add(ImageResources.Folder);
            sImageList.Images.Add(ImageResources.sounds1);
            sImageList.Images.Add(ImageResources.music1);
            sImageList.Images.Add(ImageResources.eventlogError);
        }

        private bool updatePending = false;

        private void ElementChanged(int elementId, Ares.Editor.Actions.ElementChanges.ChangeType changeType)
        {
            if (updatePending) return;
            updatePending = true;
            BeginInvoke(new Action(() =>
            {
                updatePending = false;
                updateInformationPanel();
                if (m_ProjectUseFilter.FilterMode != ProjectUseFilterMode.NoFilter && m_ProjectUseFilter.AutoUpdate)
                {
                    switch (changeType)
                    {
                        case Actions.ElementChanges.ChangeType.Changed:
                        case Actions.ElementChanges.ChangeType.Removed:
                            ReFillTree();
                            break;
                        default:
                            break;
                    }
                }
            }));
        }

        private IFileExplorerParent m_Parent;
        private Ares.Data.IProject m_Project;

        public FileExplorer(FileType fileType, IFileExplorerParent parent)
        {
            InitializeComponent();
            m_Parent = parent;
            this.Text = String.Format(StringResources.FileExplorerTitle, fileType == FileType.Music ? StringResources.Music : StringResources.Sounds);
            m_FileType = fileType;
            treeView1.ImageList = sImageList;
            if (fileType != FileType.Music)
            {
                tagFilterButton.Enabled = false;
                tagFilterButton.Visible = false;
                tagButton.Enabled = false;
                tagButton.Visible = false;
            }
            ReFillTree();
            if (fileType == FileType.Music)
            {
                Actions.FilesWatcher.Instance.MusicDirChanges += new EventHandler<EventArgs>(DirChanged);
                Actions.TagChanges.Instance.TagsDBChanged += new EventHandler<EventArgs>(Instance_TagsDBChanged);
                this.Icon = ImageResources.musicIcon;
            }
            else
            {
                Actions.FilesWatcher.Instance.SoundDirChanges += new EventHandler<EventArgs>(DirChanged);
                this.Icon = ImageResources.sounds;
            }
            if (Height > 200)
            {
                splitContainer1.SplitterDistance = Height - 100;
            }
            Ares.Editor.Actions.ElementChanges.Instance.AddListener(-1, ElementChanged);
        }

        void Instance_TagsDBChanged(object sender, EventArgs e)
        {
            if (m_TagsFilter.FilterMode != TagsFilterMode.NoFilter && mUpdateTreeOnTagChange)
            {
                RetrieveFilteredFiles();
                ReFillTree();
            }
            updateInformationPanel();
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
        }

        private void  DirChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ReFillTree));
            }
            else
            {
                ReFillTree();
            }
        } 
        
        FileType m_FileType;

        protected override string GetPersistString()
        {
            return "FileExplorer_" + (int) m_FileType;
        }

        private bool m_TreeLocked = false;

        public void ReFillTree()
        {
            if (m_TreeLocked)
                return;

            Application.UseWaitCursor = true;
            this.UseWaitCursor = true;
            Application.DoEvents();
            try
            {
                Dictionary<String, bool> states = new Dictionary<string, bool>();
                GetTreeStates(m_Root, states);

                treeView1.BeginUpdate();
                treeView1.Nodes.Clear();
                m_Root = new TreeNode(m_FileType == FileType.Music ? StringResources.Music : StringResources.Sounds);
                m_Root.SelectedImageIndex = m_Root.ImageIndex = 0;
                m_Root.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = m_FileType, RelativePath = String.Empty };
                m_Root.ContextMenuStrip = fileNodeContextMenu;
                String directory = m_FileType == FileType.Music ? Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
                FillTreeNode(m_Root, directory, directory, m_FileType, states, new ModelInfo.FileSearch());
                treeView1.Nodes.Add(m_Root);
                m_Root.Expand();
                treeView1.SelectedNode = m_Root;
                treeView1.EndUpdate();
                if (m_FileType == FileType.Music)
                {
                    m_Parent.SetSelectedFiles(GetSelectedFiles());
                }
            }
            finally
            {
                this.UseWaitCursor = false;
                Application.UseWaitCursor = false;
            }
        }

        private void GetTreeStates(TreeNode rootNode, Dictionary<String, bool> states)
        {
            if (rootNode == null)
                return;
            if (rootNode.Tag is DraggedItem)
                states[((DraggedItem)rootNode.Tag).RelativePath] = rootNode.IsExpanded;
            foreach (TreeNode child in rootNode.Nodes)
                GetTreeStates(child, states);
        }

        private void FillTreeNode(TreeNode node, String directory, String root, FileType dirType, Dictionary<String, bool> states, Ares.ModelInfo.FileSearch fileSearch)
        {
            try
            {
                String[] subDirs = System.IO.Directory.GetDirectories(directory);
                Array.Sort(subDirs);
                int subLength = directory.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) ? directory.Length : directory.Length + 1;
                int rootLength = root.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) ? root.Length : root.Length + 1;
                foreach (String subDir in subDirs)
                {
                    TreeNode subNode = new TreeNode(subDir.Substring(subLength));
                    subNode.ImageIndex = subNode.SelectedImageIndex = 0;
                    String relativeDir = subDir.Substring(rootLength);
                    subNode.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = dirType, RelativePath = relativeDir };
                    FillTreeNode(subNode, subDir, root, dirType, states, fileSearch);
                    if (m_TagsFilter.FilterMode == TagsFilterMode.NoFilter || subNode.Nodes.Count > 0)
                    {
                        subNode.ContextMenuStrip = fileNodeContextMenu;
                        node.Nodes.Add(subNode);
                        if (states.ContainsKey(relativeDir) && states[relativeDir])
                        {
                            subNode.Expand();
                        }
                    }
                }
                List<string> files = new List<string>();
                files.AddRange(FileSearch.GetFilesInDirectory(dirType, directory, false));
                files.Sort(StringComparer.CurrentCulture);
                foreach (String file in files)
                {
                    String relativeDir = file.Substring(rootLength);
                    if (m_TagsFilter.FilterMode == TagsFilterMode.NormalFilter && !m_FilteredFiles.Contains(relativeDir))
                        continue;
                    else if (m_TagsFilter.FilterMode == TagsFilterMode.UntaggedFiles && m_FilteredFiles.Contains(relativeDir))
                        continue;

                    switch (m_ProjectUseFilter.FilterMode)
                    {
                        case ProjectUseFilterMode.UnusedFilter:
                        case ProjectUseFilterMode.UsedFilter:
                            List<KeyValuePair<Ares.Data.IMode, List<Ares.Data.IModeElement>>> modeElements = fileSearch.GetRootElements(m_Project, relativeDir, 
                                dirType == FileType.Music ? Data.SoundFileType.Music : Data.SoundFileType.SoundEffect);
                            if (m_ProjectUseFilter.FilterMode == ProjectUseFilterMode.UnusedFilter && modeElements.Count > 0) continue;
                            else if (m_ProjectUseFilter.FilterMode == ProjectUseFilterMode.UsedFilter && modeElements.Count == 0) continue;
                            break;
                        case ProjectUseFilterMode.NoFilter:
                        default:
                            break;
                    }

                    TreeNode subNode = new TreeNode(file.Substring(subLength));
                    subNode.ImageIndex = subNode.SelectedImageIndex = (dirType == FileType.Sound ? 1 : 2);
                    subNode.Tag = new DraggedItem { NodeType = DraggedItemType.File, ItemType = dirType, RelativePath = relativeDir };
                    subNode.ContextMenuStrip = fileNodeContextMenu;
                    node.Nodes.Add(subNode);
                    if (states.ContainsKey(relativeDir) && states[relativeDir])
                    {
                        subNode.Expand();
                    }
                }
            }
            catch (Exception e)
            {
                TreeNode subNode = new TreeNode(e.Message);
                subNode.SelectedImageIndex = subNode.ImageIndex = 3;
                node.Nodes.Add(subNode);
            }
        }

        TreeNode m_Root;

        Rectangle m_DragStartRect;
        bool m_InDrag;

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Size dragSize = SystemInformation.DragSize;
                m_DragStartRect = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
                m_InDrag = true;
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            m_InDrag = false;
        }

        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_InDrag && m_DragStartRect != null && !m_DragStartRect.Contains(e.X, e.Y))
            {
                List<DraggedItem> items = new List<DraggedItem>();
                treeView1.SelectedNodes.ForEach(node => items.Add((DraggedItem)node.Tag));
                FileDragInfo info = new FileDragInfo();
                info.Source = FileSource.LocalFileSystem;
                info.DraggedItems = items;
                info.TagsFilter = m_TagsFilter;

                DragDropEffects allowedEffects = DragDropEffects.Copy;
                if (Settings.Settings.Instance.AllowFileMove)
                {
                    allowedEffects = DragDropEffects.Move;
                }

                DoDragDrop(info, allowedEffects);
                m_InDrag = false;
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            PlaySelectedFile();
        }

        private void PlaySelectedFile()
        {
            TreeNode node = treeView1.SelectedNode;
            DraggedItem item = node.Tag as DraggedItem;
            if (item.ItemType == FileType.Music)
            {
                String player = Ares.Settings.Settings.Instance.ExternalMusicPlayer;
                if (!String.IsNullOrEmpty(player))
                {
                    String basePath = Settings.Settings.Instance.MusicDirectory;
                    String filePath = System.IO.Path.Combine(basePath, item.RelativePath);
                    try
                    {
                        System.Diagnostics.Process.Start(player, "\"" + filePath + "\"");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(this, String.Format(StringResources.ExternalPlayerStartFail, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return;
                }
            }
            m_PlayedElement = Actions.Playing.Instance.PlayFile(item.RelativePath, item.ItemType == FileType.Music, this, () =>
                {
                    m_PlayedElement = null;
                    stopButton.Enabled = false;
                    playButton.Enabled = PlayingPossible;
                });
            stopButton.Enabled = true;
            playButton.Enabled = false;
        }

        private bool PlayingPossible
        {
            get
            {
                if (m_PlayedElement != null) 
                    return false;
                TreeNode node = treeView1.SelectedNode;
                if (node == null)
                    return false;
                DraggedItem item = node.Tag as DraggedItem;
                if (item == null)
                    return false;
                return item.NodeType == DraggedItemType.File;
            }
        }

        private Ares.Data.IElement m_PlayedElement;

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (m_PlayedElement != null)
            {
                Actions.Playing.Instance.StopElement(m_PlayedElement);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            playButton.Enabled = PlayingPossible;
            updateInformationPanel();
            if (m_FileType == FileType.Music)
            {
                m_Parent.SetSelectedFiles(GetSelectedFiles());
            }
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            DefaultNodeAction();
        }

        private void updateInformationPanel()
        {
            String msg = String.Empty;
            TreeNode node = treeView1.SelectedNode;

            if (node != null)
            {
                DraggedItem item = node.Tag as DraggedItem;
                if (!String.IsNullOrEmpty(item.RelativePath) && item.NodeType != DraggedItemType.Directory)
                {
                    Ares.Data.SoundFileType soundFileType = (item.ItemType == FileType.Music) ? Ares.Data.SoundFileType.Music : Ares.Data.SoundFileType.SoundEffect;

                    String path = soundFileType == Data.SoundFileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                    path = System.IO.Path.Combine(path, item.RelativePath);
                    Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(path, true, true);
                    if (tag != null)
                    {
                        TimeSpan duration = TimeSpan.FromSeconds(tag.duration);
                        msg = String.Format(StringResources.Length, (DateTime.Today + duration).ToString("HH::mm::ss.fff"));
                    }
                    else
                    {
                        msg = String.Format(StringResources.Length, StringResources.Unknown);
                    }

                    Ares.ModelInfo.FileSearch fileSearch = new ModelInfo.FileSearch();
                    List<KeyValuePair<Ares.Data.IMode, List<Ares.Data.IModeElement>>> modeElements = fileSearch.GetRootElements(m_Project, item.RelativePath, soundFileType);
                    if (modeElements.Count > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append(String.Format(StringResources.SearchElementsTitle, node.Text) + Environment.NewLine);
                        foreach (KeyValuePair<Ares.Data.IMode, List<Ares.Data.IModeElement>> modesData in modeElements)
                        {
                            builder.Append(String.Format(StringResources.ModeIntro, modesData.Key.Title));
                            for (int i = 0; i < modesData.Value.Count; ++i)
                            {
                                builder.Append(modesData.Value[i].Title);
                                builder.Append(i + 1 == modesData.Value.Count ? Environment.NewLine : ", ");
                            }
                        }
                        msg += Environment.NewLine + " " + Environment.NewLine + builder.ToString();
                    }
                    else
                    {
                        msg += Environment.NewLine + " " + Environment.NewLine + String.Format(StringResources.NoElementsFound, node.Text);
                    }

                    if (soundFileType == Data.SoundFileType.Music)
                    {
                        try
                        {
                            int languageId = m_Project != null ? m_Project.TagLanguageId : -1;
                            if (languageId == -1)
                                languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                            var tags = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId).GetTagsForFile(item.RelativePath);
                            StringBuilder builder = new StringBuilder();
                            int lastCatId = -1;
                            foreach (var tagInfo in tags)
                            {
                                if (tagInfo.CategoryId != lastCatId)
                                {
                                    if (builder.Length > 0)
                                        builder.Append(";");
                                    builder.Append(tagInfo.Category);
                                    builder.Append(": ");
                                    lastCatId = tagInfo.CategoryId;
                                }
                                else
                                {
                                    builder.Append(", ");
                                }
                                builder.Append(tagInfo.Name);
                            }
                            if (tags.Count > 0)
                            {
                                msg += Environment.NewLine + StringResources.Tags + builder.ToString();
                            }
                            else
                            {
                                msg += Environment.NewLine + StringResources.NoTags;
                            }
                        }
                        catch (Ares.Tags.TagsDbException /*ex*/)
                        {
                            // ignore here
                        }
                    }
                }
            }
            informationBox.Text = msg;
        }


        private void DefaultNodeAction()
        {
            if (m_PlayedElement != null)
            {
                Actions.Playing.Instance.StopElement(m_PlayedElement);
            }
            if (PlayingPossible)
            {
                PlaySelectedFile();
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RetrieveFilteredFiles();
            ReFillTree();
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                DefaultNodeAction();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                CopyFiles();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.X)
            {
                CutFiles();
                e.Handled = true;
            }
            else if (e.Control && e.KeyCode == Keys.V)
            {
                PasteFiles();
                e.Handled = true;
            }
        }

        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySelectedFile();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (m_PlayedElement != null)
            {
                Actions.Playing.Instance.StopElement(m_PlayedElement);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode node = treeView1.SelectedNode;
            if (node == null || node.Tag == null)
                return;
            DraggedItem item = (DraggedItem)node.Tag;
            if (item.NodeType != DraggedItemType.File)
                return;
            String editor = Ares.Settings.Settings.Instance.SoundFileEditor;
            if (String.IsNullOrEmpty(editor))
            {
                if (MessageBox.Show(this, StringResources.NoSoundFileEditor, StringResources.Ares, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    m_Parent.SetEditor();
                }
                editor = Ares.Settings.Settings.Instance.SoundFileEditor;
            }
            if (!String.IsNullOrEmpty(editor))
            {
                String basePath = item.ItemType == FileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                String filePath = System.IO.Path.Combine(basePath, item.RelativePath);
                try
                {
                    System.Diagnostics.Process.Start(editor, "\"" + filePath + "\"");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.SoundFileEditorStartFail, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void fileNodeContextMenu_Opening(object sender, CancelEventArgs e)
        {
            stopToolStripMenuItem.Enabled = m_PlayedElement != null;
            playToolStripMenuItem.Enabled = PlayingPossible;
            editToolStripMenuItem.Enabled = m_Parent != null && treeView1.SelectedNode != null && treeView1.SelectedNode.Tag != null &&
                treeView1.SelectedNode.Tag is DraggedItem && ((DraggedItem)treeView1.SelectedNode.Tag).NodeType == DraggedItemType.File;
            pasteToolStripMenuItem.Enabled = Clipboard.ContainsFileDropList() || Clipboard.ContainsData(DataFormats.GetFormat("AresFilesList").Name);
            editTagsMenuItem.Visible = m_FileType == FileType.Music;
            editTagsMenuItem.Enabled = m_FileType == FileType.Music && treeView1.SelectedNodes.Count > 0;
            if (editTagsMenuItem.Enabled)
            {
                // Disable if called only on empty directories
                if (GetSelectedFiles().Count == 0)
                    editTagsMenuItem.Enabled = false;
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            doSearch();
        }

        [Flags]
        private enum SearchTypes
        {
            Children = 0x1,
            Siblings = 0x2
        }

        private void doSearch()
        {
            String text = searchBox.Text;
            TreeNode node = treeView1.SelectedNode;
            if (node == null)
            {
                node = m_Root;
            }
            if (node == null || String.IsNullOrEmpty(text))
                return;
            bool found = Search(node, text, SearchTypes.Children | SearchTypes.Siblings);
            if (!found)
            {
                // end of tree: start again at the beginning
                found = SearchSingleNode(m_Root, text);
            }
            if (!found && node != m_Root)
            {
                found = Search(m_Root, text, SearchTypes.Children);
            }
            if (!found)
            {
                MessageBox.Show(StringResources.NoFileOrFolderFound, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (treeView1.SelectedNode == node)
            {
                MessageBox.Show(StringResources.NoFurtherFileOrFolderFound, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private bool SearchSingleNode(TreeNode node, String text)
        {
            if (node.Text.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) != -1)
            {
                treeView1.SelectedNode = node;
                node.EnsureVisible();
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool Search(TreeNode node, String text, SearchTypes searchTypes)
        {
            // do not consider the starting node

            // first search children
            if ((searchTypes & SearchTypes.Children) != 0)
            {
                foreach (TreeNode child in node.Nodes)
                {
                    if (SearchSingleNode(child, text))
                    {
                        return true;
                    }
                    else if (Search(child, text, SearchTypes.Children))
                    {
                        return true;
                    }
                }
            }

            if (searchTypes == SearchTypes.Children)
                return false;

            // then search siblings
            TreeNode sibling = node.NextNode;
            if (sibling != null)
            {
                if (SearchSingleNode(sibling, text))
                {
                    return true;
                }
                return Search(sibling, text, SearchTypes.Children | SearchTypes.Siblings);
            }

            // last sibling: search parent's siblings
            TreeNode parent = node.Parent;
            if (parent != null)
            {
                // do not check parent node: it's earlier in the order
                return Search(parent, text, SearchTypes.Siblings);
            }
            else
            {
                // end of tree
                return false;
            }
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                e.Handled = true;
                doSearch();
            }
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            CheckAllowedDrop(e);
        }

        private void CheckAllowedDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop) || e.Data.GetDataPresent(typeof(FileDragInfo)))
            {
                if (((e.KeyState & 8) == 8) && ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
                {
                    e.Effect = DragDropEffects.Move;
                }
                else if (e.Data.GetDataPresent(typeof(FileDragInfo)) && ((FileDragInfo)e.Data.GetData(typeof(FileDragInfo))).Source == FileSource.Online && ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy))
                {
                    e.Effect = DragDropEffects.Copy;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            CheckAllowedDrop(e);
        }

        private void treeView1_DragLeave(object sender, EventArgs e)
        {
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            bool move = false;
            if (((e.KeyState & 8) == 8) && ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy))
            {
                move = false;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                move = true;
            }
            else if (e.Data.GetDataPresent(typeof(FileDragInfo)) && ((FileDragInfo)e.Data.GetData(typeof(FileDragInfo))).Source == FileSource.Online && ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy))
            {
                move = false;
            }
            else
            {
                return;
            }

            bool onlineSource = false;

            List<String> files = new List<String>();
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                if (a != null)
                {
                    for (int i = 0; i < a.Length; ++i)
                        files.Add(a.GetValue(i).ToString());
                }
            }
            else if (e.Data.GetDataPresent(typeof(FileDragInfo)))
            {
                var fileDragInfo = ((FileDragInfo)e.Data.GetData(typeof(FileDragInfo)));
                if (fileDragInfo.Source == FileSource.LocalFileSystem)
                {
                    List<DraggedItem> items = fileDragInfo.DraggedItems;
                    if (items != null)
                    {
                        for (int i = 0; i < items.Count; ++i)
                        {
                            String dir = items[i].ItemType == FileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                            String path = System.IO.Path.Combine(dir, items[i].RelativePath);
                            files.Add(path);
                        }
                    }
                }
                else
                {
                    onlineSource = true;
                }
            }
            else
                return;

            TreeNode node = treeView1.GetNodeAt(treeView1.PointToClient(new Point(e.X, e.Y)));
            if (node == null)
                node = m_Root;
            if (onlineSource)
            {
                if (Ares.AudioSource.TargetDirectoryProvider.Current == null)
                {
                    return;
                }
                String targetDir = m_FileType == FileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
                String targetPath = System.IO.Path.Combine(targetDir, ((DraggedItem)node.Tag).RelativePath);
                if (!System.IO.Directory.Exists(targetPath))
                {
                    targetPath = System.IO.Directory.GetParent(targetPath).FullName;
                }
                if (!System.IO.Directory.Exists(targetPath))
                {
                    return;
                }
                Ares.AudioSource.TargetDirectoryProvider.Current.SetBaseTargetDirectory(targetPath);
            }
            else
            {
                this.Invoke(new Action(() => DropFiles(files, move, node)));
                this.Activate();
            }
        }

        private void DropFiles(List<String> files, bool move, TreeNode targetNode)
        {
            String targetDir = m_FileType == FileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
            String targetPath = System.IO.Path.Combine(targetDir, ((DraggedItem)targetNode.Tag).RelativePath);
            if (!System.IO.Directory.Exists(targetPath))
            {
                targetPath = System.IO.Directory.GetParent(targetPath).FullName;
            }
            if (!System.IO.Directory.Exists(targetPath))
            {
                return;
            }

            Dictionary<String, object> uniqueElements = GetUniqueParents(files);

            // check whether move is possible (no move of parent into child)
            foreach (String file in uniqueElements.Keys)
            {
                if (targetPath.Equals(file, StringComparison.InvariantCultureIgnoreCase))
                {
                    // special case didn't really move (source == target)
                    return;
                }
                if (targetPath.StartsWith(file, StringComparison.InvariantCultureIgnoreCase))
                {
                    MessageBox.Show(this, StringResources.CopyOrMoveParentIntoChild, StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            m_TreeLocked = true;
            Ares.Settings.Settings settings = Ares.Settings.Settings.Instance;
            System.Threading.CancellationTokenSource tokenSource = new System.Threading.CancellationTokenSource();
            Ares.CommonGUI.ProgressMonitorBase monitor = new TaskProgressMonitor(this, move ? StringResources.Moving : StringResources.Copying, tokenSource);
            var task = Ares.TagsImport.FileOperations.CopyOrMoveAsync(monitor, m_Project, settings.MusicDirectory, settings.SoundDirectory, uniqueElements, move, targetPath, tokenSource.Token);
            task.ContinueWith( (task2) =>
            {
                monitor.Close();
                if (task.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, task.Exception, StringResources.FileOpError);
                }
                m_TreeLocked = false;
                ReFillTree();
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.None, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private Dictionary<String, object> GetUniqueParents(List<String> files)
        {
            // make files unique
            Dictionary<String, object> uniqueElements = new Dictionary<string, object>();
            foreach (String file in files)
            {
                uniqueElements.Add(file, null);
            }
            // remove all files / directories of which a parent directory is already included
            List<String> copy = new List<String>(uniqueElements.Keys);
            foreach (String file in copy)
            {
                System.IO.DirectoryInfo parent = System.IO.Directory.GetParent(file);
                while (parent != null)
                {
                    if (copy.Contains(parent.FullName))
                    {
                        uniqueElements.Remove(file);
                        break;
                    }
                    parent = System.IO.Directory.GetParent(parent.FullName);
                }
            }
            return uniqueElements;
        }

        [Serializable]
        private class ClipboardFilesList
        {
            public List<String> Files { get; set; }
            public bool FilesAreCut { get; set; }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteFiles();
        }

        private void AddFilesToList(List<String> files, String directory)
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(directory);
            foreach (System.IO.DirectoryInfo subDir in info.GetDirectories())
            {
                AddFilesToList(files, subDir.FullName);
            }
            foreach (System.IO.FileInfo file in info.GetFiles())
            {
                files.Add(file.FullName);
            }
        }

        private void DeleteFiles()
        {
            List<String> files = GetFilesForClipboard();
            if (MessageBox.Show(this, StringResources.ReallyDelete, StringResources.Ares, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                Dictionary<String, object> uniqueParents = GetUniqueParents(files);
                List<String> deletedFiles = new List<string>();
                try
                {
                    foreach (String file in uniqueParents.Keys)
                    {
                        if (System.IO.Directory.Exists(file))
                        {
                            AddFilesToList(deletedFiles, file);
                            System.IO.Directory.Delete(file, true);
                        }
                        else if (System.IO.File.Exists(file))
                        {
                            deletedFiles.Add(file);
                            System.IO.File.Delete(file);
                        }
                    }
                    String basePath = Ares.Settings.Settings.Instance.MusicDirectory;
                    List<string> adaptedFiles = new List<string>();
                    foreach (string file in deletedFiles)
                    {
                        if (file.StartsWith(basePath, StringComparison.OrdinalIgnoreCase))
                            adaptedFiles.Add(file.Substring(basePath.Length + 1));
                    }
                    if (adaptedFiles.Count > 0)
                    {
                        Ares.Tags.TagsModule.GetTagsDB().WriteInterface.RemoveFiles(adaptedFiles);
                    }
                }
                catch (System.IO.IOException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.DeleteError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Ares.Tags.TagsDbException ex)
                {
                    MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                treeView1.SelectedNode = m_Root;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CutFiles();
        }

        private void CutFiles()
        {
            ClipboardFilesList filesList = new ClipboardFilesList();
            filesList.Files = GetFilesForClipboard();
            filesList.FilesAreCut = true;
            Clipboard.SetData(DataFormats.GetFormat("AresFilesList").Name, filesList);
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            CopyFiles();
        }

        private void CopyFiles()
        {
            ClipboardFilesList filesList = new ClipboardFilesList();
            filesList.Files = GetFilesForClipboard();
            filesList.FilesAreCut = false;
            Clipboard.SetData(DataFormats.GetFormat("AresFilesList").Name, filesList);
        }

        private List<String> GetFilesForClipboard()
        {
            List<String> items = new List<String>();
            String basePath = m_FileType == FileType.Sound ? Ares.Settings.Settings.Instance.SoundDirectory : Ares.Settings.Settings.Instance.MusicDirectory;
            treeView1.SelectedNodes.ForEach(node => items.Add(System.IO.Path.Combine(basePath, ((DraggedItem)node.Tag).RelativePath)));
            return items;
        }

        private List<String> GetSelectedFiles()
        {
            List<String> files = new List<string>();
            HashSet<string> foundFiles = new HashSet<string>();
            foreach (TreeNode node in treeView1.SelectedNodes)
            {
                AddFilesOfNode(node, foundFiles);
            }
            files.AddRange(foundFiles);
            return files;
        }

        private void AddFilesOfNode(TreeNode node, HashSet<String> foundFiles)
        {
            if (node.Tag is DraggedItem)
            {
                DraggedItem item = (DraggedItem)node.Tag;
                switch (item.NodeType)
                {
                    case DraggedItemType.Directory:
                        {
                            foreach (TreeNode subNode in node.Nodes)
                            {
                                AddFilesOfNode(subNode, foundFiles);
                            }
                        }
                        break;
                    case DraggedItemType.File:
                        foundFiles.Add(item.RelativePath);
                        break;
                    default:
                        break;
                }
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteFiles();
        }

        private void PasteFiles()
        {
            if (Clipboard.ContainsFileDropList())
            {
                System.Collections.Specialized.StringCollection filesList = Clipboard.GetFileDropList();
                List<String> files = new List<string>();
                foreach (String file in filesList)
                {
                    files.Add(file);
                }
                TreeNode target = treeView1.SelectedNode;
                DropFiles(files, false, target);
            }
            else if (Clipboard.ContainsData(DataFormats.GetFormat("AresFilesList").Name))
            {
                ClipboardFilesList filesList = (ClipboardFilesList)Clipboard.GetData(DataFormats.GetFormat("AresFilesList").Name);
                if (filesList != null)
                {
                    TreeNode target = treeView1.SelectedNode;
                    DropFiles(filesList.Files, filesList.FilesAreCut, target);
                }
            }
        }

        private void showInfoButton_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
            showInfoButton.Checked = !splitContainer1.Panel2Collapsed;
        }

        private void tagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTags();
        }

        private void SetTags()
        {
            List<String> files = GetSelectedFiles();
            m_Parent.ShowFileTagsEditor(files);
        }

        private HashSet<String> m_FilteredFiles = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
        private TagsFilter m_TagsFilter = new TagsFilter();
        private ProjectUseFilter m_ProjectUseFilter = new ProjectUseFilter();

        private void RetrieveFilteredFiles()
        {
            try
            {
                int languageId = m_Project != null ? m_Project.TagLanguageId : -1;
                if (languageId == -1)
                    languageId = Ares.Tags.TagsModule.GetTagsDB().TranslationsInterface.GetIdOfCurrentUILanguage();
                var dbRead = Ares.Tags.TagsModule.GetTagsDB().ReadInterface;
                IList<String> files;
                switch (m_TagsFilter.FilterMode)
                {
                    case TagsFilterMode.NormalFilter:
                        {
                            switch (m_TagsFilter.TagCategoryCombination)
                            {
                                case Data.TagCategoryCombination.UseOneTagOfEachCategory:
                                    files = dbRead.GetAllFilesWithAnyTagInEachCategory(m_TagsFilter.TagsByCategories);
                                    break;
                                case Data.TagCategoryCombination.UseAnyTag:
                                    {
                                        HashSet<int> allTags = new HashSet<int>();
                                        foreach (var entry in m_TagsFilter.TagsByCategories)
                                        {
                                            allTags.UnionWith(entry.Value);
                                        }
                                        files = dbRead.GetAllFilesWithAnyTag(allTags);
                                    }
                                    break;
                                case Data.TagCategoryCombination.UseAllTags:
                                default:
                                    {
                                        HashSet<int> allTags = new HashSet<int>();
                                        foreach (var entry in m_TagsFilter.TagsByCategories)
                                        {
                                            allTags.UnionWith(entry.Value);
                                        }
                                        files = dbRead.GetAllFilesWithAllTags(allTags);
                                    }
                                    break;
                            }
                        }
                        break;
                    case TagsFilterMode.UntaggedFiles:
                        files = dbRead.GetAllFilesWithAnyTag();
                        break;
                    case TagsFilterMode.NoFilter:
                    default:
                        files = null;
                        break;
                }
                m_FilteredFiles.Clear();
                if (files != null)
                {
                    m_FilteredFiles.UnionWith(files);
                }
            }
            catch (Ares.Tags.TagsDbException ex)
            {
                MessageBox.Show(this, String.Format(StringResources.TagsDbError, ex.Message), StringResources.Ares, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool mUpdateTreeOnTagChange = true;

        private void tagFilterButton_Click(object sender, EventArgs e)
        {
            Dialogs.TagFilterDialog dialog = new Dialogs.TagFilterDialog();
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
            dialog.LanguageId = languageId;
            dialog.TagCategoryCombination = m_TagsFilter.TagCategoryCombination;
            dialog.TagsByCategory = m_TagsFilter.TagsByCategories;
            dialog.FilterMode = m_TagsFilter.FilterMode != TagsFilterMode.NoFilter ? m_TagsFilter.FilterMode : TagsFilterMode.NormalFilter;
            dialog.AutoUpdateTree = mUpdateTreeOnTagChange;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                if (m_Project != null)
                {
                    m_Project.TagLanguageId = dialog.LanguageId;
                }
                m_TagsFilter.TagCategoryCombination = dialog.TagCategoryCombination;
                m_TagsFilter.TagsByCategories = dialog.TagsByCategory;
                m_TagsFilter.FilterMode = dialog.FilterMode;
                if (m_TagsFilter.FilterMode == TagsFilterMode.NormalFilter && m_TagsFilter.TagsByCategories.Count == 0)
                    m_TagsFilter.FilterMode = TagsFilterMode.NoFilter;
                tagFilterButton.Checked = m_TagsFilter.FilterMode != TagsFilterMode.NoFilter;
                mUpdateTreeOnTagChange = dialog.AutoUpdateTree;
                if (m_TagsFilter.FilterMode != TagsFilterMode.NoFilter)
                {
                    RetrieveFilteredFiles();
                }
                ReFillTree();
            }
        }

        private void tagButton_Click(object sender, EventArgs e)
        {
            SetTags();
        }

        private void projectFilterButton_Click(object sender, EventArgs e)
        {
            Dialogs.ProjectFilterDialog dialog = new Dialogs.ProjectFilterDialog();
            dialog.AutoUpdateTree = m_ProjectUseFilter.AutoUpdate;
            dialog.FilterMode = m_ProjectUseFilter.FilterMode;
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                m_ProjectUseFilter.FilterMode = dialog.FilterMode;
                m_ProjectUseFilter.AutoUpdate = dialog.AutoUpdateTree;
                projectFilterButton.Checked = m_ProjectUseFilter.FilterMode != ProjectUseFilterMode.NoFilter;
                ReFillTree();
            }
        }
    }

    public enum DraggedItemType
    {
        Directory, File
    }

    public enum FileType
    {
        Music, Sound
    }

    public class DraggedItem
    {
        public DraggedItemType NodeType { get; set; }
        public FileType ItemType { get; set; }
        public String RelativePath { get; set; }
        public String Title { get; set; }
    }

    public enum TagsFilterMode
    {
        NormalFilter,
        UntaggedFiles,
        NoFilter
    }

    public class TagsFilter
    {
        public TagsFilterMode FilterMode { get; set; }
        public Ares.Data.TagCategoryCombination TagCategoryCombination { get; set; }
        public Dictionary<int, HashSet<int>> TagsByCategories { get { return mTagsByCategories; } set { mTagsByCategories = value; } }

        private Dictionary<int, HashSet<int>> mTagsByCategories = new Dictionary<int, HashSet<int>>();

        public TagsFilter()
        {
            FilterMode = TagsFilterMode.NoFilter;
            TagCategoryCombination = Data.TagCategoryCombination.UseAnyTag;
        }
    }

    public enum ProjectUseFilterMode
    {
        NoFilter,
        UsedFilter,
        UnusedFilter
    }

    public class ProjectUseFilter
    {
        public ProjectUseFilterMode FilterMode { get; set; }
        public bool AutoUpdate { get; set; }

        public ProjectUseFilter()
        {
            FilterMode = ProjectUseFilterMode.NoFilter;
            AutoUpdate = false;
        }
    }

    public enum FileSource
    {
        Online,
        LocalFileSystem
    }

    public class FileDragInfo
    {
        public FileSource Source;
        public List<DraggedItem> DraggedItems { get; set; }
        public TagsFilter TagsFilter { get; set; }
    }

    public static class FileSearch
    {
        private static String[] GetPatterns(FileType dirType)
        {
            if (dirType == FileType.Music)
                return new String[] { "*.wav", "*.mp3", "*.m4a", "*.ogg", "*.opus", "*.pls", "*.m3u", "*.m3u8", "*.flac" };
            else
                return new String[] { "*.wav", "*.mp3", "*.m4a", "*.ogg", "*.opus", "*.flac" };
        }

        public static IEnumerable<string> GetFilesInDirectory(FileType dirType, String directory, bool descendToSubDirs)
        {
            String[] patterns = GetPatterns(dirType);
            foreach (String pattern in patterns)
            {
                String ending = pattern.Substring(2); // necessary because searching for *.wav also returns xy.wav_old etc.
                foreach (String file in System.IO.Directory.GetFiles(directory, pattern))
                {
                    if (file.EndsWith(ending, StringComparison.CurrentCultureIgnoreCase))
                    {
                        yield return file;
                    }
                }
            }
            if (descendToSubDirs)
            {
                foreach (String subDir in System.IO.Directory.GetDirectories(directory))
                {
                    foreach (String file in GetFilesInDirectory(dirType, subDir, true))
                    {
                        yield return file;
                    }
                }
            }
        }
    }

    public interface IFileExplorerParent
    {
        void SetEditor();
        void ShowFileTagsEditor(IList<String> selectedFiles);
        void SetSelectedFiles(IList<String> selectedFiles);
    }
}
