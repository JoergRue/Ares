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

namespace Ares.AudioSource.Freesound
{
    public class FreesoundAudioSource : IAudioSource
    {
        public const string AUDIO_SOURCE_ID = "freesound";

        public Bitmap Icon
        {
            get
            {
                return ImageResources.FreesoundAudioSourceIcon.ToBitmap();
            }
        }

        public string Id
        {
            get
            {
                return AUDIO_SOURCE_ID;
            }
        }

        public string Name
        {
            get
            {
                return StringResources.FreesoundAudioSourceName;
            }
        }
                
        public bool IsAudioTypeSupported(AudioSearchResultType type)
        {
            return type == AudioSearchResultType.SoundFile;
        }

        public ICollection<SearchResult> Search(string query, AudioSearchResultType type, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            // TODO: perform search using the API
            List<SearchResult> results = new List<SearchResult>();

            results.Add(new SearchResult(this, "test", "Test Sound", AudioSearchResultType.SoundFile));

            return results;
        }

        public void DownloadSearchResult(SearchResult searchResult, IProgressMonitor monitor, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    public class FreesoundApi
    {
        public static String BaseUrl { get { return Settings.Default.FreesoundApiBase; } }
        public static String TextSearchPath { get { return Settings.Default.TextSearchPath; } }
        public static String ApiKey { get { return Freesound.ApiKey.Key; } } 
    }

    public class FreesoundApiSearch
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationToken m_Token;
        
        public FreesoundApiSearch(Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            this.m_Monitor = monitor;
            this.m_Token = token;
        }

        public void DoSearch(string searchQuery)
        {
            m_Monitor.SetIndeterminate(StringResources.ExecutingSearch);

            //var proxy = System.Net.WebRequest.GetSystemWebProxy(); // Unused!
            var request = new RestSharp.RestRequest(FreesoundApi.TextSearchPath, RestSharp.Method.GET);

            request.AddParameter("page", 1);
            request.AddParameter("pageSize", 10);
            request.AddParameter("fields", "id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings");

            request.AddParameter("query", searchQuery);
            request.AddParameter("token", FreesoundApi.ApiKey);

            /*FreesoundApiSearchResult response = client.Execute<FreesoundApiSearchResult>(request);

            m_Token.ThrowIfCancellationRequested();
                if (response.ErrorException != null)
                {
                    throw new GlobalDbException(response.ErrorException);
                }
                if (response.Data == null)
                {
                    throw new GlobalDbException(String.IsNullOrEmpty(response.ErrorMessage) ? "No data received" : response.ErrorMessage);
                }
                if (response.Data.Status != 0)
                {
                    throw new GlobalDbException(response.Data.ErrorMessage);
                }
                if (i > 0)
                {
                    logBuilder.AppendLine("---------------------------------------------------------");
                }
                logBuilder.Append(response.Data.Log);
            }
            return logBuilder.ToString();
            */
        }

    }

    public class FreesoundApiSearchResult: SearchResult
    {
        private long m_FreesoundId;

        FreesoundApiSearchResult(FreesoundAudioSource source, long freesoundId, string title): base(source, freesoundId.ToString(),title,AudioSearchResultType.SoundFile)
        {
            this.m_FreesoundId = freesoundId;
        }
    }
}
