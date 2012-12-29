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

namespace Ares.Tags
{
    /// <summary>
    /// Exception thrown by all Tag Db methods if something goes wrong.
    /// </summary>
    [Serializable]
    public class TagsDbException : Exception
    {
        public TagsDbException(String message) : base(message) { }
        public TagsDbException(String message, Exception innerException) : base(message, innerException) { }

        protected TagsDbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// An item with its name in a certain language.
    /// </summary>
    public class ItemForLanguage
    {
        public int Id { get; set;  }
        public String Name { get; set;  }
    }

    public class TagForLanguage : ItemForLanguage
    {
    }

    public class CategoryForLanguage : ItemForLanguage
    {
    }

    public class LanguageForLanguage : ItemForLanguage
    {
    }

    public class TagInfoForLanguage
    {
        public int Id { get; internal set;  }
        public String Name { get; internal set;  }
        public String Category { get; internal set; }
        public int CategoryId { get; internal set; }
    }

    /// <summary>
    /// Read-interface for the tags database, independent of language,
    /// only for playing.
    /// </summary>
    public interface ITagsDBRead
    {
        /// <summary>
        /// Returns all files which have at least one of the given tags.
        /// Files are returned as relative paths.
        /// </summary>
        IList<String> GetAllFilesWithAnyTag(HashSet<int> tagIds);

        /// <summary>
        /// Returns all files which have in each given category at least one of the given tags.
        /// Files are returned as relative paths.
        /// </summary>
        /// <remarks>
        /// Note: the categories could also be retrieved from the database. But clients know them
        /// anyway.
        /// </remarks>
        IList<String> GetAllFilesWithAnyTagInEachCategory(IDictionary<int, HashSet<int>> tagsByCategory);
    }

    /// <summary>
    /// Language-dependend read interface for the tags database.
    /// The language is given once when retrieving the interface.
    /// </summary>
    public interface ITagsDBReadByLanguage
    {
        /// <summary>
        /// Gets all defined categories with their names in the given language.
        /// </summary>
        /// <remarks>
        /// If a category doesn't have a name in the language, it is not returned.
        /// </remarks>
        IList<CategoryForLanguage> GetAllCategories();

        /// <summary>
        /// Gets all defined tags in the category with their names in the given language.
        /// </summary>
        /// <remarks>
        /// If a tag doesn't have a name in the language, it is not returned.
        /// </remarks>
        IList<TagForLanguage> GetAllTags(int categoryId);

        /// <summary>
        /// Gets all tags for a specific file identified by its path.
        /// </summary>
        /// <remarks>
        /// If a tag doesn't have a name in the language, it is not returned.
        /// </remarks>
        IList<TagInfoForLanguage> GetTagsForFile(String relativePath);

        /// <summary>
        /// Gets all defined tags for the given language.
        /// </summary>
        /// <remarks>
        /// All tags are returned which have a name in the given language and
        /// whose category has a name in the given language.
        /// </remarks>
        IList<TagInfoForLanguage> GetAllTags();

        /// <summary>
        /// Gets all languages which have a name in the given language.
        /// </summary>
        IList<LanguageForLanguage> GetAllLanguages();

        /// <summary>
        /// Gets information about the given tags.
        /// </summary>
        IList<TagInfoForLanguage> GetTagInfos(ICollection<int> tagIds);
    }

    /// <summary>
    /// Write-Interface of tags database, independent of language
    /// </summary>
    public interface ITagsDBWrite
    {
        /// <summary>
        /// Sets the tags for a list of files.
        /// For each file, the list of new tags is given.
        /// Last parameter gives a function to report progress.
        /// </summary>
        void SetFileTags(IList<String> relativePaths, IList<IList<int>> tagIds);

        /// <summary>
        /// Removes tags for the given files from the database.
        /// </summary>
        void RemoveFiles(IList<String> relativePaths);

        /// <summary>
        /// Changes the path of a file.
        /// </summary>
        void MoveFile(String oldPath, String newPath);

        /// <summary>
        /// Removes the tag for all languages.
        /// </summary>
        void RemoveTag(int tagId);

        /// <summary>
        /// Remove the category and its tags for all languages.
        /// </summary>
        void RemoveCategory(int categoryId);
    }

    /// <summary>
    /// Language-dependend write interface of tags database.
    /// The language is given once when retrieving the interface.
    /// </summary>
    public interface ITagsDBWriteByLanguage
    {
        /// <summary>
        /// Adds a category with its name in the given language.
        /// </summary>
        /// <returns>The new id of the category.</returns>
        int AddCategory(String name);

        /// <summary>
        /// Adds a tag with its name in the given language.
        /// </summary>
        /// <returns>The new id of the tag.</returns>
        int AddTag(int categoryId, String name);

