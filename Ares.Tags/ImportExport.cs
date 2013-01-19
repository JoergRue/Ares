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
using System.IO;

using System.Data.SQLite;

namespace Ares.Tags
{
    #region Data Types

    class TranslationExchange
    {
        public long LanguageId { get; set; }
        public String Name { get; set; }
    }

    class LanguageExchange
    {
        public String ISO6391Code { get; set; }
        public long Id { get; set; }
        public List<TranslationExchange> Names { get; set; }
    }

    class CategoryExchange
    {
        public long Id { get; set; }
        public List<TranslationExchange> Names { get; set; }
    }

    class TagExchange
    {
        public long Id { get; set; }
        public long CategoryId { get; set; }
        public List<TranslationExchange> Names { get; set; }
    }

    class TagsForFileExchange
    {
        public long FileId { get; set; }
        public List<long> TagIds { get; set; }
    }

    class FileExchange
    {
        public String RelativePath { get; set; }
        public long Id { get; set; }
    }

    class TagsExportedData
    {
        public List<LanguageExchange> Languages { get; set; }
        public List<CategoryExchange> Categories { get; set; }
        public List<TagExchange> Tags { get; set; }
        public List<FileExchange> Files { get; set; }
        public List<TagsForFileExchange> TagsForFiles { get; set; }
    }

    #endregion

    partial class SQLiteTagsDB
    {

        #region Export

