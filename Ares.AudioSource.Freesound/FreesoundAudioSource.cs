using Ares.AudioSource;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using Ares.ModelInfo;
using Ares.Data;
using System.Net;
using RestSharp.Deserializers;

namespace Ares.AudioSource.Freesound
{
    public class FreesoundAudioSource : IAudioSource
    {
        public const string AUDIO_SOURCE_ID = "freesound";

        private IRestClient m_Client = new RestClient(Settings.Default.FreesoundApiBase);

        #region IAudioSource interface implementation

        public Bitmap Icon { get { return ImageResources.FreesoundAudioSourceIcon.ToBitmap(); } }
        public string Id { get { return AUDIO_SOURCE_ID; } }
        public string Name { get { return StringResources.FreesoundAudioSourceName; } }
                
        public bool IsAudioTypeSupported(AudioSearchResultType type)
        {
            return type == AudioSearchResultType.Unknown || type == AudioSearchResultType.SoundFile;
        }

        public int RetrieveNumberOfSearchResults(string query, AudioSearchResultType type, IProgressMonitor monitor, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public ICollection<ISearchResult> GetSearchResults(string query, AudioSearchResultType type, int pageSize, int pageIndex, IProgressMonitor monitor, CancellationToken token, out int? totalNumberOfResults)
        {
            ICollection<ISearchResult> results = new List<ISearchResult>();
            
            // Perform the search
            results = new FreesoundApiSearch(this, this.m_Client, monitor, token)
                        .GetSearchResults(query, pageSize, pageIndex, out totalNumberOfResults);

            return results;
        }

        internal AudioDownloadResult DownloadSoundFile(FreesoundApiSearchResult searchResult, string downloadTargetPath)
        {
            // Make sure the target directory exists
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(downloadTargetPath));
            
            // Only download if the target file doesn't exist
            if (!System.IO.File.Exists(downloadTargetPath))
            {
                // Create a WebClient
                WebClient client = new WebClient();
                // Download the file (synchronously)
                client.DownloadFile(searchResult.PreviewUrls["preview-hq-mp3"], downloadTargetPath);

                // TODO: set additional ID3 information regarding source, author, license, tags etc. on the file
            }

            return AudioDownloadResult.SUCCESS;
        }

        #endregion
    }
}
