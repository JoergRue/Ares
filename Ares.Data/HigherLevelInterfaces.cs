﻿/*
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

    /// <summary>
    /// Tag retrieval modes.
    /// </summary>
    public enum TagCategoryCombination
    {
        /// <summary>
        /// Or: music must have at least one of the given tag.
        /// Use GetAllFilesWithAnyTag for retrieval.
        /// </summary>
        UseAnyTag = 0,
        /// <summary>
        /// Category-And: music must have at least one of the given 
        /// tags in each given category. Use GetAllFilesWithAnyTagInEachCategory
        /// for retrieval.
        /// </summary>
        UseOneTagOfEachCategory = 1,
        /// <summary>
        /// And: music must have all given tags.
        /// Use GetAllFilesWithAllTags for retrieval.
        /// </summary>
        UseAllTags = 2
    }

    /// <summary>
    /// Set to play music with a specific selection of tags.
    /// </summary>
    public interface IMusicByTags : IElement
    {
        /// <summary>
        /// The selected tags.
        /// </summary>
        IDictionary<int, HashSet<int>> GetTags();

        /// <summary>
        /// The selected tags.
        /// </summary>
        HashSet<int> GetAllTags();

        /// <summary>
        /// Adds a tag
        /// </summary>
        void AddTag(int categoryId, int tagId);

        /// <summary>
        /// Removes a tag
        /// </summary>
        void RemoveTag(int categoryId, int tagId);

        /// <summary>
        /// Whether the categories are put together 
        /// using 'and' or 'or'
        /// </summary>
        TagCategoryCombination TagCategoryCombination { get; set; }

        /// <summary>
        /// Fade time for each music file
        /// </summary>
        int FadeTime { get; set; }
    }

    /// <summary>
    /// Light Effects to be sent via tpm2.net
    /// </summary>
    public interface ILightEffects : IElement
    {
        /// <summary>
        /// Whether to set Master Brightness value in the tpm2.net command
        /// </summary>
        bool SetsMasterBrightness { get; set; }
        /// <summary>
        /// Master Brightness value of this element
        /// </summary>
        int MasterBrightness { get; set; }

        /// <summary>
        /// Whether to change left/right mix in the tpm2.net command
        /// </summary>
        bool SetsLeftRightMix { get; set; }
        /// <summary>
        /// Left/Right mix value of this element
        /// </summary>
        int LeftRightMix { get; set; }

        /// <summary>
        /// Whether to change left scene in the tpm2.net command
        /// </summary>
        bool SetsLeftScene { get; set; }
        /// <summary>
        /// Left Scene numeric value of this element
        /// </summary>
        int LeftScene { get; set; }

        /// <summary>
        /// Whether to change right scene in the tpm2.net command
        /// </summary>
        bool SetsRightScene { get; set; }
        /// <summary>
        /// Right Scene numeric value of this element
        /// </summary>
        int RightScene { get; set; }
    }

}