        /// <summary>
        /// Removes a category for the language.
        /// </summary>
        /// <remarks>
        /// Actually removes only the translation at first.
        /// If no translations remain for the category, it is 
        /// removed completely from the database.
        /// </remarks>
        void RemoveCategory(int categoryId);

        /// <summary>
        /// Removes a tag for the language.
        /// </summary>
        /// <remarks>
        /// Actually removes only the translation at first.
        /// If no translations remain for the tag, it is 
        /// removed completely from the database.
        /// </remarks>
        void RemoveTag(int tagId);

        /// <summary>
        /// Changes the name of a tag for the language.
        /// </summary>
        void SetCategoryName(int categoryId, String name);

        /// <summary>
        /// Changes the name of a tag for the language.
        /// </summary>
        void SetTagName(int tagId, String name);
    }

    public interface ITagsDBTranslations
    {
        /// <summary>
        /// Returns the language which best fits the current UI language.
        /// </summary>
        int GetIdOfCurrentUILanguage();

        /// <summary>
        /// Adds a language.
        /// </summary>
        /// <returns>The new Id of the language</returns>
        /// <remarks>The name is the name in the new language.</remarks>
        int AddLanguage(String code, String name);

        /// <summary>
        /// Adds a language.
        /// </summary>
        /// <returns>The new Id of the language</returns>
        /// <remarks>The name is given in the language with the given Id</remarks>
        int AddLanguage(String code, String name, int languageId);

        /// <summary>
        /// Changes the name of a language for a language.
        /// </summary>
        /// <param name="languageIdOfName">The language of the new name (e.g. German for the name "Franzoesisch")</param>
        /// <param name="languageIdOfLanguage">The named language (e.g. French for the name "Franzoesisch")</param>
        /// <param name="name">The new name</param>
        void SetLanguageName(int languageIdOfName, int languageIdOfLanguage, String name);

        /// <summary>
        /// Removes a translation of a language.
        /// </summary>
        /// <param name="languageIdOfName">the language of the translation which will be removed</param>
        /// <param name="languageIdOfLanguage">the translated language</param>
        void RemoveLanguageTranslation(int languageIdOfName, int languageIdOfLanguage);


        /// <summary>
        /// Removes a language and all translations into that language.
        /// </summary>
        void RemoveLanguage(int languageId);

        /// <summary>
        /// Returns whether there exists a language for a specific code.
        /// </summary>
        bool HasLanguageForCode(String code);

        /// <summary>
        /// Returns all translations for a tag.
        /// </summary>
        IDictionary<int, String> GetTagTranslations(int tagId);

        /// <summary>
        /// Returns all translations for a category.
        /// </summary>
        IDictionary<int, String> GetCategoryTranslations(int categoryId);

        /// <summary>
        /// Returns all translations for a language.
        /// </summary>
        IDictionary<int, String> GetLanguageTranslations(int languageId);
    }

    /// <summary>
    /// Interface for file management of tags database
    /// </summary>
    public interface ITagsDBFiles
    {
        /// <summary>
        /// Opens a database at the specified path,
        /// or creates a new one if none exists.
        /// </summary>
        void OpenOrCreateDatabase(String filePath);

        /// <summary>
        /// Closes the database.
        /// </summary>
        void CloseDatabase();

        /// <summary>
        /// Adds all entries of the given database
        /// to the currently opened database.
        /// </summary>
        void ImportDatabase(String filePath);

        /// <summary>
        /// Exports the part of the database relevant to the given files.
        /// </summary>
        void ExportDatabase(IList<String> filePaths, String targetFilePath);

        /// <summary>
        /// The default file name for the tags database.
        /// </summary>
        String DefaultFileName { get; }
    }

    /// <summary>
    /// Interface for the tags database.
    /// </summary>
    public interface ITagsDB
    {
        ITagsDBRead ReadInterface { get; }
        ITagsDBReadByLanguage GetReadInterfaceByLanguage(int languageId);
        ITagsDBWrite WriteInterface { get; }
        ITagsDBWriteByLanguage GetWriteInterfaceByLanguage(int languageId);
        ITagsDBTranslations TranslationsInterface { get; }
        ITagsDBFiles FilesInterface { get; }
    }

    /// <summary>
    /// Access to the tags database.
    /// </summary>
    public static class TagsModule
    {
        /// <summary>
        /// Returns the tags database.
        /// </summary>
        public static ITagsDB GetTagsDB()
        {
            if (s_TagsDB == null)
            {
                s_TagsDB = new SQLiteTagsDB();
            }
            return s_TagsDB;
        }

        private static ITagsDB s_TagsDB;
    }
}
