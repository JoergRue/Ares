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
using System.Text;

using System.Data.Common;

namespace Ares.Tags
{
    class TagsDBWriting : ITagsDBWrite, IConnectionClient
    {
        private DbConnection m_Connection;

        internal TagsDBWriting(DbConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(DbConnection connection)
        {
            m_Connection = connection;
        }

        public void AddFileTags(IList<string> relativePaths, IList<IList<int>> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (relativePaths.Count == 0)
                {
                    return;
                }

                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    for (int i = 0; i < relativePaths.Count; ++i)
                    {
                        DoAddFileTags(transaction, relativePaths[i], tagIds[i]);
                    }

                    transaction.Commit();
                }
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

        public void AddFileTags(IList<int> fileIds, IList<IList<int>> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (fileIds.Count == 0)
                {
                    return;
                }

                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    for (int i = 0; i < fileIds.Count; ++i)
                    {
                        DoAddFileTags(transaction, fileIds[i], tagIds[i]);
                    }

                    transaction.Commit();
                }
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

        public void RemoveFileTags(IList<string> relativePaths, IList<IList<int>> tagIds)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (relativePaths.Count == 0)
                {
                    return;
                }

                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    for (int i = 0; i < relativePaths.Count; ++i)
                    {
                        DoRemoveFileTags(transaction, relativePaths[i], tagIds[i]);
                    }

                    transaction.Commit();
                }
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

        public void RemoveFiles(IList<String> files)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (files.Count == 0)
                {
                    return;
                }
                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    foreach (String path in files)
                    {
                        DoRemoveFile(transaction, path);
                    }
                    transaction.Commit();
                }
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

