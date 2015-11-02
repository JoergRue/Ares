/*
 Copyright (c) 2015 [Joerg Ruedenauer]
 
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

namespace Ares.Data
{
    /// <summary>
    /// An object which can be serialized to an XML stream.
    /// </summary>
    public interface IXmlWritable
    {
        /// <summary>
        /// Title of the object.
        /// </summary>
        String Title { get; set; }

        /// <summary>
        /// Serializes to an xml stream.
        /// </summary>
        /// <param name="writer">Writer for the xml</param>
        void WriteToXml(System.Xml.XmlWriter writer);
    }

    /// <summary>
    /// A mode in the project.
    /// </summary>
    public interface IMode : IXmlWritable
    {
        /// <summary>
        /// Key which switches to the mode.
        /// </summary>
        Int32 KeyCode { get; set; }

        /// <summary>
        /// Adds a top-level element to the mode.
        /// </summary>
        void AddElement(IModeElement element);

        /// <summary>
        /// Inserts a top-level elment to the node at the specified index
        /// </summary>
        void InsertElement(int index, IModeElement element);

        /// <summary>
        /// Removes an element from the mode.
        /// </summary>
        void RemoveElement(IModeElement element);

        /// <summary>
        /// Returns all elements in the mode.
        /// </summary>
        IList<IModeElement> GetElements();

        /// <summary>
        /// Returns whether a certain key triggers an element in this mode.
        /// </summary>
        bool ContainsKeyTrigger(Int32 keyCode);

        /// <summary>
        /// Returns the element which is triggered by a certain key.
        /// </summary>
        IModeElement GetTriggeredElement(Int32 keyCode);
    }

    /// <summary>
    /// Represents an ARES project.
    /// </summary>
    public interface IProject : IXmlWritable
    {
        /// <summary>
        /// File where the project is stored.
        /// </summary>
        String FileName { get; set; }

        /// <summary>
        /// Language Id for tags.
        /// </summary>
        int TagLanguageId { get; set; }

        /// <summary>
        /// Whether the project has unsaved changes.
        /// </summary>
        bool Changed { get; set;  }

        /// <summary>
        /// Adds a mode to the project.
        /// </summary>
        IMode AddMode(String title);

        /// <summary>
        /// Adds an existing mode to the project at the specified index
        /// </summary>
        void InsertMode(int index, IMode mode);

        /// <summary>
        /// Removes a mode from the project.
        /// </summary>
        void RemoveMode(IMode mode);

        /// <summary>
        /// Returns all modes in the project.
        /// </summary>
        IList<IMode> GetModes();

        /// <summary>
        /// Returns whether the project contains a mode for a certain key.
        /// </summary>
        bool ContainsKeyMode(Int32 keyCode);

        /// <summary>
        /// Returns the mode for a certain key.
        /// </summary>
        IMode GetMode(Int32 keyCode);

        /// <summary>
        /// Sets whether a music tag category shall be shown in 
        /// player / controllers.
        /// </summary>
        void SetTagCategoryHidden(int categoryId, bool isHidden);
        /// <summary>
        /// Sets whether a music tag shall be shown in 
        /// player / controllers.
        /// </summary>
        void SetTagHidden(int tagId, bool isHidden);

        /// <summary>
        /// Returns the music tag categories which shall be hidden in 
        /// player / controllers.
        /// </summary>
        HashSet<int> GetHiddenTagCategories();
        /// <summary>
        /// Returns the music tags which shall be hidden in 
        /// player / controllers.
        /// </summary>
        HashSet<int> GetHiddenTags();
    }
}
