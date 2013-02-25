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
using System.Linq;
using System.Text;

using System.Data.SQLite;

namespace Ares.Tags
{
    class TagsDBWriting : ITagsDBWrite, IConnectionClient
    {
        private SQLiteConnection m_Connection;

        internal TagsDBWriting(SQLiteConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(SQLiteConnection connection)
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

                using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
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
            catch (SQLiteException ex)
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

                using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
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
            catch (SQLiteException ex)
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
                using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
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
            catch (SQLiteException ex)
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
                using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
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
            catch (SQLiteException ex)
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
                using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
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
            catch (SQLiteException ex)
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
            catch (SQLiteException ex)
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
            catch (SQLiteException ex)
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
            catch (SQLiteException ex)
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
            catch (SQLiteException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        private void DoAddFileTags(SQLiteTransaction transaction, String path, IList<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                return;
            }
            long fileId = InsertOrFindFile(transaction, path);
            String queryExistingString = String.Format("SELECT {0}, {1} FROM {2} WHERE {3}=@File AND {4}=@Tag",
                Schema.ID_COLUMN, Schema.USER_COLUMN, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String removeRemovedString = String.Format("DELETE FROM {0} WHERE {1}=@File AND {2}=@Tag",
                Schema.REMOVEDTAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            String insertTagString = String.Format("INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@Id, @File, @Tag, @User)",
                Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.USER_COLUMN);
            String updateUserString = String.Format("UPDATE {0} SET {1}=@User WHERE {2}=@File AND {3}=@Tag",
                Schema.FILETAGS_TABLE, Schema.USER_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            using (SQLiteCommand queryExistingCommand = new SQLiteCommand(queryExistingString, m_Connection, transaction),
                removeRemovedCommand = new SQLiteCommand(removeRemovedString, m_Connection, transaction),
                insertTagCommand = new SQLiteCommand(insertTagString, m_Connection, transaction),
                updateUserCommand = new SQLiteCommand(updateUserString, m_Connection, transaction))
            {
                queryExistingCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter queryParam = queryExistingCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                removeRemovedCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter removeParam = removeRemovedCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                insertTagCommand.Parameters.AddWithValue("@Id", DBNull.Value);
                insertTagCommand.Parameters.AddWithValue("@File", fileId);
                insertTagCommand.Parameters.AddWithValue("@User", Schema.ARES_GUI_USER);
                SQLiteParameter addParam = insertTagCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                updateUserCommand.Parameters.AddWithValue("@User", Schema.ARES_GUI_USER);
                updateUserCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter updateParam = updateUserCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                foreach (int tagId in tagIds)
                {
                    queryParam.Value = (Int64)tagId;
                    using (SQLiteDataReader reader = queryExistingCommand.ExecuteReader())
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

        private void DoRemoveFileTags(SQLiteTransaction transaction, String path, IList<int> tagIds)
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
            using (SQLiteCommand queryExistingCommand = new SQLiteCommand(queryExistingString, m_Connection, transaction),
                removeAddedCommand = new SQLiteCommand(removeAddedString, m_Connection, transaction),
                removeTagCommand = new SQLiteCommand(removeTagString, m_Connection, transaction),
                updateUserCommand = new SQLiteCommand(updateUserString, m_Connection, transaction))
            {
                queryExistingCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter queryParam = queryExistingCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                removeAddedCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter removeParam = removeAddedCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                removeTagCommand.Parameters.AddWithValue("@Id", DBNull.Value);
                removeTagCommand.Parameters.AddWithValue("@File", fileId);
                removeTagCommand.Parameters.AddWithValue("@User", Schema.ARES_GUI_USER);
                SQLiteParameter addParam = removeTagCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                updateUserCommand.Parameters.AddWithValue("@User", Schema.ARES_GUI_USER);
                updateUserCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter updateParam = updateUserCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                foreach (int tagId in tagIds)
                {
                    queryParam.Value = (Int64)tagId;
                    using (SQLiteDataReader reader = queryExistingCommand.ExecuteReader())
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

        private void DoMoveFile(String oldPath, String newPath, SQLiteTransaction transaction)
        {
            Object fileKey = DoFindFile(null, oldPath);
            if (fileKey != null)
            {
                String commandString = String.Format("UPDATE {0} SET {1}=@NewPath WHERE {2}=@Id", Schema.FILES_TABLE, Schema.PATH_COLUMN, Schema.ID_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(commandString, m_Connection, transaction))
                {
                    command.Parameters.AddWithValue("@NewPath", newPath);
                    command.Parameters.AddWithValue("@Id", fileKey);
                    int res = command.ExecuteNonQuery();
                    if (res != 1)
                    {
                        throw new TagsDbException("Updating Files table failed.");
                    }
                }
            }
        }

        private void DoCopyFile(String oldPath, String newPath, SQLiteTransaction transaction)
        {
            Object fileKey = DoFindFile(transaction, oldPath);
            if (fileKey != null)
            {
                String queryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@FileKey", Schema.TAG_COLUMN, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(queryString, m_Connection, transaction))
                {
                    command.Parameters.AddWithValue("@FileKey", fileKey);
                    List<int> tags = new List<int>();
                    using (SQLiteDataReader reader = command.ExecuteReader())
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
                using (SQLiteCommand command = new SQLiteCommand(queryString, m_Connection, transaction))
                {
                    command.Parameters.AddWithValue("@FileKey", fileKey);
                    List<int> tags = new List<int>();
                    using (SQLiteDataReader reader = command.ExecuteReader())
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

        private long InsertOrFindFile(SQLiteTransaction transaction, String path)
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
                using (SQLiteCommand command = new SQLiteCommand(insertString, m_Connection, transaction))
                {
                    command.Parameters.AddWithValue("@Id", DBNull.Value);
                    command.Parameters.AddWithValue("@Path", path);
                    int result = command.ExecuteNonQuery();
                    if (result != 1)
                    {
                        throw new TagsDbException("Insertion into Files table failed.");
                    }

                    return m_Connection.LastInsertRowId;
                }
            }
        }

        private Object DoFindFile(SQLiteTransaction transaction, String path)
        {
            String queryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@Path", Schema.ID_COLUMN, Schema.FILES_TABLE, Schema.PATH_COLUMN);
            using (SQLiteCommand command = new SQLiteCommand(queryString, m_Connection, transaction))
            {
                command.Parameters.AddWithValue("@Path", path);
                return command.ExecuteScalar();
            }
        }

        private void DoRemoveAllTagsForFile(SQLiteTransaction transaction, Object fileKey)
        {
            String deleteString = String.Format("DELETE FROM {0} WHERE {1}=@Id", Schema.FILETAGS_TABLE, Schema.FILE_COLUMN);
            using (SQLiteCommand deleteCommand = new SQLiteCommand(deleteString, m_Connection, transaction))
            {
                deleteCommand.Parameters.AddWithValue("@Id", fileKey);
                deleteCommand.ExecuteNonQuery();
            }
        }

        private void DoRemoveFile(SQLiteTransaction transaction, String path)
        {
            Object fileKey = DoFindFile(transaction, path);
            if (fileKey != null)
            {
                // DoRemoveAllTagsForFile(transaction, fileKey); // ON DELETE CASCADE
                String removeString = String.Format("DELETE FROM {0} WHERE {1}=@Id", Schema.FILES_TABLE, Schema.ID_COLUMN);
                using (SQLiteCommand deleteCommand = new SQLiteCommand(removeString, m_Connection, transaction))
                {
                    deleteCommand.Parameters.AddWithValue("@Id", fileKey);
                    deleteCommand.ExecuteNonQuery();
                }
            }
        }

        private void DoRemoveCategory(int categoryId)
        {
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                String deleteCatCommand = String.Format("DELETE FROM {0} WHERE {1}=@CatId", Schema.CATEGORIES_TABLE, Schema.ID_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(deleteCatCommand, m_Connection, transaction))
                {
                    command.Parameters.AddWithValue("@CatId", categoryId);
                    command.ExecuteNonQuery();
                }
                transaction.Commit();
            }
        }

        private void DoRemoveTag(int tagId)
        {
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                List<long> tags = new List<long>();
                tags.Add(tagId);
                DoRemoveTags(tags, transaction);

                transaction.Commit();
            }
        }

        private static readonly String REMOVE_TAG_COMMAND = "DELETE FROM {0} WHERE {1}=@TagId";

        private void DoRemoveTags(List<long> tagIds, SQLiteTransaction transaction)
        {
            if (tagIds.Count == 0)
            {
                return;
            }

            // Remove from Tags table
            using (SQLiteCommand command3 = new SQLiteCommand(String.Format(REMOVE_TAG_COMMAND, Schema.TAGS_TABLE, Schema.ID_COLUMN), m_Connection, transaction))
            {
                SQLiteParameter param3 = command3.Parameters.Add("@TagId", System.Data.DbType.Int64);

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
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                using (SQLiteCommand updateCommand = new SQLiteCommand(updateString, m_Connection, transaction),
                                     insertCommand = new SQLiteCommand(insertString, m_Connection, transaction))
                {
                    SQLiteParameter artistParam = updateCommand.Parameters.Add("@Artist", System.Data.DbType.String);
                    SQLiteParameter albumParam = updateCommand.Parameters.Add("@Album", System.Data.DbType.String);
                    SQLiteParameter titleParam = updateCommand.Parameters.Add("@Title", System.Data.DbType.String);
                    SQLiteParameter acoustIdParam = updateCommand.Parameters.Add("@AcoustId", System.Data.DbType.String);
                    SQLiteParameter idParam = updateCommand.Parameters.Add("@Id", System.Data.DbType.Int64);

                    SQLiteParameter artistParam2 = insertCommand.Parameters.Add("@Artist", System.Data.DbType.String);
                    SQLiteParameter albumParam2 = insertCommand.Parameters.Add("@Album", System.Data.DbType.String);
                    SQLiteParameter titleParam2 = insertCommand.Parameters.Add("@Title", System.Data.DbType.String);
                    SQLiteParameter acoustIdParam2 = insertCommand.Parameters.Add("@AcoustId", System.Data.DbType.String);
                    SQLiteParameter idParam2 = insertCommand.Parameters.AddWithValue("@Id", System.DBNull.Value);
                    SQLiteParameter pathParam2 = insertCommand.Parameters.Add("@Path", System.Data.DbType.String);

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
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                using (SQLiteCommand cmd = new SQLiteCommand(cmdString, m_Connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@NewUser", Schema.ARES_GUI_USER);
                    cmd.Parameters.AddWithValue("@OldUser", Schema.GLOBAL_DB_USER);
                    SQLiteParameter fileIdParam = cmd.Parameters.Add("@FileId", System.Data.DbType.Int64);
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

        private static void AssignStringOrNull(SQLiteParameter param, String value)
        {
            if (value == null)
                param.Value = DBNull.Value;
            else
                param.Value = value;
        }

    }

}