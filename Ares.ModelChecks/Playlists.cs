/*
 Copyright (c) 2012 [Joerg Ruedenauer]
 
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
using Ares.Data;
using System.IO;

namespace Ares.ModelInfo
{
    public static class Playlists
    {
        private static void AddPath(List<String> result, String line, String playlistDir, Action<String> errorHandler)
        {
            if (Path.IsPathRooted(line))
            {
                result.Add(line);
            }
            else if (line.Contains("://"))
            {
                errorHandler(String.Format(StringResources.CannotPlayURLS, line));
            }
            else
            {
                result.Add(Path.Combine(playlistDir, line));
            }
        }

        private static List<String> ReadM3UList(StreamReader reader, String playlistDir, Action<String> errorHandler)
        {
            List<String> result = new List<string>();
            String line = reader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (line.Length != 0 && !line.StartsWith("#"))
                {
                    AddPath(result, line, playlistDir, errorHandler);
                }
                line = reader.ReadLine();
            }
            return result;
        }

        private static List<String> ReadPLSList(StreamReader reader, String playlistDir, Action<String> errorHandler)
        {
            List<String> result = new List<String>();
            String line = reader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                String[] elements = line.Split('=');
                if (elements.Length == 2 && elements[0].StartsWith("File"))
                {
                    int nr = 0;
                    if (Int32.TryParse(elements[0].Substring(4), out nr))
                    {
                        AddPath(result, elements[1], playlistDir, errorHandler);
                    }
                }
                line = reader.ReadLine();
            }
            return result;
        }

        private static void InsertElement(IElementContainer<IChoiceElement> choiceContainer, IFileElement newElement, IChoiceElement origElement)
        {
            IChoiceElement choiceElement = choiceContainer.AddElement(newElement);
            choiceElement.RandomChance = origElement.RandomChance;
        }

        private static void InsertElement(IElementContainer<ISequentialElement> sequentialContainer, IFileElement newElement, ISequentialElement origElement)
        {
            ISequentialElement seqElement = sequentialContainer.AddElement(newElement);
            seqElement.MaximumRandomStartDelay = origElement.MaximumRandomStartDelay;
            seqElement.FixedStartDelay = origElement.FixedStartDelay;
        }

        private static int AddPlaylistElements<T>(IElementContainer<T> container, IFileElement playListElement, T origElement, Action<String> errorHandler) where T : IContainerElement
        {
            String playlistPath = Path.Combine(Ares.Settings.Settings.Instance.MusicDirectory, playListElement.FilePath);
            if (!File.Exists(playlistPath))
            {
                errorHandler(String.Format(StringResources.PlaylistNotFound, playlistPath));
                return 0;
            }
            int count = 0;
            try
            {
                String dir = Path.GetDirectoryName(playlistPath);
                using (StreamReader reader = new StreamReader(playlistPath, true))
                {
                    List<String> filePaths = null;
                    if (playlistPath.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase)
                        || playlistPath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filePaths = ReadM3UList(reader, dir, errorHandler);
                    }
                    else if (playlistPath.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase))
                    {
                        filePaths = ReadPLSList(reader, dir, errorHandler);
                    }
                    else
                    {
                        errorHandler("Unknown playlist type: " + playlistPath);
                        return 0;
                    }
                    foreach (String filePath in filePaths)
                    {
                        IFileElement newElement = Ares.Data.DataModule.ElementFactory.CreateFileElement(filePath, SoundFileType.Music);
                        newElement.SetsMusicVolume = playListElement.SetsMusicVolume;
                        newElement.SetsSoundVolume = playListElement.SetsSoundVolume;
                        newElement.MusicVolume = playListElement.MusicVolume;
                        newElement.SoundVolume = playListElement.SoundVolume;
                        if (origElement is IChoiceElement)
                        {
                            InsertElement((IElementContainer<IChoiceElement>)container, newElement, (IChoiceElement)origElement);
                            ++count;
                        }
                        else if (origElement is ISequentialElement)
                        {
                            InsertElement((IElementContainer<ISequentialElement>)container, newElement, (ISequentialElement)origElement);
                            ++count;
                        }
                        else
                        {
                            errorHandler("Internal error: Unknown music list type.");
                            return 0;
                        }
                    }
                }
            }
            catch (IOException e)
            {
                errorHandler(String.Format(StringResources.ErrorReadingPlaylist, playlistPath, e.Message));
            }
            return count;
        }

        private static void ExpandPlaylists<T>(IElementContainer<T> playlist, IList<T> elements, Action<String> errorHandler) where T : IContainerElement
        {
            int count = 0;
            foreach (T element in elements)
            {
                IElement inner = element.InnerElement;
                if (inner is IFileElement)
                {
                    IFileElement fileElement = (IFileElement)inner;
                    if (fileElement.FilePath.EndsWith(".m3u", StringComparison.InvariantCultureIgnoreCase)
                        || fileElement.FilePath.EndsWith(".m3u8", StringComparison.InvariantCultureIgnoreCase)
                        || fileElement.FilePath.EndsWith(".pls", StringComparison.InvariantCultureIgnoreCase))
                    {
                        count += AddPlaylistElements(playlist, fileElement, element, errorHandler);
                    }
                    else
                    {
                        playlist.InsertGeneralElement(count, element);
                        ++count;
                    }
                }
                else
                {
                    playlist.InsertGeneralElement(count, element);
                    ++count;
                }
            }
        }

        public static ISequentialBackgroundMusicList ExpandSequentialMusicList(ISequentialBackgroundMusicList original, Action<String> errorHandler)
        {
            ISequentialBackgroundMusicList result = Ares.Data.DataModule.ElementFactory.CreateSequentialBackgroundMusicList(original.Title);
            result.FixedIntermediateDelay = original.FixedIntermediateDelay;
            result.MaximumRandomIntermediateDelay = original.MaximumRandomIntermediateDelay;
            result.MusicVolume = original.MusicVolume;
            result.RepeatCount = original.RepeatCount;
            result.SetsMusicVolume = original.SetsMusicVolume;
            result.SetsSoundVolume = original.SetsSoundVolume;
            result.SoundVolume = original.SoundVolume;
            ExpandPlaylists(result, original.GetElements(), errorHandler);
            return result;
        }

        public static IRandomBackgroundMusicList ExpandRandomMusicList(IRandomBackgroundMusicList original, Action<String> errorHandler)
        {
            IRandomBackgroundMusicList result = Ares.Data.DataModule.ElementFactory.CreateRandomBackgroundMusicList(original.Title);
            result.FixedStartDelay = original.FixedStartDelay;
            result.MaximumRandomStartDelay = original.MaximumRandomStartDelay;
            result.FixedIntermediateDelay = original.FixedIntermediateDelay;
            result.MaximumRandomIntermediateDelay = original.MaximumRandomIntermediateDelay;
            result.MusicVolume = original.MusicVolume;
            result.RepeatCount = original.RepeatCount;
            result.SetsMusicVolume = original.SetsMusicVolume;
            result.SetsSoundVolume = original.SetsSoundVolume;
            result.SoundVolume = original.SoundVolume;
            ExpandPlaylists(result, original.GetElements(), errorHandler);
            return result;
        }

    }
}