        public void ExportDatabase(IList<String> filePaths, String targetFileName)
        {
            if (filePaths == null)
            {
                throw new ArgumentException("File paths must be given", "filePaths");
            }
            if (String.IsNullOrEmpty(targetFileName))
            {
                throw new ArgumentException("File name must be given", "targetFileName");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            try
            {
                DoExportDatabase(filePaths, targetFileName);
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

        private void DoExportDatabase(IList<String> filePaths, String targetFileName)
        {
            TagsExportedData data = CreateExportedData(filePaths);
            WriteDataToFile(data, targetFileName);
        }

        private void WriteDataToFile(TagsExportedData data, String targetFileName)
        {
            using (StreamWriter writer = new StreamWriter(targetFileName, false, Encoding.UTF8))
            {
                ServiceStack.Text.TypeSerializer.SerializeToWriter(data, writer);
                writer.Flush();
            }
        }

        private static readonly String GET_TRANSLATION_INFO = "SELECT {1}, {2} FROM {0} WHERE {0}.{3} = {4}";

        private static SQLiteCommand CreateTranslationInfosCommand(String translationTable, String mainIdColumn, String languageIdColumn, String mainIdParam, 
            SQLiteConnection connection, SQLiteTransaction transaction)
        {
            return new SQLiteCommand(String.Format(GET_TRANSLATION_INFO, translationTable, languageIdColumn, Schema.NAME_COLUMN, mainIdColumn, mainIdParam),
                connection, transaction);
        }


        private TagsExportedData CreateExportedData(IList<String> filePaths)
        {
            TagsExportedData data = new TagsExportedData();
            
            // Use temporary table with all file IDs. This is so we don't have to
            // use a "where File.Path in (........)" in each query or even manually
            // iterate over each file.
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                // no need to clear export table: transaction will always be rolled back
                String moveCommand = String.Format("INSERT INTO {0} ({1}, {2}) SELECT {3}, {4} FROM {5} WHERE {4} = @FilePath",
                    Schema.FILEEXPORT_TABLE, Schema.FILE_COLUMN, Schema.PATH_COLUMN, Schema.ID_COLUMN, Schema.PATH_COLUMN, Schema.FILES_TABLE, Schema.PATH_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(moveCommand, m_Connection, transaction))
                {
                    SQLiteParameter param = command.Parameters.Add("@FilePath", System.Data.DbType.String);
                    foreach (String file in filePaths)
                    {
                        param.Value = file;
                        command.ExecuteNonQuery();
                    }
                }

                // file information
                String fileInfo = String.Format("SELECT {0}, {1} FROM {2}", Schema.FILE_COLUMN, Schema.PATH_COLUMN, Schema.FILEEXPORT_TABLE);
                List<FileExchange> fileExchange = new List<FileExchange>();
                using (SQLiteCommand fileCommand = new SQLiteCommand(fileInfo, m_Connection, transaction))
                {
                    SQLiteDataReader reader = fileCommand.ExecuteReader();
                    while (reader.Read())
                    {
                        fileExchange.Add(new FileExchange() { Id = reader.GetInt64(0), RelativePath = reader.GetString(1) });
                    }
                }
                data.Files = fileExchange;

                // file tags information
                String fileTagsInfo = String.Format("SELECT {0}.{1}, {0}.{2} FROM {0}, {3} WHERE {0}.{1} = {3}.{4} ORDER BY {0}.{1}",
                    Schema.FILETAGS_TABLE, Schema.FILE_COLUMN, Schema.TAG_COLUMN, Schema.FILEEXPORT_TABLE, Schema.FILE_COLUMN);
                List<TagsForFileExchange> tagsForFileExchange = new List<TagsForFileExchange>();
                using (SQLiteCommand fileTagsCommand = new SQLiteCommand(fileTagsInfo, m_Connection, transaction))
                {
                    SQLiteDataReader reader = fileTagsCommand.ExecuteReader();
                    TagsForFileExchange tagsForFiles = null;
                    while (reader.Read())
                    {
                        long fileId = reader.GetInt64(0);
                        if (tagsForFiles == null || fileId != tagsForFiles.FileId)
                        {
                            tagsForFiles = new TagsForFileExchange() { FileId = fileId, TagIds = new List<long>() };
                            tagsForFileExchange.Add(tagsForFiles);
                        }
                        tagsForFiles.TagIds.Add(reader.GetInt64(1));
                    }
                }
                data.TagsForFiles = tagsForFileExchange;

                // tags information
                String tagsInfo = String.Format("SELECT DISTINCT {0}.{1}, {0}.{2} FROM {0}, {3}, {4} WHERE {0}.{1} = {3}.{5} AND {3}.{6} = {4}.{7}",
                    Schema.TAGS_TABLE, Schema.ID_COLUMN, Schema.CATEGORY_COLUMN, Schema.FILETAGS_TABLE, Schema.FILEEXPORT_TABLE, Schema.TAG_COLUMN, Schema.FILE_COLUMN, Schema.FILE_COLUMN);
                List<TagExchange> tags = new List<TagExchange>();
                using (SQLiteCommand tagsInfoCommand = new SQLiteCommand(tagsInfo, m_Connection, transaction))
                {
                    SQLiteDataReader reader = tagsInfoCommand.ExecuteReader();
                    using (SQLiteCommand translationInfoCommand = CreateTranslationInfosCommand(Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, "@TagId", m_Connection, transaction))
                    {
                        SQLiteParameter param = translationInfoCommand.Parameters.Add("@TagId", System.Data.DbType.Int64);
                        while (reader.Read())
                        {
                            long tagId = reader.GetInt64(0);
                            param.Value = tagId;
                            List<TranslationExchange> translations = new List<TranslationExchange>();
                            using (SQLiteDataReader reader2 = translationInfoCommand.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    translations.Add(new TranslationExchange() { LanguageId = reader2.GetInt64(0), Name = reader2.GetString(1) });
                                }
                            }
                            tags.Add(new TagExchange() { Id = tagId, CategoryId = reader.GetInt64(1), Names = translations });
                        }
                    }
                }
                data.Tags = tags;

                // category information (no need for category table, just select distinct category ids from tags table)
                String categoryInfo = String.Format("SELECT DISTINCT {0}.{2} FROM {0}, {3}, {4} WHERE {0}.{1} = {3}.{5} AND {3}.{6} = {4}.{7}",
                    Schema.TAGS_TABLE, Schema.ID_COLUMN, Schema.CATEGORY_COLUMN, Schema.FILETAGS_TABLE, Schema.FILEEXPORT_TABLE, Schema.TAG_COLUMN, Schema.FILE_COLUMN, Schema.FILE_COLUMN);
                List<CategoryExchange> categories = new List<CategoryExchange>();
                using (SQLiteCommand categoryInfoCommand = new SQLiteCommand(categoryInfo, m_Connection, transaction))
                {
                    SQLiteDataReader reader = categoryInfoCommand.ExecuteReader();
                    using (SQLiteCommand translationInfoCommand = CreateTranslationInfosCommand(Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, "@CategoryId", m_Connection, transaction))
                    {
                        SQLiteParameter param = translationInfoCommand.Parameters.Add("@CategoryId", System.Data.DbType.Int64);
                        while (reader.Read())
                        {
                            long categoryId = reader.GetInt64(0);
                            param.Value = categoryId;
                            List<TranslationExchange> translations = new List<TranslationExchange>();
                            using (SQLiteDataReader reader2 = translationInfoCommand.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    translations.Add(new TranslationExchange() { LanguageId = reader2.GetInt64(0), Name = reader2.GetString(1) });
                                }
                            }
                            categories.Add(new CategoryExchange() { Id = categoryId, Names = translations });
                        }
                    }
                }
                data.Categories = categories;

                // language information: always simply add all languages, those are few anyway
                String languageInfo = String.Format("SELECT {0}.{1}, {0}.{2}, {3}.{4}, {3}.{5} FROM {0}, {3} WHERE {0}.{1} = {3}.{6} ORDER BY {0}.{1}",
                    Schema.LANGUAGE_TABLE, Schema.ID_COLUMN, Schema.LC_COLUMN, Schema.LANGUAGENAMES_TABLE, Schema.LANGUAGE_OF_NAME_COLUMN, Schema.NAME_COLUMN, Schema.NAMED_LANGUAGE_COLUMN);
                List<LanguageExchange> languages = new List<LanguageExchange>();
                using (SQLiteCommand languageInfoCommand = new SQLiteCommand(languageInfo, m_Connection, transaction))
                {
                    SQLiteDataReader reader = languageInfoCommand.ExecuteReader();
                    LanguageExchange language = null;
                    while (reader.Read())
                    {
                        long langId = reader.GetInt64(0);
                        if (language == null || language.Id != langId)
                        {
                            language = new LanguageExchange() { Id = langId, ISO6391Code = reader.GetString(1), Names = new List<TranslationExchange>() };
                            languages.Add(language);
                        }
                        language.Names.Add(new TranslationExchange() { LanguageId = reader.GetInt64(2), Name = reader.GetString(3) });
                    }
                }
                data.Languages = languages;

                transaction.Rollback();
            }

            return data;
        }

