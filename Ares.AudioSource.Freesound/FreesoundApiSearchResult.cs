using Ares.ModelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Ares.Data;

namespace Ares.AudioSource.Freesound
{
    public class FreesoundApiSearchResult : UrlFileSearchResult
    {
        public FreesoundApiSearchResult(FreesoundAudioSource audioSource, string url) : base(audioSource,SoundFileType.SoundEffect,url)
        {

        }

        public long FreesoundId { get; set; }
        public Dictionary<string, string> PreviewUrls { get; set; }
        public new string Id { get { return FreesoundId.ToString(); } }

        public new FreesoundAudioSource AudioSource { get; internal set; }

        public new string License
        {
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

        

        /*public string Title { get; set; }
        public string Author { get; set; }

        

        private FreesoundAudioSource m_FreesoundAudioSource;

        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
        

        public List<string> Tags { get; set; }

        public IAudioSource AudioSource { get { return this.m_FreesoundAudioSource; } }
        public double DownloadSize { get { return 1; } }
        
        public AudioSearchResultType ResultType { get { return AudioSearchResultType.SoundFile; } }

        public SoundFileType Type
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public FreesoundApiSearchResult(FreesoundAudioSource freesoundAudioSource)
        {
            this.m_FreesoundAudioSource = freesoundAudioSource;
        }

        /// <summary>
        /// Deploy (download) this file
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="totalSize"></param>
        /// <param name="musicTargetDirectory"></param>
        /// <param name="soundsTargetDirectory"></param>
        /// <returns></returns>
        public AudioDownloadResult Deploy(IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize, string musicTargetDirectory, string soundsTargetDirectory)
        {

            DownloadTempHelper.FindTempFile()
            return this.Download(monitor, cancellationToken, totalSize, musicTargetDirectory, soundsTargetDirectory);
        }

        public string GetTargetFilename()
        {
            String filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Title);
            return filenameWithoutExtension + " (Freesound Sound " + this.FreesoundId + " by " + this.Author + ").mp3";
        }

        public IEnumerable<string> GetFilePathsToBeDownloaded()
        {
            return new string[] { System.IO.Path.Combine(m_FreesoundAudioSource.SoundsTargetDirectory, GetTargetFilename()) };
        }

        public string GetSourceUrl()
        {
            throw new NotImplementedException();
        }

        public AudioDownloadResult Download(IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize, string musicTargetDirectory, string soundsTargetDirectory)
        {
            double percentageForThisDownload = DownloadSize / totalSize;
            string downloadTargetPath = System.IO.Path.Combine(m_FreesoundAudioSource.SoundsTargetDirectory, GetTargetFilename());

            return this.m_FreesoundAudioSource.DownloadSoundFile(this, downloadTargetPath);
        }*/



    }
}
