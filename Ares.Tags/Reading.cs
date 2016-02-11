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
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if ANDROID
using Mono.Data.Sqlite;
#else
using System.Data.SQLite;
#endif
using System.Data.Common;

namespace Ares.Tags
{
    class TagsDBReading : ITagsDBRead, IConnectionClient
    {
        private DbConnection m_Connection;

        internal TagsDBReading(DbConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(DbConnection connection)
        {
            m_Connection = connection;
        }

        public IList<string> GetAllFilesWithAnyTag(HashSet<int> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return new List<String>(DoGetAllFilesWithAnyTag(tagIds));
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<string> GetAllFilesWithAnyTag()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAllFilesWithAnyTag();
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<String> GetAllFilesWithAnyTagInEachCategory(IDictionary<int, HashSet<int>> tagsByCategory)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (tagsByCategory.Count == 0)
                {
                    return new List<string>();
                }
                int[] categories = tagsByCategory.Keys.ToArray();
                // get the files with tags in the first category
                HashSet<String> files = DoGetAllFilesWithAnyTag(tagsByCategory[categories[0]]);
                for (int i = 1; i < categories.Length; ++i)
                {
                    // no files left which match all categories
                    if (files.Count == 0)
                    {
                        return new List<String>();
                    }
                    // get all files with tags in the next category
                    HashSet<String> otherFiles = DoGetAllFilesWithAnyTag(tagsByCategory[categories[i]]);
                    // make the intersection of both sets: a file must have a tag of both categories
                    files.IntersectWith(otherFiles);
                }
                return new List<string>(files);
                
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<string> GetAllFilesWithAllTags(HashSet<int> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return new List<String>(DoGetAllFilesWithAllTags(tagIds));
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<FileIdentification> GetIdentificationForFiles(IList<String> filePaths)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetIdentificationForFiles(filePaths);
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<FileIdentification> GetIdentificationForFiles(IList<int> fileIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetIdentificationForFiles(fileIds);
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        public IList<FileIdentification> GetFilesForTag(long tagId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetFilesForTag(tagId);
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
            catch (DbException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        private HashSet<string> DoGetAllFilesWithAnyTag(HashSet<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return new HashSet<String>();
            }
            String queryString = String.Format("SELECT DISTINCT {0}.{1} FROM {0}, {2} WHERE {0}.{3}={2}.{4} AND {2}.{5} IN (",
                Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            int i = 0;
            foreach (int tagId in tagIds)
            {
                queryString += tagId;
                if (i != tagIds.Count - 1)
                    queryString += ",";
                ++i;
            }
            queryString += ")";
            using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    HashSet<String> result = new HashSet<string>();
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                    return result;
                }
            }
        }

        private List<string> DoGetAllFilesWithAnyTag()
        {
            String queryString = String.Format("SELECT DISTINCT {0}.{1} FROM {0}, {2} WHERE {0}.{3}={2}.{4}",
                Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN);
            using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    List<String> result = new List<string>();
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                    return result;
                }
            }
        }

        private HashSet<String> DoGetAllFilesWithAllTags(HashSet<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return new HashSet<String>();
            }
            // inner query "CountedTags":
            // - select all records which match any of the given tags
            // - group them by file ID and count the number of records by file ID
            // - select (having) those where the number of records is the number of the given tags
            // --> those are the files which match all tags
            String queryString = String.Format("SELECT DISTINCT {0}.{1} FROM {0}, " +
                "(SELECT {2} AS File, COUNT({2}) AS NrOfTags FROM {4} WHERE {3} IN (", 
                Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.FILETAGS_TABLE);
            int i = 0;
            foreach (int tagId in tagIds)
            {
                queryString += tagId;
                if (i != tagIds.Count - 1)
                    queryString += ",";
                ++i;
            }
            queryString += String.Format(") GROUP BY {0} HAVING COUNT({0})={1}) AS CountedTags " +
                "WHERE {2}.{3}=CountedTags.File",
                Schema.FILE_COLUMN, tagIds.Count, Schema.FILES_TABLE, Schema.ID_COLUMN);
            using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    HashSet<String> result = new HashSet<string>();
                    while (reader.Read())
                    {
                        result.Add(reader.GetString(0));
                    }
                    return result;
                }
            }
        }

