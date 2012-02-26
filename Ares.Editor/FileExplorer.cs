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

        private IFileExplorerParent m_Parent;

        public FileExplorer(FileType fileType, IFileExplorerParent parent)
        {
            InitializeComponent();
            m_Parent = parent;
            this.Text = String.Format(StringResources.FileExplorerTitle, fileType == FileType.Music ? StringResources.Music : StringResources.Sounds);
            m_FileType = fileType;
            treeView1.ImageList = sImageList;
            ReFillTree();
            if (fileType == FileType.Music)
            {
                Actions.FilesWatcher.Instance.MusicDirChanges += new EventHandler<EventArgs>(DirChanged);
                this.Icon = ImageResources.musicIcon;
            }
            else
            {
                Actions.FilesWatcher.Instance.SoundDirChanges += new EventHandler<EventArgs>(DirChanged);
                this.Icon = ImageResources.sounds;
            }
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

#if !MONO
        protected override string GetPersistString()
        {
            return "FileExplorer_" + (int) m_FileType;
        }
#endif

        public void ReFillTree()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            m_Root = new TreeNode(m_FileType == FileType.Music ? StringResources.Music : StringResources.Sounds);
            m_Root.SelectedImageIndex = m_Root.ImageIndex = 0;
            m_Root.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = m_FileType, RelativePath = String.Empty };
            String directory = m_FileType == FileType.Music ? Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
            FillTreeNode(m_Root, directory, directory, m_FileType);
            treeView1.Nodes.Add(m_Root);
            m_Root.Expand();
            treeView1.EndUpdate();
        }

        private void FillTreeNode(TreeNode node, String directory, String root, FileType dirType)
        {
            try
            {
                String[] subDirs = System.IO.Directory.GetDirectories(directory);
                int subLength = directory.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) ? directory.Length : directory.Length + 1;
                int rootLength = root.EndsWith(new String(System.IO.Path.DirectorySeparatorChar, 1)) ? root.Length : root.Length + 1;
                foreach (String subDir in subDirs)
                {
                    TreeNode subNode = new TreeNode(subDir.Substring(subLength));
                    subNode.ImageIndex = subNode.SelectedImageIndex = 0;
                    subNode.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = dirType, RelativePath = subDir.Substring(rootLength) };
                    FillTreeNode(subNode, subDir, root, dirType);
                    node.Nodes.Add(subNode);
                }
                List<string> files = new List<string>();
                String[] patterns = { "*.wav", "*.mp3", "*.ogg" };
                files.AddRange(FileSearch.GetFilesInDirectory(directory, false));
                files.Sort(StringComparer.CurrentCulture);
                foreach (String file in files)
                {
                    TreeNode subNode = new TreeNode(file.Substring(subLength));
                    subNode.ImageIndex = subNode.SelectedImageIndex = (dirType == FileType.Sound ? 1 : 2);
                    subNode.Tag = new DraggedItem { NodeType = DraggedItemType.File, ItemType = dirType, RelativePath = file.Substring(rootLength) };
                    subNode.ContextMenuStrip = fileNodeContextMenu;
                    node.Nodes.Add(subNode);
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

        Point m_DragPoint;
        bool m_InDrag;

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                m_DragPoint = new Point(e.X, e.Y);
                m_InDrag = true;
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            m_InDrag = false;
        }

        private void treeView1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_InDrag)
            {
                List<DraggedItem> items = new List<DraggedItem>();
                treeView1.SelectedNodes.ForEach(node => items.Add((DraggedItem)node.Tag));
                DoDragDrop(items, DragDropEffects.Copy);
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
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            DefaultNodeAction();
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
            ReFillTree();
        }

        private void treeView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                DefaultNodeAction();
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
                    System.Diagnostics.Process.Start(editor, filePath);
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
            bool found = Search(node, text, SearchTypes.Children | SearchTypes.Siblings);
            if (!found)
            {
                // end of tree: start again at the beginning
                found = SearchSingleNode(m_Root, text);
            }
            if (!found)
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
    }

    public static class FileSearch
    {
        public static IEnumerable<string> GetFilesInDirectory(String directory, bool descendToSubDirs)
        {
            String[] patterns = { "*.wav", "*.mp3", "*.ogg" };
            foreach (String pattern in patterns)
            {
                foreach (String file in System.IO.Directory.GetFiles(directory, pattern))
                {
                    yield return file;
                }
            }
            if (descendToSubDirs)
            {
                foreach (String subDir in System.IO.Directory.GetDirectories(directory))
                {
                    foreach (String file in GetFilesInDirectory(subDir, true))
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
    }
}
