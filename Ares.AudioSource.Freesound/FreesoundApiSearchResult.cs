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
    /// <summary>
    /// ISearchResult implementation for a file coming from Freesound.org
    /// </summary>
    public class FreesoundApiSearchResult : UrlFileSearchResult
    {
        public FreesoundApiSearchResult(FreesoundAudioSource audioSource, string url) : base(audioSource,SoundFileType.SoundEffect,url)
        {
            this.AudioSource = audioSource;
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

    }
}
