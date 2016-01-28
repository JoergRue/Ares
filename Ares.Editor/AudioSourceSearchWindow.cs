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
    /// TODO: Add "Download as" functionality
    /// TODO: Add "Preview" functionality
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

        private const int SEARCH_PAGE_SIZE = 100;
        private const bool VIRTUAL_MODE = false;

        private PluginManager m_PluginManager;
        private ICollection<IAudioSource> m_AudioSources;
        private CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();
        private Ares.Data.IProject m_Project;
        private int m_selectedAudioSourceIndex = 0;

        public AudioSourceSearchWindow(PluginManager pluginManager)
        {
            // Find the audio sources
            this.m_PluginManager = pluginManager;
            this.m_AudioSources = m_PluginManager.GetPluginInstances<IAudioSource>();

            // Initialize the component
            InitializeComponent();

            // Populate the audio sources combo box
            foreach (IAudioSource audioSource in this.m_AudioSources)
            {
                this.audioSourceComboBox.Items.Add(audioSource.Name);
            }
            this.audioSourceComboBox.SelectedIndex = 0;

            // Setup window title & icon
            this.Text = String.Format(StringResources.AudioSourceSearchTitle);
            this.Icon = ImageResources.AudioSourceSearchIcon;

            // Define the page size, image lists used for the list view
            if (VIRTUAL_MODE)
            {
                this.resultsListView.VirtualListSize = 0;
                this.resultsListView.VirtualMode = true;
            }

            this.resultsListView.SmallImageList = sImageList;
            this.resultsListView.LargeImageList = sImageList;

            if (Height > 200)
            {
                splitContainer1.SplitterDistance = Height - 100;
            }
            Ares.Editor.Actions.ElementChanges.Instance.AddListener(-1, ElementChanged);

            this.UpdateButtonsForSelection();
            this.UpdateInformationPanel();
        }

        #region Search 

        /// <summary>
        /// Execute a search with the query given in the UI, retrieving the first result page with the default
        /// (SEARCH_PAGE_SIZE) page size.
        /// </summary>
        public void ExecuteSearchAndApplyResults()
        {
            // Read the search query from the UI
            string query = this.searchBox.Text;

            ExecuteSearchAndApplyResults(query, 0, SEARCH_PAGE_SIZE);
        }

        /// <summary>
        /// Execute a search with the given parameters (search query, page number, page size)
        /// The results will 
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public void ExecuteSearchAndApplyResults(string query, int pageIndex, int pageSize)
        {
            // By default search for any kind of audio (in the future this might be narrowed down based on user input through the UI)
            AudioSearchResultType requestedResultType = AudioSearchResultType.Unknown;

            TaskProgressMonitor monitor = new TaskProgressMonitor(this, StringResources.SearchingForAudio, this.m_cancellationTokenSource);
            CancellationToken token = this.m_cancellationTokenSource.Token;

            int? totalNumberOfResults = null;

            // Start a separate task for executing the search
            Task<IEnumerable<ISearchResult>> task = Task.Factory.StartNew(() =>
            {
                IEnumerable<ISearchResult> results = null;
                IAudioSource audioSource = this.m_AudioSources.ElementAt(this.m_selectedAudioSourceIndex);

                monitor.SetIndeterminate(StringResources.SearchingForAudio);

                results = ExecuteSearchAgainstAudioSource(query, pageSize, pageIndex, audioSource, requestedResultType, monitor, token, out totalNumberOfResults);

                return results;
            });

            // What to do when the search completes
            task.ContinueWith((t) =>
            {
                IEnumerable<ISearchResult> results = task.Result;
                monitor.Close();

                // If the list is in virtual mode cache the results & pass the total number of results to the list
                if (VIRTUAL_MODE)
                {
                    this.m_ItemCacheQuery = query;
                    this.m_ItemCache = results
                                        .Select(result => new SearchResultListItem(result))
                                        .ToArray();
                    this.m_ItemCacheFirstIndex = pageIndex * pageSize;
                    this.resultsListView.VirtualListSize = totalNumberOfResults.GetValueOrDefault(0);
                }
                else
                // Otherwise put the results right into the list
                {
                    this.resultsListView.BeginUpdate();
                    // Clear the results list in the UI
                    this.resultsListView.Items.Clear();
                    // Go through all search results
                    foreach (ISearchResult result in results)
                    {
                        // Wrap them into AudioSourceSearchResultItems and add to UI results list
                        this.resultsListView.Items.Add(
                            new SearchResultListItem(result)
                        );
                    }
                    this.resultsListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    this.resultsListView.EndUpdate();
                }
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

        /// <summary>
        /// Actually execute a search against a specific IAudioSource
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="audioSource"></param>
        /// <param name="requestedResultType"></param>
        /// <param name="monitor"></param>
        /// <param name="token"></param>
        /// <param name="totalNumberOfResults"></param>
        /// <returns></returns>
        private IEnumerable<ISearchResult> ExecuteSearchAgainstAudioSource(string query, int pageSize, int pageIndex, IAudioSource audioSource, AudioSearchResultType requestedResultType, IProgressMonitor monitor, CancellationToken token, out int? totalNumberOfResults)
        {
            IEnumerable<ISearchResult> results = null;

            // Only search through sources that actually support the requested result type
            if (audioSource.IsAudioTypeSupported(requestedResultType))
            {
                // Retrieve the results
                results = audioSource.GetSearchResults(query, AudioSearchResultType.Unknown, pageSize, pageIndex, monitor, token, out totalNumberOfResults);
            }
            else
            {
                // Other sources return no results
                totalNumberOfResults = 0;
                results = new List<ISearchResult>();
            }

            token.ThrowIfCancellationRequested();

            return results;
        }

        #endregion

        #region Drag & Drop of search results into the project

        /// <summary>
        /// Start a drag&drop operation for the items currently selected in the results list
        /// </summary>
        private void StartDragSelectedItems()
        {
            // Make sure there actually is a selection
            if (resultsListView.SelectedItems.Count < 1)
            {
                // No item selecte, don't drag
                return;
            }

            IEnumerable<SearchResultListItem> selectedItems = resultsListView.SelectedItems.Cast<SearchResultListItem>();

            AudioSearchResultType overallItemAudioType = FindAndVerifySelectedAudioType(selectedItems);

            // Decide depending on the overall AudioType of the selected items
            DragDropEffects dragDropResult = DragDropEffects.None;
            switch (overallItemAudioType)
            {
                // If the dragged items are Music or Sound files
                case AudioSearchResultType.MusicFile:
                case AudioSearchResultType.SoundFile:
                    dragDropResult = StartDragFileSearchResults(
                        selectedItems
                        // Extract the IFileSearchResult from the SearchResultListItem
                        .Select(item => item.SearchResult as IFileSearchResult)
                        // Filter out null/incompatible search results
                        .Where(result => result != null)
                    , overallItemAudioType);
                    break;
                // If the dragged items are ModeElements
                case AudioSearchResultType.ModeElement:
                    dragDropResult = StartDragModeElementSearchResults(
                        selectedItems
                        // Extract the IModeElementSearchResult from the SearchResultListItem
                        .Select(item => item.SearchResult as IModeElementSearchResult)
                        // Filter out null/incompatible search results
                        .Where(result => result != null)
                    );
                    break;
            }

            if (dragDropResult == DragDropEffects.Copy)
            {
                DownloadFiles(selectedItems,null);
            }
        }

        /// <summary>
        /// Determine the AudioSearchResultType of the given selected items.
        /// The first item's AudioSearchResultType is assumed to apply to all items, but
        /// this is verified and an InvalidOperationException is thrown if any item in the 
        /// list has a different AudioSearchResultType than the first one.
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        private static AudioSearchResultType FindAndVerifySelectedAudioType(IEnumerable<SearchResultListItem> selectedItems)
        {
            // Determine the overall AudioType from the first selected item
            SearchResultListItem firstItem = selectedItems.First();
            AudioSearchResultType overallItemAudioType = firstItem.ItemAudioType;

            // Make sure all other items' AudioTypes are compatible (the same)
            foreach (SearchResultListItem item in selectedItems)
            {
                if (item.ItemAudioType != overallItemAudioType)
                {
                    // TODO: show a message to the user that he should only select items of the same type?
                    throw new InvalidOperationException();
                }
            }

            return overallItemAudioType;
        }

        /// <summary>
        /// Start a drag&drop operation with the given IModelElementSearchResults
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        private DragDropEffects StartDragModeElementSearchResults(IEnumerable<IModeElementSearchResult> searchResults)
        {
            DragDropEffects dragDropResult;
            List<IXmlWritable> draggedItems = new List<IXmlWritable>();

            foreach (IModeElementSearchResult modeElementSearchResult in searchResults)
            {
                // Determine relevant directories
                string musicBaseDirectory = Ares.Settings.Settings.Instance.MusicDirectory;
                string soundBaseDirectory = Ares.Settings.Settings.Instance.SoundDirectory;
                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(modeElementSearchResult);

                // Get the ModeElement definition
                draggedItems.Add(modeElementSearchResult.GetModeElementDefinition(relativeDownloadPath));
            }

            // Start a drag & drop operation for those project elements
            dragDropResult = StartDragXmlWritables(draggedItems);
            return dragDropResult;
        }

        /// <summary>
        /// Start a drag&drop operation with the given IFileSearchResults 
        /// </summary>
        /// <param name="searchResults"></param>
        /// <param name="overallItemAudioType"></param>
        /// <returns></returns>
        private DragDropEffects StartDragFileSearchResults(IEnumerable<IFileSearchResult> searchResults, AudioSearchResultType overallItemAudioType)
        {
            DragDropEffects dragDropResult;
            List<DraggedItem> draggedFiles = new List<DraggedItem>();

            foreach (IFileSearchResult fileSearchResult in searchResults)
            {
                // Create a new DraggedItem (dragged file/folder)
                DraggedItem draggedFile = new DraggedItem();

                // Set item & node type for the file
                draggedFile.ItemType = overallItemAudioType == AudioSearchResultType.MusicFile ? FileType.Music : FileType.Sound;
                draggedFile.NodeType = DraggedItemType.File;

                // Determine tthe relative path where the downloaded file will be placed
                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(fileSearchResult);
                draggedFile.RelativePath = System.IO.Path.Combine(relativeDownloadPath, fileSearchResult.GetDownloadFilename());

                draggedFiles.Add(draggedFile);
            }

            // Start a file/folder drag & drop operation for those files
            dragDropResult = StartDragFiles(draggedFiles);
            return dragDropResult;
        }

        /// <summary>
        /// Start a drag&drop operation with a collection of Project elements (IXmlWritable) to be dropped into the project structure
        /// </summary>
        /// <param name="exportItems"></param>
        public DragDropEffects StartDragXmlWritables(List<IXmlWritable> draggedItems)
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

        /// <summary>
        /// Get the relative path below the music/sounds directories where files downloaded for the given ISearchResult should be placed
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
        /// Download the files for all selected items to the default locations (relative path determined by GetRelativeDownloadPathForSearchResult)
        /// based on their type (sound/music)
        /// </summary>
        /// <param name="selected"></param>
        private void DownloadFiles(IEnumerable<SearchResultListItem> selected, string overrideTargetPath)
        {
            // Collect download actions for each dragged element to be executed after a drag completes
            List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>> downloadFunctions = new List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>>();
            double totalDownloadSize = 0;

            foreach (SearchResultListItem searchResultItem in selected)
            {
                ISearchResult searchResult = searchResultItem.SearchResult;
                totalDownloadSize += searchResult.DownloadSize;

                string relativeDownloadPath = GetRelativeDownloadPathForSearchResult(searchResult);

                // Determine the music & sound target directories
                // If given, use the overrideTargetPath, otherwise use the defaults
                string musicTargetDirectory = overrideTargetPath;
                if (musicTargetDirectory == null) { 
                    musicTargetDirectory = System.IO.Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, relativeDownloadPath);
                }
                string soundTargetDirectory = overrideTargetPath;
                if (soundTargetDirectory == null)
                {
                    System.IO.Path.Combine(Ares.Settings.Settings.Instance.SoundDirectory, relativeDownloadPath);
                }

                // Create a function to download audio to the applicable relative download path
                downloadFunctions.Add((IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize) =>
                {
                    return searchResult.Download(musicTargetDirectory, soundTargetDirectory, monitor, cancellationToken, totalSize);
                });

            }

            // If the drag&drop resulted in a Copy action, download the audio content
            ExecuteDownloadFunctionsInTask(downloadFunctions, totalDownloadSize);            
        }

        /// <summary>
        /// Start a Task to sequentially execute the given download functions.
        /// Progress is monitored using a TaskProgressMonitor and the downloads can be cancelled using the given CancellationToken
        /// </summary>
        /// <param name="downloadFunctions"></param>
        /// <param name="totalDownloadSize"></param>
        private void ExecuteDownloadFunctionsInTask(List<Func<IProgressMonitor, CancellationToken, double, AudioDownloadResult>> downloadFunctions, double totalDownloadSize)
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
            Action action = new Action(() => {
                IEnumerable<SearchResultListItem> selectedItems = resultsListView.SelectedItems.Cast<SearchResultListItem>();
                int count = resultsListView.SelectedItems.Count;

                if (count > 1)
                {
                    // multiple elements selected
                    // show either an appropriate message ("Multiple entries selected") or common info in the info box
                    informationBox.Text = StringResources.PleaseSelectASingleItemToSeeMoreInfo;
                }
                else if (count == 1)
                {
                    // Just a single element has been selected
                    string msg = "";
                    SearchResultListItem item = selectedItems.First();
                    msg += String.Format(StringResources.Length, (DateTime.Today + item.SearchResult.Duration).ToString("HH::mm::ss.fff"));

                    if (item.SearchResult.ResultType == AudioSearchResultType.MusicFile)
                    {
                        if (item.SearchResult.Tags.Count > 0)
                        {
                            msg += Environment.NewLine + StringResources.Tags + item.SearchResult.Tags.Aggregate((s1, s2) =>
                            {
                                return s1 + ";" + s2;
                            });
                        }
                        else
                        {
                            msg += Environment.NewLine + StringResources.NoTags;
                        }
                    }
                    informationBox.Text = msg;
                }
                else if (count < 1)
                {
                    // no elements selected
                    informationBox.Text = "";
                }
            });

            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        /// <summary>
        /// Update all relevant buttons in the window depending on whether any items are selected
        /// </summary>
        /// <param name="selectionIsEmpty"></param>
        private void UpdateButtonsForSelection()
        {
            Action action = new Action(() => {
                bool selectionIsNotEmpty = resultsListView.SelectedItems.Count > 0;
                bool isCurrentlyPlaying = false;

                downloadMenuItem.Enabled = selectionIsNotEmpty;
                //downloadToMenuItem.Enabled = selectionIsNotEmpty;
                downloadToMenuItem.Enabled = false;
                
                playButton.Enabled = selectionIsNotEmpty && !isCurrentlyPlaying;
                playToolStripMenuItem.Enabled = selectionIsNotEmpty && !isCurrentlyPlaying;
                stopButton.Enabled = selectionIsNotEmpty && isCurrentlyPlaying;
                stopToolStripMenuItem.Enabled = selectionIsNotEmpty && isCurrentlyPlaying;
            });

            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        public void PlaySelectedElement()
        {
            
        }

        #region Event Handlers

        private void resultsListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            StartDragSelectedItems();
        }


        private void searchButton_Click(object sender, EventArgs e)
        {
            ExecuteSearchAndApplyResults();
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
                this.ExecuteSearchAndApplyResults();
            }
        }

        private void resultsListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInformationPanel();
            UpdateButtonsForSelection();
        }

        private void ElementChanged(int elementId, Ares.Editor.Actions.ElementChanges.ChangeType changeType)
        {
            //updateInformationPanel();
        }

        public void SetProject(Ares.Data.IProject project)
        {
            m_Project = project;
        }

        private void downloadMenuItem_Click(object sender, EventArgs e)
        {
            // Download the selected files to their default target path
            DownloadFiles(this.resultsListView.SelectedItems.Cast<SearchResultListItem>(),null);
        }

        private void downloadToMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: provide "download to" functionality overriding the default target folder
        }

        private void audioSourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.m_selectedAudioSourceIndex = this.audioSourceComboBox.SelectedIndex;   
            // If there is an active search query, re-run it
            if (!String.IsNullOrWhiteSpace(this.searchBox.Text))
            {
                ExecuteSearchAndApplyResults();
            }
        }

        #endregion

        #region List View item cache

        private string m_ItemCacheQuery = "";
        private int m_ItemCacheFirstIndex = 0;
        private SearchResultListItem[] m_ItemCache = null;

        private void resultsListView_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            
        }

        private void resultsListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            // Check whether the requested result index is in our current cache
            if (m_ItemCache != null && e.ItemIndex >= m_ItemCacheFirstIndex && e.ItemIndex < m_ItemCacheFirstIndex + m_ItemCache.Length)
            {
                //A cache hit, so get the item from the cache instead of making a new one.
                e.Item = m_ItemCache[e.ItemIndex - m_ItemCacheFirstIndex];
            }
            else
            {
                // Otherwise figure out which page the item is on
                int pageIndex = e.ItemIndex / SEARCH_PAGE_SIZE;
                // Search for & load that page into the cache
                ExecuteSearchAndApplyResults(m_ItemCacheQuery,pageIndex,SEARCH_PAGE_SIZE);

                // TODO: figure out how to (in an async way) get the requested item back into the event once the search returns
            }
        }

        #endregion
    }

    public class SearchResultListItem : ListViewItem
    {

        private AudioSearchResultType m_ItemAudioType;
        private ISearchResult m_SearchResult;

        public AudioSearchResultType ItemAudioType { get { return m_ItemAudioType; } }
        public ISearchResult SearchResult { get { return m_SearchResult; } }

        /// <summary>
        /// Create an AudioSourceSearchResultItem from te given AudioSource SearchResult
        /// </summary>
        /// <param name="result"></param>
        public SearchResultListItem(ISearchResult result): base()
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
