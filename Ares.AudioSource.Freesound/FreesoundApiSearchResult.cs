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
        public FreesoundApiSearchResult(FreesoundAudioSource audioSource, string url, string filename) : base(audioSource,SoundFileType.SoundEffect,url,filename)
        {
        }

        public long FreesoundId { get; set; }
        public Dictionary<string, string> PreviewUrls { get; set; }

        public override string Id { get { return FreesoundId.ToString(); } }

        public override string License
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
