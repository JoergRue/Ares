using Ares.ModelInfo;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Ares.AudioSource.Freesound
{
    public class FreesoundApiSearch
    {      
        private FreesoundAudioSource m_AudioSource;
        private IRestClient m_Client;
        private IAbsoluteProgressMonitor m_Monitor;

        public FreesoundApiSearch(FreesoundAudioSource audioSource, IRestClient client, IAbsoluteProgressMonitor monitor)
        {
            this.m_Monitor = monitor;
            this.m_Client = client;
            this.m_AudioSource = audioSource;
        }

        internal IEnumerable<ISearchResult> GetSearchResults(string query, int pageSize, int pageIndex, out int? totalNumberOfResults)
        {
            // Console.WriteLine("Searching Freesound for \"{0}\" (page index {1} sized {2}", query, pageIndex, pageSize);

            //var proxy = System.Net.WebRequest.GetSystemWebProxy(); // Unused!
            var request = new RestSharp.RestRequest(Settings.Default.TextSearchPath, RestSharp.Method.GET);

            request.AddParameter("page", pageIndex + 1);
            request.AddParameter("page_size", pageSize);
            request.AddParameter("fields", "id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings");

            request.AddParameter("query", query);
            request.AddParameter("token", Freesound.ApiKey.Key);

            request.RequestFormat = DataFormat.Json;

            return ExecuteSearch(request, out totalNumberOfResults);
        }

        internal IEnumerable<ISearchResult> GetSimilarSearchResults(string id, int pageSize, int pageIndex, out int? totalNumberOfResults)
        {
            // Console.WriteLine("Searching Freesound for \"{0}\" (page index {1} sized {2}", query, pageIndex, pageSize);

            //var proxy = System.Net.WebRequest.GetSystemWebProxy(); // Unused!
            var request = new RestSharp.RestRequest(Settings.Default.SimilarSearchPath, RestSharp.Method.GET);

            request.AddParameter("page", pageIndex + 1);
            request.AddParameter("page_size", pageSize);
            request.AddParameter("fields", "id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings");

            request.AddParameter("target", id);
            request.AddParameter("token", Freesound.ApiKey.Key);

            request.RequestFormat = DataFormat.Json;

            return ExecuteSearch(request, out totalNumberOfResults);
        }

        private IEnumerable<ISearchResult> ExecuteSearch(RestSharp.RestRequest request, out int? totalNumberOfResults)
        { 
            IRestResponse<SearchResultDtos.RootObject> response = m_Client.Execute<SearchResultDtos.RootObject>(request);

            m_Monitor.ThrowIfCancellationRequested();
            if (response.ErrorException != null)
            {
                // If an exception occurred during the request, throw it now
                throw response.ErrorException;
            }
            else if (response.ErrorMessage != null)
            {
                // If there is an error message without exception, throw a new exception with that message
                throw new Exception(response.ErrorMessage);
            }
            else if (response.Data.count > 0)
            {
                // Otherwise work with the results and return them
                ICollection<ISearchResult> searchResults = new List<ISearchResult>();
                foreach (SearchResultDtos.Result result in response.Data.results)
                {
                    String fileName = result.name;
                    fileName = fileName.Replace('"', '\'');
                    foreach (var c in System.IO.Path.GetInvalidPathChars())
                    {
                        fileName = fileName.Replace(c, '_');
                    }
                    foreach (var c in System.IO.Path.GetInvalidFileNameChars())
                    {
                        fileName = fileName.Replace(c, '_');
                    }
                    fileName = System.IO.Path.ChangeExtension(fileName, "mp3");
                    FreesoundApiSearchResult searchResult = new FreesoundApiSearchResult(m_AudioSource, result.previews["preview-hq-mp3"],fileName);
                    searchResult.Title = result.name;
                    searchResult.PreviewUrls = result.previews;
                    searchResult.License = result.license;
                    searchResult.Author = result.username;
                    searchResult.FreesoundId = result.id;
                    searchResult.AverageRating = result.avg_rating;
                    searchResult.NumberOfRatings = result.num_ratings;
                    searchResult.Duration = TimeSpan.FromSeconds(result.duration);
                    searchResult.Tags = result.tags;

                    searchResults.Add(searchResult);
                }

                totalNumberOfResults = response.Data.count;
                return searchResults;
            }
            else
            {
                totalNumberOfResults = 0;
                return new List<ISearchResult>();
            }
        }
    }

    // https://www.freesound.org/apiv2/search/text/?query=fanfare&page_size=10&fields=id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings&token=29459bf2d6e537cc4240d8b366207d439dd28272
    /*
    {
        "count": 130
        "next": "http://www.freesound.org/apiv2/search/text/?&query=fanfare&page=2&page_size=10&fields=id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings"
        "results": [10]
            0:  {
                "id": 49477
                "url": "https://www.freesound.org/people/neonaeon/sounds/49477/"
                "name": "fanfare1.flac"
                "tags": [12]
                    0:  "announcement"
                    1:  "environmental-sounds-research"
                    2:  "fan"
                    3:  "fanfair"
                    4:  "fanfare"
                    5:  "fare"
                    6:  "horn"
                    7:  "king"
                    8:  "royal"
                    9:  "trom"
                    10:  "trombone"
                    11:  "trumpet"
                    -
                "description": "This is just a short fanfare used to announce a king in a short play. The is a recording I made for a fund raiser i am taking part in. I recorded this trombone with a Zoom h4 and an AKG c3000 microphone at 48k/24bit. After editing i encoded them at 44.1k/16bit and then encoded to FLAC for the website."
                "license": "http://creativecommons.org/licenses/by/3.0/"
                "duration": 8.66253968254
                "username": "neonaeon"
                "previews": {
                    "preview-lq-ogg": "https://www.freesound.org/data/previews/49/49477_52325-lq.ogg"
                    "preview-lq-mp3": "https://www.freesound.org/data/previews/49/49477_52325-lq.mp3"
                    "preview-hq-ogg": "https://www.freesound.org/data/previews/49/49477_52325-hq.ogg"
                    "preview-hq-mp3": "https://www.freesound.org/data/previews/49/49477_52325-hq.mp3"
                }-
                "images": {
                    "waveform_l": "https://www.freesound.org/data/displays/49/49477_52325_wave_L.png"
                    "waveform_m": "https://www.freesound.org/data/displays/49/49477_52325_wave_M.png"
                    "spectral_m": "https://www.freesound.org/data/displays/49/49477_52325_spec_M.jpg"
                    "spectral_l": "https://www.freesound.org/data/displays/49/49477_52325_spec_L.jpg"
                }-
                "num_downloads": 16228
                "avg_rating": 4.18609865470852
                "num_ratings": 223
            }-
            1:  {
                "id": 49478
                "url": "https://www.freesound.org/people/neonaeon/sounds/49478/"
                "name": "fanfarejazz1.flac"
                "tags": [12]
                    0:  "announcement"
                    1:  "fan"
                    2:  "fanfair"
                    3:  "fanfare"
                    4:  "fare"
                    5:  "horn"
                    6:  "jazz"
                    7:  "king"
                    8:  "royal"
                    9:  "trom"
                    10:  "trombone"
                    11:  "trumpet"
                    -
                "description": "This is just a short fanfare used to announce a king in a short play. After the fanfare the horn player goes into a jazz riff and is stopped. Take 1 The is a recording I made for a fund raiser i am taking part in. I recorded this trombone with a Zoom h4 and an AKG c3000 microphone at 48k/24bit. After editing i encoded them at 44.1k/16bit and then encoded to FLAC for the website."
                "license": "http://creativecommons.org/licenses/by/3.0/"
                "duration": 13.6306349206
                "username": "neonaeon"
                "previews": {
                    "preview-lq-ogg": "https://www.freesound.org/data/previews/49/49478_52325-lq.ogg"
                    "preview-lq-mp3": "https://www.freesound.org/data/previews/49/49478_52325-lq.mp3"
                    "preview-hq-ogg": "https://www.freesound.org/data/previews/49/49478_52325-hq.ogg"
                    "preview-hq-mp3": "https://www.freesound.org/data/previews/49/49478_52325-hq.mp3"
                }-
                "images": {
                    "waveform_l": "https://www.freesound.org/data/displays/49/49478_52325_wave_L.png"
                    "waveform_m": "https://www.freesound.org/data/displays/49/49478_52325_wave_M.png"
                    "spectral_m": "https://www.freesound.org/data/displays/49/49478_52325_spec_M.jpg"
                    "spectral_l": "https://www.freesound.org/data/displays/49/49478_52325_spec_L.jpg"
                }-
                "num_downloads": 879
                "avg_rating": 3.921052631578945
                "num_ratings": 19
            }
    }
    */

    /// <summary>
    /// Data transfer objects used for the search results
    /// </summary>
    namespace SearchResultDtos
    {

        public class Images
        {
            public string waveform_l { get; set; }
            public string waveform_m { get; set; }
            public string spectral_m { get; set; }
            public string spectral_l { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public List<string> tags { get; set; }
            public string description { get; set; }
            public string license { get; set; }
            public double duration { get; set; }
            public string username { get; set; }
            public Dictionary<string, string> previews { get; set; }
            public Images images { get; set; }
            public int num_downloads { get; set; }
            public double avg_rating { get; set; }
            public int num_ratings { get; set; }
        }

        public class RootObject
        {
            public int count { get; set; }
            public string next { get; set; }
            public List<Result> results { get; set; }
            public string previous { get; set; }
        }


    }
}
