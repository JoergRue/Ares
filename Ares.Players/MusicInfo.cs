/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
using System.Text;
using Ares.Data;

namespace Ares.Players
{
    public class MusicInfo
    {
        public String LongTitle { get; private set; }
        public String ShortTitle { get; private set; }

        private MusicInfo(String longTitle, String shortTitle)
        {
            LongTitle = longTitle;
            ShortTitle = shortTitle;
        }

        public static MusicInfo GetInfo(int musicElementId)
        {
            if (musicElementId != -1)
            {
                IElement musicElement = Ares.Data.DataModule.ElementRepository.GetElement(musicElementId);
                IFileElement fileElement = musicElement as IFileElement;
                String shortTitle = musicElement.Title;
                String longTitle = musicElement.Title;
                if (fileElement != null)
                {
                    String path = Settings.Settings.Instance.MusicDirectory;
                    path = System.IO.Path.Combine(path, fileElement.FilePath);
                    Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(path, true, false);
                    if (tag != null)
                    {
                        StringBuilder musicInfoBuilder = new StringBuilder();
                        musicInfoBuilder.Append(tag.artist);
                        if (musicInfoBuilder.Length > 0)
                            musicInfoBuilder.Append(" - ");
                        musicInfoBuilder.Append(tag.album);
                        if (musicInfoBuilder.Length > 0)
                            musicInfoBuilder.Append(" - ");
                        musicInfoBuilder.Append(tag.title);
                        longTitle = musicInfoBuilder.ToString();
                    }
                }
                return new MusicInfo(longTitle, shortTitle);
            }
            else
            {
                return new MusicInfo(String.Empty, String.Empty);
            }
        }
    }
}