        public void MoveFiles(IDictionary<String, String> oldPathsToNewPaths)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (oldPathsToNewPaths.Count == 0)
                    return;
                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    foreach (var entry in oldPathsToNewPaths)
                    {
                        DoMoveFile(entry.Key, entry.Value, transaction);
                    }
                    transaction.Commit();
                }
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

        public void CopyFiles(IDictionary<String, String> oldPathsToNewPaths)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                if (oldPathsToNewPaths.Count == 0)
                    return;
                using (DbTransaction transaction = m_Connection.BeginTransaction())
                {
                    foreach (var entry in oldPathsToNewPaths)
                    {
                        DoCopyFile(entry.Key, entry.Value, transaction);
                    }
                    transaction.Commit();
                }
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

        public void RemoveCategory(int categoryId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoRemoveCategory(categoryId);
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

        public void RemoveTag(int tagId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoRemoveTag(tagId);
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

        public void SetFileIdentifications(IList<String> files, IList<FileIdentification> identifications)
        {
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (identifications == null)
            {
                throw new ArgumentNullException("identifications");
            }
            if (files.Count != identifications.Count)
            {
                throw new ArgumentException("file count must match identifications count");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoSetFileIdentifications(files, identifications);
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

        public void ConfirmTags(IList<String> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (files.Count == 0)
                return;
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoConfirmTags(files);
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

        public bool CanConfirmTags(IList<String> files)
        {
            if (files == null)
            {
                throw new ArgumentNullException("files");
            }
            if (files.Count == 0)
                return false;
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoCanConfirmTags(files);
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

        public void CleanupDB(System.IO.TextWriter logStream, String musicPath, int langId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoCleanupDB(logStream, musicPath, langId);
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

        public int AssignTagsByIdentification(IList<FileIdentification> identifications, IList<String> files)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoAssignTagsByIdentification(identifications, files);
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

        private void DoAddFileTags(DbTransaction transaction, String path, IList<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return;
            }
            long fileId = InsertOrFindFile(transaction, path);
            DoAddFileTags(transaction, fileId, tagIds);
        }

        private void DoAddFileTags(DbTransaction transaction, long fileId, IList<int> tagIds)
        {
            String queryExistingString = String.Format("SELECT {0}, {1} FROM {2} WHERE {3}=@File AND {4}=@Tag",
                Schema.ID_COLUMN, Schema.USER_COLUMN, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String removeRemovedString = String.Format("DELETE FROM {0} WHERE {1}=@File AND {2}=@Tag",
                Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String insertTagString = String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@Id, @File, @Tag, @User)",
                Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.USER_COLUMN);
            String updateUserString = String.Format("UPDATE {0} SET {1}=@User WHERE {2}=@File AND {3}=@Tag",
                Schema.FILETAGS_TABLE, Schema.USER_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            using (DbCommand queryExistingCommand = DbUtils.CreateDbCommand(queryExistingString, m_Connection, transaction),
                removeRemovedCommand = DbUtils.CreateDbCommand(removeRemovedString, m_Connection, transaction),
                insertTagCommand = DbUtils.CreateDbCommand(insertTagString, m_Connection, transaction),
                updateUserCommand = DbUtils.CreateDbCommand(updateUserString, m_Connection, transaction))
            {
                queryExistingCommand.AddParameterWithValue("@File", fileId);
                DbParameter queryParam = queryExistingCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                removeRemovedCommand.AddParameterWithValue("@File", fileId);
                DbParameter removeParam = removeRemovedCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                insertTagCommand.AddParameterWithValue("@Id", DBNull.Value);
                insertTagCommand.AddParameterWithValue("@File", fileId);
                insertTagCommand.AddParameterWithValue("@User", Schema.ARES_GUI_USER);
                DbParameter addParam = insertTagCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                updateUserCommand.AddParameterWithValue("@User", Schema.ARES_GUI_USER);
                updateUserCommand.AddParameterWithValue("@File", fileId);
                DbParameter updateParam = updateUserCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                foreach (int tagId in tagIds)
                {
                    queryParam.Value = (Int64)tagId;
                    using (DbDataReader reader = queryExistingCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // tag exists already
                            // if the old user was the global db, tag is now 'confirmed' --> change the user
                            if (reader.GetString(1) == Schema.GLOBAL_DB_USER)
                            {
                                updateParam.Value = (Int64)tagId;
                                updateUserCommand.ExecuteNonQuery();
                            }
                            // else do nothing
                        }
                        else
                        {
                            // insert into table
                            addParam.Value = (Int64)tagId;
                            int res = insertTagCommand.ExecuteNonQuery();
                            if (res != 1)
                            {
                                throw new TagsDbException("Insertion into FileTags table failed.");
                            }
                        }
                    }
                    // also remove the tag from the 'removed tags' table
                    removeParam.Value = (Int64)tagId;
                    removeRemovedCommand.ExecuteNonQuery();
                }
            }
        }

        private void DoRemoveFileTags(DbTransaction transaction, String path, IList<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return;
            }
            long fileId = InsertOrFindFile(transaction, path);
            String queryExistingString = String.Format("SELECT {0}, {1} FROM {2} WHERE {3}=@File AND {4}=@Tag",
                Schema.ID_COLUMN, Schema.USER_COLUMN, Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String removeAddedString = String.Format("DELETE FROM {0} WHERE {1}=@File AND {2}=@Tag",
                Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String removeTagString = String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@Id, @File, @Tag, @User)",
                Schema.REMOVEDTAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.USER_COLUMN);
            String updateUserString = String.Format("UPDATE {0} SET {1}=@User WHERE {2}=@File AND {3}=@Tag",
                Schema.REMOVEDTAGS_TABLE, Schema.USER_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            using (DbCommand queryExistingCommand = DbUtils.CreateDbCommand(queryExistingString, m_Connection, transaction),
                removeAddedCommand = DbUtils.CreateDbCommand(removeAddedString, m_Connection, transaction),
                removeTagCommand = DbUtils.CreateDbCommand(removeTagString, m_Connection, transaction),
                updateUserCommand = DbUtils.CreateDbCommand(updateUserString, m_Connection, transaction))
            {
                queryExistingCommand.AddParameterWithValue("@File", fileId);
                DbParameter queryParam = queryExistingCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                removeAddedCommand.AddParameterWithValue("@File", fileId);
                DbParameter removeParam = removeAddedCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                removeTagCommand.AddParameterWithValue("@Id", DBNull.Value);
                removeTagCommand.AddParameterWithValue("@File", fileId);
                removeTagCommand.AddParameterWithValue("@User", Schema.ARES_GUI_USER);
                DbParameter addParam = removeTagCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                updateUserCommand.AddParameterWithValue("@User", Schema.ARES_GUI_USER);
                updateUserCommand.AddParameterWithValue("@File", fileId);
                DbParameter updateParam = updateUserCommand.AddParameter("@Tag", System.Data.DbType.Int64);
                foreach (int tagId in tagIds)
                {
                    queryParam.Value = (Int64)tagId;
                    using (DbDataReader reader = queryExistingCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // tag was removed already
                            // if the old user was the global db, tag is now 'confirmed' to be removed --> change the user
                            if (reader.GetString(1) == Schema.GLOBAL_DB_USER)
                            {
                                updateParam.Value = (Int64)tagId;
                                updateUserCommand.ExecuteNonQuery();
                            }
                            // else do nothing
                        }
                        else
                        {
                            // insert into table
                            addParam.Value = (Int64)tagId;
                            int res = removeTagCommand.ExecuteNonQuery();
                            if (res != 1)
                            {
                                throw new TagsDbException("Insertion into RemovedTags table failed.");
                            }
                        }
                    }
                    // finally, remove the tag from the file tags table
                    removeParam.Value = (Int64)tagId;
                    removeAddedCommand.ExecuteNonQuery();
                }
            }
        }

        private void DoMoveFile(String oldPath, String newPath, DbTransaction transaction)
        {
            Object fileKey = DoFindFile(null, oldPath);
            if (fileKey != null)
            {
                String commandString = String.Format("UPDATE {0} SET {1}=@NewPath WHERE {2}=@Id", Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.ID_COLUMN);
                using (DbCommand command = DbUtils.CreateDbCommand(commandString, m_Connection, transaction))
                {
                    command.AddParameterWithValue("@NewPath", newPath);
                    command.AddParameterWithValue("@Id", (long)fileKey);
                    int res = command.ExecuteNonQuery();
                    if (res != 1)
                    {
                        throw new TagsDbException("Updating Files table failed.");
                    }
                }
            }
        }

        private void DoCopyFile(String oldPath, String newPath, DbTransaction transaction)
        {
            Object fileKey = DoFindFile(transaction, oldPath);
            if (fileKey != null)
            {
                String queryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@FileKey", Schema.TAG_COLUMN, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN);
                using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection, transaction))
                {
                    command.AddParameterWithValue("@FileKey", (long)fileKey);
                    List<int> tags = new List<int>();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        while (reader.Read())
                        {
                            tags.Add((int)reader.GetInt64(0));
                        }
                    }
                    DoAddFileTags(transaction, newPath, tags);
                }

                queryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@FileKey", Schema.TAG_COLUMN, Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN);
                using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection, transaction))
                {
                    command.AddParameterWithValue("@FileKey", (long)fileKey);
                    List<int> tags = new List<int>();
                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return;
                        while (reader.Read())
                        {
                            tags.Add((int)reader.GetInt64(0));
                        }
                    }
                    DoRemoveFileTags(transaction, newPath, tags);
                }
            }
        }

        private long InsertOrFindFile(DbTransaction transaction, String path)
        {
            // Search for the file
            Object fileKey = DoFindFile(transaction, path);
            if (fileKey != null)
            {
                return (long)fileKey;
            }
            else
            {
                // File does not exist yet. Insert it.
                String insertString = String.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Id, @Path)", Schema.FILES_TABLE, Schema.ID_COLUMN, Schema.PATH_COLUMN);
                using (DbCommand command = DbUtils.CreateDbCommand(insertString, m_Connection, transaction))
                {
                    command.AddParameterWithValue("@Id", DBNull.Value);
                    command.AddParameterWithValue("@Path", path);
                    int result = command.ExecuteNonQuery();
                    if (result != 1)
                    {
                        throw new TagsDbException("Insertion into Files table failed.");
                    }

                    return m_Connection.LastInsertRowId();
                }
            }
        }

        private Object DoFindFile(DbTransaction transaction, String path)
        {
            String queryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@Path", Schema.ID_COLUMN, Schema.FILES_TABLE, Schema.PATH_COLUMN);
            using (DbCommand command = DbUtils.CreateDbCommand(queryString, m_Connection, transaction))
            {
                command.AddParameterWithValue("@Path", path);
                return command.ExecuteScalar();
            }
        }

        private void DoRemoveAllTagsForFile(DbTransaction transaction, long fileKey)
        {
            String deleteString = String.Format("DELETE FROM {0} WHERE {1}=@Id", Schema.FILETAGS_TABLE, Schema.FILE_COLUMN);
            using (DbCommand deleteCommand = DbUtils.CreateDbCommand(deleteString, m_Connection, transaction))
            {
                deleteCommand.AddParameterWithValue("@Id", fileKey);
                deleteCommand.ExecuteNonQuery();
            }
        }

        private void DoRemoveFile(DbTransaction transaction, String path)
        {
            Object fileKey = DoFindFile(transaction, path);
            if (fileKey != null)
            {
                // DoRemoveAllTagsForFile(transaction, fileKey); // ON DELETE CASCADE
                String removeString = String.Format("DELETE FROM {0} WHERE {1}=@Id", Schema.FILES_TABLE, Schema.ID_COLUMN);
                using (DbCommand deleteCommand = DbUtils.CreateDbCommand(removeString, m_Connection, transaction))
                {
                    deleteCommand.AddParameterWithValue("@Id", (long)fileKey);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }

        private void DoRemoveCategory(int categoryId)
        {
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                String deleteCatCommand = String.Format("DELETE FROM {0} WHERE {1}=@CatId", Schema.CATEGORIES_TABLE, Schema.ID_COLUMN);
                using (DbCommand command = DbUtils.CreateDbCommand(deleteCatCommand, m_Connection, transaction))
                {
                    command.AddParameterWithValue("@CatId", categoryId);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void DoRemoveTag(int tagId)
        {
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                List<long> tags = new List<long>();
                tags.Add(tagId);
                DoRemoveTags(tags, transaction);

                transaction.Commit();
            }
        }

        private static readonly String REMOVE_TAG_COMMAND = "DELETE FROM {0} WHERE {1}=@TagId";

        private void DoRemoveTags(List<long> tagIds, DbTransaction transaction)
        {
            if (tagIds.Count == 0)
            {
                return;
            }

            // Remove from Tags table
            using (DbCommand command3 = DbUtils.CreateDbCommand(String.Format(REMOVE_TAG_COMMAND, Schema.TAGS_TABLE, Schema.ID_COLUMN), m_Connection, transaction))
            {
                DbParameter param3 = command3.AddParameter("@TagId", System.Data.DbType.Int64);

                // ... do it for all tags
                foreach (long tagId in tagIds)
                {
                    param3.Value = tagId;
                    command3.ExecuteNonQuery();
                }
            }
        }

        private void DoSetFileIdentifications(IList<String> files, IList<FileIdentification> identifications)
        {
            if (identifications == null || identifications.Count == 0)
                return;
            String updateString = String.Format("UPDATE {0} SET {1}=@Artist, {2}=@Album, {3}=@Title, {4}=@AcoustId WHERE {5}=@Id",
                Schema.FILES_TABLE, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.ACOUST_ID_COLUMN, Schema.ID_COLUMN);
            String insertString = String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}, {5}, {6}) VALUES (@Id, @Path, @Artist, @Album, @Title, @AcoustId)",
                Schema.FILES_TABLE, Schema.ID_COLUMN, Schema.PATH_COLUMN, Schema.ARTIST_COLUMN, Schema.ALBUM_COLUMN, Schema.TITLE_COLUMN, Schema.ACOUST_ID_COLUMN);
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                using (DbCommand updateCommand = DbUtils.CreateDbCommand(updateString, m_Connection, transaction),
                                     insertCommand = DbUtils.CreateDbCommand(insertString, m_Connection, transaction))
                {
                    DbParameter artistParam = updateCommand.AddParameter("@Artist", System.Data.DbType.String);
                    DbParameter albumParam = updateCommand.AddParameter("@Album", System.Data.DbType.String);
                    DbParameter titleParam = updateCommand.AddParameter("@Title", System.Data.DbType.String);
                    DbParameter acoustIdParam = updateCommand.AddParameter("@AcoustId", System.Data.DbType.String);
                    DbParameter idParam = updateCommand.AddParameter("@Id", System.Data.DbType.Int64);

                    DbParameter artistParam2 = insertCommand.AddParameter("@Artist", System.Data.DbType.String);
                    DbParameter albumParam2 = insertCommand.AddParameter("@Album", System.Data.DbType.String);
                    DbParameter titleParam2 = insertCommand.AddParameter("@Title", System.Data.DbType.String);
                    DbParameter acoustIdParam2 = insertCommand.AddParameter("@AcoustId", System.Data.DbType.String);
                    DbParameter idParam2 = insertCommand.AddParameterWithValue("@Id", System.DBNull.Value);
                    DbParameter pathParam2 = insertCommand.AddParameter("@Path", System.Data.DbType.String);

                    for (int i = 0; i < identifications.Count; ++i)
                    {
                        FileIdentification file = identifications[i];
                        if (file.Id != -1)
                        {
                            // update existing file
                            AssignStringOrNull(artistParam, file.Artist);
                            AssignStringOrNull(albumParam, file.Album);
                            AssignStringOrNull(titleParam, file.Title);
                            AssignStringOrNull(acoustIdParam, file.AcoustId);
                            idParam.Value = file.Id;
                            updateCommand.ExecuteNonQuery();
                        }
                        else
                        {
                            // insert new file
                            AssignStringOrNull(artistParam2, file.Artist);
                            AssignStringOrNull(albumParam2, file.Album);
                            AssignStringOrNull(titleParam2, file.Title);
                            AssignStringOrNull(acoustIdParam2, file.AcoustId);
                            pathParam2.Value = files[i];
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        private void DoConfirmTags(IList<String> files)
        {
            String cmdString = String.Format("UPDATE {0} SET {1}=@NewUser WHERE {1}=@OldUser AND {2}=@FileId",
                Schema.FILETAGS_TABLE, Schema.USER_COLUMN, Schema.FILE_COLUMN);
            // Note: do not confirm removed tags, because they are not shown anywhere to be confirmed
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                using (DbCommand cmd = DbUtils.CreateDbCommand(cmdString, m_Connection, transaction))
                {
                    cmd.AddParameterWithValue("@NewUser", Schema.ARES_GUI_USER);
                    cmd.AddParameterWithValue("@OldUser", Schema.GLOBAL_DB_USER);
                    DbParameter fileIdParam = cmd.AddParameter("@FileId", System.Data.DbType.Int64);
                    foreach (String file in files)
                    {
                        Object fileId = DoFindFile(transaction, file);
                        if (fileId != null)
                        {
                            fileIdParam.Value = fileId;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                transaction.Commit();
            }
        }

        private bool DoCanConfirmTags(IList<String> files)
        {
            String query = String.Format("SELECT COUNT(*) FROM {0} WHERE {1}=@User AND {2}=@FileId",
                Schema.FILETAGS_TABLE, Schema.USER_COLUMN, Schema.FILE_COLUMN);
            using (DbCommand cmd = DbUtils.CreateDbCommand(query, m_Connection))
            {
                cmd.AddParameterWithValue("@User", Schema.GLOBAL_DB_USER);
                DbParameter fileIdParam = cmd.AddParameter("@FileId", System.Data.DbType.Int64);
                foreach (String file in files)
                {
                    Object fileId = DoFindFile(null, file);
                    if (fileId != null)
                    {
                        fileIdParam.Value = fileId;
                        Object res = cmd.ExecuteScalar();
                        if (res != null && (long)res > 0)
                            return true;
                    }
                }
            }
            return false;
        }

        private void DoCleanupDB(System.IO.TextWriter logStream, String musicPath, int langId)
        {
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                // 1. Remove all files which do not exist
                if (logStream != null)
                {
                    logStream.WriteLine("Removing entries for files which don't exist any more:");
                }
                String query = String.Format("SELECT DISTINCT {0} FROM {1}", Schema.PATH_COLUMN, Schema.FILES_TABLE);
                String removal = String.Format("DELETE FROM {0} WHERE {1}=@Path", Schema.FILES_TABLE, Schema.PATH_COLUMN);
                using (DbCommand queryCmd = DbUtils.CreateDbCommand(query, m_Connection, transaction),
                       removalCmd = DbUtils.CreateDbCommand(removal, m_Connection, transaction))
                {
                    DbParameter pathParam = removalCmd.AddParameter("@Path", System.Data.DbType.String);
                    using (DbDataReader reader = queryCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            String relativePath = reader.GetString(0);
                            String path = System.IO.Path.Combine(musicPath, relativePath);
                            if (!System.IO.File.Exists(path))
                            {
                                if (logStream != null)
                                    logStream.WriteLine(path);
                                pathParam.Value = relativePath;
                                removalCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                
                // 2. Remove all tags which are not assigned to any file
                if (logStream != null)
                {
                    logStream.WriteLine();
                    logStream.WriteLine("Removing unused tags:");
                    String tagsQuery = String.Format("SELECT DISTINCT {0} FROM {1} WHERE {0} NOT IN (SELECT DISTINCT {2} FROM {3}) AND {0} NOT IN (SELECT DISTINCT {2} FROM {4})",
                        Schema.ID_COLUMN, Schema.TAGS_TABLE, Schema.TAG_COLUMN, Schema.FILETAGS_TABLE, Schema.REMOVEDTAGS_TABLE);
                    // must use a separate query for the tag name because the tag might not have a name in the language
                    String tagNameQuery = String.Format("SELECT {0} FROM {1} WHERE {2}=@TagId AND {3}=@LangId",
                        Schema.NAME_COLUMN, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN);
                    using (DbCommand tagsQueryCmd = DbUtils.CreateDbCommand(tagsQuery, m_Connection, transaction),
                           tagNameQueryCmd = DbUtils.CreateDbCommand(tagNameQuery, m_Connection, transaction))
                    {
                        tagNameQueryCmd.AddParameterWithValue("@LangId", (long)langId);
                        DbParameter tagIdParam = tagNameQueryCmd.AddParameter("@TagId", System.Data.DbType.Int64);
                        using (DbDataReader reader = tagsQueryCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long id = reader.GetInt64(0);
                                tagIdParam.Value = id;
                                using (DbDataReader reader2 = tagNameQueryCmd.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {
                                        logStream.WriteLine(String.Format("{1} (Name: {0})", reader2.GetString(0), id));
                                    }
                                    else
                                    {
                                        logStream.WriteLine(String.Format("{0}", id));
                                    }
                                }
                            }
                        }
                    }
                }
                String tagRemoval = String.Format("DELETE FROM {0} WHERE {1} NOT IN (SELECT DISTINCT {2} FROM {3}) AND {1} NOT IN (SELECT DISTINCT {2} FROM {4})",
                    Schema.TAGS_TABLE, Schema.ID_COLUMN, Schema.TAG_COLUMN, Schema.FILETAGS_TABLE, Schema.REMOVEDTAGS_TABLE);
                using (DbCommand tagRemoveCmd = DbUtils.CreateDbCommand(tagRemoval, m_Connection, transaction))
                {
                    tagRemoveCmd.ExecuteNonQuery();
                }

                // 3. Remove all categories without tags
                if (logStream != null)
                {
                    logStream.WriteLine();
                    logStream.WriteLine("Removing empty categories: ");
                    String categoryQuery = String.Format("SELECT DISTINCT {0} FROM {1} WHERE {0} NOT IN (SELECT DISTINCT {2} FROM {3})",
                        Schema.ID_COLUMN, Schema.CATEGORIES_TABLE, Schema.CATEGORY_COLUMN, Schema.TAGS_TABLE);
                    String categoryNameQuery = String.Format("SELECT {0} FROM {1} WHERE {2}=@CategoryId AND {3}=@LangId",
                        Schema.NAME_COLUMN, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN);
                    using (DbCommand categoryQueryCmd = DbUtils.CreateDbCommand(categoryQuery, m_Connection, transaction),
                           categoryNameQueryCmd = DbUtils.CreateDbCommand(categoryNameQuery, m_Connection, transaction))
                    {
                        categoryNameQueryCmd.AddParameterWithValue("@LangId", (long)langId);
                        DbParameter categoryIdParam = categoryNameQueryCmd.AddParameter("@CategoryId", System.Data.DbType.Int64);
                        using (DbDataReader reader = categoryQueryCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                long id = reader.GetInt64(0);
                                categoryIdParam.Value = id;
                                using (DbDataReader reader2 = categoryNameQueryCmd.ExecuteReader())
                                {
                                    if (reader2.Read())
                                    {
                                        logStream.WriteLine(String.Format("{1} (Name: {0})", reader2.GetString(0), id));
                                    }
                                    else
                                    {
                                        logStream.WriteLine(String.Format("{0}", id));
                                    }
                                }
                            }
                        }
                    }
                }
                String categoryRemoval = String.Format("DELETE FROM {0} WHERE {1} NOT IN (SELECT DISTINCT {2} FROM {3})",
                    Schema.CATEGORIES_TABLE, Schema.ID_COLUMN, Schema.CATEGORY_COLUMN, Schema.TAGS_TABLE);
                using (DbCommand categoryRemoveCmd = DbUtils.CreateDbCommand(categoryRemoval, m_Connection, transaction))
                {
                    categoryRemoveCmd.ExecuteNonQuery();
                }

                // Done
                transaction.Commit();
            }
        }

        private int DoAssignTagsByIdentification(IList<FileIdentification> identifications, IList<String> files)
        {
            if (identifications == null || files == null || identifications.Count == 0)
                return 0;
            if (identifications.Count != files.Count)
                return 0;

            int count = 0;
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                using (FileFinder finder = new FileFinder(m_Connection, transaction))
                {
                    String stmtFmt = "INSERT INTO {0} ({1}, {2}, {3}) SELECT @NewId, {2}, {3} FROM {0} WHERE {1}=@OldId";
                    String insertTagsCmdString = String.Format(stmtFmt, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.USER_COLUMN);
                    String removeTagsCmdString = String.Format(stmtFmt, Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.USER_COLUMN);
                    using (DbCommand insertTagsCmd = DbUtils.CreateDbCommand(insertTagsCmdString, m_Connection, transaction),
                           removeTagsCmd = DbUtils.CreateDbCommand(removeTagsCmdString, m_Connection, transaction))
                    {
                        DbParameter insertNewIdParam = insertTagsCmd.AddParameter("@NewId", System.Data.DbType.Int64);
                        DbParameter removeNewIdParam = removeTagsCmd.AddParameter("@NewId", System.Data.DbType.Int64);
                        DbParameter insertOldIdParam = insertTagsCmd.AddParameter("@OldId", System.Data.DbType.Int64);
                        DbParameter removeOldIdParam = removeTagsCmd.AddParameter("@OldId", System.Data.DbType.Int64);
                        for (int i = 0; i < identifications.Count; ++i)
                        {
                            if (identifications[i].Id == -1)
                                continue; // no identification available
                            long foundId = finder.FindFileByIdentification(identifications[i], null, null);
                            if (foundId != -1)
                            {
                                long newId = InsertOrFindFile(transaction, files[i]);
                                if (foundId == newId)
                                    continue;
                                insertNewIdParam.Value = newId;
                                insertOldIdParam.Value = foundId;
                                insertTagsCmd.ExecuteNonQuery();
                                removeNewIdParam.Value = newId;
                                removeOldIdParam.Value = foundId;
                                removeTagsCmd.ExecuteNonQuery();
                                ++count;
                            }
                        }
                    }
                }
                transaction.Commit();
            }
            return count;
        }

        private static void AssignStringOrNull(DbParameter param, String value)
        {
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
        }
    }

}