using System;
using System.Collections.Generic;

namespace Ares.Data
{
    /// <summary>
    /// Basic interface for an audio element.
    /// </summary>
    public interface IElement
    {
        /// <summary>
        /// Unique ID with which the element can be referenced.
        /// </summary>
        int ID { get; }
        /// <summary>
        /// Title of the element.
        /// </summary>
        String Title { get; set; }

        /// <summary>
        /// Clones the element; makes a shallow copy for container elements.
        /// </summary>
        IElement Clone();
    }

    /// <summary>
    /// An element consisting of a physical file.
    /// </summary>
    public interface IFileElement
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        String FileName { get; }
        /// <summary>
        /// The full path of the file.
        /// </summary>
        String FilePath { get; set; }
    }

    /// <summary>
    /// An element which can be given a random chance to play.
    /// </summary>
    public interface IRandomizedElement
    {
        /// <summary>
        /// The chance -- between 0 and 100, in percent.
        /// </summary>
        int RandomChance { get; set; }
    }

    /// <summary>
    /// An element whose start can be delayed.
    /// </summary>
    public interface IDelayableElement
    {
        /// <summary>
        /// Fixed time to delay the element.
        /// </summary>
        TimeSpan FixedStartDelay { get; set; }

        /// <summary>
        /// Maximum additional time to delay the element.
        /// </summary>
        /// <remarks>
        /// When playing, the element will be delayed by a random time between
        /// the minimum random delay and the maximum random delay.
        /// </remarks>
        TimeSpan MaximumRandomStartDelay { get; set;  }
    }

    /// <summary>
    /// An element which can be repeated automatically.
    /// </summary>
    public interface IRepeatableElement
    {
        /// <summary>
        /// Whether the element shall be repeated.
        /// </summary>
        bool Repeat { get; set; }
        /// <summary>
        /// Fixed delay before repeating the element.
        /// </summary>
        TimeSpan FixedIntermediateDelay { get; set; }
        /// <summary>
        /// Maximum additional time before repeating the element.
        /// </summary>
        /// <remarks>
        /// When playing, the repetition will be delayed by a random time between
        /// the minimum random delay and the maximum random delay.
        /// </remarks>
        TimeSpan MaximumRandomIntermediateDelay { get; set;  }
    }

    /// <summary>
    /// Tagging interface for the 'special' interface for elements in a container.
    /// </summary>
    public interface IContainerElement : IElement
    {
    }

    /// <summary>
    /// Base interface for an element in a parallel container. 
    /// Parallel elements can be randomized, delayed, and repeated.
    /// </summary>
    public interface IParallelElement : IContainerElement, IDelayableElement, IRepeatableElement
    {
    }

    /// <summary>
    /// Base interface for an element in a choice container, e.g. a music playlist.
    /// Choice elements can be randomized, but not delayed or repeated.
    /// </summary>
    public interface IChoiceElement : IContainerElement, IRandomizedElement
    {
    }

    /// <summary>
    /// Base interface for an element in a sequential container.
    /// Sequential elements can be delayed, but not randomized or repeated.
    /// </summary>
    public interface ISequentialElement : IContainerElement, IDelayableElement
    {
    }

    /// <summary>
    /// Interface for a container.
    /// A container for choice elements could e.g. be a 'playlist'.
    /// A container for parallel elements could e.g. be a background for fighting.
    /// A container for sequential elements could e.g. be a special effect.
    /// </summary>
    public interface IElementContainer<T> : IElement where T : IContainerElement
    {
        /// <summary>
        /// Adds an element to the container.
        /// </summary>
        T AddElement(IElement element);
        /// <summary>
        /// Removes an element from the container. Has no effect if the element is not in the container.
        /// </summary>
        void RemoveElement(int ID);

        /// <summary>
        /// Returns a shallow copy of the elements in the container.
        /// </summary>
        IList<T> GetElements();
    }

    /// <summary>
    /// Type of triggers which can start the playback of an element.
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// Triggered by a key press.
        /// </summary>
        Key,
        /// <summary>
        /// Triggered by an another element which has finished playing.
        /// </summary>
        ElementFinished
    }

    /// <summary>
    /// A trigger for a playback of an element
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// The type of the trigger
        /// </summary>
        TriggerType TriggerType { get; }

        /// <summary>
        /// The element which is triggered
        /// </summary>
        IElement TargetElement { get; set; }
    }

    /// <summary>
    /// A trigger that reacts on a key press
    /// </summary>
    public interface IKeyTrigger : ITrigger
    {
        /// <summary>
        /// Code of the key which triggers
        /// </summary>
        Int32 KeyCode { get; set; }
    }

    /// <summary>
    /// A trigger that reacts on the finish of an element
    /// </summary>
    public interface IElementFinishTrigger : ITrigger
    {
        /// <summary>
        /// ID of the element which triggers
        /// </summary>
        Int32 ElementID { get; set; }
    }

}
