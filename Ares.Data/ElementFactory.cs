/*
 Copyright (c) 2010 [Joerg Ruedenauer]
 
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
    /// Interface for the factory which creates the elements.
    /// </summary>
    public interface IElementFactory
    {
        /// <summary>
        /// Creates a file element.
        /// </summary>
        IFileElement CreateFileElement(String filePath, SoundFileType fileType);

        /// <summary>
        /// Creates a choice container
        /// </summary>
        IElementContainer<IChoiceElement> CreateChoiceContainer(String title);

        /// <summary>
        /// Creates a parallel container
        /// </summary>
        IElementContainer<IParallelElement> CreateParallelContainer(String title);

        /// <summary>
        /// Creates a sequential container
        /// </summary>
        ISequentialContainer CreateSequentialContainer(String title);

        /// <summary>
        /// Creates a key trigger
        /// </summary>
        IKeyTrigger CreateKeyTrigger();

        /// <summary>
        /// Creates an element finish trigger
        /// </summary>
        IElementFinishTrigger CreateElementFinishTrigger();

        /// <summary>
        /// Create a trigger which isn't completely defined (yet)
        /// </summary>
        ITrigger CreateNoTrigger();

        /// <summary>
        /// Crates a random background music list
        /// </summary>
        IRandomBackgroundMusicList CreateRandomBackgroundMusicList(String title);

        /// <summary>
        /// Creates a sequential background music list
        /// </summary>
        ISequentialBackgroundMusicList CreateSequentialBackgroundMusicList(String title);

        /// <summary>
        /// Creates a background sound library.
        /// </summary>
        IBackgroundSounds CreateBackgroundSounds(String title);

        /// <summary>
        /// Creates a mode.
        /// </summary>
        IModeElement CreateModeElement(String title, IElement firstElement);

        /// <summary>
        /// Creates a macro
        /// </summary>
        IMacro CreateMacro(String title);

        /*
        /// <summary>
        /// Creates a reference to a container.
        /// </summary>
        /// <typeparam name="T">Type of the container.</typeparam>
        /// <typeparam name="T">Type of the container elements.</typeparam>
        /// <param name="container">The container.</param>
        /// <returns>New reference to the container.</returns>
        IContainerReference<T, U> CreateContainerReference<T, U>(T container) where T : IElementContainer<U> where U : IContainerElement;
         */
    }

    class ElementFactory : IElementFactory
    {
        public IFileElement CreateFileElement(String filePath, SoundFileType fileType)
        {
            return new BasicFileElement(GetNextID(), filePath, fileType);
        }

        public IElementContainer<IChoiceElement> CreateChoiceContainer(String title)
        {
            return new ChoiceContainer(GetNextID(), title);
        }

        public IElementContainer<IParallelElement> CreateParallelContainer(String title)
        {
            return new ParallelContainer(GetNextID(), title);
        }

        internal IElementContainer<IParallelElement> CreateParallelContainer(String title, int id)
        {
            UpdateNextID(id);
            return new ParallelContainer(id, title);
        }

        public ISequentialContainer CreateSequentialContainer(String title)
        {
            return new SequentialContainer(GetNextID(), title);
        }

        public IKeyTrigger CreateKeyTrigger()
        {
            return new KeyTrigger();
        }

        public IElementFinishTrigger CreateElementFinishTrigger()
        {
            return new ElementFinishTrigger();
        }

        public ITrigger CreateNoTrigger()
        {
            return new NoTrigger();
        }

        public IRandomBackgroundMusicList CreateRandomBackgroundMusicList(String title)
        {
            return new RandomBackgroundMusicList(GetNextID(), title);
        }

        public ISequentialBackgroundMusicList CreateSequentialBackgroundMusicList(String title)
        {
            return new SequentialBackgroundMusicList(GetNextID(), title);
        }

        public IBackgroundSounds CreateBackgroundSounds(String title)
        {
            return new BackgroundSounds(GetNextID(), title);
        }

        public IModeElement CreateModeElement(String title, IElement firstElement)
        {
            return new ModeElement(GetNextID(), title, firstElement);
        }

        public IMacro CreateMacro(String title)
        {
            return new Macro(GetNextID(), title);
        }

        /*
        public IContainerReference<T, U> CreateContainerReference<T, U>(T container) where T : IElementContainer<U> where U : IContainerElement
        {
            IContainerReference<T, U> result = new ContainerReference<T, U>(GetNextID(), container);
            container.AddReference(result);
            return result;
        }
         */


        internal ElementFactory()
        {
            // m_NextID = 0;
        }

        internal int GetNextID()
        {
            return m_NextID++;
        }

        internal IElementContainer<IParallelElement> CreateParallelContainer(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("ParallelContainer"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "ParallelContainer"), reader);
            }
            return new ParallelContainer(reader);
        }

        internal ISequentialContainer CreateSequentialContainer(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("SequentialContainer"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "SequentialContainer"), reader);
            }
            return new SequentialContainer(reader);
        }

        internal IElementContainer<IChoiceElement> CreateChoiceContainer(System.Xml.XmlReader reader)
        {
            if (!reader.IsStartElement("ChoiceContainer"))
            {
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "ChoiceContainer"), reader);
            }
            return new ChoiceContainer(reader);
        }

        internal ITrigger CreateTrigger(System.Xml.XmlReader reader)
        {
            if (reader.IsStartElement("KeyTrigger"))
            {
                return new KeyTrigger(reader);
            }
            else if (reader.IsStartElement("ElementFinishTrigger"))
            {
                return new ElementFinishTrigger(reader);
            }
            else if (reader.IsStartElement("NoTrigger"))
            {
                return new NoTrigger(reader);
            }
            else
            {
                return null;
            }
        }

        internal IElement CreateElement(System.Xml.XmlReader reader)
        {
            if (reader.IsStartElement("SequentialMusicList"))
            {
                return new SequentialBackgroundMusicList(reader);
            }
            else if (reader.IsStartElement("RandomMusicList"))
            {
                return new RandomBackgroundMusicList(reader);
            }
            else if (reader.IsStartElement("BackgroundSounds"))
            {
                return new BackgroundSounds(reader);
            }
            else if (reader.IsStartElement("SequentialContainer"))
            {
                return new SequentialContainer(reader);
            }
            else if (reader.IsStartElement("ChoiceContainer"))
            {
                return new ChoiceContainer(reader);
            }
            else if (reader.IsStartElement("ParallelContainer"))
            {
                return new ParallelContainer(reader);
            }
            else if (reader.IsStartElement("FileElement"))
            {
                return new BasicFileElement(reader);
            }
            else if (reader.IsStartElement("BackgroundSoundChoice"))
            {
                BackgroundSoundChoice choice =  new BackgroundSoundChoice(reader);
                if (reader.IsStartElement("ParallelElementData"))
                {
                    BackgroundSounds.AdditionalChoiceData data = BackgroundSounds.ReadAdditionalData(reader);
                    return new BackgroundSounds.ImportedChoice(choice, data);
                }
                else
                    return null;
            }
            else if (reader.IsStartElement("Parallel"))
            {
                return new ParallelElement(reader);
            }
            else if (reader.IsStartElement("Sequential"))
            {
                return new SequentialElement(reader);
            }
            else if (reader.IsStartElement("Choice"))
            {
                return new ChoiceElement(reader);
            }
            else if (reader.IsStartElement("Macro"))
            {
                return new Macro(reader);
            }
            else
            {
                IMacroCommand macroCommand = DataModule.TheMacroFactory.CreateMacroCommand(reader);
                if (macroCommand != null)
                {
                    return macroCommand;
                }
                else
                {
                    reader.ReadOuterXml();
                    return null;
                }
            }
        }

        internal void UpdateNextID(int Id)
        {
            if (m_NextID <= Id)
            {
                m_NextID = Id + 1;
            }
        }

        private int m_NextID;

    }

    [Serializable]
    abstract class ElementBase : IElement, System.Runtime.Serialization.IDeserializationCallback
    {
        public int Id { get { return m_ID; } set { m_ID = Id; } }

        public String Title { get; set; }

        public bool SetsMusicVolume { get; set; }

        public int MusicVolume { get; set; }

        public bool SetsSoundVolume { get; set; }

        public int SoundVolume { get; set; }

        public IElement Clone()
        {
            ElementBase clone = (ElementBase)this.MemberwiseClone();
            clone.m_ID = DataModule.TheElementFactory.GetNextID();
            DataModule.TheElementRepository.AddElement(ref clone.m_ID, this);
            return clone;
        }

        public abstract void Visit(IElementVisitor visitor);

        protected ElementBase(int ID)
        {
            m_ID = ID;
            DataModule.TheElementRepository.AddElement(ref m_ID, this);
            SetsMusicVolume = false;
            SetsSoundVolume = false;
            MusicVolume = 100;
            SoundVolume = 100;
        }

        protected ElementBase(System.Xml.XmlReader reader)
        {
            m_ID = reader.GetIntegerAttribute("Id");
            Title = reader.GetNonEmptyAttribute("Title");
            SetsMusicVolume = reader.GetBooleanAttributeOrDefault("SetsMusicVolume", false);
            SetsSoundVolume = reader.GetBooleanAttributeOrDefault("SetsSoundVolume", false);
            MusicVolume = reader.GetIntegerAttributeOrDefault("MusicVolume", 100);
            if (MusicVolume < 0 || MusicVolume > 100)
                XmlHelpers.ThrowException(StringResources.InvalidVolume, reader);
            SoundVolume = reader.GetIntegerAttributeOrDefault("SoundVolume", 100);
            if (SoundVolume < 0 || SoundVolume > 100)
                XmlHelpers.ThrowException(StringResources.InvalidVolume, reader);
            
            DataModule.TheElementRepository.AddElement(ref m_ID, this);
            DataModule.TheElementFactory.UpdateNextID(m_ID);
        }

        private int m_ID;

        public void OnDeserialization(object sender)
        {
            DataModule.TheElementRepository.AddElement(ref m_ID, this);
            DataModule.TheElementFactory.UpdateNextID(m_ID);
        }

        protected virtual void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("Id", Id.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("Title", Title);
            writer.WriteAttributeString("SetsMusicVolume", SetsMusicVolume ? "true" : "false");
            writer.WriteAttributeString("SetsSoundVolume", SetsSoundVolume ? "true" : "false");
            writer.WriteAttributeString("MusicVolume", MusicVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("SoundVolume", SoundVolume.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public abstract void WriteToXml(System.Xml.XmlWriter writer);

        /*
        private List<IElementReference> m_References;

        public IList<IElementReference> References
        {
            get
            {
                return m_References;
            }
        }

        public void AddReference(IElementReference reference)
        {
            if (m_References == null)
                m_References = new List<IElementReference>();
            m_References.Add(reference);
        }
         */
    }
}