        #endregion

        #region Import

        public void ImportDatabase(string filePath, TextWriter logStream)
        {
            if (String.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File name must be given", "filePath");
            }
            if (m_Connection == null)
            {
                throw new TagsDbException("No Connection to DB file!");
            }
            if (logStream == null)
            {
                // use dummy
                logStream = new StringWriter();
            }
            try
            {
                DoImportDatabase(filePath, logStream);
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                throw new TagsDbException(ex.Message, ex);
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

        private void DoImportDatabase(String filePath, TextWriter logStream)
        {
            TagsExportedData data = ReadTagsExportedData(filePath);
            ImportExportedData(data, logStream);
        }

        private TagsExportedData ReadTagsExportedData(String filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return ServiceStack.Text.TypeSerializer.DeserializeFromStream<TagsExportedData>(stream);
            }
        }

        private void ImportExportedData(TagsExportedData data, TextWriter logStream)
        {
            if (data == null)
                return;
            using (SQLiteTransaction transaction = m_Connection.BeginTransaction())
            {
                if (data.Languages == null)
                    return;
                if (data.Languages.Count == 0)
                    return;

                ImportHelper helper = new ImportHelper(this, m_Connection, transaction, logStream);
                helper.ImportExportedData(data);

                transaction.Commit();
            }            
        }

        // helper class mainly to avoid passing many parameters around
        private class ImportHelper
        {
            private SQLiteTagsDB m_TagsDb;
            private SQLiteConnection m_Connection;
            private TextWriter m_LogStream;
            private SQLiteTransaction m_Transaction;
            private Dictionary<long, long> m_LanguageMap = new Dictionary<long, long>();
            private Dictionary<long, long> m_CategoriesMap = new Dictionary<long, long>();
            private Dictionary<long, long> m_TagsMap = new Dictionary<long, long>();
            private Dictionary<long, long> m_FilesMap = new Dictionary<long, long>();

            public ImportHelper(SQLiteTagsDB tagsDB, SQLiteConnection connection, SQLiteTransaction transaction, TextWriter logStream)
            {
                m_TagsDb = tagsDB;
                m_LogStream = logStream;
                m_Transaction = transaction;
                m_Connection = connection;
            }

            public void ImportExportedData(TagsExportedData data)
            {
                ImportLanguages(data.Languages);
                ImportCategories(data.Categories);
                ImportTags(data.Tags);
                ImportFiles(data.Files);
                ImportFileTags(data.TagsForFiles);
            }

            private void ImportLanguages(List<LanguageExchange> languages)
            {
                // first add / find languages itself
                foreach (LanguageExchange le in languages)
                {
                    m_LanguageMap[le.Id] = FindOrImportLanguage(le);
                }
                // then add the translation (needs Ids of added languages)
                foreach (LanguageExchange le in languages)
                {
                    ImportLanguageTranslations(le);
                }
            }

            private long FindOrImportLanguage(LanguageExchange le)
            {
                long id;
                if (TagsTranslations.DoGetIdOfLanguage(le.ISO6391Code, out id, m_Connection, m_Transaction))
                {
                    m_LogStream.WriteLine(String.Format("Found language {0} (imported id {1}) with id {2}", le.ISO6391Code, le.Id, id));
                    return id;
                }
                else
                {
                    String name = String.Empty;
                    if (le.Names != null)
                    {
                        foreach (TranslationExchange translation in le.Names)
                        {
                            if (String.IsNullOrEmpty(translation.Name))
                            {
                                continue;
                            }
                            if (translation.LanguageId == le.Id)
                            {
                                name = translation.Name;
                                break;
                            }
                        }
                    }
                    if (name.Length > 0)
                    {
                        TagsTranslations translations = new TagsTranslations(m_Connection);
                        int res = translations.DoAddLanguage(le.ISO6391Code, name, m_Transaction);
                        m_LogStream.WriteLine(String.Format("Insert new language {0} (imported id {1}) with id {2}", le.ISO6391Code, le.Id, res));
                        return res;
                    }
                    else
                    {
                        throw new TagsDbException("A language must have a name in its own language");
                    }
                }
            }

            private void ImportLanguageTranslations(LanguageExchange le)
            {
                long languageId = m_LanguageMap[le.Id];
                ImportTranslations(le.Names, languageId, Schema.LANGUAGENAMES_TABLE, Schema.NAMED_LANGUAGE_COLUMN, Schema.LANGUAGE_OF_NAME_COLUMN, "language");
            }

            private void ImportCategories(List<CategoryExchange> categories)
            {
                if (categories == null)
                    return;
                foreach (CategoryExchange category in categories)
                {
                    long catId;
                    if (FindOrAddCategory(category, out catId))
                    {
                        m_CategoriesMap[category.Id] = catId;
                        ImportCategoryTranslations(category);
                    }
                }
            }

            private bool FindOrAddCategory(CategoryExchange category, out long catId)
            {
                // first try to find an existing category
                HashSet<long> existingIds = new HashSet<long>();
                String nameQueryString = String.Format("SELECT {0} FROM {1} WHERE {2}=@LangId AND {3}=@Name",
                    Schema.CATEGORY_COLUMN, Schema.CATEGORYNAMES_TABLE, Schema.LANGUAGE_COLUMN, Schema.NAME_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(nameQueryString, m_Connection, m_Transaction))
                {
                    SQLiteParameter langParam = command.Parameters.Add("@LangId", System.Data.DbType.Int64);
                    SQLiteParameter nameParam = command.Parameters.Add("@Name", System.Data.DbType.String);
                    if (category.Names != null)
                    {
                        foreach (TranslationExchange translation in category.Names)
                        {
                            if (String.IsNullOrEmpty(translation.Name))
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Ignoring empty name for category {0} translation into language {1}", category.Id, translation.LanguageId));
                                continue;
                            }
                            if (m_LanguageMap.ContainsKey(translation.LanguageId))
                            {
                                langParam.Value = m_LanguageMap[translation.LanguageId];
                                nameParam.Value = translation.Name;
                                Object val = command.ExecuteScalar();
                                if (val != null)
                                {
                                    existingIds.Add((long)val);
                                }
                            }
                            else
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Language id {0} for category translation '{1}' not found", translation.LanguageId, translation.Name));
                            }
                        }
                    }
                }
                if (existingIds.Count == 1)
                {
                    catId = existingIds.ToList()[0];
                    m_LogStream.WriteLine(String.Format("Found existing category {0} for imported category {1}", catId, category.Id));
                }
                else if (existingIds.Count > 1)
                {
                    catId = existingIds.ToList()[0];
                    m_LogStream.WriteLine(String.Format("WARNING: Found {0} existing categories for imported category {1}, using {2}", existingIds.Count, category.Id, catId));
                }
                else
                {
                    // no existing category found, insert a new one
                    String name = String.Empty;
                    long langId = 0;
                    if (category.Names != null)
                    {
                        foreach (TranslationExchange translation in category.Names)
                        {
                            if (String.IsNullOrEmpty(translation.Name))
                            {
                                continue;
                            }
                            if (m_LanguageMap.ContainsKey(translation.LanguageId))
                            {
                                name = translation.Name;
                                langId = m_LanguageMap[translation.LanguageId];
                                break;
                            }
                        }
                    }
                    if (name.Length > 0)
                    {
                        LanguageWriting writing = (LanguageWriting)m_TagsDb.GetWriteInterfaceByLanguage((int)langId);
                        catId = writing.DoAddCategory(name, m_Transaction);
                        m_LogStream.WriteLine(String.Format("Added category {0} (language {1}, new Id {2}) for Id {3}", name, langId, catId, category.Id));
                        m_CategoriesMap[category.Id] = catId;
                    }
                    else
                    {
                        m_LogStream.WriteLine(String.Format("WARNING: No usable translation found for category {0}; ignoring category", category.Id));
                        catId = 0;
                        return false;
                    }
                }
                return true;
            }

