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
    partial class SQLiteTagsDB
    {
        public void OpenOrCreateDatabase(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {
                    DoOpenDatabase(filePath);
                    UpdateDatabase();
                }
                else
                {
                    CreateDatabase(filePath);
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
            NotifyConnectionChanged();
        }

        public void CloseDatabase()
        {
            DoCloseDatabase();
            NotifyConnectionChanged();
        }

        public string DefaultFileName
        {
            get { return "Ares.sqlite"; }
        }

        private void NotifyConnectionChanged()
        {
            foreach (IConnectionClient client in m_Clients)
            {
                client.ConnectionChanged(m_Connection);
            }
        }

        private void DoCloseDatabase()
        {
            try
            {
                if (m_Connection != null)
                {
                    m_Connection.Close();
                    m_Connection.Dispose();
                    m_Connection = null;
                }
            }
            catch (System.Data.DataException ex)
            {
                throw new TagsDbException(ex.Message, ex);
            }
        }

        private void CreateDatabase(String filePath)
        {
            DoOpenDatabase(filePath);
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                CreateSchema(transaction);
                InsertDefaultValues(transaction);

                transaction.Commit();
            }
        }

        private void DoOpenDatabase(String filePath)
        {
            if (m_Connection != null)
            {
                DoCloseDatabase();
            }
            
            m_Connection = new SQLiteConnection(String.Format("Data Source={0};Version=3;foreign keys=true;", filePath));
            
            m_Connection.Open();
        }

        #region DB Update

        private void UpdateDatabase()
        {
            using (SQLiteCommand command = new SQLiteCommand(String.Format("SELECT {0} FROM {1}", Schema.VERSION_COLUMN, Schema.DBINFO_TABLE), m_Connection))
            {
                object version = command.ExecuteScalar();
                int v = 0;
                if (version != null)
                {
                    v = (int)(long)version;
                }
                if (v == 0)
                {
                    using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
                    {
                        CreateSchema(transaction);
                        InsertDefaultValues(transaction);
                        transaction.Commit();
                    }
                }
                else if (v == 1)
                {
                    using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
                    {
                        UpdateSchemaFromVersion1(transaction);
                        transaction.Commit();
                    }
                }
            }
        }

        private void UpdateSchemaFromVersion1(SQLiteTransaction transaction)
        {
            String alterTableCommand = "ALTER TABLE {0} ADD COLUMN ";
            String createTableCommand = "CREATE TABLE IF NOT EXISTS {0} ( ";
            String idColumnCommand = "{0} INTEGER NOT NULL PRIMARY KEY";
            String idRefColumnCommand = "{0} INTEGER NOT NULL REFERENCES {1} ({2}) ON DELETE CASCADE";

            // update files table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(alterTableCommand, Schema.FILES_TABLE) +
                String.Format("{0} VARCHAR(500)", Schema.ARTIST_COLUMN), 
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(alterTableCommand, Schema.FILES_TABLE) +
                String.Format("{0} VARCHAR(500)", Schema.ALBUM_COLUMN), 
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(alterTableCommand, Schema.FILES_TABLE) +
                String.Format("{0} VARCHAR(500)", Schema.TITLE_COLUMN), 
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(alterTableCommand, Schema.FILES_TABLE) +
                String.Format("{0} VARCHAR(500)", Schema.ACOUST_ID_COLUMN), 
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // update filetags table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(alterTableCommand, Schema.FILETAGS_TABLE) +
                String.Format("{0} VARCHAR(100)", Schema.USER_COLUMN),
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // create removedtags table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.REMOVEDTAGS_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.FILE_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(100)", Schema.USER_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // create tagexport table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.TAGEXPORT_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }


            // update version info
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format("UPDATE {0} SET {1}=@version", Schema.DBINFO_TABLE, Schema.VERSION_COLUMN), m_Connection, transaction))
            {
                command.Parameters.AddWithValue("@version", Schema.DB_VERSION);
                command.ExecuteNonQuery();
            }
        }

        #endregion

        #region New DB Initialization

        private void CreateSchema(SQLiteTransaction transaction)
        {
            String createTableCommand = "CREATE TABLE IF NOT EXISTS {0} ( ";
            String idColumnCommand = "{0} INTEGER NOT NULL PRIMARY KEY";
            String idRefColumnCommand = "{0} INTEGER NOT NULL REFERENCES {1} ({2}) ON DELETE CASCADE";
            String nameColumnCommand = "{0} VARCHAR(100) NOT NULL";

            // DBInfo table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.DBINFO_TABLE)
                + String.Format("{0} INTEGER NOT NULL", Schema.VERSION_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
            using (SQLiteCommand command = new SQLiteCommand(String.Format("INSERT INTO {0} ({1}) VALUES({2})",
                Schema.DBINFO_TABLE, Schema.VERSION_COLUMN, Schema.DB_VERSION),
                m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // Languages table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.LANGUAGE_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(5) NOT NULL", Schema.LC_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // LanguageNames table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.LANGUAGENAMES_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.LANGUAGE_OF_NAME_COLUMN, Schema.LANGUAGE_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(nameColumnCommand, Schema.NAME_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // Categories table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.CATEGORIES_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
             
            // CategoryNames table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.CATEGORYNAMES_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.LANGUAGE_COLUMN, Schema.LANGUAGE_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.CATEGORY_COLUMN, Schema.CATEGORIES_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(nameColumnCommand, Schema.NAME_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // Tags table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.TAGS_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.CATEGORY_COLUMN, Schema.CATEGORIES_TABLE, Schema.ID_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // TagNames table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.TAGNAMES_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.LANGUAGE_COLUMN, Schema.LANGUAGE_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(nameColumnCommand, Schema.NAME_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // Files table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.FILES_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500) NOT NULL", Schema.PATH_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500)", Schema.ARTIST_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500)", Schema.ALBUM_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500)", Schema.TITLE_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500)", Schema.ACOUST_ID_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // FileTags table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.FILETAGS_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.FILE_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(100)", Schema.USER_COLUMN) 
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // RemovedTags table
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.REMOVEDTAGS_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.FILE_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(100)", Schema.USER_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // FileExport table (see export for explanation)
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.FILEEXPORT_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.FILE_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN) + ", "
                + String.Format("{0} VARCHAR(500) NOT NULL", Schema.PATH_COLUMN)
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }

            // TagExport table (see export for explanation)
            using (SQLiteCommand command = new SQLiteCommand(
                String.Format(createTableCommand, Schema.TAGEXPORT_TABLE)
                + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN) 
                + ");", m_Connection, transaction))
            {
                command.ExecuteNonQuery();
            }
        }

        private void InsertDefaultValues(SQLiteTransaction transaction)
        {
            // Languages
            TagsTranslations translations = new TagsTranslations(m_Connection);

            int enId = translations.DoAddLanguage("en", "English", transaction);
            int deId = translations.DoAddLanguage("de", "German", enId, transaction);
            int frId = translations.DoAddLanguage("fr", "French", enId, transaction);
            int esId = translations.DoAddLanguage("es", "Spanish", enId, transaction);

            translations.DoSetLanguageName(deId, enId, "Englisch", transaction);
            translations.DoSetLanguageName(deId, deId, "Deutsch", transaction);
            translations.DoSetLanguageName(deId, frId, "Französisch", transaction);
            translations.DoSetLanguageName(deId, esId, "Spanisch", transaction);

            translations.DoSetLanguageName(frId, enId, "Anglais", transaction);
            translations.DoSetLanguageName(frId, deId, "Allemand", transaction);
            translations.DoSetLanguageName(frId, frId, "Français", transaction);
            translations.DoSetLanguageName(frId, esId, "Espagnol", transaction);

            translations.DoSetLanguageName(esId, enId, "Inglés", transaction);
            translations.DoSetLanguageName(esId, deId, "Aléman", transaction);
            translations.DoSetLanguageName(esId, frId, "Francés", transaction);
            translations.DoSetLanguageName(esId, esId, "Castellano", transaction);

            // Categories
            LanguageWriting en = new LanguageWriting(enId, m_Connection);
            LanguageWriting de = new LanguageWriting(deId, m_Connection);
            LanguageWriting fr = new LanguageWriting(frId, m_Connection);
            LanguageWriting es = new LanguageWriting(esId, m_Connection);

            int atmId = AddDefaultCategory("Mood", "Stimmung", "Ambiance", "Ambiente", en, de, fr, es, transaction);
            int sitId = AddDefaultCategory("Situation", "Situation", "Situation", "Situación", en, de, fr, es, transaction);
            int placeId = AddDefaultCategory("Place", "Ort", "Lieu", "Lugar", en, de, fr, es, transaction);

            // Tags
            String[][][] tags = new String[][][]
            {
                new String[][]
                {
                    new String[]{"Suspense", "Spannung", "Suspense", "Suspense"},
                    new String[]{"Recovery", "Erholung", "Répos", "Recuperación"},
                    new String[]{"Mourning", "Trauer", "Deuil", "Duelo"}
                },
                new String[][]
                {
                    new String[]{"Fight", "Kampf", "Combat", "Combate"},
                    new String[]{"Ritual", "Ritual", "Rituel", "Ritual"},
                    new String[]{"Travel", "Reise", "Voyage", "Viaje"}
                },
                new String[][]
                {
                    new String[]{"Orient", "Orient", "Orient", "Oriente"},
                    new String[]{"Jungle", "Dschungel", "Jungle", "Jungla"},
                    new String[]{"Sea", "Meer", "Mer", "Mar"}
                }
            };

            int[] catIds = new int[] { atmId, sitId, placeId };
            for (int i = 0; i < catIds.Length; ++i)
            {
                String[][] catTags = tags[i];
                for (int j = 0; j < catTags.Length; ++j)
                {
                    String[] names = catTags[j];
                    AddDefaultTag(catIds[i], names[0], names[1], names[2], names[3], en, de, fr, es, transaction);
                }
            }
        }

        private int AddDefaultCategory(String en, String de, String fr, String es, 
            LanguageWriting enWrite, LanguageWriting deWrite, LanguageWriting frWrite, LanguageWriting esWrite,
            SQLiteTransaction transaction)
        {
            int id = enWrite.DoAddCategory(en, transaction);
            deWrite.DoSetCategoryName(id, de, transaction);
            frWrite.DoSetCategoryName(id, fr, transaction);
            esWrite.DoSetCategoryName(id, es, transaction);
            return id;
        }

        private void AddDefaultTag(int category, String en, String de, String fr, String es,
            LanguageWriting enWrite, LanguageWriting deWrite, LanguageWriting frWrite, LanguageWriting esWrite,
            SQLiteTransaction transaction)
        {
            int id = enWrite.DoAddTag(category, en, transaction);
            deWrite.DoSetTagName(id, de, transaction);
            frWrite.DoSetTagName(id, fr, transaction);
            esWrite.DoSetTagName(id, es, transaction);
        }

        #endregion

        private SQLiteConnection m_Connection;
    }
}