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
using System.Linq;

using Ares.Data;

namespace Ares.Editor
{
    class DragAndDrop
    {
        public static IEnumerable<IElement> GetElementsFromDroppedItems(List<DraggedItem> draggedItems)
        {
            Dictionary<string, DraggedItem> uniqueItems = new Dictionary<string, DraggedItem>();
            foreach (DraggedItem item in draggedItems)
            {
                AddItemsToSet(uniqueItems, item);
            }
            foreach (DraggedItem item in uniqueItems.Values)
            {
                yield return CreateFileElement(item);
            }
        }

        private static IElement CreateFileElement(DraggedItem item)
        {
            IFileElement element = Ares.Data.DataModule.ElementFactory.CreateFileElement(item.RelativePath, item.ItemType == FileType.Music ? SoundFileType.Music : SoundFileType.SoundEffect);
            String dir = item.ItemType == FileType.Music ? Settings.Settings.Instance.MusicDirectory : Settings.Settings.Instance.SoundDirectory;
            String path = System.IO.Path.Combine(dir, item.RelativePath);
            Un4seen.Bass.AddOn.Tags.TAG_INFO tag = Un4seen.Bass.AddOn.Tags.BassTags.BASS_TAG_GetFromFile(path, true, true);
            if (tag != null)
            {
                element.Title = tag.title;
            }
            return element;
        }

        private static void AddItemsToSet(Dictionary<String, DraggedItem> uniqueItems, DraggedItem item)
        {
            String baseDir = item.ItemType == FileType.Music ? Ares.Settings.Settings.Instance.MusicDirectory : Ares.Settings.Settings.Instance.SoundDirectory;
            String path = baseDir + item.RelativePath;
            if (item.NodeType == DraggedItemType.Directory)
            {
                if (System.IO.Directory.Exists(path))
                {
                    foreach (String file in FileSearch.GetFilesInDirectory(path, true))
                    {
                        AddItemsToSet(uniqueItems, new DraggedItem { NodeType = DraggedItemType.File, ItemType = item.ItemType, RelativePath = file.Substring(baseDir.Length) });
                    }
                }
            }
            else
            {
                String key = System.IO.Path.GetFullPath(path);
                if (!uniqueItems.ContainsKey(path))
                {
                    uniqueItems[key] = item;
                }
            }
        }

    }
}
