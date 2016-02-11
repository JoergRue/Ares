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
using System.Data.Common;
using System.Text;

namespace Ares.Tags
{
    class TagsDBBrowsing: ITagsDBBrowse, IConnectionClient
    {
        private DbConnection m_Connection;

        internal TagsDBBrowsing(DbConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(DbConnection connection)
        {
            m_Connection = connection;
        }

        public IList<Artist> GetAllArtists()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAllArtists();
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

        public IList<Album> GetAllAlbums()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAllAlbums();
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

        public IList<Album> GetAlbumsByArtist(string artist)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAlbumsByArtist(artist);
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

        public IList<FileIdentification> GetAllFiles()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetAllFiles();
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

        public IList<FileIdentification> GetFilesByAlbum(String artist, String album)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetFilesByAlbum(album, artist);
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

        public Statistics GetStatistics()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetStatistics();
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

        private IList<Artist> DoGetAllArtists()
        {
            List<Artist> result = new List<Artist>();
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0} FROM {1} ORDER BY {0}", Schema.ARTIST_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Artist() { Name = reader.GetString(0) });
                    }
                }
            }
            return result;
        }

        private IList<Album> DoGetAlbumsByArtist(String artist)
        {
            if (String.IsNullOrEmpty(artist))
                return DoGetAllAlbums();
            List<Album> result = new List<Album>();
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1} FROM {2} WHERE {0}=@Artist ORDER BY {1}", Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                command.AddParameterWithValue("@Artist", artist);
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Album() { Artist = reader.GetString(0), Name = reader.GetString(1) });
                    }
                }
            }
            return result;
        }

        private IList<Album> DoGetAllAlbums()
        {
            List<Album> result = new List<Album>();
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1} FROM {2} ORDER BY {0}, {1}", Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new Album() { Artist = reader.GetString(0), Name = reader.GetString(1) });
                    }
                }
            }
            return result;
        }

        private IList<FileIdentification> DoGetAllFiles()
        {
            List<FileIdentification> result = new List<FileIdentification>();
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1}, {2}, {3} FROM {4} ORDER BY {1}, {2}, {3}",
                Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                using (DbDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new FileIdentification() 
                         { 
                             Id = (int)reader.GetInt64(0), 
                             Artist = reader.GetStringOrEmpty(1), 
                             Album = reader.GetStringOrEmpty(2), 
                             Title = reader.GetStringOrEmpty(3) 
                         });
                    }
                }
            }
            return result;
        }

        private IList<FileIdentification> DoGetFilesByAlbum(String album, String artist)
        {
            if (String.IsNullOrEmpty(album) && String.IsNullOrEmpty(artist))
                return DoGetAllFiles();
            List<FileIdentification> result = new List<FileIdentification>();
            if (String.IsNullOrEmpty(album))
            {
                using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1}, {2}, {3} FROM {4} WHERE {1}=@Artist ORDER BY {1}, {2}, {3}",
                    Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.FILES_TABLE), m_Connection))
                {
                    command.AddParameterWithValue("@Artist", artist);
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new FileIdentification() { Id = (int)reader.GetInt64(0), Artist = reader.GetString(1), Album = reader.GetString(2), Title = reader.GetString(3) });
                        }
                    }
                }
            }
            else if (String.IsNullOrEmpty(artist))
            {
                using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1}, {2}, {3} WHERE {2}=@Album FROM {4} ORDER BY {1}, {2}, {3}",
                    Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.FILES_TABLE), m_Connection))
                {
                    command.AddParameterWithValue("@Album", album);
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new FileIdentification() { Id = (int)reader.GetInt64(0), Artist = reader.GetString(1), Album = reader.GetString(2), Title = reader.GetString(3) });
                        }
                    }
                }
            }
            else
            {
                using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT DISTINCT {0}, {1}, {2}, {3} FROM {4}  WHERE {1}=@Artist AND {2}=@Album ORDER BY {1}, {2}, {3}",
                    Schema.ID_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.FILES_TABLE), m_Connection))
                {
                    command.AddParameterWithValue("@Artist", artist);
                    command.AddParameterWithValue("@Album", album);
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new FileIdentification() { Id = (int)reader.GetInt64(0), Artist = reader.GetString(1), Album = reader.GetString(2), Title = reader.GetString(3) });
                        }
                    }
                }
            }
            return result;
        }

        public Statistics DoGetStatistics()
        {
            Statistics result = new Statistics();
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM {0}", Schema.FILES_TABLE), m_Connection))
            {
                Object res = command.ExecuteScalar();
                result.FilesCount = (Int32)(Int64)res;
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM {0}", Schema.CATEGORIES_TABLE), m_Connection))
            {
                result.CategoriesCount = (Int32)(Int64)command.ExecuteScalar();
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM {0}", Schema.TAGS_TABLE), m_Connection))
            {
                result.TagsCount = (Int32)(Int64)command.ExecuteScalar();
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM (SELECT DISTINCT {0} FROM {1})", Schema.USER_COLUMN, Schema.FILETAGS_TABLE), m_Connection))
            {
                result.UsersCount = (Int32)(Int64)command.ExecuteScalar();
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM (SELECT DISTINCT {0}, {1} FROM {2})", Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.FILETAGS_TABLE), m_Connection))
            {
                int count = (Int32)(Int64)command.ExecuteScalar();
                result.AvgTagsPerFile = result.TagsCount > 0 ? (double)count / (double)result.FilesCount : 0.0;
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM (SELECT DISTINCT {0}, {1} FROM {2})", Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                result.AlbumsCount = (Int32)(Int64)command.ExecuteScalar();
            }
            using (DbCommand command = DbUtils.CreateDbCommand(String.Format("SELECT COUNT(*) FROM (SELECT DISTINCT {0} FROM {1})", Schema.ARTIST_COLUMN, Schema.FILES_TABLE), m_Connection))
            {
                result.ArtistsCount = (Int32)(Int64)command.ExecuteScalar();
            }
            return result;
        }
    }
}