        private IList<FileIdentification> DoGetIdentificationForFiles(IList<String> filePaths)
        {
            List<FileIdentification> result = new List<FileIdentification>();
            if (filePaths == null || filePaths.Count == 0)
                return result;
            String queryString = String.Format("SELECT {0}, {1}, {2}, {3}, {4} FROM {5} WHERE {6}=@Path",
                Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.ACOUST_ID_COLUMN, Schema.FILES_TABLE, Schema.PATH_COLUMN);
            using (DbCommand queryCommand = DbUtils.CreateDbCommand(queryString, m_Connection))
            {
                DbParameter param = queryCommand.AddParameter("@Path", System.Data.DbType.String);
                foreach (String filePath in filePaths)
                {
                    param.Value = filePath;
                    using (DbDataReader reader = queryCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Add(new FileIdentification()
                            {
                                Id = (int)reader.GetInt64(0),
                                Artist = reader.GetStringOrEmpty(1),
                                Album = reader.GetStringOrEmpty(2),
                                Title = reader.GetStringOrEmpty(3),
                                AcoustId = reader.GetStringOrEmpty(4)
                            });
                        }
                        else
                        {
                            result.Add(new FileIdentification()
                            {
                                Id = -1,
                                Artist = String.Empty,
                                Album = String.Empty,
                                Title = String.Empty,
                                AcoustId = String.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        private IList<FileIdentification> DoGetIdentificationForFiles(IList<int> fileIds)
        {
            List<FileIdentification> result = new List<FileIdentification>();
            if (fileIds == null || fileIds.Count == 0)
                return result;
            String queryString = String.Format("SELECT {0}, {1}, {2}, {3}, {4} FROM {5} WHERE {6}=@FileId",
                Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.ACOUST_ID_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN);
            using (DbCommand queryCommand = DbUtils.CreateDbCommand(queryString, m_Connection))
            {
                DbParameter param = queryCommand.AddParameter("@Fileid", System.Data.DbType.Int64);
                foreach (int fileId in fileIds)
                {
                    param.Value = fileId;
                    using (DbDataReader reader = queryCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result.Add(new FileIdentification()
                            {
                                Id = (int)reader.GetInt64(0),
                                Artist = reader.GetStringOrEmpty(1),
                                Album = reader.GetStringOrEmpty(2),
                                Title = reader.GetStringOrEmpty(3),
                                AcoustId = reader.GetStringOrEmpty(4)
                            });
                        }
                        else
                        {
                            result.Add(new FileIdentification()
                            {
                                Id = -1,
                                Artist = String.Empty,
                                Album = String.Empty,
                                Title = String.Empty,
                                AcoustId = String.Empty
                            });
                        }
                    }
                }
            }
            return result;
        }

        private IList<FileIdentification> DoGetFilesForTag(long tagId)
        {
            List<FileIdentification> result = new List<FileIdentification>();
            if (tagId == -1)
                return result;

            // first get the files which have the tag assigned
            String findFilesQuery = String.Format("SELECT DISTINCT {1}.{0} AS FileId FROM {1},{2} WHERE {1}.{0}={2}.{3} AND {2}.{4}=@TagId",
                Schema.ID_COLUMN, Schema.FILES_TABLE, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            // then also check that the tag is assigned more often than it is removed (this query is for the global DB)
            String countAddsQuery = String.Format("SELECT count(*) FROM {0} WHERE {0}.{1}=Inner.FileId AND {0}.{2}=@TagId2",
                Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String countRemovesQuery = String.Format("SELECT count(*) FROM {0} WHERE {0}.{1}=Inner.FileId AND {0}.{2}=@TagId3",
                Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            // finally get information for the file
            String getInfoQuery = String.Format("SELECT {5}.{0}, {5}.{1}, {5}.{2}, {5}.{3}, {5}.{4} FROM {5}, ({6}) AS Inner WHERE {5}.{0}=Inner.FileId AND ({7}) > ({8}) ORDER BY {5}.{1}, {5}.{2}, {5}.{3}",
                Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.ACOUST_ID_COLUMN, Schema.FILES_TABLE, findFilesQuery, countAddsQuery, countRemovesQuery);
            using (DbCommand getInfoCommand = DbUtils.CreateDbCommand(getInfoQuery, m_Connection))
            {
                getInfoCommand.AddParameterWithValue("@TagId", tagId);
                getInfoCommand.AddParameterWithValue("@TagId2", tagId);
                getInfoCommand.AddParameterWithValue("@TagId3", tagId);
                using (DbDataReader reader = getInfoCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new FileIdentification()
                        {
                            Id = (int)reader.GetInt64(0),
                            Artist = reader.GetStringOrEmpty(1),
                            Album = reader.GetStringOrEmpty(2),
                            Title = reader.GetStringOrEmpty(3),
                            AcoustId = reader.GetStringOrEmpty(4)
                        });
                    }
                }
            }
            return result;
        }
    }
}