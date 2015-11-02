/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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

namespace Ares.Playing
{
    public enum StreamEncoding
    {
        Wma,
        Ogg,
        Lame,
        Wav,
        Opus
    }

    public enum StreamerType
    {
        Shoutcast,
        Icecast
    }

    public enum StreamingBitrate
    {
            kbps_32 = 32,
            kbps_48 = 48,
            kbps_64 = 64,
            kbps_96 = 96,
            kbps_128 = 128,
            kbps_144 = 144,
            kbps_192 = 192
    }

    public class StreamingParameters
    {
        public StreamEncoding Encoding { get; set; }
        public StreamerType Streamer { get; set; }
        public String Username { get; set; }
        public String Password { get; set; }
        public String ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public String StreamName { get; set; }
        public StreamingBitrate Bitrate { get; set; }
    }

    public interface IStreamer
    {
        void BeginStreaming(StreamingParameters parameters);
        bool IsStreaming { get; }
        void EndStreaming();
    }
}