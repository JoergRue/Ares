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
    class TagsTranslations : ITagsDBTranslations, IConnectionClient
    {
        private SQLiteConnection m_Connection;

        internal TagsTranslations(SQLiteConnection connection)
        {
            m_Connection = connection;
        }

        public void ConnectionChanged(SQLiteConnection connection)
        {
            m_Connection = connection;
        }

        public int AddLanguage(string code, string name)
        {
            if (String.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Value required", "code");
            }
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
                return DoAddLanguage(code, name);
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

        public int AddLanguage(string code, string name, int languageId)
        {
            if (String.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Value required", "code");
            }
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
                return DoAddLanguage(code, name, languageId);
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

        public void SetLanguageName(int languageIdOfName, int languageIdOfLanguage, string name)
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
                DoSetLanguageName(languageIdOfName, languageIdOfLanguage, name);
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

        public void RemoveLanguageTranslation(int languageIdOfName, int languageIdOfLanguage)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoRemoveTranslation(m_Connection, null, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, 
                    languageIdOfLanguage, languageIdOfName);
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

        public void RemoveLanguage(int languageId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoRemoveLanguage(languageId);
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

        public bool HasLanguageForCode(String languageCode)
        {
            if (String.IsNullOrEmpty(languageCode))
            {
                throw new ArgumentException("Value required", "languageCode");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                int id;
                return DoGetIdOfLanguage(languageCode, out id);
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

        public IDictionary<int, String> GetLanguageTranslations(int languageId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetTranslations(Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, languageId);
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

        public IDictionary<int, String> GetCategoryTranslations(int categoryId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetTranslations(Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, categoryId);
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

        public IDictionary<int, String> GetTagTranslations(int tagId)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetTranslations(Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, tagId);
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

        public int GetIdOfCurrentUILanguage()
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetIdOfLanguage(System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
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

        public int GetIdOfLanguage(String languageCode)
        {
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                return DoGetIdOfLanguage(languageCode);
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

        internal int DoAddLanguage(string code, string name)
        {
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                int newId = DoAddLanguage(code, name, transaction);

                transaction.Commit();

                return newId;
            }
        }

        internal int DoAddLanguage(string code, string name, SQLiteTransaction transaction)
        {
            // insert into language table
            long newId = DoInsertLanguage(transaction, code);

            // insert translation
            DoAddTranslation(m_Connection, transaction, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, newId, newId, name);

            return (int)newId;
        }

        internal int DoAddLanguage(string code, string name, int languageId)
        {
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                int newId = DoAddLanguage(code, name, languageId, transaction);

                transaction.Commit();

                return newId;
            }
        }

        internal int DoAddLanguage(string code, string name, int languageId, SQLiteTransaction transaction)
        {
            // insert into language table
            long newId = DoInsertLanguage(transaction, code);

            // insert translation
            DoAddTranslation(m_Connection, transaction, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, newId, languageId, name);

            return (int)newId;
        }

        private long DoInsertLanguage(SQLiteTransaction transaction, string code)
        {
            String addLanguageCommand = String.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Id, @Code)", Schema.LANGUAGE_TABLE, Schema.ID_COLUMN, Schema.LC_COLUMN);

            using (SQLiteCommand command = new SQLiteCommand(addLanguageCommand, m_Connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", DBNull.Value);
                command.Parameters.AddWithValue("@Code", code);
                int res = command.ExecuteNonQuery();
                if (res != 1)
                {
                    throw new TagsDbException("Insertion into Language Table failed.");
                }
            }

            return m_Connection.LastInsertRowId;
        }

        internal void DoSetLanguageName(int languageIdOfName, int languageIdOfLanguage, string name)
        {
            DoSetTranslation(m_Connection, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, languageIdOfLanguage, languageIdOfName, name);
        }

        internal void DoSetLanguageName(int languageIdOfName, int languageIdOfLanguage, string name, SQLiteTransaction transaction)
        {
            DoSetTranslation(m_Connection, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, 
                languageIdOfLanguage, languageIdOfName, name, transaction);
        }

        private static String GET_TRANSLATIONS_COMMAND = "SELECT {0}, {1} FROM {2} WHERE {3}=@RefId";

        private IDictionary<int, String> DoGetTranslations(String tableName, String refColName, String langColName, long refId)
        {
            return DoGetTranslations(tableName, refColName, langColName, refId, m_Connection, null);
        }

        internal static IDictionary<int, String> DoGetTranslations(String tableName, String refColName, String langColName, long refId, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            String getTranslationsCommand = String.Format(GET_TRANSLATIONS_COMMAND, langColName, Schema.NAME_COLUMN, tableName, refColName);
            using (SQLiteCommand queryCommand = new SQLiteCommand(getTranslationsCommand, connection, transaction))
            {
                queryCommand.Parameters.AddWithValue("@RefId", refId);
                using (SQLiteDataReader reader = queryCommand.ExecuteReader())
                {
                    Dictionary<int, String> result = new Dictionary<int, string>();
                    while (reader.Read())
                    {
                        result[(int)reader.GetInt64(0)] = reader.GetString(1);
                    }
                    return result;
                }
            }
        }

        private int DoGetIdOfLanguage(String languageCode)
        {
            int id = 0;
            bool res = DoGetIdOfLanguage(languageCode, out id);
            if (res)
                return id;
            // Language not found: try English
            res = DoGetIdOfLanguage("en", out id);
            if (res)
                return id;
            // English not found: return first language
            res = DoGetIdOfFirstLanguage(out id);
            if (res)
                return id;
            // No language found: insert at least English
            id = DoAddLanguage("en", "English");
            return id;
        }

        private static String FIND_LANGUAGE_COMMAND = "SELECT {0} FROM {1} WHERE {2}=@Code";

        private bool DoGetIdOfLanguage(String languageCode, out int id)
        {
            long lid;
            bool res = DoGetIdOfLanguage(languageCode, out lid, m_Connection, null);
            id = (int)lid;
            return res;
        }

        internal static bool DoGetIdOfLanguage(String languageCode, out long id, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            String findLanguageCommand = String.Format(FIND_LANGUAGE_COMMAND, Schema.ID_COLUMN, Schema.LANGUAGE_TABLE, Schema.LC_COLUMN);
            using (SQLiteCommand command = new SQLiteCommand(findLanguageCommand, connection))
            {
                command.Parameters.AddWithValue("@Code", languageCode);
                Object res = command.ExecuteScalar();
                if (res != null)
                {
                    id = (int)(long)res;
                    return true;
                }
                else
                {
                    id = 0;
                    return false;
                }
            }
        }

        private static String FIND_ANY_LANGUAGE_COMMAND = "SELECT {0} FROM {1}";

        private bool DoGetIdOfFirstLanguage(out int id)
        {
            String findAnyLanguageCommand = String.Format(FIND_ANY_LANGUAGE_COMMAND, Schema.ID_COLUMN, Schema.LANGUAGE_TABLE);
            using (SQLiteCommand command = new SQLiteCommand(findAnyLanguageCommand, m_Connection))
            {
                Object res = command.ExecuteScalar();
                if (res != null)
                {
                    id = (int)(long)res;
                    return true;
                }
                else
                {
                    id = 0;
                    return false;
                }
            }
        }

        private static String REMOVE_LANGUAGE_COMMAND = "DELETE FROM {0} WHERE {1}=@LangId";

        private void DoRemoveLanguage(int languageId)
        {
            // translations are removed automatically through ON DELETE CASCADE
            String removeLanguageCommand = String.Format(REMOVE_LANGUAGE_COMMAND, Schema.LANGUAGE_TABLE, Schema.ID_COLUMN);
            using (SQLiteCommand command = new SQLiteCommand(removeLanguageCommand, m_Connection))
            {
                command.Parameters.AddWithValue("@LangId", languageId);
                command.ExecuteNonQuery();
            }
        }

        private static String FIND_TRANSLATION_COMMAND = "SELECT {3} FROM {0} WHERE {1}=@RefId AND {2}=@LangId";
        private static String SET_TRANSLATION_COMMAND = "UPDATE {0} SET {1}=@Name WHERE {2}=@Id";

        internal static void DoSetTranslation(SQLiteConnection connection, String table, String refColName, String langColName, long refId, long langId, String name)
        {
            DoSetTranslation(connection, table, refColName, langColName, refId, langId, name, null);
        }

        internal static void DoSetTranslation(SQLiteConnection connection, String table, String refColName, String langColName, 
            long refId, long langId, String name, SQLiteTransaction transaction)
        {
            // First find an existing entry (if any)
            String queryLanguageCommand = String.Format(FIND_TRANSLATION_COMMAND, table, refColName, langColName, Schema.ID_COLUMN);
            using (SQLiteCommand queryCommand = new SQLiteCommand(queryLanguageCommand, connection, transaction))
            {
                queryCommand.Parameters.AddWithValue("@RefId", refId);
                queryCommand.Parameters.AddWithValue("@LangId", langId);
                Object res = queryCommand.ExecuteScalar();
                if (res != null)
                {
                    // entry exists, just change its name
                    String updateLanguageCommand = String.Format(SET_TRANSLATION_COMMAND, table, Schema.NAME_COLUMN, Schema.ID_COLUMN);
                    using (SQLiteCommand updateCommand = new SQLiteCommand(updateLanguageCommand, connection, transaction))
                    {
                        updateCommand.Parameters.AddWithValue("@Name", name);
                        updateCommand.Parameters.AddWithValue("@Id", res);
                        updateCommand.ExecuteNonQuery();
                    }
                }
                else
                {
                    // entry doesn't exist, insert it
                    DoAddTranslation(connection, transaction, table, refColName, langColName, refId, langId, name);
                }
            }
        }

        private static String ADD_TRANSLATION_COMMAND = "INSERT INTO {0} ({1}, {2}, {3}, {4}) VALUES (@Id, @RefId, @LangId, @Name)";

        internal static void DoAddTranslation(SQLiteConnection connection, SQLiteTransaction transaction, 
            String table, String refColName, String langColName, long refId, long langId, String name)
        {
            String addTranslationCommand = String.Format(ADD_TRANSLATION_COMMAND, table, Schema.ID_COLUMN, refColName, langColName, Schema.NAME_COLUMN);
            using (SQLiteCommand command = new SQLiteCommand(addTranslationCommand, connection, transaction))
            {
                command.Parameters.AddWithValue("@Id", DBNull.Value);
                command.Parameters.AddWithValue("@RefId", refId);
                command.Parameters.AddWithValue("@LangId", langId);
                command.Parameters.AddWithValue("@Name", name);
                command.ExecuteNonQuery();
            }
        }

        private static String REMOVE_TRANSLATION_COMMAND = "DELETE FROM {0} WHERE {1}=@RefId AND {2}=@LangId";

        internal static void DoRemoveTranslation(SQLiteConnection connection, SQLiteTransaction transaction,
            String table, String refColName, String langColName, long refId, long langId)
        {
            String removeTranslationCommand = String.Format(REMOVE_TRANSLATION_COMMAND, table, refColName, langColName);
            using (SQLiteCommand command = new SQLiteCommand(removeTranslationCommand, connection, transaction))
            {
                command.Parameters.AddWithValue("@RefId", refId);
                command.Parameters.AddWithValue("@LangId", langId);
                command.ExecuteNonQuery();
            }
        }
    }
}
