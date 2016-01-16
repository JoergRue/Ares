/*
 Copyright (c) 2015 [Martin Ried]
 
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
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.AudioSource
{
    public enum AudioType
    {
        Music, Sound, ModeElement
    }

    public interface IAudioSource
    {
        /// <summary>
        /// The identifier of this AudioSource (the identifier is constant and can be used as a directory name)
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The display name of the AudioSource
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The display icon of the AudioSource
        /// </summary>
        Bitmap Icon { get; }

        /// <summary>
        /// Check whether an audio type is supported/available for this source
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool IsAudioTypeSupported(AudioType type);

        /// <summary>
        /// Search for audio (music, sounds, ...)
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type">optional parameter indicating the requested AudioType</param>
        /// <returns></returns>
        ICollection<AudioSourceSearchResult> Search(string query, AudioType type, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token);

        // TODO: Download search results

        /// <summary>
        /// Download the given search result to the target directory
        /// </summary>
        /// <param name="searchResult"></param>
        /// <param name="monitor"></param>
        /// <param name="token"></param>
        void DownloadSearchResult(AudioSourceSearchResult searchResult, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token);
    }

    public class AudioSourceSearchResult
    {

        private IAudioSource m_Source;
        private string m_Id = "";
        private string m_Title = "";
        private AudioType m_AudioType = AudioType.Music;

        public AudioSourceSearchResult(IAudioSource source, string id, string title, AudioType audioType)
        {
            this.m_Source = source;
            this.m_Id = id;
            this.m_Title = title;
            this.m_AudioType = audioType;
        }

        public string Id { get { return m_Id; } }
        public string Title { get { return m_Title; } }
        public IAudioSource AudioSource { get { return m_Source; } }
        public AudioType AudioType { get { return m_AudioType; } }

        /// <summary>
        /// Download this search result
        /// </summary>
        /// <param name="musicDownloadDirectoryForSourceAndItem"></param>
        /// <param name="soundsDownloadDirectoryForSourceAndItem"></param>
        /// <returns></returns>
        public AudioDownloadResult Download(string musicDownloadDirectory, string soundsDownloadDirectory)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// This class encapsulates information on the outcome of downloading an audio file.
    /// </summary>
    public class AudioDownloadResult
    {

    }

    [Serializable]
    public class AudioSourceException : Exception
    {
        public AudioSourceException(String message)
            : base(message)
        {
        }

        public AudioSourceException(Exception inner)
            : base(inner.Message, inner)
        {
        }

        private AudioSourceException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }
    }
}
