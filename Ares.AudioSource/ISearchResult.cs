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
    public enum AudioSearchResultType
    {
        MusicFile,
        SoundFile,
        ModeElement
    }

    /// <summary>
    /// Generic search result
    /// </summary>
    public interface ISearchResult<out T> where T : IAudioSource
    {
        string Id { get; }
        string Title { get; }
        string Author { get; }
        string License { get; }
        TimeSpan Duration { get; }
        string Description { get; }
        double AverageRating { get; }
        int NumberOfRatings { get; }

        List<String> Tags { get; }

        T AudioSource { get; }

        /// <summary>
        /// The AudioSearchResultType of the search result
        /// </summary>
        AudioSearchResultType ResultType { get; }

        IEnumerable<IDownloadableAudioFile<T>> FilesToBeDownloaded { get; }
    }

    /// <summary>
    /// An ISearchResult comprised of just a single file which can be added to file containers
    /// within the project
    /// </summary>
    public interface IFileSearchResult<T> : ISearchResult<T>, IDownloadableAudioFile<T> where T : IAudioSource
    {

    }

    /// <summary>
    /// An ISearchResult that can be added as a child to a ModeElement
    /// </summary>
    public interface IModeElementSearchResult<T> : ISearchResult<T> where T : IAudioSource
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

    public class BaseSearchResult<T> : ISearchResult<T> where T : IAudioSource
    {
        protected BaseSearchResult(T audioSource, AudioSearchResultType resultType)
        {
            this.ResultType = resultType;
            this.AudioSource = audioSource;
            this.FilesToBeDownloaded = new List<IDownloadableAudioFile<T>>();
        }

        public T AudioSource { get; internal set; }

        public string Author { get; set; }
        public double AverageRating { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public string Id { get; set; }
        public string License { get; set; }
        public int NumberOfRatings { get; set; }
        public AudioSearchResultType ResultType { get; set; }
        public List<string> Tags { get; set; }
        public string Title { get; set; }

        public IList<IDownloadableAudioFile<T>> FilesToBeDownloaded { get; internal set; }

        IEnumerable<IDownloadableAudioFile<T>> ISearchResult<T>.FilesToBeDownloaded
        {
            get
            {
                return FilesToBeDownloaded;
            }
        }
    }

    public class UrlFileSearchResult<T> : BaseSearchResult<T>, IFileSearchResult<T> where T : IAudioSource
    {
        static WebClient client = new WebClient();

        public UrlFileSearchResult(T audioSource, SoundFileType fileType, string url): 
            base(audioSource, fileType == SoundFileType.Music ? AudioSearchResultType.MusicFile : AudioSearchResultType.SoundFile)
        {
            this.FileType = fileType;
            this.SourceUrl = url;
            this.FilesToBeDownloaded.Add(this);
        }

        public double? DownloadSize { get; set; }

        public string Filename { get; set;  }

        public SoundFileType FileType { get; internal set; }

        public string SourceUrl { get; internal set; }

        public AudioDownloadResult Download(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider)
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

            return AudioDownloadResult.SUCCESS;
        }
    }

}
