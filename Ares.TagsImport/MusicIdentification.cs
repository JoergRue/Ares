/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml;

using System.Threading;
using System.Threading.Tasks;

using Un4seen.Bass;

namespace Ares.TagsImport
{
    public class MusicIdentification
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationToken m_Token;

        public static Task<int> UpdateMusicIdentificationAsync(Ares.ModelInfo.IProgressMonitor monitor, IList<String> files, String musicDirectory,
            CancellationToken cancellationToken)
        {
            MusicIdentification identification = new MusicIdentification(monitor, cancellationToken);
            return identification.DoUpdateAsync(files, musicDirectory);
        }


        private MusicIdentification(Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token)
        {
            m_Monitor = monitor;
            m_Token = token;
        }

        private static bool NeedsRetrieval(Ares.Tags.FileIdentification item)
        {
            if (String.IsNullOrEmpty(item.Artist)) return true;
            if (String.IsNullOrEmpty(item.Album)) return true;
            if (String.IsNullOrEmpty(item.Title)) return true;
            if (String.IsNullOrEmpty(item.AcoustId)) return true;
            return false;
        }

        private List<Task> CreateTaskPackage(List<String> filesNeedRetrieval, String basePath, 
            MusicIdentificationRetriever retriever, 
            List<Ares.Tags.FileIdentification> retrievedInfo,
            Ares.ModelInfo.IProgressMonitor progressMon,
            int packageIndex, int packageSize)
        {
            List<Task> subTasks = new List<Task>();
            for (int i = 0; i < packageSize; ++i)
            {
                int fileIndex = packageIndex * packageSize + i;
                if (fileIndex >= filesNeedRetrieval.Count)
                    break;
                String path = System.IO.Path.Combine(basePath, filesNeedRetrieval[fileIndex]);
                var task2 = Task.Factory.StartNew(() =>
                {
                    retriever.RetrieveFileInfo(path, retrievedInfo[fileIndex], progressMon);
                }, m_Token);
                subTasks.Add(task2);
            }
            return subTasks;
        }

