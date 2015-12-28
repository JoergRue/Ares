using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.AudioSource.Freesound
{
    public class FreesoundAudioSource
    {
        

    }

    public class FreesoundApi
    {
        public static String BaseUrl { get { return Settings.Default.FreesoundApiBase; } }
        public static String TextSearchPath { get { return Settings.Default.TextSearchPath; } }
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
            request.AddParameter("query", searchQuery);
            request.AddParameter("token",   )
            var client = new RestSharp.RestClient();

            client.BaseUrl = FreesoundApi.BaseUrl;
            client.Timeout = 20 * 1000;

            var response = client.Execute<SearchResponse>(request);

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
        }
    }
}
