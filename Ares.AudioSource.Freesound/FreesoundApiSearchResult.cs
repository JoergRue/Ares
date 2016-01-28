using Ares.ModelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Ares.AudioSource.Freesound
{
    public class FreesoundApiSearchResult : IFileSearchResult
    {
        public long FreesoundId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string License
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
        private FreesoundAudioSource m_FreesoundAudioSource;

        public TimeSpan Duration { get; set; }
        public string Description { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
        public Dictionary<string, string> PreviewUrls { get; set; }

        public List<string> Tags { get; set; }

        public IAudioSource AudioSource { get { return this.m_FreesoundAudioSource; } }
        public double DownloadSize { get { return 1; } }
        public string Id { get { return FreesoundId.ToString(); } }
        public AudioSearchResultType ResultType { get { return AudioSearchResultType.SoundFile; } }

        public FreesoundApiSearchResult(FreesoundAudioSource freesoundAudioSource)
        {
            this.m_FreesoundAudioSource = freesoundAudioSource;
        }

        public AudioDownloadResult Download(string musicTargetDirectory, string soundsTargetDirectory, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize)
        {
            double percentageForThisDownload = DownloadSize / totalSize;
            string downloadTargetPath = System.IO.Path.Combine(soundsTargetDirectory, GetDownloadFilename());
            
            return this.m_FreesoundAudioSource.DownloadSoundFile(this,downloadTargetPath);
        }
        
        public string GetDownloadFilename()
        {
            String filenameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(Title);
            return filenameWithoutExtension + " (Freesound Sound " + this.FreesoundId + " by " + this.Author + ").mp3";
        }
    }
}
