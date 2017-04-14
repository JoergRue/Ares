/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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
using System.Collections.Generic;
using Ares.Data;
using System.Linq;
using System;

namespace Ares.ModelInfo
{
    public static class DataExtensions
    {
        public static bool IsEndless(this IModeElement element)
        {
            if (element.StartElement == null)
            {
                return false;
            }
            return IsEndless(element.StartElement);
        }

        public static bool AlwaysStartsMusic(this IModeElement element)
        {
            if (element.StartElement == null)
                return false;
            return AlwaysStartsMusic(element.StartElement);
        }

        private static bool AlwaysStartsMusic(IElement element)
        {
            if (element is IMusicList)
            {
                return true;
            }
            else if (element is IContainerElement)
            {
                IElement inner = (element as IContainerElement).InnerElement;
                if (inner != element && AlwaysStartsMusic(inner))
                    return true;
            }
            else if (element is IElementContainer<IParallelElement>)
            {
                IList<IParallelElement> parElements = (element as IElementContainer<IParallelElement>).GetElements();
                foreach (IParallelElement parallel in parElements)
                {
                    if (AlwaysStartsMusic(parallel))
                        return true;
                }
            }
            else if (element is IElementContainer<ISequentialElement>)
            {
                IList<ISequentialElement> seqElements = (element as IElementContainer<ISequentialElement>).GetElements();
                if (seqElements.Count > 0 && AlwaysStartsMusic(seqElements[0]))
                    return true;
            }
            else if (element is IMusicByTags)
                return true;
            else if (element is IWebRadioElement)
                return true;
            return false;
        }

        private static bool IsEndless(IElement element)
        {
            return IsEndless(new HashSet<IElement>(), element);
        }

        private static bool IsEndless(HashSet<IElement> checkedReferences, IElement element)
        {
            if (element is IRepeatableElement)
            {
                if ((element as IRepeatableElement).RepeatCount == -1)
                    return true;
            }
            if (element is IContainerElement)
            {
                IElement inner = (element as IContainerElement).InnerElement;
                if (inner != element && IsEndless(checkedReferences, inner))
                    return true;
            }
            else if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement subElement in ((element as IGeneralElementContainer).GetGeneralElements()))
                {
                    if (IsEndless(checkedReferences, subElement))
                        return true;
                }
            }
            else if (element is Ares.Data.IReferenceElement)
            {
                if (checkedReferences.Contains(element))
                    return false;
                checkedReferences.Add(element);
                Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                if (referencedElement != null && IsEndless(checkedReferences, referencedElement))
                {
                    return true;
                }
            }
            else if (element is IMusicByTags)
                return true;
            else if (element is IWebRadioElement)
                return true;
            return false;
        }

#if !MONO
        private static IFileElement GetFileElement(this IElement element)
        {
            if (element == null) return null;
            switch (element)
            {
                case IFileElement fe: return fe;
                case IContainerElement ce: return ce.InnerElement.GetFileElement();
                case IReferenceElement re:
                    {
                        var referencedElement = Ares.Data.DataModule.ElementRepository.GetElement(re.ReferencedId);
                        return referencedElement.GetFileElement();
                    }
                default:
                    return null;
            }
        }

        public static MusicFileInfo GetMusicFileInfo(this IElement element)
        {
            MusicFileInfo result = new MusicFileInfo();
            var fileElement = element.GetFileElement();
            if (fileElement != null)
            {
                String path = fileElement.FilePath;
                try
                {
                    var dbInfo = Ares.Tags.TagsModule.GetTagsDB().ReadInterface.GetIdentificationForFiles(new[] { path }.ToList());
                    if (dbInfo != null && dbInfo.Count > 0)
                    {
                        result.Artist = dbInfo[0].Artist;
                        result.Album = dbInfo[0].Album;
                    }
                }
                catch (Ares.Tags.TagsDbException)
                { }
                if (String.IsNullOrEmpty(result.Artist) || String.IsNullOrEmpty(result.Album))
                {
                    String basePath = Settings.Settings.Instance.MusicDirectory;
                    String completePath = System.IO.Path.Combine(basePath, path);
                    Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(completePath, true, true);
                    if (tag != null)
                    {
                        if (string.IsNullOrEmpty(result.Album))
                            result.Album = tag.album;
                        if (string.IsNullOrEmpty(result.Artist))
                            result.Artist = tag.artist;
                    }
                }
            }
            return result;
        }
#endif
    }

    public class MusicFileInfo
    {
        public string Artist { get; set; }
        public string Album { get; set; }
    }
}
