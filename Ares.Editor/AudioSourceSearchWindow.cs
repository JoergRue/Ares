/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 Copyright (c) 2016 [Martin Ried]
 
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
    /// The target path for each audio file to be downloaded is determined when the drag & drop operation is started.
    /// The actual download only occurs when (and if) the drag & drop operation completes.
    /// 
    /// TODO: Find a way to delay the ARES missing-file-detection.
    ///       Since the actual download only occurs after the drag & drop is completed, ARES comes to think that the
    ///       files don't exist (the check occurs "on drop", the actual download only after the drag & drop is completed)
    ///       One solution might be to touch the files (create empty placeholders) when the drag & drop starts and either
    ///       replace or remove them upon completion, depending on whether the drag & drop was completed or cancelled.
    /// 
    /// TODO: Add "Download as" functionality so the user can download a search result to a specific folder & filename
    ///
    /// TODO: Possibly add additional preview functionality through temporary downloads so that any ISearchResult can be previewed
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

        /// <summary>
        /// Default page size for retrieving IAudioSource search results.
        /// </summary>
        private int m_searchPageSize = 50;
        private int m_searchPageIndex = 0;

        private PluginManager m_PluginManager;
        private ICollection<IAudioSource> m_AudioSources;
        private Ares.Data.IProject m_Project;
        private IAudioSource m_selectedAudioSource = null;
        private IElement m_PlayedElement;

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
            // Initially selected the first available audio source
            this.audioSourceComboBox.SelectedIndex = 0;
            this.m_selectedAudioSource = m_AudioSources.ElementAt(0);

            // Setup window title & icon
            this.Text = String.Format(StringResources.AudioSourceSearchTitle);
            this.Icon = ImageResources.AudioSourceSearchIcon;

            // Define the page size, image lists used for the list view
            this.m_searchPageSize = 100;
            this.resultsListView.SmallImageList = sImageList;
            this.resultsListView.LargeImageList = sImageList;

            // Initialize the splitter
            if (Height > 200)
            {
                splitContainer1.SplitterDistance = Height - 100;
            }

            // Set up a listener for when project model elements change
            Ares.Editor.Actions.ElementChanges.Instance.AddListener(-1, ElementChanged);

            // Initialize the buttons & info panel state
            this.UpdateButtonsForSelection();
            this.UpdateInformationPanel();
            this.UpdatePagingControls(0, 0, null);
        }

        #region Search 

        /// <summary>
        /// Execute a search with the query given in the UI (searchBox), retrieving the first result page with the default
        /// (m_searchPageSize) page size.
        /// </summary>
        public void SearchWithUiQueryAsync()
        {
            // Read the search query from the UI
            string query = this.searchBox.Text;

            // Reset to the first page of search results
            m_searchPageIndex = 0;

            // Run a search for the now current parameters
            SearchAsync(query, m_searchPageIndex, m_searchPageSize);
        }

        /// <summary>
        /// Execute a search with the given parameters (search query, page number, page size)
        /// The results will either be passed to ProcessSearchResults.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public Task<IEnumerable<ISearchResult>> SearchAsync(string query, int pageIndex, int pageSize)
        {
            TaskProgressMonitor baseMonitor = new TaskProgressMonitor(this, StringResources.SearchingForAudio, new CancellationTokenSource());
            IAbsoluteProgressMonitor absoluteMonitor = new AbsoluteProgressMonitor(baseMonitor, 1, StringResources.SearchingForAudio);

            int? totalNumberOfResults = null;

            Task<IEnumerable<ISearchResult>> task = Task.Factory.StartNew(() =>
            {
                absoluteMonitor.SetIndeterminate();

                // Ask the AudioSource for (async) search results
                try {
                    Task<IEnumerable<ISearchResult>> searchSubtask = this.m_selectedAudioSource.Search(query, pageSize, pageIndex, absoluteMonitor, out totalNumberOfResults);

                    return searchSubtask.Result;
                } catch (OperationCanceledException e) {
                    return new List<ISearchResult>();
                }
            });            

            // What to do when the search fails
            task.ContinueWith((t) =>
            {
                // Close the progress monitor
                baseMonitor.Close();

                // If an actual exception occurred (which should be the case)...
                if (t.Exception != null)
                {
                    // ... show an error popup
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.SearchError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            // What to do when the search completes
            return task.ContinueWith((t) =>
            {
                // Close the progress monitor
                baseMonitor.Close();

                // Retrieve the results
                IEnumerable<ISearchResult> results = task.Result;
                
                // Perform an update on the results ListView
                this.resultsListView.BeginUpdate();
                // Process the results
                ProcessSearchResults(results, query, pageIndex, pageSize, totalNumberOfResults);
                this.resultsListView.EndUpdate();

                return results;
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Process search results.
        /// </summary>
        /// <param name="results"></param>
        private void ProcessSearchResults(IEnumerable<ISearchResult> results, string query, int pageIndex, int pageSize, int? totalNumberOfResults)
        {
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

            UpdatePagingControls(pageIndex, pageSize, totalNumberOfResults);
        }

        /// <summary>
        /// Update the paging controls: current page number, total number of pages, previous/next page buttons
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="totalNumberOfResults"></param>
        private void UpdatePagingControls(int pageIndex, int pageSize, int? totalNumberOfResults)
        {
            if (!totalNumberOfResults.HasValue)
            {
                splitContainer1.SplitterDistance = Height;
            } else
            {
                splitContainer1.SplitterDistance = Height - 100;
            }

            int? totalNumberOfPages = (int?)null;
            if (totalNumberOfResults.HasValue) {
                totalNumberOfPages = (int)Math.Ceiling((decimal)totalNumberOfResults.Value / (decimal)pageSize);
            }

            nextPageButton.Enabled = totalNumberOfPages == null || (pageIndex + 1) < totalNumberOfPages;
            prevPageButton.Enabled = pageIndex > 0;
            pageLabel.Text = (pageIndex + 1) + " / " + (totalNumberOfPages.HasValue ? totalNumberOfPages.Value.ToString() : "?");
        }

#endregion

        #region Drag & Drop of search results into the project

        /// <summary>
        /// Start a drag&drop operation for the items currently selected in the results list
        /// </summary>
        private void StartDragSelectedResults()
        {
            // Make sure there actually is a selection
            if (resultsListView.SelectedIndices.Count < 1)
            {
                // No item selecte, don't drag
                return;
            }

            IEnumerable<SearchResultListItem> selectedItems = GetSelectedItems();

            AresTargetDirectoryProvider targetDirectoryProvider = new AresTargetDirectoryProvider(m_selectedAudioSource);
            AudioSearchResultType overallItemAudioType = FindAndVerifySelectedAudioType(selectedItems);

            // Decide depending on the overall AudioType of the selected items
            DragDropEffects dragDropResult = DragDropEffects.None;
            switch (overallItemAudioType)
            {
                // If the dragged items are Music or Sound files
                case AudioSearchResultType.MusicFile:
                case AudioSearchResultType.SoundFile:
                    IEnumerable<IFileSearchResult> selectedFileResults = 
                        selectedItems
                        // Extract the IFileSearchResult from the SearchResultListItem
                        .Select(item => item.SearchResult as IFileSearchResult)
                        // Filter out null/incompatible search results
                        .Where(result => result != null);

                    BeforeStartDrag(selectedFileResults, targetDirectoryProvider);
                    dragDropResult = StartDragFileSearchResults(selectedFileResults, overallItemAudioType, targetDirectoryProvider);

                    if (dragDropResult == DragDropEffects.Copy)
                    {
                        AfterCompleteDrag(selectedFileResults, targetDirectoryProvider);
                    }
                    else
                    {
                        AfterCancelDrag(selectedFileResults, targetDirectoryProvider);
                    }
                    return;
                // If the dragged items are ModeElements
                case AudioSearchResultType.ModeElement:
                    IEnumerable<IModeElementSearchResult> selectedModeElementResults =
                        selectedItems
                        // Extract the IModeElementSearchResult from the SearchResultListItem
                        .Select(item => item.SearchResult as IModeElementSearchResult)
                        // Filter out null/incompatible search results
                        .Where(result => result != null);

                    BeforeStartDrag(selectedModeElementResults, targetDirectoryProvider);
                    dragDropResult = StartDragModeElementSearchResults(selectedModeElementResults, targetDirectoryProvider);

                    if (dragDropResult == DragDropEffects.Copy)
                    {
                        AfterCompleteDrag(selectedModeElementResults, targetDirectoryProvider);
                    }
                    else
                    {
                        AfterCancelDrag(selectedModeElementResults, targetDirectoryProvider);
                    }
                    return;
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
                    // Show a message to the user that he should only select items of the same type?
                    throw new InvalidOperationException(StringResources.PleaseSelectOnlyEntriesOfTheSameType);
                }
            }

            return overallItemAudioType;
        }

        /// <summary>
        /// Start a drag&drop operation with the given IModelElementSearchResults
        /// </summary>
        /// <param name="selectedItems"></param>
        /// <returns></returns>
        private DragDropEffects StartDragModeElementSearchResults(IEnumerable<IModeElementSearchResult> searchResults, AresTargetDirectoryProvider targetDirectoryProvider)
        {
            List<IXmlWritable> draggedItems = new List<IXmlWritable>();

            foreach (IModeElementSearchResult modeElementSearchResult in searchResults)
            {
                // Get the ModeElement definition
                draggedItems.Add(modeElementSearchResult.GetModeElementDefinition(targetDirectoryProvider));
            }

            // Start a drag & drop operation for those project elements
            StringBuilder serializedForm = new StringBuilder();
            Data.DataModule.ProjectManager.ExportElements(draggedItems, serializedForm);
            ProjectExplorer.ClipboardElements cpElements = new ProjectExplorer.ClipboardElements() { SerializedForm = serializedForm.ToString() };
            return DoDragDrop(cpElements, DragDropEffects.Move);
        }

        /// <summary>
        /// Start a drag&drop operation with the given IFileSearchResults 
        /// </summary>
        /// <param name="searchResults"></param>
        /// <param name="overallItemAudioType"></param>
        /// <returns></returns>
        private DragDropEffects StartDragFileSearchResults(IEnumerable<IFileSearchResult> searchResults, AudioSearchResultType overallItemAudioType, AresTargetDirectoryProvider targetDirectoryProvider)
        {
            List<DraggedItem> draggedFiles = new List<DraggedItem>();

            foreach (IFileSearchResult fileSearchResult in searchResults)
            {
                // Create a new DraggedItem (dragged file/folder)
                DraggedItem draggedFile = new DraggedItem();

                // Set item & node type for the file
                draggedFile.ItemType = overallItemAudioType == AudioSearchResultType.MusicFile ? FileType.Music : FileType.Sound;
                draggedFile.NodeType = DraggedItemType.File;

                // Determine the relative path where the downloaded file will be placed
                string relativeDownloadPath = targetDirectoryProvider.GetFolderWithinLibrary(fileSearchResult);
                draggedFile.RelativePath = System.IO.Path.Combine(relativeDownloadPath, fileSearchResult.Filename);
                draggedFiles.Add(draggedFile);
            }

            // Start a file/folder drag & drop operation for those files
            FileDragInfo info = new FileDragInfo();
            info.DraggedItems = draggedFiles;
            info.TagsFilter = new TagsFilter();
            return DoDragDrop(info, DragDropEffects.Copy);
        }

        public void BeforeStartDrag(IEnumerable<ISearchResult> selectedResults, ITargetDirectoryProvider targetDirectoryProvider)
        {
            // TODO: Create stubs for all required files which don't exist yet
        }

        public void AfterCompleteDrag(IEnumerable<ISearchResult> selectedResults, ITargetDirectoryProvider targetDirectoryProvider)
        {
            // TODO: actually download all required files (unless already available)
            DeployRequiredFilesForSearchResults(selectedResults,targetDirectoryProvider);
        }

        public void AfterCancelDrag(IEnumerable<ISearchResult> selectedResults, ITargetDirectoryProvider targetDirectoryProvider)
        {
            // TODO: remove stubs for all required files which haven't yet been downloaded
        }

        #endregion

        #region File downloading / deployment

        /// <summary>
        /// Download the files for all given ISearchResults to the locations specified by the given ITargetDirectoryProvider.
        /// based on their type (sound/music)
        /// </summary>
        /// <param name="results"></param>
        private void DeployRequiredFilesForSearchResults(IEnumerable<ISearchResult> results, ITargetDirectoryProvider targetDirectoryProvider)
        {
            // Collect download size & action for each result to be executed after a drag completes
            double totalDownloadSize = 0;
            foreach (SearchResultListItem searchResultItem in results)
            {
                ISearchResult searchResult = searchResultItem.SearchResult;
                foreach (IDeployableAudioFile deployableFile in searchResult.RequiredFiles) {
                    if (deployableFile is IDownloadableAudioFile) {
                        totalDownloadSize += (deployableFile as IDownloadableAudioFile).DownloadSize.GetValueOrDefault(1);
                    } else
                    {
                        totalDownloadSize += 1;
                    }
                    
                }                
            }

            // Initialize a task monitor for the download process
            TaskProgressMonitor baseMonitor = new TaskProgressMonitor(this, StringResources.DownloadingAudio, new CancellationTokenSource());
            IAbsoluteProgressMonitor absoluteMonitor = new AbsoluteProgressMonitor(baseMonitor, totalDownloadSize, StringResources.DownloadingAudio);

            // Start a separate task for downloading the results
            Task<List<AudioDeploymentResult>> task = Task.Factory.StartNew(() =>
            {
                absoluteMonitor.SetIndeterminate();
                List<AudioDeploymentResult> downloadResults = new List<AudioDeploymentResult>();

                // Go through all results
                foreach (SearchResultListItem searchResultItem in results)
                {
                    ISearchResult searchResult = searchResultItem.SearchResult;
                    foreach (IDeployableAudioFile deployableFile in searchResult.RequiredFiles)
                    {
                        downloadResults.Add(DeployFile(deployableFile, absoluteMonitor, targetDirectoryProvider));
                        if (deployableFile is IDownloadableAudioFile)
                        {
                            absoluteMonitor.IncreaseProgress((deployableFile as IDownloadableAudioFile).DownloadSize.GetValueOrDefault(1));
                        }
                        else
                        {
                            absoluteMonitor.IncreaseProgress(1);
                        }
                    }
                }

                return downloadResults;
            });

            // What to do when the downloads complete
            task.ContinueWith((t) =>
            {
                baseMonitor.Close();

                // TODO: Do something with the collected DownloadResults
                List<AudioDeploymentResult> downloadResults = task.Result;
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.NotOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());

            // What to do when the downloads fail
            task.ContinueWith((t) =>
            {
                baseMonitor.Close();
                if (t.Exception != null)
                {
                    TaskHelpers.HandleTaskException(this, t.Exception, StringResources.SearchError);
                }
            }, System.Threading.CancellationToken.None, System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
        }

        private AudioDeploymentResult DeployFile(IDeployableAudioFile downloadableFile, IAbsoluteProgressMonitor absoluteMonitor, ITargetDirectoryProvider targetDirectoryProvider)
        {
            // TODO: possibly add caching / temp-files to avoid duplicate downloads after a preview
            return downloadableFile.Deploy(absoluteMonitor, targetDirectoryProvider);
        }

        #endregion

        #region Playback / Preview
        public ISearchResult GetSelectedPlaybackCandidate()
        {
            SearchResultListItem item = GetSelectedItems().FirstOrDefault();
            if (item != null)
            {
                return item.SearchResult;
            } else
            {
                return null;
            }
        }

        public bool CanPlaySelectedElement()
        {
            if (GetSelectedPlaybackCandidate() is IDownloadableAudioFile) return true;
            if (GetSelectedPlaybackCandidate() is IStreamingModeElementSearchResult) return true;

            // TODO: possibly enable preview for any SearchResult (once temporary download is implemented)

            return false;
        }

        public void PlaySelectedElement()
        {
            // 
            if (GetSelectedPlaybackCandidate() is IDownloadableAudioFile) {
                IDownloadableAudioFile result = GetSelectedPlaybackCandidate() as IDownloadableAudioFile;
                this.m_PlayedElement =
                    Actions.Playing.Instance.PlayURL(result.SourceUrl, result.FileType == SoundFileType.Music, this, () =>
                    //Actions.Playing.Instance.PlayFile(item.RelativePath, item.ItemType == FileType.Music, this, () =>
                {
                    this.m_PlayedElement = null;
                    UpdateButtonsForSelection();
                });
                UpdateButtonsForSelection();
            }

            //
            if (GetSelectedPlaybackCandidate() is IStreamingModeElementSearchResult)
            {
                IStreamingModeElementSearchResult result = GetSelectedPlaybackCandidate() as IStreamingModeElementSearchResult;
                IElement modeElement = result.GetStreamingModeElementDefinition();
                this.m_PlayedElement = modeElement;
                Actions.Playing.Instance.PlayElement(modeElement, this, () =>
                //Actions.Playing.Instance.PlayFile(item.RelativePath, item.ItemType == FileType.Music, this, () =>
                {
                    this.m_PlayedElement = null;
                    UpdateButtonsForSelection();
                });
                UpdateButtonsForSelection();
            }

            // TODO: possibly enable temporary download to preview any SearchResult
        }

        public void StopPlayingElement()
        {
            if (m_PlayedElement != null)
            {
                Actions.Playing.Instance.StopElement(m_PlayedElement);
            }
            UpdateButtonsForSelection();
        }

        #endregion

        #region UI helpers

        public void UpdateInformationPanel()
        {
            Action action = new Action(() => {
                IEnumerable<SearchResultListItem> selectedItems = GetSelectedItems();
                int count = resultsListView.SelectedIndices.Count;

                if (count > 1)
                {
                    // multiple elements selected
                    // show either an appropriate message ("Multiple entries selected") or common info in the info box
                    informationBox.Text = StringResources.PleaseSelectASingleItemToSeeMoreInfo;
                }
                else if (count == 1 && selectedItems.First().SearchResult != null)
                {
                    // Just a single element has been selected
                    string msg = "";
                    SearchResultListItem item = selectedItems.First();
                    msg += String.Format(StringResources.Length, (DateTime.Today + item.SearchResult.Duration).ToString("HH::mm::ss.fff"));

                    if (item.SearchResult.ResultType == AudioSearchResultType.MusicFile)
                    {
                        if (item.SearchResult.Tags != null && item.SearchResult.Tags.Count > 0)
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
                bool selectionIsNotEmpty = resultsListView.SelectedIndices.Count > 0;
                bool isCurrentlyPlaying = this.m_PlayedElement != null;
                bool isPlaybackAllowed = CanPlaySelectedElement();
                

                downloadMenuItem.Enabled = selectionIsNotEmpty;
                //downloadToMenuItem.Enabled = selectionIsNotEmpty;
                downloadToMenuItem.Enabled = false;
                
                playButton.Enabled = selectionIsNotEmpty && !isCurrentlyPlaying && isPlaybackAllowed;
                playToolStripMenuItem.Enabled = selectionIsNotEmpty && !isCurrentlyPlaying && isPlaybackAllowed;
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

        public IEnumerable<SearchResultListItem> GetSelectedItems()
        {
            return this.resultsListView.SelectedItems.Cast<SearchResultListItem>();
        }

        #endregion

        #region Event Handlers

        private void resultsListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            StartDragSelectedResults();
        }


        private void searchButton_Click(object sender, EventArgs e)
        {
            SearchWithUiQueryAsync();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            PlaySelectedElement();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopPlayingElement();
        }

        private void searchBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Data.Keys.Return)
            {
                e.Handled = true;
                this.SearchWithUiQueryAsync();
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


        private void playToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySelectedElement();
        }

        private void downloadMenuItem_Click(object sender, EventArgs e)
        {
            // Download the selected files to their default target path
            DeployRequiredFilesForSearchResults(
                GetSelectedItems()
                // Extract the IFileSearchResult from the SearchResultListItem
                .Select(item => item.SearchResult as IFileSearchResult)
                // Filter out null/incompatible search results
                .Where(result => result != null),
                new AresTargetDirectoryProvider(m_selectedAudioSource)
            );
        }

        private void downloadToMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: provide "download to" functionality overriding the default target folder
        }

        private void audioSourceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.m_selectedAudioSource = this.m_AudioSources.ElementAt(this.audioSourceComboBox.SelectedIndex);
            // If there is an active search query, re-run it
            if (!String.IsNullOrWhiteSpace(this.searchBox.Text))
            {
                SearchWithUiQueryAsync();
            }
        }

        private void nextPageButton_Click(object sender, EventArgs e)
        {

        }

        private void prevPageButton_Click(object sender, EventArgs e)
        {

        }

        #endregion

    }

    public class SearchResultListItem : ListViewItem
    {

        private AudioSearchResultType m_ItemAudioType;
        private ISearchResult m_SearchResult;

        public AudioSearchResultType ItemAudioType { get { return m_ItemAudioType; } }
        public ISearchResult SearchResult { get { return m_SearchResult; } }

        public SearchResultListItem() : base()
        {
            this.Text = "";
            this.SubItems.Add("");
            this.SubItems.Add("");
        }

        /// <summary>
        /// Create an AudioSourceSearchResultItem from the given AudioSource SearchResult
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

    /// <summary>
    /// ITargetDirectoryProvider that places downloaded files in the ARES sound/music libraries.
    /// All downloaded files will be placed in a top-level folder named "OnlineAudioSources" and a subfolder
    /// specific to the IAudioSource.
    /// </summary>
    class AresTargetDirectoryProvider: ITargetDirectoryProvider
    {
        private IAudioSource m_audioSource;

        public AresTargetDirectoryProvider(IAudioSource audioSource)
        {
            this.m_audioSource = audioSource;
        }

        public string GetFolderWithinLibrary(IDeployableAudioFile audioFile)
        {
            string audioSourceId = audioFile.AudioSource.Id;
            return System.IO.Path.Combine("OnlineAudioSources", audioSourceId);
        }

        public string GetPathWithinLibrary(IDeployableAudioFile audioFile)
        {
            return System.IO.Path.Combine(GetFolderWithinLibrary(audioFile),audioFile.Filename);
        }

        public string GetFullPath(IDeployableAudioFile audioFile)
        {
            string basePath = null;
            switch (audioFile.FileType)
            {
                case SoundFileType.Music:
                    basePath = Ares.Settings.Settings.Instance.MusicDirectory;
                    break;
                case SoundFileType.SoundEffect:
                    basePath = Ares.Settings.Settings.Instance.SoundDirectory;
                    break;
            }
            return System.IO.Path.Combine(basePath, GetPathWithinLibrary(audioFile));
        }

    }

}
