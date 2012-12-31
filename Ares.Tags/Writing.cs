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

        public void SetFileTags(IList<string> relativePaths, IList<IList<int>> tagIds)
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
                        DoSetFileTags(transaction, relativePaths[i], tagIds[i]);
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

        private void DoSetFileTags(SQLiteTransaction transaction, String path, IList<int> tagIds)
        {
            if (tagIds.Count == 0)
            {
                DoRemoveFile(transaction, path);
                return;
            }
            long fileId = InsertFileOrRemoveItsTags(transaction, path);
            String insertTagString = String.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (@Id, @File, @Tag)",
                Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
            using (SQLiteCommand insertTagCommand = new SQLiteCommand(insertTagString, m_Connection, transaction))
            {
                insertTagCommand.Parameters.AddWithValue("@Id", DBNull.Value);
                insertTagCommand.Parameters.AddWithValue("@File", fileId);
                SQLiteParameter tagParam = insertTagCommand.Parameters.Add("@Tag", System.Data.DbType.Int64);
                foreach (int tagId in tagIds)
                {
                    tagParam.Value = (Int64)tagId;
                    int res = insertTagCommand.ExecuteNonQuery();
                    if (res != 1)
                    {
                        throw new TagsDbException("Insertion into FileTags table failed.");
                    }
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
                    SQLiteDataReader reader = command.ExecuteReader();
                    if (!reader.HasRows)
                        return;
                    List<int> tags = new List<int>();
                    while (reader.Read())
                    {
                        tags.Add((int)reader.GetInt64(0));
                    }
                    DoSetFileTags(transaction, newPath, tags);
                }
            }
        }

        private long InsertFileOrRemoveItsTags(SQLiteTransaction transaction, String path)
        {
            // Search for the file
            Object fileKey = DoFindFile(transaction, path);
            if (fileKey != null)
            {
                // File exists. Remove all its tags in preparation for setting new tags.
                DoRemoveAllTagsForFile(transaction, fileKey);
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

    }

}