using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Ares.Data;

namespace Ares.Editor.ElementEditors
{
    partial class RandomPlaylistEditor : EditorBase
    {
        public RandomPlaylistEditor()
        {
            InitializeComponent();
        }

        public void SetPlaylist(IRandomBackgroundMusicList playList)
        {
            ElementId = playList.Id;
            m_Playlist = playList;
            delayableControl.SetElement(playList);
            repeatableControl.SetElement(playList);
            choiceContainerControl.SetContainer(playList);
            Update(m_Playlist.Id, Actions.ElementChanges.ChangeType.Renamed);
            Actions.ElementChanges.Instance.AddListener(m_Playlist.Id, Update);
        }

        private void Update(int elementId, Actions.ElementChanges.ChangeType changeType)
        {
            if (elementId == m_Playlist.Id)
            {
                if (changeType == Actions.ElementChanges.ChangeType.Renamed)
                {
                    this.Text = m_Playlist.Title;
                }
                else if (changeType == Actions.ElementChanges.ChangeType.Removed)
                {
                    Close();
                }
            }
        }

        private IRandomBackgroundMusicList m_Playlist;

        private void RandomPlaylistEditor_SizeChanged(object sender, EventArgs e)
        {
            int space = this.Width - 24;
            if (space < 0) space = 1;
            delayableControl.Width = space;
            repeatableControl.Width = space;
            Font font = label1.Font;
            String text = label1.Text;
            using (Graphics g = label1.CreateGraphics())
            {
                float textWidth = g.MeasureString(text, font).Width;
                float factor = label1.Width / textWidth;
                label1.Font = new Font(font.Name, font.SizeInPoints * factor);
            }
        }

        private bool m_AcceptDrop;

        private void RandomPlaylistEditor_DragEnter(object sender, DragEventArgs e)
        {
            m_AcceptDrop = e.Data.GetDataPresent(typeof(List<DraggedItem>));
        }

        private void RandomPlaylistEditor_DragLeave(object sender, EventArgs e)
        {
            m_AcceptDrop = false;
        }

        private void RandomPlaylistEditor_DragOver(object sender, DragEventArgs e)
        {
            if (m_AcceptDrop && (e.AllowedEffect & DragDropEffects.Copy) != 0)
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void RandomPlaylistEditor_DragDrop(object sender, DragEventArgs e)
        {
            List<DraggedItem> list = e.Data.GetData(typeof(List<DraggedItem>)) as List<DraggedItem>;
            if (list != null)
            {
                List<IElement> elements = new List<IElement>(GetElementsFromDroppedItems(list));
                choiceContainerControl.AddElements(elements);
            }
        }

        private IEnumerable<IElement> GetElementsFromDroppedItems(List<DraggedItem> draggedItems)
        {
            Dictionary<string, DraggedItem> uniqueItems = new Dictionary<string, DraggedItem>();
            foreach (DraggedItem item in draggedItems)
            {
                AddItemsToSet(uniqueItems, item);
            }
            foreach (DraggedItem item in uniqueItems.Values)
            {
                yield return CreateFileElement(item);
            }
        }

        private static IElement CreateFileElement(DraggedItem item)
        {
            // TODO: track title
            return Ares.Data.DataModule.ElementFactory.CreateFileElement(item.RelativePath, item.ItemType == FileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect);
        }

        private static void AddItemsToSet(Dictionary<String, DraggedItem> uniqueItems, DraggedItem item)
        {
            String baseDir = item.ItemType == FileType.Music ? Settings.Instance.MusicDirectory : Settings.Instance.SoundDirectory;
            String path = baseDir + item.RelativePath;
            if (item.NodeType == DraggedItemType.Directory)
            {
                if (System.IO.Directory.Exists(path))
                {
                    foreach (String file in FileSearch.GetFilesInDirectory(path, true))
                    {
                        AddItemsToSet(uniqueItems, new DraggedItem { NodeType = DraggedItemType.File, ItemType = item.ItemType, RelativePath = file.Substring(baseDir.Length) });
                    }
                }
            }
            else
            {
                String key = System.IO.Path.GetFullPath(path);
                if (!uniqueItems.ContainsKey(path))
                {
                    uniqueItems[key] = item;
                }
            }
        }

    }
}
