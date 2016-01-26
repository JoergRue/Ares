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

        private IRestClient m_Client = new RestClient("https://www.freesound.org/apiv2/");

        #region IAudioSource interface implementation

        public Bitmap Icon { get { return ImageResources.FreesoundAudioSourceIcon.ToBitmap(); } }
        public string Id { get { return AUDIO_SOURCE_ID; } }
        public string Name { get { return StringResources.FreesoundAudioSourceName; } }
                
        public bool IsAudioTypeSupported(AudioSearchResultType type)
        {
            return type == AudioSearchResultType.Unknown || type == AudioSearchResultType.SoundFile;
        }

        public ICollection<ISearchResult> Search(string query, AudioSearchResultType type, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            List<ISearchResult> results = new List<ISearchResult>();
            return new FreesoundApiSearch(this,this.m_Client, monitor, token).DoSearch(query);            
        }

        #endregion
    }

    public class FreesoundApi
    {
        public static String BaseUrl { get { return Settings.Default.FreesoundApiBase; } }
        public static String TextSearchPath { get { return Settings.Default.TextSearchPath; } }
        public static String ApiKey { get { return Freesound.ApiKey.Key; } } 
    }

    public class FreesoundApiSearch
    {
        private FreesoundAudioSource m_AudioSource;
        private IRestClient m_Client;
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationToken m_Token;
        
        public FreesoundApiSearch(FreesoundAudioSource audioSource, IRestClient client, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            this.m_Monitor = monitor;
            this.m_Token = token;
            this.m_Client = client;
            this.m_AudioSource = audioSource;
        }

        public List<ISearchResult> DoSearch(string searchQuery)
        {
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

            //var proxy = System.Net.WebRequest.GetSystemWebProxy(); // Unused!
            var request = new RestSharp.RestRequest(FreesoundApi.TextSearchPath, RestSharp.Method.GET);

            request.AddParameter("page", 1);
            request.AddParameter("pageSize", 10);
            request.AddParameter("fields", "id,url,name,tags,images,description,license,duration,username,previews,num_downloads,avg_rating,num_ratings");

            request.AddParameter("query", searchQuery);
            request.AddParameter("token", FreesoundApi.ApiKey);

            IRestResponse<Dao.RootObject> response = m_Client.Execute<Dao.RootObject>(request);

            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            } else
            {
                List<ISearchResult> searchResults = new List<ISearchResult>();
                foreach (Dao.Result result in response.Data.results) {
                    FreesoundApiSearchResult searchResult = new FreesoundApiSearchResult(m_AudioSource);
                    searchResult.Title = result.name;
                    searchResult.PreviewUrls = result.previews;
                    searchResult.License = result.license;
                    searchResult.Author = result.username;
                    searchResult.FreesoundId = result.id;
                    searchResult.AverageRating = result.avg_rating;
                    searchResult.NumberOfRatings = result.num_ratings;
                    searchResult.Duration = TimeSpan.FromSeconds(result.duration);

                    searchResults.Add(searchResult);
                }

                return searchResults;
            }
        }

    }

    namespace Dao
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
            public Dictionary<string,string> previews { get; set; }
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
            public object previous { get; set; }
        }


    }

    public class FreesoundApiSearchResult: IFileSearchResult
    {
        public long FreesoundId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string License {
            get
            {
                return m_License;
            }
            set
            {
                m_License = value;
                // TODO: parse License URLs provided by Freesound into human-readable License descriptions/names
            }
        }
        private string m_License;
        private FreesoundAudioSource m_FreesoundAudioSource;

        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
        public Dictionary<string, string> PreviewUrls { get; set; }



        public IAudioSource AudioSource { get { return this.m_FreesoundAudioSource; } }
        public double DownloadSize { get { return 1; } }
        public string Id { get { return FreesoundId.ToString(); } }
        public AudioSearchResultType ResultType { get { return AudioSearchResultType.SoundFile; } }

        public FreesoundApiSearchResult(FreesoundAudioSource freesoundAudioSource)
        {
            this.m_FreesoundAudioSource = freesoundAudioSource;
        }

        public AudioDownloadResult Download(string musicBaseDirectory, string soundsBaseDirectory, string relativeDownloadPath, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize)
        {
            double percentageForThisDownload = DownloadSize / totalSize;
            string downloadTargetPath = System.IO.Path.Combine(soundsBaseDirectory,GetRelativeDownloadFilePath(relativeDownloadPath));
            string downloadTargetDirectory = System.IO.Path.GetDirectoryName(downloadTargetPath);

            System.IO.Directory.CreateDirectory(downloadTargetDirectory);

            if (!System.IO.File.Exists(downloadTargetPath))
            {
                WebClient client = new WebClient();
                client.DownloadFile(PreviewUrls["preview-hq-mp3"], downloadTargetPath);
            }

            return new AudioDownloadResult();
        }

        public string GetRelativeDownloadFilePath(string relativeDownloadPath)
        {
            String filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Title);
            return System.IO.Path.Combine(relativeDownloadPath, filenameWithoutExtension + " (Freesound Sound "+this.FreesoundId+" by "+this.Author+").mp3");
        }
    }
}
