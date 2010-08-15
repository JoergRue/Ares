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
    partial class FileExplorer : WeifenLuo.WinFormsUI.Docking.DockContent
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

        public FileExplorer(FileType fileType)
        {
            InitializeComponent();
            this.Text = String.Format(StringResources.FileExplorerTitle, fileType == FileType.Music ? StringResources.Music : StringResources.Sounds);
            m_FileType = fileType;
            HideOnClose = true;
            treeView1.ImageList = sImageList;
            ReFillTree();
            if (fileType == FileType.Music)
            {
                Actions.FilesWatcher.Instance.MusicDirChanges += new EventHandler<EventArgs>(DirChanged);
                this.Icon = ImageResources.music;
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

        protected override string GetPersistString()
        {
            return "FileExplorer_" + (int) m_FileType;
        }

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

        private static void FillTreeNode(TreeNode node, String directory, String root, FileType dirType)
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

        TreeNode m_SelectedNode;
        Point m_DragPoint;
        bool m_InDrag;

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            m_SelectedNode = treeView1.GetNodeAt(e.X, e.Y);
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
}
