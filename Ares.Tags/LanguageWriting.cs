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

using System.Data.Common;

namespace Ares.Tags
{

    class LanguageWriting : ITagsDBWriteByLanguage, IConnectionClient
    {
        private int m_LanguageId;
        private DbConnection m_Connection;

        internal LanguageWriting(int languageId, DbConnection connection)
        {
            m_LanguageId = languageId;
            m_Connection = connection;
        }

        public void ConnectionChanged(DbConnection connection)
        {
            m_Connection = connection;
        }

        public int AddCategory(string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value required", "name");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoAddCategory(name);
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

        public int AddTag(int categoryId, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value required", "name");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoAddTag(categoryId, name);
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

        public void SetCategoryName(int categoryId, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value required", "name");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoSetCategoryName(categoryId, name);
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

        public void SetTagName(int tagId, string name)
        {
            if (String.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Value required", "name");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoSetTagName(tagId, name);
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

        internal int DoAddCategory(String name)
        {
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                int newId = DoAddCategory(name, transaction);

                transaction.Commit();

                return newId;
            }
        }

        internal int DoAddCategory(String name, DbTransaction transaction)
        {
            // Insert into categories table
            String insertCommand = String.Format("INSERT INTO {0} ({1}) VALUES (@Id)", Schema.CATEGORIES_TABLE, Schema.ID_COLUMN);
            using (DbCommand command = DbUtils.CreateDbCommand(insertCommand, m_Connection, transaction))
            {
                command.AddParameterWithValue("@Id", DBNull.Value);
                int res = command.ExecuteNonQuery();
                if (res != 1)
                {
                    throw new TagsDbException("Insertion into Category table failed.");
                }
            }
            long newId = m_Connection.LastInsertRowId();

            // Insert name into translation table
            TagsTranslations.DoAddTranslation(m_Connection, transaction, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, newId, m_LanguageId, name);

            return (int)newId;
        }

        private void DoSetCategoryName(int categoryId, String name)
        {
            TagsTranslations.DoSetTranslation(m_Connection, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, categoryId, m_LanguageId, name);
        }

        internal void DoSetCategoryName(int categoryId, String name, DbTransaction transaction)
        {
            TagsTranslations.DoSetTranslation(m_Connection, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, 
                categoryId, m_LanguageId, name, transaction);
        }

        private int DoAddTag(int categoryId, string name)
        {
            using (DbTransaction transaction = m_Connection.BeginTransaction())
            {
                int newId = DoAddTag(categoryId, name, transaction);

                transaction.Commit();

                return newId;
            }
        }

        internal int DoAddTag(int categoryId, string name, DbTransaction transaction)
        {
            // Insert into names table
            String insertCommand = String.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Id, @Category)", Schema.TAGS_TABLE, Schema.ID_COLUMN, Schema.CATEGORY_COLUMN);
            using (DbCommand command = DbUtils.CreateDbCommand(insertCommand, m_Connection, transaction))
            {
                command.AddParameterWithValue("@Id", DBNull.Value);
                command.AddParameterWithValue("@Category", categoryId);
                int res = command.ExecuteNonQuery();
                if (res != 1)
                {
                    throw new TagsDbException("Insertion into Tags table failed.");
                }
            }
            long newId = m_Connection.LastInsertRowId();

            // Insert name into translation table
            TagsTranslations.DoAddTranslation(m_Connection, transaction, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, newId, m_LanguageId, name);

            return (int)newId;
        }


        private void DoSetTagName(int tagId, String name)
        {
            TagsTranslations.DoSetTranslation(m_Connection, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, tagId, m_LanguageId, name);
        }

        internal void DoSetTagName(int tagId, String name, DbTransaction transaction)
        {
            TagsTranslations.DoSetTranslation(m_Connection, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, tagId, m_LanguageId, name, transaction);
        }

        private void DoRemoveCategory(int categoryId)
        {
            // Note: only removes the translation!
            TagsTranslations.DoRemoveTranslation(m_Connection, null, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, categoryId, m_LanguageId);
        }

        private void DoRemoveTag(int tagId)
        {
            // Note: only removes the translation!
            TagsTranslations.DoRemoveTranslation(m_Connection, null, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, tagId, m_LanguageId);
        }
    }
}