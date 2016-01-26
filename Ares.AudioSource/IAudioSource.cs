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
        /// <param name="type">optional parameter indicating the requested AudioType</param>
        /// <returns></returns>
        ICollection<ISearchResult> Search(string query, AudioSearchResultType type, Ares.ModelInfo.IProgressMonitor monitor, CancellationToken token);

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

        IAudioSource AudioSource { get; }

        AudioSearchResultType ResultType { get; }

        double DownloadSize { get; }

        /// <summary>
        /// Download this search result
        /// </summary>
        /// <param name="musicBaseDirectory"></param>
        /// <param name="soundsBaseDirectory"></param>
        /// <param name="relativeDownloadPath"></param>
        /// <param name="monitor"></param>
        /// <param name="totalSize"></param>
        /// <returns></returns>
        AudioDownloadResult Download(string musicBaseDirectory, string soundsBaseDirectory, string relativeDownloadPath, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize);
    }

    public interface IModeElementSearchResult: ISearchResult
    {
        IModeElement GetModeElementDefinition(string musicBaseDirectory, string soundsBaseDirectory, string relativeDownloadPath);
    }

    public interface IFileSearchResult: ISearchResult
    {
        string GetRelativeDownloadFilePath(string relativeDownloadPath);
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
