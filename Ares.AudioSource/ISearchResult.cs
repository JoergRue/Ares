using Ares.AudioSource;
using Ares.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ares.ModelInfo;
using System.Net;
using System.Threading;

namespace Ares.AudioSource
{
    /// <summary>
    /// Available types of search results
    /// </summary>
    public enum AudioSearchResultType
    {
        /// <summary>
        /// Audio files containing music
        /// </summary>
        MusicFile,
        /// <summary>
        /// Audio files containing sound effects
        /// </summary>
        SoundFile,
        /// <summary>
        /// Complete arrangements of audio files (IModeElements)
        /// </summary>
        ModeElement
    }

    /// <summary>
    /// Generic search result interface
    /// </summary>
    public interface ISearchResult
    {
        /// <summary>
        /// Identifier of this search result entry (unique within the IAudioSource)
        /// </summary>
        string Id { get; }
        /// <summary>
        /// Title of this search result entry
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Author of the search result entry
        /// </summary>
        string Author { get; }
        /// <summary>
        /// Licensing information for this search result entry
        /// </summary>
        string License { get; }
        /// <summary>
        /// Duration of this search result entry, if available
        /// </summary>
        TimeSpan Duration { get; }
        /// <summary>
        /// More elaborate description of this search result entry
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Average rating of this search result entry - may be absent if it hasn't been rated yet.
        /// </summary>
        double? AverageRating { get; }
        /// <summary>
        /// Average rating of this search result entry
        /// </summary>
        int NumberOfRatings { get; }

        /// <summary>
        /// List of tags associated with this search result entry
        /// </summary>
        List<String> Tags { get; }

        /// <summary>
        /// The IAudioSource that returned this search result
        /// </summary>
        IAudioSource AudioSource { get; }

        /// <summary>
        /// The AudioSearchResultType of the search result
        /// </summary>
        AudioSearchResultType ResultType { get; }

        /// <summary>
        /// List of actual audio files required by this search result.
        /// Normally this might contain only one entry (the file itself), but results of 
        /// type ModeElement may require multiple files.
        /// </summary>
        IEnumerable<IDeployableAudioFile> RequiredFiles { get; }
    }

    /// <summary>
    /// An ISearchResult comprised of just a single file which can be added to file containers
    /// within the project
    /// </summary>
    public interface IFileSearchResult : ISearchResult, IDeployableAudioFile
    {

    }

    /// <summary>
    /// An ISearchResult that can be added as a child to a ModeElement
    /// </summary>
    public interface IModeElementSearchResult : ISearchResult
    {
        /// <summary>
        /// Returns the IModeElement definition of this search result.
        /// The ITargetDirectoryProvider is responsible for providing the paths within the ARES library where downloaded audio files
        /// will be placed.
        /// </summary>
        /// <param name="targetDirectoryProvider"></param>
        /// <returns></returns>
        IModeElement GetModeElementDefinition(ITargetDirectoryProvider targetDirectoryProvider);
    }

    /// <summary>
    /// A specific version of the IModeElementSearchResult that can, in addition to a "normal" IModeElement definition
    /// return an IModeElement definition which uses only content streamed directly from the internet.
    /// 
    /// This is relevant for preview/pre-listen mode, where streaming is required so we don't have to download
    /// all files in advance.
    /// </summary>
    public interface IStreamingModeElementSearchResult : IModeElementSearchResult
    {
        /// <summary>
        /// Returns the IModeElement definition of this search result.
        /// The ITargetDirectoryProvider is responsible for providing the paths within the ARES library where downloaded audio files
        /// will be placed.
        /// </summary>
        /// <returns></returns>
        IModeElement GetStreamingModeElementDefinition();
    }

    //#####################################################################################################
    //#
    //# Basic implementations of the above interfaces
    //#
    //#####################################################################################################

    /// <summary>
    /// Basic implementation of ISearchResult
    /// </summary>
    public class BaseSearchResult : ISearchResult
    {
        protected BaseSearchResult(IAudioSource audioSource, AudioSearchResultType resultType)
        {
            this.ResultType = resultType;
            this.AudioSource = audioSource;
            this.FilesToBeDownloaded = new List<IDeployableAudioFile>();
        }

        public IAudioSource AudioSource { get; internal set; }

        public string Author { get; set; }
        public double? AverageRating { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string Id { get; set; }
        public string License { get; set; }
        public int NumberOfRatings { get; set; }
        public AudioSearchResultType ResultType { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }

        public IList<IDeployableAudioFile> FilesToBeDownloaded { get; internal set; }

        IEnumerable<IDeployableAudioFile> ISearchResult.RequiredFiles
        {
            get
            {
                return FilesToBeDownloaded;
            }
        }
    }

    /// <summary>
    /// Basic implementation of an IFileSearchResult downloaded from the internet
    /// </summary>
    public class UrlFileSearchResult : BaseSearchResult, IFileSearchResult, IDownloadableAudioFile
    {
        static WebClient client = new WebClient();

        public UrlFileSearchResult(IAudioSource audioSource, SoundFileType fileType, string url): 
            base(audioSource, fileType == SoundFileType.Music ? AudioSearchResultType.MusicFile : AudioSearchResultType.SoundFile)
        {
            this.FileType = fileType;
            this.SourceUrl = url;
            this.FilesToBeDownloaded.Add(this);
        }

        public double? DownloadSize { get; set; }

        public double? DeploymentCost { get { return DownloadSize; } }

        public string Filename { get; set;  }

        public SoundFileType FileType { get; internal set; }

        public string SourceUrl { get; internal set; }

        public AudioDeploymentResult Deploy(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider)
        {
            return Download(monitor, targetDirectoryProvider);
        }

        public AudioDeploymentResult Download(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider)
        {
            string downloadTargetPath = targetDirectoryProvider.GetFullPath(this);

            // Make sure the target directory exists
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(downloadTargetPath));

            // Only download if the target file doesn't exist
            if (!System.IO.File.Exists(downloadTargetPath))
            {
                // Download the file (synchronously)
                client.DownloadFile(new Uri(SourceUrl), downloadTargetPath);

                // TODO: set additional ID3 information regarding source, author, license, tags etc. on the file
            }
            monitor.IncreaseProgress(DownloadSize.GetValueOrDefault(1));

            return AudioDeploymentResult.SUCCESS;
        }
    }

}
