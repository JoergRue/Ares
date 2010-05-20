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
                XmlHelpers.ThrowException(String.Format(StringResources.ExpectedElement, "Trigger"), reader);
                return null; // to make the compiler happy
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
        }

        protected ElementBase(System.Xml.XmlReader reader)
        {
            m_ID = reader.GetIntegerAttribute("Id");
            Title = reader.GetNonEmptyAttribute("Title");
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
        }

        public abstract void WriteToXml(System.Xml.XmlWriter writer);
    }
}
