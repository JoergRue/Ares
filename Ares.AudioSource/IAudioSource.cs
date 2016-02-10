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
using Ares.Data;
using Ares.ModelInfo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ares.AudioSource
{

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
        bool IsAudioTypeSupported(AudioSearchResultType type);

        /// <summary>
        /// Search for audio (music, sounds, ...) 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="type"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="monitor"></param>
        /// <param name="token"></param>
        /// <param name="totalNumberOfResults">Optional out parameter that *may* return the total number of results</param>
        /// <returns></returns>
        ICollection<ISearchResult> GetSearchResults(string query, AudioSearchResultType type, int pageSize, int pageIndex, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token, out int? totalNumberOfResults);
    }

    public enum AudioSearchResultType
    {
        Unknown,
        MusicFile,
        SoundFile,
        ModeElement
    }

    public interface ISearchResult
    {
        string Id { get; }
        string Title { get; }
        string Author { get; }
        string License { get; }
        TimeSpan Duration { get; }
        string Description { get; }
        double AverageRating { get; }
        int NumberOfRatings { get; }
    
        List<String> Tags { get; }

        IAudioSource AudioSource { get; }

        AudioSearchResultType ResultType { get; }

        double DownloadSize { get; }

        /// <summary>
        /// Download this search result (including anything that is required, i.e. audio files required by an IModeElementSearchResult).
        /// All audio files will be placed at the given relative path beneath either the sounds or music directory - depending on their type.
        /// </summary>
        /// <param name="musicBaseDirectory"></param>
        /// <param name="soundsBaseDirectory"></param>
        /// <param name="relativeDownloadPath"></param>
        /// <param name="monitor"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        AudioDownloadResult Download(string musicTargetDirectory, string soundsTargetDirectory, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize);
    }

    public interface IModeElementSearchResult: ISearchResult
    {
        /// <summary>
        /// Returns the IModeElement definition of this search result.
        /// The assumption is, that all required audio files will be placed at the given relative path beneath
        /// the sounds/music directories when downloaded.
        /// </summary>
        /// <param name="relativeDownloadPath"></param>
        /// <returns></returns>
        IModeElement GetModeElementDefinition(string relativeDownloadPath);
    }

    public interface IFileSearchResult: ISearchResult
    {
        /// <summary>
        /// Returns the filename under which this download will be saved as determined by the source
        /// </summary>
        /// <returns></returns>
        string GetDownloadFilename();
    }

    /// <summary>
    /// This class encapsulates information on the outcome of downloading an audio file.
    /// </summary>
    public class AudioDownloadResult
    {
        private ResultState m_State;
        private string m_Message;
        private Exception m_Cause;

        enum ResultState
        {
            SUCCESS,
            ERROR
        }

        public static AudioDownloadResult SUCCESS = new AudioDownloadResult(ResultState.SUCCESS, null);
        public static AudioDownloadResult ERROR(string message)
        {
            return new AudioDownloadResult(ResultState.ERROR, message);
        }
        public static AudioDownloadResult ERROR(string message, Exception cause)
        {
            return new AudioDownloadResult(ResultState.ERROR, message, cause);
        }

        private AudioDownloadResult(ResultState state, string message)
        {
            this.m_State = state;
            this.m_Message = message;
            this.m_Cause = null;
        }

        private AudioDownloadResult(ResultState state, string message, Exception cause)
        {
            this.m_State = state;
            this.m_Message = message;
            this.m_Cause = cause;
        }

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
