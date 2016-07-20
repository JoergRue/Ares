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
using System.IO;
using System.Linq;
using System.Net;
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
        /// Search for audio (music, sounds, ...) through this IAudioSource
        /// </summary>
        /// <param name="type">The requested IAudioSearchResultType. May be either null indicate that any
        ///                    type of result will be fine.</param>
        /// <param name="query">The actual search query/keywords.</param>
        /// <param name="pageSize">The page size (number of results per page) to be retrieved.</param>
        /// <param name="pageIndex">The zero-based index of the results page to be retrieved (first page is 0).</param>
        /// <param name="monitor">The IAbsoluteProgressMonitor which the audio source should use to give feedback on the search progress.
        ///                       Initially the monitor will be set to "indeterminate" progress, but the audio source can and should
        ///                       use it to indicate actual progress where possible.</param>
        /// <param name="totalNumberOfResults">An optional output parameter to indicate the total number of results (if known), regardless of the selected page size & index.</param>
        /// <returns></returns>
        Task<IEnumerable<ISearchResult>> Search(
            string query, 
            int pageSize, 
            int pageIndex, 
            IAbsoluteProgressMonitor monitor, 
            out int? totalNumberOfResults
        );
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
