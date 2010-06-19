using System;
using System.Collections.Generic;

namespace Ares.Data
{

    /// <summary>
    /// A mode in the project.
    /// </summary>
    public interface IMode
    {
        /// <summary>
        /// Title of the mode.
        /// </summary>
        String Title { get; set; }

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

        /// <summary>
        /// Serializes to an xml stream.
        /// </summary>
        /// <param name="writer">Writer for the xml</param>
        void WriteToXml(System.Xml.XmlWriter writer);
    }

    /// <summary>
    /// Represents an ARES project.
    /// </summary>
    public interface IProject
    {
        /// <summary>
        /// Title of the project.
        /// </summary>
        String Title { get; set; }

        /// <summary>
        /// File where the project is stored.
        /// </summary>
        String FileName { get; set; }

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
        /// Serializes to an xml stream.
        /// </summary>
        /// <param name="writer">Writer for the xml</param>
        void WriteToXml(System.Xml.XmlWriter writer);
    }
}
