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

        public void ImportDatabase(string filePath)
        {
            throw new NotImplementedException();
        }

        public void ExportDatabase(IList<String> filePaths, String targetFileName)
        {
            throw new NotImplementedException();
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
            CreateSchema();
            InsertDefaultValues();
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

        private void CreateSchema()
        {
            String createTableCommand = "CREATE TABLE IF NOT EXISTS {0} ( ";
            String idColumnCommand = "{0} INTEGER NOT NULL PRIMARY KEY";
            String idRefColumnCommand = "{0} INTEGER NOT NULL REFERENCES {1} ({2}) ON DELETE CASCADE";
            String nameColumnCommand = "{0} VARCHAR(100) NOT NULL";

            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
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
                    + String.Format("{0} VARCHAR(500) NOT NULL", Schema.PATH_COLUMN)
                    + ");", m_Connection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                // FileTags table
                using (SQLiteCommand command = new SQLiteCommand(
                    String.Format(createTableCommand, Schema.FILETAGS_TABLE)
                    + String.Format(idColumnCommand, Schema.ID_COLUMN) + ", "
                    + String.Format(idRefColumnCommand, Schema.FILE_COLUMN, Schema.FILES_TABLE, Schema.ID_COLUMN) + ", "
                    + String.Format(idRefColumnCommand, Schema.TAG_COLUMN, Schema.TAGS_TABLE, Schema.ID_COLUMN)
                    + ");", m_Connection, transaction))
                {
                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
        }

        private void InsertDefaultValues()
        {
            // Languages
            TagsTranslations translations = new TagsTranslations(m_Connection);

            int enId = translations.DoAddLanguage("en", "English");
            int deId = translations.DoAddLanguage("de", "German", enId);
            int frId = translations.DoAddLanguage("fr", "French", enId);
            int esId = translations.DoAddLanguage("es", "Spanish", enId);

            translations.DoSetLanguageName(deId, enId, "Englisch");
            translations.DoSetLanguageName(deId, deId, "Deutsch");
            translations.DoSetLanguageName(deId, frId, "Französisch");
            translations.DoSetLanguageName(deId, esId, "Spanisch");

            translations.DoSetLanguageName(frId, enId, "Anglais");
            translations.DoSetLanguageName(frId, deId, "Allemand");
            translations.DoSetLanguageName(frId, frId, "Français");
            translations.DoSetLanguageName(frId, esId, "Espagnol");

            translations.DoSetLanguageName(esId, enId, "Inglés");
            translations.DoSetLanguageName(esId, deId, "Aléman");
            translations.DoSetLanguageName(esId, frId, "Francés");
            translations.DoSetLanguageName(esId, esId, "Castellano");

            // Categories
            LanguageWriting en = new LanguageWriting(enId, m_Connection);
            LanguageWriting de = new LanguageWriting(deId, m_Connection);
            LanguageWriting fr = new LanguageWriting(frId, m_Connection);
            LanguageWriting es = new LanguageWriting(esId, m_Connection);

            int atmId = AddDefaultCategory("Atmosphere", "Stimmung", "Ambiance", "Ambiente", en, de, fr, es);
            int sitId = AddDefaultCategory("Situation", "Situation", "Situation", "Situación", en, de, fr, es);
            int placeId = AddDefaultCategory("Place", "Ort", "Lieu", "Lugar", en, de, fr, es);

            // Tags
            String[][][] tags = new String[][][]
            {
                new String[][]
                {
                    new String[]{"Suspense", "Spannung", "Suspense", "Suspense"}
                },
                new String[][]
                {
                    new String[]{"Fight", "Kampf", "Combat", "Combate"}
                },
                new String[][]
                {
                    new String[]{"Orient", "Orient", "Orient", "Oriente"}
                }
            };

            int[] catIds = new int[] { atmId, sitId, placeId };
            for (int i = 0; i < catIds.Length; ++i)
            {
                String[][] catTags = tags[i];
                for (int j = 0; j < catTags.Length; ++j)
                {
                    String[] names = catTags[j];
                    AddDefaultTag(catIds[i], names[0], names[1], names[2], names[3], en, de, fr, es);
                }
            }
        }

        private int AddDefaultCategory(String en, String de, String fr, String es, 
            LanguageWriting enWrite, LanguageWriting deWrite, LanguageWriting frWrite, LanguageWriting esWrite)
        {
            int id = enWrite.AddCategory(en);
            deWrite.SetCategoryName(id, de);
            frWrite.SetCategoryName(id, fr);
            esWrite.SetCategoryName(id, es);
            return id;
        }

        private void AddDefaultTag(int category, String en, String de, String fr, String es,
            LanguageWriting enWrite, LanguageWriting deWrite, LanguageWriting frWrite, LanguageWriting esWrite)
        {
            int id = enWrite.AddTag(category, en);
            deWrite.SetTagName(id, de);
            frWrite.SetTagName(id, fr);
            esWrite.SetTagName(id, es);
        }

        private SQLiteConnection m_Connection;
    }
}