        private Task<int> DoUpdateAsync(IList<String> files, String musicDirectory)
        {
            m_Monitor.IncreaseProgress(0.0);
            int countWithId = 0;
            IList<Ares.Tags.FileIdentification> identification = Ares.Tags.TagsModule.GetTagsDB().ReadInterface.GetIdentificationForFiles(files);

            List<Ares.Tags.FileIdentification> retrievedInfo = new List<Tags.FileIdentification>();
            List<Ares.Tags.FileIdentification> needsRetrieval = new List<Tags.FileIdentification>();
            List<String> filesNeedRetrieval = new List<string>();
            for (int i = 0; i < files.Count; ++i)
            {
                if (files[i].EndsWith(".m3u", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (files[i].EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
                    continue;
                if (NeedsRetrieval(identification[i]))
                {
                    retrievedInfo.Add(identification[i]);
                    filesNeedRetrieval.Add(files[i]);
                }
                else
                {
                    ++countWithId;
                }
            }
            int nrOfRetrievals = filesNeedRetrieval.Count;
            if (nrOfRetrievals == 0)
                return Task.Factory.StartNew(() => { return countWithId;  });

            String basePath = musicDirectory;
            MusicIdentificationRetriever retriever = new MusicIdentificationRetriever(m_Token);
            
            List<Task> tasks = new List<Task>();
            m_Monitor.IncreaseProgress(5);
            SequentialProgressMonitor seqMon = new SequentialProgressMonitor(m_Monitor, 5.0, 95.0);
            ParallelProgressMonitor parallelMon = new ParallelProgressMonitor(seqMon, 100.0, filesNeedRetrieval.Count);
            // use packages of 30 files to reduce system load
            const int PACKAGE_SIZE = 30;
            int subTaskCount = (filesNeedRetrieval.Count + PACKAGE_SIZE - 1) / PACKAGE_SIZE;

            var task = Task.Factory.StartNew(() =>
                {
                    for (int t = 0; t < subTaskCount; ++t)
                    {
                        List<Task> subTasks = CreateTaskPackage(filesNeedRetrieval, basePath, retriever, retrievedInfo, parallelMon, t, PACKAGE_SIZE);
                        Task.WaitAll(subTasks.ToArray());
                        List<String> retrievedFiles = new List<String>();
                        List<Ares.Tags.FileIdentification> retrievedInfo2 = new List<Tags.FileIdentification>();
                        for (int i = 0; i < PACKAGE_SIZE; ++i)
                        {
                            int fileIndex = t * PACKAGE_SIZE + i;
                            if (fileIndex >= filesNeedRetrieval.Count)
                                break;
                            retrievedFiles.Add(filesNeedRetrieval[fileIndex]);
                            retrievedInfo2.Add(retrievedInfo[fileIndex]);
                            if (!String.IsNullOrEmpty(retrievedInfo[fileIndex].AcoustId))
                            {
                                ++countWithId;
                            }
                        }
                        Ares.Tags.TagsModule.GetTagsDB().WriteInterface.SetFileIdentifications(retrievedFiles, retrievedInfo2);
                        subTasks = null;
                        retrievedFiles = null;
                        retrievedInfo2 = null;
                        System.GC.Collect();
                    }
                    retriever.Dispose();
                    return countWithId;
                }, m_Token);

            return task;
        }

    }

    class MusicIdentificationRetriever : IDisposable
    {
        private const int ACOUST = 1;
        private const int ALBUM = 2;
        private const int ARTIST = 4;
        private const int TITLE = 8;

        private CancellationToken m_Token;
        private AcoustAPI m_AcoustAPI;

        public MusicIdentificationRetriever(CancellationToken token)
        {
            m_Token = token;
            m_AcoustAPI = new AcoustAPI();
        }

        public void Dispose()
        {
            m_AcoustAPI.Dispose();
        }

        public void RetrieveFileInfo(String fileName, Ares.Tags.FileIdentification data, Ares.ModelInfo.IProgressMonitor progressMonitor)
        {
            int query = 0;
            if (String.IsNullOrEmpty(data.AcoustId))
                query |= ACOUST;
            if (String.IsNullOrEmpty(data.Album))
                query |= ALBUM;
            if (String.IsNullOrEmpty(data.Artist))
                query |= ARTIST;
            if (String.IsNullOrEmpty(data.Title))
                query |= TITLE;
            RetrieveInfo(fileName, data, query, progressMonitor);
        }

        private void RetrieveInfo(String fileName, Ares.Tags.FileIdentification data, int query, Ares.ModelInfo.IProgressMonitor progressMonitor)
        {
            var id3Task = Task.Factory.StartNew(() =>
                {
                    if ((query & (ARTIST | ALBUM | TITLE)) != 0)
                    {
                        Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(fileName, true, true);
                        if (tag != null)
                        {
                            if (!String.IsNullOrEmpty(tag.artist) && ((query & ARTIST) != 0))
                            {
                                data.Artist = tag.artist;
                                query &= ~ARTIST;
                            }
                            if (!String.IsNullOrEmpty(tag.album) && ((query & ALBUM) != 0))
                            {
                                data.Album = tag.album;
                                query &= ~ALBUM;
                            }
                            if (!String.IsNullOrEmpty(tag.title) && ((query & TITLE) != 0))
                            {
                                data.Title = tag.title;
                                // API always returns a title
                                // try to get a better title through MusicBrainz if it must be queried anyway
                                if ((query & (ARTIST | ALBUM)) == 0)
                                {
                                    query &= ~TITLE;
                                }
                            }
                        }
                    }
                    progressMonitor.IncreaseProgress(15);
                    return query;
                }, m_Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);


            var acoustIdTask = id3Task.ContinueWith((task) =>
                {
                    SequentialProgressMonitor subMon = new SequentialProgressMonitor(progressMonitor, 15, 55);
                    return id3Task.Result != 0 ? QueryForAcoustId(fileName, subMon) : null;
                }, m_Token, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

            var lastTask = acoustIdTask.ContinueWith((task) =>
                {
                    if (acoustIdTask.Result != null)
                    {
                        int adaptedQuery = id3Task.Result;
                        data.AcoustId = acoustIdTask.Result.AcoustId;
                        if (((adaptedQuery & (ARTIST | ALBUM | TITLE)) != 0) && !String.IsNullOrEmpty(acoustIdTask.Result.MusicBrainzId))
                        {
                            QueryForMusicInfo(data, adaptedQuery, acoustIdTask.Result.MusicBrainzId);
                        }
                    }
                    progressMonitor.IncreaseProgress(30);
                }, m_Token, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);
                
        }

        private AcoustIdInfo QueryForAcoustId(String fileName, Ares.ModelInfo.IProgressMonitor progressMonitor)
        {
            var pcmTask = Task.Factory.StartNew(() =>
                {
                    double seconds;
                    Int16[] data = PcmExtractor.ExtractPcm(fileName, out seconds);
                    progressMonitor.IncreaseProgress(25);
                    return new PcmInfo { Data = data, Seconds = seconds };
                }, m_Token, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);

            var chromaTask = pcmTask.ContinueWith((task) =>
                {
                    var result = (pcmTask.Result != null && pcmTask.Result.Data != null) ? new ChromaPrintInfo { Code = ChromaPrint.CalculateChromaprintCode(pcmTask.Result.Data), Seconds = pcmTask.Result.Seconds } : null;
                    progressMonitor.IncreaseProgress(15);
                    return result;
                }, m_Token, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);

            var acoustTask = chromaTask.ContinueWith((task) =>
                {
                    var result = (chromaTask.Result != null && !String.IsNullOrEmpty(chromaTask.Result.Code)) ? m_AcoustAPI.IdentifySong(chromaTask.Result.Code, chromaTask.Result.Seconds) : null;
                    progressMonitor.IncreaseProgress(60);
                    return result;
                }, m_Token, TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);
            try
            {
                return acoustTask.Result;
            }
            catch (OperationCanceledException)
            {
                return new AcoustIdInfo() { AcoustId = String.Empty, MusicBrainzId = String.Empty };
            }
            catch (AggregateException)
            {
                return new AcoustIdInfo() { AcoustId = String.Empty, MusicBrainzId = String.Empty };
            }
        }

        private void QueryForMusicInfo(Ares.Tags.FileIdentification data, int query, String musicBrainzId)
        {
            String title, artist, album;
            MusicBrainzApi musicBrainzApi = new MusicBrainzApi();
            musicBrainzApi.RetrieveMusicInfos(musicBrainzId, out title, out album, out artist);
            if (!String.IsNullOrEmpty(title) && ((query & TITLE) != 0))
                data.Title = title;
            if (!String.IsNullOrEmpty(album) && ((query & ALBUM) != 0))
                data.Album = album;
            if (!String.IsNullOrEmpty(artist) && ((query & ARTIST) != 0))
                data.Artist = artist;
        }
    }

    class AcoustIdInfo
    {
        public String AcoustId { get; set; }
        public String MusicBrainzId { get; set; }
    }

    class PcmInfo
    {
        public Int16[] Data { get; set; }
        public double Seconds { get; set; }
    }

    class ChromaPrintInfo
    {
        public String Code { get; set; }
        public double Seconds { get; set; }
    }

    class ChromaPrintDll
    {
        public static readonly Int32 CHROMAPRINT_ALGORITHM_DEFAULT = 1;

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr chromaprint_new(Int32 algorithm);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void chromaprint_free(IntPtr context);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 chromaprint_start(IntPtr context, Int32 sampleRate, Int32 numChannels);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 chromaprint_feed(IntPtr context, System.Int16[] data, Int32 size);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 chromaprint_finish(IntPtr context);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 chromaprint_get_fingerprint(IntPtr context, IntPtr[] fingerPrint);

        [DllImport("chromaprint.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void chromaprint_dealloc(IntPtr ptr);
    }

    class ChromaPrint
    {
        public static String CalculateChromaprintCode(Int16[] pcm)
        {
            IntPtr context = ChromaPrintDll.chromaprint_new(ChromaPrintDll.CHROMAPRINT_ALGORITHM_DEFAULT);
            if (context == IntPtr.Zero)
                return String.Empty;
            Int32 result = ChromaPrintDll.chromaprint_start(context, 22050, 1);
            if (result == 0)
            {
                ChromaPrintDll.chromaprint_free(context);
                return String.Empty;
            }
            result = ChromaPrintDll.chromaprint_feed(context, pcm, pcm.Length);
            if (result == 0)
            {
                ChromaPrintDll.chromaprint_free(context);
                return String.Empty;
            }
            result = ChromaPrintDll.chromaprint_finish(context);
            if (result == 0)
            {
                ChromaPrintDll.chromaprint_free(context);
                return String.Empty;
            }
            IntPtr[] fingerPrint = new IntPtr[1];
            result = ChromaPrintDll.chromaprint_get_fingerprint(context, fingerPrint);
            if (result == 0 || fingerPrint[0] == IntPtr.Zero)
            {
                ChromaPrintDll.chromaprint_free(context);
                return String.Empty;
            }
            String res = Marshal.PtrToStringAnsi(fingerPrint[0]);
            ChromaPrintDll.chromaprint_dealloc(fingerPrint[0]);
            ChromaPrintDll.chromaprint_free(context);
            return res;
        }
    }

    class PcmExtractor
    {
        public static System.Int16[] ExtractPcm(String file, out double seconds)
        {
            seconds = 0;
            int handle = Bass.BASS_StreamCreateFile(file, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_MONO | BASSFlag.BASS_STREAM_PRESCAN);
            if (handle == 0)
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                // System.Console.WriteLine("ERROR: " + error);
                return null;
            }
            long length = Bass.BASS_ChannelGetLength(handle);
            seconds = Bass.BASS_ChannelBytes2Seconds(handle, length);
            int mixHandle = Un4seen.Bass.AddOn.Mix.BassMix.BASS_Mixer_StreamCreate(22050, 1, BASSFlag.BASS_STREAM_DECODE);
            if (mixHandle == 0)
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                // System.Console.WriteLine("ERROR: " + error);
                return null;
            }
            if (!Un4seen.Bass.AddOn.Mix.BassMix.BASS_Mixer_StreamAddChannel(mixHandle, handle, BASSFlag.BASS_DEFAULT))
            {
                BASSError error = Bass.BASS_ErrorGetCode();
                // System.Console.WriteLine("ERROR: " + error);
                return null;
            }
            List<System.Int16> data = new List<System.Int16>();
            while (true)
            {
                Int16[] buffer = new Int16[512];
                int num = Bass.BASS_ChannelGetData(mixHandle, buffer, buffer.Length * 2);
                if (num == -1)
                {
                    BASSError error = Bass.BASS_ErrorGetCode();
                    Bass.BASS_StreamFree(handle);
                    // System.Console.WriteLine("ERROR: " + error);
                    return null;
                }
                for (int i = 0; i < num / 2; ++i)
                {
                    if (i < buffer.Length)
                        data.Add(buffer[i]);
                    else
                        throw new ApplicationException();
                }
                if (num < buffer.Length * 2)
                    break;
            }
            Bass.BASS_StreamFree(handle);
            try
            {
                return data.ToArray();
            }
            catch (System.OutOfMemoryException)
            {
                System.GC.Collect();
                return null;
            }
        }
    }

    namespace Acoust
    {
        class AcoustRecording
        {
            public String id { get; set; }
        }

        class AcoustResult
        {
            public double Score { get; set; }
            public String Id { get; set; }
            public List<AcoustRecording> Recordings { get; set; }
        }

        class AcoustResponse
        {
            public String Status { get; set; }
            public List<AcoustResult> Results { get; set; }
        }

    }

    class AcoustAPI : IDisposable
    {
        private const String baseUrl = "http://api.acoustid.org";
        private readonly String apiKey = "DBnkLPC9";

        private RateGate m_RateGate;

        public AcoustAPI()
        {
            m_RateGate = new RateGate(3, TimeSpan.FromSeconds(1));
        }

        public void Dispose()
        {
            m_RateGate.Dispose();
        }

        private T Execute<T>(RestSharp.RestRequest request) where T : new()
        {
            var client = new RestSharp.RestClient();
            client.BaseUrl = baseUrl;
            request.AddParameter("client", apiKey);
            m_RateGate.WaitToProceed();
            var response = client.Execute<T>(request);
            if (response.ErrorException != null)
            {
                throw response.ErrorException;
            }
            return response.Data;
        }

        public AcoustIdInfo IdentifySong(String chromaprintCode, double seconds)
        {
            var request = new RestSharp.RestRequest("/v2/lookup", RestSharp.Method.POST);
            request.AddParameter("duration", "" + (int)seconds);
            request.AddParameter("fingerprint", chromaprintCode);
            request.AddParameter("meta", "recordingids");

            var result = Execute<Acoust.AcoustResponse>(request);
            if (result != null && result.Results != null && result.Results.Count > 0)
            {
                String acoustId = result.Results[0].Id;
                String mbId = (result.Results[0].Recordings != null && result.Results[0].Recordings.Count > 0) ? result.Results[0].Recordings[0].id : String.Empty;
                return new AcoustIdInfo { AcoustId = acoustId, MusicBrainzId = mbId };
            }
            else
            {
                return new AcoustIdInfo { AcoustId = String.Empty, MusicBrainzId = String.Empty };
            }
        }
    }

    class MusicBrainzApi
    {
        private const String baseURL = "http://musicbrainz.org";

        public MusicBrainzApi()
        {
        }

        private String RetrieveMusicInfo(String musicBrainzId)
        {
            var client = new RestSharp.RestClient();
            client.BaseUrl = baseURL;
            var request = new RestSharp.RestRequest("/ws/2/recording/" + musicBrainzId + "?inc=artists+releases", RestSharp.Method.GET);
            var response = client.Execute(request);
            return (response != null && response.Content != null) ? response.Content : String.Empty;
        }

        public void RetrieveMusicInfos(String musicBrainzId, out String title, out String album, out String artist)
        {
            title = String.Empty;
            album = String.Empty;
            artist = String.Empty;

            String response = RetrieveMusicInfo(musicBrainzId);
            if (String.IsNullOrEmpty(response))
                return;

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(response);
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("mb", doc.DocumentElement.NamespaceURI);
            XmlNode titleNode = doc.SelectSingleNode(@"/mb:metadata/mb:recording/mb:title", nsmgr);
            if (titleNode != null)
            {
                title = titleNode.InnerText;
            }
            XmlNode artistNode = doc.SelectSingleNode(@"/mb:metadata/mb:recording/mb:artist-credit/mb:name-credit/mb:artist/mb:name", nsmgr);
            if (artistNode != null)
            {
                artist = artistNode.InnerText;
            }
            XmlNode albumNode = doc.SelectSingleNode(@"/mb:metadata/mb:recording/mb:release-list/mb:release/mb:title", nsmgr);
            if (albumNode != null)
            {
                album = albumNode.InnerText;
            }
        }
    }
}
