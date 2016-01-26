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
    /// A note on downlading the actual audio files:
    /// The target path for each audio file to be downloaded is determined when the drag operation is started.
    /// The actual download only occurs when and if the drag completes.
    /// 
    /// TODO: Possibly use ObjectListView instead of normal Listview http://objectlistview.sourceforge.net/cs/index.html
    /// </summary>
    partial class AudioSourceSearchWindow : ToolWindow
    {
        #region Static ImageList for icons

        private static ImageList sImageList = null;

        // Image indices in the ListView ImageList
        public static int IMAGE_INDEX_MODE_ELEMENT = 0;
        public static int IMAGE_INDEX_SOUND = 1;
        public static int IMAGE_INDEX_MUSIC = 2;

        static AudioSourceSearchWindow()
        {
            sImageList = new ImageList();
            // Image Index 0: ModeElement
            sImageList.Images.Add(ImageResources.parallel);
            // Image Index 1: Sound
            sImageList.Images.Add(ImageResources.sounds1);
            // Image Index 2: Music
            sImageList.Images.Add(ImageResources.music1);
        }

        #endregion

        private PluginManager m_PluginManager;
        private ICollection<IAudioSource> m_AudioSources;
        private CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();
        private Ares.Data.IProject m_Project;

        public AudioSourceSearchWindow(PluginManager pluginManager)
        {
            // Find the audio sources
            this.m_PluginManager = pluginManager;
            this.m_AudioSources = m_PluginManager.GetPluginInstances<IAudioSource>();

            // Initialize the component
            InitializeComponent();

            // Setup window title & icon
            this.Text = String.Format(StringResources.AudioSourceSearchTitle);
            this.Icon = ImageResources.AudioSourceSearchIcon;

            // Define the image lists used for the list view
            resultsListView.SmallImageList = sImageList;
            resultsListView.LargeImageList = sImageList;

            if (Height > 200)
            {
                splitContainer1.SplitterDistance = Height - 100;
            }
            Ares.Editor.Actions.ElementChanges.Instance.AddListener(-1, ElementChanged);
        }

        #region Search across all (or only selected) available sources

        /// <summary>
        /// Execute a search with the parameters given in the UI
        /// </summary>
        public void ExecuteSearch()
        {
            // Read the search query from the UI
            string query = this.searchBox.Text;

            // By default search for any kind of audio (in the future this might be narrowed down based on user input through the UI)
            AudioSearchResultType requestedResultType = AudioSearchResultType.Unknown;

            TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.SearchingForAudio, this.m_cancellationTokenSource);
            CancellationToken token = this.m_cancellationTokenSource.Token;

            Task<List<ISearchResult>> task = Task.Factory.StartNew(() =>
            {
                List<ISearchResult> results = new List<ISearchResult>();

                monitor.SetIndeterminate(StringResources.SearchingForAudio);
                foreach (IAudioSource audioSource in m_AudioSources)
                {
                    // TODO: in the future the list of sources to be searched might be narrowed down (possibly even limited to just one source)
                    // based on user input through the UI

                    // Only search through sources that actually support the requested result type
                    if (audioSource.IsAudioTypeSupported(requestedResultType))
                    {
                        results.AddRange(audioSource.Search(query, AudioSearchResultType.Unknown, monitor, token));
                    }
                    //monitor.IncreaseProgress(1.0 / m_AudioSources.Count);
                    token.ThrowIfCancellationRequested();
                }

                return results;
            });

            // What to do when the search completes
            task.ContinueWith((t) =>
            {
                monitor.Close();

                this.resultsListView.BeginUpdate();
                // Clear the results list in the UI
                this.resultsListView.Items.Clear();
                // Go through all search results
                foreach (ISearchResult result in task.Result)
                {
                    // Wrap them into AudioSourceSearchResultItems and add to UI results list
                    this.resultsListView.Items.Add(
                        new AudioSourceSearchResultItem(result)
                    );
                }
                this.resultsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                this.resultsListView.EndUpdate();
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            // What to do when the search fails
            task.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.SearchError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }
        
        #endregion

        #region Drag & Drop of search results into the project

        private void resultsListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            // Make sure there actually is a selection
            if (resultsListView.SelectedItems.Count < 1)
            {
                // No item selecte, don't drag
                return;
            }

            IEnumerable<AudioSourceSearchResultItem> selected = resultsListView.SelectedItems.Cast<AudioSourceSearchResultItem>();

            AudioSearchResultType overallItemAudioType = FindAndVerifySelectedAudioType(selected);

            // Collect download actions for each dragged element to be executed after a drag completes
            List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>> downloadFunctions = new List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>>();
            double totalDownloadSize = CollectDownloadFunctionsAndSize(selected, downloadFunctions);

            // Decide depending on the overall AudioType of the selected items
            DragDropEffects dragDropResult = DragDropEffects.None;
            switch (overallItemAudioType)
            {
                // If the dragged items are Music or Sound files
                case AudioSearchResultType.MusicFile:
                case AudioSearchResultType.SoundFile:
                    dragDropResult = StartDragSelectedMusicOrSoundFile(selected, overallItemAudioType);
                    break;
                // If the dragged items are ModeElements
                case AudioSearchResultType.ModeElement:
                    dragDropResult = StartDragSelectedModeElement(selected);
                    break;
            }

            // If the drag&drop resulted in a Copy action, download the audio content
            if (dragDropResult == DragDropEffects.Copy)
            {
                ExecuteDownloads(downloadFunctions, totalDownloadSize);
            }
        }

        /// <summary>
        /// Determine the AudioSearchResultType of the given selected items.
        /// The first item's AudioSearchResultType is assumed to apply to all items, but
        /// this is verified and an InvalidOperationException is thrown if any item in the 
        /// list has a different AudioSearchResultType than the first one.
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        private static AudioSearchResultType FindAndVerifySelectedAudioType(IEnumerable<AudioSourceSearchResultItem> selected)
        {
            // Determine the overall AudioType from the first selected item
            AudioSourceSearchResultItem firstItem = selected.First();
            AudioSearchResultType overallItemAudioType = firstItem.ItemAudioType;

            // Make sure all other items' AudioTypes are compatible (the same)
            foreach (AudioSourceSearchResultItem item in selected)
            {
                if (item.ItemAudioType != overallItemAudioType)
                {
                    // TODO: show a message to the user that he should only select items of the same type?
                    throw new InvalidOperationException();
                }
            }

            return overallItemAudioType;
        }
     
        private DragDropEffects StartDragSelectedModeElement(IEnumerable<AudioSourceSearchResultItem> selected)
        {
            DragDropEffects dragDropResult;
            List<IXmlWritable> draggedItems = new List<IXmlWritable>();

            foreach (AudioSourceSearchResultItem searchResultItem in selected)
            {
                // Cast the SearchResult
                IModeElementSearchResult modeElementSearchResult = searchResultItem.SearchResult as IModeElementSearchResult;

                // Determine relevant directories
                string musicBaseDirectory = Ares.Settings.Settings.Instance.MusicDirectory;
                string soundBaseDirectory = Ares.Settings.Settings.Instance.SoundDirectory;
                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(modeElementSearchResult);

                // Get the ModeElement definition
                draggedItems.Add(modeElementSearchResult.GetModeElementDefinition(musicBaseDirectory, soundBaseDirectory, relativeDownloadPath));
            }

            // Start a drag & drop operation for those project elements
            dragDropResult = StartDragProjectElements(draggedItems);
            return dragDropResult;
        }

        private DragDropEffects StartDragSelectedMusicOrSoundFile(IEnumerable<AudioSourceSearchResultItem> selected, AudioSearchResultType overallItemAudioType)
        {
            DragDropEffects dragDropResult;
            List<DraggedItem> draggedFiles = new List<DraggedItem>();

            foreach (AudioSourceSearchResultItem searchResultItem in selected)
            {
                // Cast the SearchResult
                IFileSearchResult fileSearchResult = searchResultItem.SearchResult as IFileSearchResult;
                // Create a new DraggedItem (dragged file/folder)
                DraggedItem draggedFile = new DraggedItem();

                // Set item & node type for the file
                draggedFile.ItemType = overallItemAudioType == AudioSearchResultType.MusicFile ? FileType.Music : FileType.Sound;
                draggedFile.NodeType = DraggedItemType.File;

                // Get the relative path where the downloaded file will be placed
                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(searchResultItem.SearchResult);
                draggedFile.RelativePath = fileSearchResult.GetRelativeDownloadFilePath(relativeDownloadPath);

                draggedFiles.Add(draggedFile);
            }

            // Start a file/folder drag & drop operation for those files
            dragDropResult = StartDragFiles(draggedFiles);
            return dragDropResult;
        }

        /// <summary>
        /// Get the relative path below the music/sounds directories where files downloaded for the given search result should be placed
        /// </summary>
        /// <param name="searchResult"></param>
        /// <param name="musicDownloadDirectory"></param>
        /// <param name="soundsDownloadDirectory"></param>
        public string GetRelativeDownloadPathForSearchResult(ISearchResult searchResult)
        {
            string audioSourceId = searchResult.AudioSource.Id;
            return System.IO.Path.Combine("OnlineAudioSources", audioSourceId);
        }

        /// <summary>
        /// Start a drag&drop operation with a collection of Project elements (IXmlWritable) to be dropped into the project structure
        /// </summary>
        /// <param name="exportItems"></param>
        public DragDropEffects StartDragProjectElements(List<IXmlWritable> draggedItems)
        {
            StringBuilder serializedForm = new StringBuilder();
            Data.DataModule.ProjectManager.ExportElements(draggedItems, serializedForm);
            ProjectExplorer.ClipboardElements cpElements = new ProjectExplorer.ClipboardElements() { SerializedForm = serializedForm.ToString() };
            return DoDragDrop(cpElements, DragDropEffects.Copy);
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
            return DoDragDrop(info, DragDropEffects.Copy);
        }

        #endregion

        #region File downloading

        private double CollectDownloadFunctionsAndSize(IEnumerable<AudioSourceSearchResultItem> selected, List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>> downloadFunctions)
        {
            double totalDownloadSize = 0;

            foreach (AudioSourceSearchResultItem searchResultItem in selected)
            {
                ISearchResult searchResult = searchResultItem.SearchResult;

                string musicBaseDirectory = Ares.Settings.Settings.Instance.MusicDirectory;
                string soundBaseDirectory = Ares.Settings.Settings.Instance.SoundDirectory;
                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(searchResult);

                // Create a function to download audio to the applicable relative download path
                downloadFunctions.Add((IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize) =>
                {
                    return searchResult.Download(musicBaseDirectory, soundBaseDirectory, relativeDownloadPath, monitor, cancellationToken, totalSize);
                });
                totalDownloadSize += searchResult.DownloadSize;
            }

            return totalDownloadSize;
        }

        /// <summary>
        /// Start a Task to sequentially execute the given download functions.
        /// Progress is monitored using a TaskProgressMonitor and the downloads can be cancelled using the given CancellationToken
        /// </summary>
        /// <param name="downloadFunctions"></param>
        /// <param name="totalDownloadSize"></param>
        private void ExecuteDownloads(List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>> downloadFunctions, double totalDownloadSize)
        {
            TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.DownloadingAudio, this.m_cancellationTokenSource);
            CancellationToken token = this.m_cancellationTokenSource.Token;

            Task<List<AudioDownloadResult>> task = Task.Factory.StartNew(() =>
            {
                monitor.SetIndeterminate(StringResources.DownloadingAudio);
                List<AudioDownloadResult> downloadResults = new List<AudioDownloadResult>();

                foreach (Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult> downloadFunction in downloadFunctions)
                {
                    downloadResults.Add(downloadFunction(monitor, token, totalDownloadSize));
                    token.ThrowIfCancellationRequested();
                }

                return downloadResults;
            });

            // What to do when the downloads complete
            task.ContinueWith((t) =>
            {
                monitor.Close();

                // TODO: Do something with the collected DownloadResults
                List<AudioDownloadResult> downloadResults = task.Result;
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            // What to do when the downloads fail
            task.ContinueWith((t) =>
            {
                monitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.SearchError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

        }

        #endregion

        public void UpdateInformationPanel()
        {
            IEnumerable<AudioSourceSearchResultItem> selectedItems = resultsListView.SelectedItems.Cast<AudioSourceSearchResultItem>();
            int count = resultsListView.SelectedItems.Count;

            if (count > 1)
            {
                // TODO: multiple elements selected, show either an appropriate message ("Multiple entries selected") or common info in the info box
            }
            else if (count == 1)
            {
                // TODO: just a single element selected, show info in the info box
            }
            else if (count < 1)
            {
                // TODO: no elements selected
            }
        }

        #region Event Handlers

        private void searchButton_Click(object sender, EventArgs e)
        {
            ExecuteSearch();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            // TODO: preview/play the selected result
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            // TODO: stop the preview
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Data.Keys.Return)
            {
                e.Handled = true;
                this.ExecuteSearch();
            }
        }

        private void resultsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInformationPanel();
        }

        private void ElementChanged(int elementId, Ares.Editor.Actions.ElementChanges.ChangeType changeType)
        {
            //updateInformationPanel();
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
        }

        #endregion
    }

    public class AudioSourceSearchResultItem : ListViewItem
    {

        private AudioSearchResultType m_ItemAudioType;
        private ISearchResult m_SearchResult;

        public AudioSearchResultType ItemAudioType { get { return m_ItemAudioType; } }
        public ISearchResult SearchResult { get { return m_SearchResult; } }

        /// <summary>
        /// Create an AudioSourceSearchResultItem from te given AudioSource SearchResult
        /// </summary>
        /// <param name="result"></param>
        public AudioSourceSearchResultItem(ISearchResult result): base()
        {
            this.Text = result.Title;
            this.SubItems.Add(result.Author);
            this.SubItems.Add(result.Duration.ToString(@"mm\:ss\:fff"));
            this.m_ItemAudioType = result.ResultType;
            this.m_SearchResult = result;

            switch (result.ResultType)
            {
                case AudioSearchResultType.ModeElement:
                    this.ImageIndex = AudioSourceSearchWindow.IMAGE_INDEX_MODE_ELEMENT;
                    break;
                case AudioSearchResultType.MusicFile:
                    this.ImageIndex = AudioSourceSearchWindow.IMAGE_INDEX_MUSIC;
                    break;
                case AudioSearchResultType.SoundFile:
                    this.ImageIndex = AudioSourceSearchWindow.IMAGE_INDEX_SOUND;
                    break;
            }
        }
    }

}
