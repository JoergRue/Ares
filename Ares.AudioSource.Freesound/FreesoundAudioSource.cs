using Ares.AudioSource;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Ares.ModelInfo;
using Ares.Data;
using System.Net;
using RestSharp.Deserializers;
using System.Threading.Tasks;

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

        public Task<ICollection<ISearchResult<IAudioSource>>> Search(string query, int pageSize, int pageIndex, IAbsoluteProgressMonitor monitor, out int? totalNumberOfResults)
        {
            ICollection<ISearchResult<FreesoundAudioSource>> results = new List<ISearchResult<FreesoundAudioSource>>();

            // Perform the search
            results = new FreesoundApiSearch(this, this.m_Client, monitor)
                        .GetSearchResults(query, pageSize, pageIndex, out totalNumberOfResults);

            var completionSource = new TaskCompletionSource<ICollection<ISearchResult<IAudioSource>>>();
            completionSource.SetResult((ICollection<ISearchResult<IAudioSource>>)results);
            return completionSource.Task;
        }

        #endregion
    }
}
