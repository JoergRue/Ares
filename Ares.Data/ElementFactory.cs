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
        IFileElement CreateFileElement(String filePath, BasicFileCategory category);

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
    }

    class ElementFactory : IElementFactory
    {
        public IFileElement CreateFileElement(String filePath, BasicFileCategory category)
        {
            return new BasicFileElement(GetNextID(), filePath, category);
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

        internal ElementFactory()
        {
            // m_NextID = 0;
        }

        internal int GetNextID()
        {
            return m_NextID++;
        }

        private int m_NextID;

    }

    abstract class ElementBase : IElement
    {
        public int Id { get { return m_ID; } }

        public String Title { get; set; }

        public IElement Clone()
        {
            ElementBase clone = (ElementBase)this.MemberwiseClone();
            clone.m_ID = DataModule.TheElementFactory.GetNextID();
            return clone;
        }

        public abstract void Visit(IElementVisitor visitor);

        protected ElementBase(int ID)
        {
            m_ID = ID;
        }

        [NonSerialized]
        private int m_ID;
    }
}
