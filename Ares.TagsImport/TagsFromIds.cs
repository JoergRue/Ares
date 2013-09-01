/*
 Copyright (c) 2013 [Joerg Ruedenauer]
 
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ares.Tags;

namespace Ares.TagsImport
{
    public class TagsFromIds
    {
        private Ares.ModelInfo.IProgressMonitor m_Monitor;
        private CancellationToken m_Token;

        public static Task<int> SetTagsFromIdsAsync(Ares.ModelInfo.IProgressMonitor progressMonitor, IList<FileIdentification> files, int languageId, bool interpret, bool album, CancellationToken token)
        {
            return Task.Factory.StartNew(() =>
                {
                    TagsFromIds processor = new TagsFromIds(progressMonitor, token);
                    return processor.DoSetTags(files, languageId, interpret, album);

                });
        }

        private TagsFromIds(Ares.ModelInfo.IProgressMonitor progressMonitor, CancellationToken token)
        {
            m_Monitor = progressMonitor;
            m_Token = token;
        }

        private int DoSetTags(IList<FileIdentification> files, int languageId, bool interpret, bool album)
        {
            Dictionary<String, int> tags = new Dictionary<string, int>();
            int interpretCatId = -1;
            int albumCatId = -1;

            List<String> interprets = new List<string>();
            List<String> albums = new List<string>();
            Dictionary<String, int> interpretTags = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
            Dictionary<String, int> albumTags = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);

            int lastPercent = 0;
            int count = 0;
            foreach (FileIdentification file in files)
            {
                if (!String.IsNullOrEmpty(file.Artist))
                {
                    interpretTags[file.Artist] = -1;
                }
                if (!String.IsNullOrEmpty(file.Album))
                {
                    albumTags[file.Album] = -1;
                }
                if (!String.IsNullOrEmpty(file.Artist) || !String.IsNullOrEmpty(file.Album))
                {
                    ++count;
                }
                interprets.Add(file.Artist != null ? file.Artist : String.Empty);
                albums.Add(file.Album != null ? file.Album : String.Empty);
            }
            m_Monitor.SetProgress(90, StringResources.AddingTags);
            m_Token.ThrowIfCancellationRequested();
            var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);
            var dbWrite = Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(languageId);
            var categories = dbRead.GetAllCategories();
            int tagCount = 0;
            if (interpret)
                tagCount += interpretTags.Count;
            if (album)
                tagCount += albumTags.Count;
            int currentCount = 0;
            if (interpret && interpretTags.Count > 0)
            {
                var interpretCat = categories.FirstOrDefault((Ares.Tags.CategoryForLanguage cat) => { return cat.Name.Equals(StringResources.Interpret, StringComparison.CurrentCultureIgnoreCase); });
                if (interpretCat != null)
                {
                    interpretCatId = interpretCat.Id;
                }
                else
                {
                    interpretCatId = dbWrite.AddCategory(StringResources.Interpret);
                }
                AddTags(interpretTags, interpretCatId, languageId, currentCount, tagCount, ref lastPercent);
                currentCount += interpretTags.Count;
            }
            if (album && albumTags.Count > 0)
            {
                var albumCat = categories.FirstOrDefault((Ares.Tags.CategoryForLanguage cat) => { return cat.Name.Equals(StringResources.Album, StringComparison.CurrentCultureIgnoreCase); });
                if (albumCat != null)
                {
                    albumCatId = albumCat.Id;
                }
                else
                {
                    albumCatId = dbWrite.AddCategory(StringResources.Album);
                }
                AddTags(albumTags, albumCatId, languageId, currentCount, tagCount, ref lastPercent);
                currentCount += albumTags.Count;
            }
            m_Monitor.SetProgress(99, StringResources.SettingFileTags);
            m_Token.ThrowIfCancellationRequested();

            List<IList<int>> newTags = new List<IList<int>>();
            List<int> fileIds = new List<int>();
            for (int i = 0; i < files.Count; ++i)
            {
                List<int> fileTags = new List<int>();
                if (interpret && !String.IsNullOrEmpty(interprets[i]))
                    fileTags.Add(interpretTags[interprets[i]]);
                if (album && !String.IsNullOrEmpty(albums[i]))
                    fileTags.Add(albumTags[albums[i]]);
                newTags.Add(fileTags);
                fileIds.Add(files[i].Id);
            }
            var dbWrite2 = Ares.Tags.TagsModule.GetTagsDB().WriteInterface;
            dbWrite2.AddFileTags(fileIds, newTags);
            return count;
        }

        private void AddTags(Dictionary<String, int> tags, int categoryId, int languageId, int currentCount, int totalCount, ref int lastPercent)
        {
            var dbRead = Ares.Tags.TagsModule.GetTagsDB().GetReadInterfaceByLanguage(languageId);
            var dbWrite = Ares.Tags.TagsModule.GetTagsDB().GetWriteInterfaceByLanguage(languageId);
            var existingTags = dbRead.GetAllTags(categoryId);
            List<String> keys = new List<string>(tags.Keys);
            foreach (String key in keys)
            {
                var existingTag = existingTags.FirstOrDefault((Ares.Tags.TagForLanguage tag) => { return tag.Name.Equals(key, StringComparison.CurrentCultureIgnoreCase); });
                if (existingTag != null)
                {
                    tags[key] = existingTag.Id;
                }
                else
                {
                    tags[key] = dbWrite.AddTag(categoryId, key);
                }
                int percent = (++currentCount) * 9 / totalCount + 90;
                if (percent > lastPercent)
                {
                    m_Monitor.SetProgress(percent, StringResources.AddingTags);
                    lastPercent = percent;
                    m_Token.ThrowIfCancellationRequested();
                }
            }
        }
    }
}
