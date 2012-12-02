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
using System.Text;

namespace Ares.Data
{
    /// <summary>
    /// A top-level element inside a mode which is triggered 
    /// externally.
    /// </summary>
    public interface IModeElement : IElement
    {
        /// <summary>
        /// The trigger for the element.
        /// </summary>
        ITrigger Trigger { get; set; }

        /// <summary>
        /// Returns the first element in the mode (the one which will be 
        /// attached to the trigger).
        /// </summary>
        IElement StartElement { get; }

        /// <summary>
        /// Whether the element is currently playing
        /// </summary>
        bool IsPlaying { get; set; }

        /// <summary>
        /// Whether the element is visible in Player and Controller.
        /// </summary>
        bool IsVisibleInPlayer { get; set; }
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
        IDelayableElement, IElementContainer<IChoiceElement>, IMusicList
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
        ISequentialContainer, IMusicList
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
    public interface IBackgroundSounds : IElement, IElementContainer<IBackgroundSoundChoice>
    {
        /// <summary>
        /// Adds an sound choice to the library.
        /// </summary>
        IBackgroundSoundChoice AddElement(String title);

        /// <summary>
        /// Inserts an existing sound choice to the library.
        /// </summary>
        void InsertElement(int index, IBackgroundSoundChoice element);

        /// <summary>
        /// Adds an imported sound choice to the library.
        /// </summary>
        IBackgroundSoundChoice AddImportedElement(IXmlWritable element);
    }
}
