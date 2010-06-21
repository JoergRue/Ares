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
        IElementContainer<ISequentialElement> CreateSequentialContainer(String title);

        /// <summary>
        /// Creates a key trigger
        /// </summary>
        IKeyTrigger CreateKeyTrigger();

        /// <summary>
        /// Creates an element finish trigger
        /// </summary>
        IElementFinishTrigger CreateElementFinishTrigger();

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

        public IElementContainer<ISequentialElement> CreateSequentialContainer(String title)
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

        internal IElementContainer<ISequentialElement> CreateSequentialContainer(System.Xml.XmlReader reader)
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
            else
            {
                reader.ReadOuterXml();
                return null;
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
        public int Id { get { return m_ID; } }

        public String Title { get; set; }

        public bool SetsMusicVolume { get; set; }

        public int MusicVolume { get; set; }

        public bool SetsSoundVolume { get; set; }

        public int SoundVolume { get; set; }

        public IElement Clone()
        {
            ElementBase clone = (ElementBase)this.MemberwiseClone();
            clone.m_ID = DataModule.TheElementFactory.GetNextID();
            DataModule.TheElementRepository.AddElement(clone.m_ID, this);
            return clone;
        }

        public abstract void Visit(IElementVisitor visitor);

        protected ElementBase(int ID)
        {
            m_ID = ID;
            DataModule.TheElementRepository.AddElement(m_ID, this);
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
            
            DataModule.TheElementRepository.AddElement(m_ID, this);
            DataModule.TheElementFactory.UpdateNextID(m_ID);
        }

        private int m_ID;

        public void OnDeserialization(object sender)
        {
            DataModule.TheElementRepository.AddElement(m_ID, this);
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
    }
}
