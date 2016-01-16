/*
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
using Ares.AudioSource;
using Ares.Data;
using Ares.Editor.Plugins;
using Ares.ModelInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares.Editor.AudioSourceSearch
{

    /// <summary>
    /// This window/form allows the user to search for audio (Music, Sounds, complete Mode Elements) in various online sources (IAudioSource).
    /// The search results are displayed in a list an can be dragged into either the file-list containers or the project structure,
    /// depending on the type of the result.
    /// 
    /// A note on actual audio files:
    /// The target path for each audio file to be downloaded is determined when the drag operation is started.
    /// The actual download only occurs when and if the drag completes.
    /// </summary>
    partial class AudioSourceSearchWindow : ToolWindow
    {
        private static ImageList sImageList = null;

        static AudioSourceSearchWindow()
        {
            sImageList = new ImageList();
            sImageList.Images.Add(ImageResources.Folder);
            sImageList.Images.Add(ImageResources.sounds1);
            sImageList.Images.Add(ImageResources.music1);
        }

        private void ElementChanged(int elementId, Ares.Editor.Actions.ElementChanges.ChangeType changeType)
        {
            //updateInformationPanel();
        }

        private PluginManager m_PluginManager;
        private ICollection<IAudioSource> m_AudioSources;

        private CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();
        
        private Ares.Data.IProject m_Project;

        public AudioSourceSearchWindow(PluginManager pluginManager)
        {
            this.m_PluginManager = pluginManager;
            this.m_AudioSources = m_PluginManager.GetPluginInstances<IAudioSource>();

            InitializeComponent();
            this.Text = String.Format(StringResources.AudioSourceSearchTitle);

            this.Icon = ImageResources.AudioSourceSearchIcon;

            resultsListView.SmallImageList = sImageList;
            //ReFillTree();
            if (Height > 200)
            {
                splitContainer1.SplitterDistance = Height - 100;
            }
            Ares.Editor.Actions.ElementChanges.Instance.AddListener(-1, ElementChanged);
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
        }

        #region Event handlers to start a drag & drop operation

        private void resultsListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Make sure there actually is a selection
            if (resultsListView.SelectedItems.Count < 1)
            {
                // No item selecte, don't drag
                return;
            }

            IEnumerable<AudioSourceSearchResultItem> selected = resultsListView.SelectedItems.Cast<AudioSourceSearchResultItem>();

            // Determine the overall AudioType from the first selected item
            AudioSourceSearchResultItem firstItem = selected.First();
            AudioType overallItemAudioType = firstItem.ItemAudioType;
            
            // Make sure all other items' AudioTypes are compatible (the same)
            foreach (AudioSourceSearchResultItem item in selected)
            {
                if (item.ItemAudioType != overallItemAudioType)
                {
                    // TODO: show a message to the user that he should only select items of the same type?
                    throw new InvalidOperationException();
                }
            }

            // Generate a task to download all selected items
            { 

                // TODO: queue each selected item for download to the local music/sound directories
                IProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.DownloadingAudio, this.m_cancellationTokenSource);

                
            }

            // Collect download actions for each dragged element to be executed after a drag completes
            List<Func<AudioDownloadResult>> downloadFunctions = new List<Func<AudioDownloadResult>>();
            foreach (AudioSourceSearchResultItem searchResultItem in selected) {
                AudioSourceSearchResult searchResult = searchResultItem.SearchResult;

                // TODO: determine download target directories
                musicDownloadDirectory = Ares.Settings.Settings.Instance.MusicDirectory + GetRelativeDownloadPathForSearchResult(searchResult);
                soundsDownloadDirectory = Ares.Settings.Settings.Instance.SoundDirectory + GetRelativeDownloadPathForSearchResult(searchResult);

                downloadFunctions.Add(() => {
                    return searchResult.Download(musicDownloadDirectoryForSourceAndItem, soundsDownloadDirectoryForSourceAndItem);
                });
            }

            // Decide depending on the overall AudioType of the selected items
            DragDropEffects dragDropResult;
            switch (overallItemAudioType)
            {
                case AudioType.Music:
                case AudioType.Sound:
                    dragDropResult = StartDragFiles();
                    break;
            }
        }

        /// <summary>
        /// Get the relative path below the music/sounds directories where files downloaded for the given search result should be placed
        /// </summary>
        /// <param name="searchResult"></param>
        /// <param name="musicDownloadDirectory"></param>
        /// <param name="soundsDownloadDirectory"></param>
        public string GetRelativeDownloadPathForSearchResult(AudioSourceSearchResult searchResult)
        {
            IAudioSource audioSource = searchResult.AudioSource;
            return @"\OnlineAudioSources\";
        }

        /**
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
                info.DraggedItems = items;
                info.TagsFilter = m_TagsFilter;
                DoDragDrop(info, DragDropEffects.Copy | DragDropEffects.Move);
                m_InDrag = false;
            }
        }
        */

        #endregion  

        /// <summary>
        /// Start a drag&drop operation with a collection of Project elements (IXmlWritable) to be dropped into the project structure
        /// </summary>
        /// <param name="exportItems"></param>
        public DragDropEffects StartDragProjectElements(List<IXmlWritable> draggedItems)
        {
            StringBuilder serializedForm = new StringBuilder();
            Data.DataModule.ProjectManager.ExportElements(draggedItems, serializedForm);
            ProjectExplorer.ClipboardElements cpElements = new ProjectExplorer.ClipboardElements() { SerializedForm = serializedForm.ToString() };
            return DoDragDrop(cpElements, DragDropEffects.Copy | DragDropEffects.Move | DragDropEffects.Link);
        }

        /// <summary>
        /// Start a drag&drop operation with a collection of Files (DraggedItem) to be dropped into a file container
        /// </summary>
        /// <param name="draggedItems"></param>
        public DragDropEffects StartDragFiles(List<DraggedItem> draggedItems)
        {
            FileDragInfo info = new FileDragInfo();
            info.DraggedItems = draggedItems;
            info.TagsFilter = new TagsFilter();
            return DoDragDrop(info, DragDropEffects.Copy | DragDropEffects.Move);
        }
    }

    public class AudioSourceSearchResultItem : ListViewItem
    {

        private AudioType m_ItemAudioType;
        private AudioSourceSearchResult m_SearchResult;

        public AudioType ItemAudioType { get { return m_ItemAudioType; } }
        public AudioSourceSearchResult SearchResult { get { return m_SearchResult; } }

        /// <summary>
        /// Create an AudioSourceSearchResultItem from te given AudioSource SearchResult
        /// </summary>
        /// <param name="result"></param>
        public AudioSourceSearchResultItem(AudioSourceSearchResult result): base()
        {
            this.Text = result.Title;
            this.m_ItemAudioType = result.AudioType;
            this.m_SearchResult = result;
        }
    }

}