            private void ImportCategoryTranslations(CategoryExchange ce)
            {
                long categoryId = m_CategoriesMap[ce.Id];
                ImportTranslations(ce.Names, categoryId, Schema.CATEGORYNAMES_TABLE, Schema.CATEGORY_COLUMN, Schema.LANGUAGE_COLUMN, "category");
            }

            private void ImportTags(List<TagExchange> tags)
            {
                if (tags == null)
                    return;
                foreach (TagExchange tag in tags)
                {
                    long tagId;
                    if (FindOrAddTag(tag, out tagId))
                    {
                        m_TagsMap[tag.Id] = tagId;
                        ImportTagTranslations(tag);
                    }
                }
            }

            private bool FindOrAddTag(TagExchange tag, out long tagId)
            {
                // first try to find an existing tag

                // get all tags which have a name in a language which matches one of the translations for the imported tag
                Dictionary<long, long> existingIds = new Dictionary<long, long>();
                String nameQueryString = String.Format("SELECT {0}.{1}, {0}.{2} FROM {0}, {3} WHERE {0}.{1}={3}.{4} AND {3}.{5}=@LangId AND {3}.{6}=@Name",
                    Schema.TAGS_TABLE, Schema.ID_COLUMN, Schema.CATEGORY_COLUMN, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, Schema.NAME_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(nameQueryString, m_Connection, m_Transaction))
                {
                    SQLiteParameter langParam = command.Parameters.Add("@LangId", System.Data.DbType.Int64);
                    SQLiteParameter nameParam = command.Parameters.Add("@Name", System.Data.DbType.String);
                    if (tag.Names != null)
                    {
                        foreach (TranslationExchange translation in tag.Names)
                        {
                            if (String.IsNullOrEmpty(translation.Name))
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Ignoring empty name for tag {0} translation into language {1}", tag.Id, translation.LanguageId));
                                continue;
                            }
                            if (m_LanguageMap.ContainsKey(translation.LanguageId))
                            {
                                langParam.Value = m_LanguageMap[translation.LanguageId];
                                nameParam.Value = translation.Name;
                                using (SQLiteDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        existingIds[reader.GetInt64(0)] = reader.GetInt64(1);
                                    }
                                }
                            }
                            else
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Language id {0} for tag translation '{1}' not found", translation.LanguageId, translation.Name));
                            }
                        }
                    }
                }

                // check the categories of the found tags
                if (m_CategoriesMap.ContainsKey(tag.CategoryId))
                {
                    long categoryId = m_CategoriesMap[tag.CategoryId];
                    int countWithCorrectCategory = existingIds.Count((KeyValuePair<long, long> entry) => { return entry.Value == categoryId; });
                    if (countWithCorrectCategory > 0)
                    {
                        tagId = existingIds.First((KeyValuePair<long, long> entry) => { return entry.Value == categoryId; }).Key;
                        if (existingIds.Count > countWithCorrectCategory)
                        {
                            m_LogStream.WriteLine(String.Format("Found {0} existing tags for tag {1}, {2} with matching category; dismissing the rest",
                                existingIds.Count, tag.Id, countWithCorrectCategory));
                        }
                        if (countWithCorrectCategory > 1)
                        {
                            m_LogStream.WriteLine(String.Format("Found {0} matching tags for imported tag {1}, using {2}", countWithCorrectCategory, tag.Id, tagId));
                        }
                        else
                        {
                            m_LogStream.WriteLine(String.Format("Found existing tag {0} for imported tag {1}", tagId, tag.Id));
                        }
                        return true;
                    }
                    else if (existingIds.Count == 1)
                    {
                        m_LogStream.WriteLine(String.Format("Found an existing tag for imported tag {0}, but with different category. Adding tag with new category.", tag.Id));
                    }
                    else if (existingIds.Count > 1)
                    {
                        m_LogStream.WriteLine(String.Format("Found {0} existing tags for imported tag {1}, but with different categories. Adding tag with new category.",
                            existingIds.Count, tag.Id));
                    }
                }
                else if (existingIds.Count == 1)
                {
                    var entry = existingIds.ToList()[0];
                    tagId = entry.Key;
                    m_LogStream.WriteLine(String.Format("WARNING: Unknown category for imported tag {0}. Using found tag {1} with category {2}.", tag.Id, entry.Key, entry.Value));
                    return true;
                }
                else if (existingIds.Count > 1)
                {
                    var entry = existingIds.ToList()[0];
                    tagId = entry.Key;
                    m_LogStream.WriteLine(String.Format("WARNING: Unknown category for imported tag {0}. Found {1} matching tags; using found tag {2} with category {3}.", 
                        tag.Id, existingIds.Count, entry.Key, entry.Value));
                    return true;
                }

                // no existing tag found, insert a new one
                if (!m_CategoriesMap.ContainsKey(tag.CategoryId))
                {
                    m_LogStream.WriteLine(String.Format("WARNING: Unknown category {0} for imported tag {1} and no matching tags found. Skipping tag.", tag.CategoryId, tag.Id));
                    tagId = 0;
                    return false;
                }
                String name = String.Empty;
                long langId = 0;
                if (tag.Names != null)
                {
                    foreach (TranslationExchange translation in tag.Names)
                    {
                        if (String.IsNullOrEmpty(translation.Name))
                        {
                            continue;
                        }
                        if (m_LanguageMap.ContainsKey(translation.LanguageId))
                        {
                            name = translation.Name;
                            langId = m_LanguageMap[translation.LanguageId];
                            break;
                        }
                    }
                }
                if (name.Length > 0)
                {
                    LanguageWriting writing = (LanguageWriting)m_TagsDb.GetWriteInterfaceByLanguage((int)langId);
                    tagId = writing.DoAddTag((int)m_CategoriesMap[tag.CategoryId], name, m_Transaction);
                    m_LogStream.WriteLine(String.Format("Added tag {0} (language {1}, new Id {2}) for Id {3}", name, langId, tagId, tag.Id));
                    return true;
                }
                else
                {
                    m_LogStream.WriteLine(String.Format("WARNING: No usable translation found for tag {0}; ignoring tag", tag.Id));
                    tagId = 0;
                    return false;
                }
            }

            private void ImportTagTranslations(TagExchange te)
            {
                long tagId = m_TagsMap[te.Id];
                ImportTranslations(te.Names, tagId, Schema.TAGNAMES_TABLE, Schema.TAG_COLUMN, Schema.LANGUAGE_COLUMN, "tag");
            }

            private void ImportFiles(List<FileExchange> files)
            {
                if (files == null)
                    return;
                String fileQuery = String.Format("SELECT {0} FROM {1} WHERE {2}=@FilePath", Schema.ID_COLUMN, Schema.FILES_TABLE, Schema.PATH_COLUMN);
                String insertString = String.Format("INSERT INTO {0} ({1}, {2}) VALUES (@Id, @Path)", Schema.FILES_TABLE, Schema.ID_COLUMN, Schema.PATH_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(fileQuery, m_Connection, m_Transaction))
                {
                    SQLiteParameter param = command.Parameters.Add("@FilePath", System.Data.DbType.String);
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertString, m_Connection, m_Transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@Id", DBNull.Value);
                        SQLiteParameter insertParam = insertCommand.Parameters.Add("@Path", System.Data.DbType.String);
                        foreach (FileExchange file in files)
                        {
                            if (String.IsNullOrEmpty(file.RelativePath))
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Ignoring file {0} with missing path", file.Id));
                                continue;
                            }
                            param.Value = file.RelativePath;
                            Object existingId = command.ExecuteScalar();
                            if (existingId != null)
                            {
                                m_LogStream.WriteLine(String.Format("Found existing id {0} for imported file {1}", existingId, file.RelativePath));
                                m_FilesMap[file.Id] = (long)existingId;
                            }
                            else
                            {
                                insertParam.Value = file.RelativePath;
                                insertCommand.ExecuteNonQuery();
                                long id = m_Connection.LastInsertRowId;
                                m_FilesMap[file.Id] = id;
                                m_LogStream.WriteLine(String.Format("Insert new file {0} with id {1} (imported id {2})", file.RelativePath, id, file.Id));
                            }
                        }
                    }
                }
            }

            private void ImportFileTags(List<TagsForFileExchange> tags)
            {
                if (tags == null)
                    return;
                String tagsQuery = String.Format("SELECT DISTINCT {0} FROM {1} WHERE {2}=@FileId", Schema.TAG_COLUMN, Schema.FILETAGS_TABLE, Schema.FILE_COLUMN);
                String insertString = String.Format("INSERT INTO {0} ({1}, {2}, {3}) VALUES (@NewId, @FileId, @TagId)",
                    Schema.FILETAGS_TABLE, Schema.ID_COLUMN, Schema.FILE_COLUMN, Schema.TAG_COLUMN);
                using (SQLiteCommand command = new SQLiteCommand(tagsQuery, m_Connection, m_Transaction))
                {
                    SQLiteParameter param = command.Parameters.Add("@FileId", System.Data.DbType.Int64);
                    using (SQLiteCommand insertCommand = new SQLiteCommand(insertString, m_Connection, m_Transaction))
                    {
                        insertCommand.Parameters.AddWithValue("@NewId", DBNull.Value);
                        SQLiteParameter fileInsertParam = insertCommand.Parameters.Add("@FileId", System.Data.DbType.Int64);
                        SQLiteParameter tagInsertParam = insertCommand.Parameters.Add("@TagId", System.Data.DbType.Int64);
                        foreach (TagsForFileExchange fileTags in tags)
                        {
                            if (m_FilesMap.ContainsKey(fileTags.FileId))
                            {
                                long fileId = m_FilesMap[fileTags.FileId];
                                // find tags to add
                                HashSet<long> importedTags = new HashSet<long>();
                                foreach (long tagId in fileTags.TagIds)
                                {
                                    if (m_TagsMap.ContainsKey(tagId))
                                    {
                                        importedTags.Add(m_TagsMap[tagId]);
                                    }
                                    else
                                    {
                                        m_LogStream.WriteLine(String.Format("WARNING: Unknown tag {0} for file {1}; ignoring assignment", tagId, fileTags.FileId));
                                    }
                                }
                                // find existing tags --> those don't need to be added
                                param.Value = fileId;
                                using (SQLiteDataReader reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        long tagId = reader.GetInt64(0);
                                        if (importedTags.Contains(tagId))
                                        {
                                            m_LogStream.WriteLine(String.Format("Tag {0} already assigned to file {1}", tagId, fileId));
                                            importedTags.Remove(tagId);
                                        }
                                    }
                                }
                                // insert remaining tags
                                if (importedTags.Count > 0)
                                {
                                    fileInsertParam.Value = fileId;
                                    foreach (Int64 tagId in importedTags)
                                    {
                                        tagInsertParam.Value = tagId;
                                        insertCommand.ExecuteNonQuery();
                                        m_LogStream.WriteLine(String.Format("Assigned tag {0} to file {1}", tagId, fileId));
                                    }
                                }
                            }
                            else
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Unknown file {0} in tags; ignoring assignment", fileTags.FileId));
                            }
                        }
                    }
                }
            }

            private void ImportTranslations(List<TranslationExchange> names, long id, String tableName, String refColumnName, String langColumnName, String typeNameForLog)
            {
                if (names == null || names.Count == 0)
                    return;
                var translations = TagsTranslations.DoGetTranslations(tableName, refColumnName, langColumnName, id, m_Connection, m_Transaction);
                foreach (TranslationExchange translation in names)
                {
                    if (m_LanguageMap.ContainsKey(translation.LanguageId))
                    {
                        long realId = m_LanguageMap[translation.LanguageId];
                        if (translations.ContainsKey((int)realId))
                        {
                            if (String.IsNullOrEmpty(translation.Name))
                            {
                                m_LogStream.WriteLine(String.Format("WARNING: Ignoring empty name for {2} {0} translation into language {1}", id, translation.LanguageId, typeNameForLog));
                                continue;
                            }
                            String oldName = translations[(int)realId];
                            if (oldName.Equals(translation.Name, StringComparison.OrdinalIgnoreCase))
                            {
                                m_LogStream.WriteLine(String.Format("Translation '{0}' for {2} {1} already exists", oldName, id, typeNameForLog));
                                continue;
                            }
                            else
                            {
                                m_LogStream.WriteLine(String.Format("Changing {4} translation for language {0} into language {1} from '{2}' to '{3}'",
                                    id, realId, oldName, translation.Name, typeNameForLog));
                            }
                        }
                        else
                        {
                            m_LogStream.WriteLine(String.Format("Adding {3} translation '{0}' for language {1} into language {2}", translation.Name, id, realId, typeNameForLog));
                        }
                        TagsTranslations.DoSetTranslation(m_Connection, tableName, refColumnName, langColumnName, id, realId, translation.Name, m_Transaction);
                    }
                    else
                    {
                        m_LogStream.WriteLine(String.Format("WARNING: Language id {0} for {2} translation '{1}' not found", translation.LanguageId, translation.Name, typeNameForLog));
                    }
                }
            }
        }

        #endregion

    }
}