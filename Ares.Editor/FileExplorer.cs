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
        public FileExplorer()
        {
            InitializeComponent();
            HideOnClose = true;
            ReFillTree();
        }

        protected override string GetPersistString()
        {
            return "FileExplorer";
        }

        public void ReFillTree()
        {
            treeView1.BeginUpdate();
            treeView1.Nodes.Clear();
            m_MusicRoot = new TreeNode(StringResources.Music);
            m_MusicRoot.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = FileType.Music, RelativePath = String.Empty };
            FillTreeNode(m_MusicRoot, Settings.Instance.MusicDirectory, Settings.Instance.MusicDirectory, FileType.Music);
            treeView1.Nodes.Add(m_MusicRoot);
            m_SoundsRoot = new TreeNode(StringResources.Sounds);
            m_SoundsRoot.Tag = new DraggedItem { NodeType = DraggedItemType.Directory, ItemType = FileType.Sound, RelativePath = String.Empty };
            FillTreeNode(m_SoundsRoot, Settings.Instance.SoundDirectory, Settings.Instance.SoundDirectory, FileType.Sound);
            treeView1.Nodes.Add(m_SoundsRoot);
            m_MusicRoot.Expand();
            m_SoundsRoot.Expand();
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
                    subNode.Tag = new DraggedItem { NodeType = DraggedItemType.File, ItemType = dirType, RelativePath = file.Substring(rootLength) };
                    node.Nodes.Add(subNode);
                }
            }
            catch (Exception e)
            {
                TreeNode subNode = new TreeNode(e.Message);
                node.Nodes.Add(subNode);
            }
        }

        TreeNode m_MusicRoot;
        TreeNode m_SoundsRoot;

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
