using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Data
{
    /// <summary>
    /// An element which contains several other elements.
    /// </summary>
    public interface ICompositeElement
    {
        /// <summary>
        /// Returns whether the element will finish playing
        /// without an external trigger or whether it will
        /// play forever.
        /// </summary>
        bool IsEndless();
    }

    /// <summary>
    /// A top-level element inside a mode which is triggered 
    /// externally.
    /// </summary>
    public interface IModeElement : ICompositeElement
    {
        /// <summary>
        /// The trigger for the element.
        /// </summary>
        ITrigger Trigger { get; set; }
    }

    /// <summary>
    /// A randomized background music list. 
    /// </summary>
    /// <remarks>
    /// The list may be repeatable, in which
    /// case, one song after the other is played, which is the normal mode. It may
    /// start delayed, but then the delay is only for the first song, not between the
    /// songs. The delay between the songs can be set through the IRepeatableElement 
    /// interface methods.
    /// </remarks>
    public interface IRandomBackgroundMusicList : IElement, IRepeatableElement,
        IDelayableElement, IElementContainer<IChoiceElement>, IModeElement
    {
    }

    /// <summary>
    /// A non-randomized background list.
    /// </summary>
    /// <remarks>
    /// The list may be repeatable, but that is not the default. Each element in the 
    /// list can be delayed, which means an interval between the songs.
    /// </remarks>
    public interface ISequentialBackgroundMusicList : IElement, IRepeatableElement,
        IElementContainer<ISequentialElement>, IModeElement
    {
    }

    /// <summary>
    /// Element inside a background sound library. Each element, in addition to 
    /// its properties of being a parallel element, can contain a choice of 
    /// other elements.
    /// </summary>
    public interface IBackgroundSoundChoice :  IElementContainer<IChoiceElement>, IParallelElement
    {
    }

    /// <summary>
    /// A background sounds library.
    /// </summary>
    /// <remarks>
    /// The background sounds library contains a list of parallel elements, and
    /// each of these in turn contains a list of choice elements.
    /// </remarks>
    public interface IBackgroundSounds : IElement, IModeElement
    {
        /// <summary>
        /// Adds an sound choice to the library.
        /// </summary>
        IBackgroundSoundChoice AddElement(IElement element);

        /// <summary>
        /// Removes a sound choice from the library.
        /// </summary>
        void RemoveElement(int ID);

        /// <summary>
        /// Returns all sound choices in the library.
        /// </summary>
        IList<IBackgroundSoundChoice> GetElements();
    }
}
