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
    /// Visitor pattern on elements.
    /// </summary>
    public interface IElementVisitor
    {
        /// <summary>
        /// Visit a file element.
        /// </summary>
        void VisitFileElement(IFileElement fileElement);
        /// <summary>
        /// Visit a sequential container.
        /// </summary>
        void VisitSequentialContainer(ISequentialContainer sequentialContainer);
        /// <summary>
        /// Visit a parallel container.
        /// </summary>
        void VisitParallelContainer(IElementContainer<IParallelElement> parallelContainer);
        /// <summary>
        /// Visit a choice container.
        /// </summary>
        void VisitChoiceContainer(IElementContainer<IChoiceElement> choiceContainer);
        /// <summary>
        /// Visits a sequential music list
        /// </summary>
        void VisitSequentialMusicList(ISequentialBackgroundMusicList musicList);
        /// <summary>
        /// Visits a randomized music list
        /// </summary>
        void VisitRandomMusicList(IRandomBackgroundMusicList musicList);
        /// <summary>
        /// Visits a macro.
        /// </summary>
        void VisitMacro(IMacro macro);
        /// <summary>
        /// Visits a macro command.
        /// </summary>
        void VisitMacroCommand(IMacroCommand macroCommand);
        /// <summary>
        /// Visits a reference to an element.
        /// </summary>
        void VisitReference(IReferenceElement reference);
        /// <summary>
        /// Visits a music by tags element.
        /// </summary>
        void VisitMusicByTags(IMusicByTags musicByTags);
    }

    /// <summary>
    /// Basic interface for an audio element.
    /// </summary>
    public interface IElement : IXmlWritable
    {
        /// <summary>
        /// Unique ID with which the element can be referenced.
        /// </summary>
        int Id { get; set;  }

        /// <summary>
        /// Whether the start of the element changes the global music volume
        /// </summary>
        bool SetsMusicVolume { get; set; }

        /// <summary>
        /// The global music volume to set
        /// </summary>
        int MusicVolume { get; set; }

        /// <summary>
        /// Whether the start of the element changes the global sounds volume
        /// </summary>
        bool SetsSoundVolume { get; set; }

        /// <summary>
        /// The global sounds volume to set
        /// </summary>
        int SoundVolume { get; set; }

        /// <summary>
        /// Clones the element; makes a shallow copy for container elements.
        /// </summary>
        IElement Clone();

        /// <summary>
        /// Visitor pattern.
        /// </summary>
        void Visit(IElementVisitor visitor);
    }

    /// <summary>
    /// Interface for a music list.
    /// </summary>
    public interface IMusicList
    {
        /// <summary>
        /// Visitor pattern; put through to the elements
        /// </summary>
        void VisitElements(IElementVisitor visitor);

        /// <summary>
        /// Gets all files in the music list.
        /// </summary>
        IList<IFileElement> GetFileElements();
    }

    /// <summary>
    /// Type of a sound file: music or effect
    /// </summary>
    public enum SoundFileType
    {
        /// <summary>
        /// Music, longer file
        /// </summary>
        Music,
        /// <summary>
        /// Sound effect, shorter file
        /// </summary>
        SoundEffect
    }

    /// <summary>
    /// Base interface for a special effect
    /// </summary>
    public interface IEffect
    {
        /// <summary>
        /// Is the effect activated?
        /// </summary>
        bool Active { get; set; }
        /// <summary>
        /// Has the effect random settings?
        /// </summary>
        bool Random { get; set; }
    }

    /// <summary>
    /// Interface for an effect based on an integer
    /// </summary>
    public interface IIntEffect : IEffect
    {
        /// <summary>
        /// Value if the value is fix.
        /// </summary>
        int FixValue { get; set; }

        /// <summary>
        /// Minimum random value
        /// </summary>
        int MinRandomValue { get; set; }

        /// <summary>
        /// Maximum random value
        /// </summary>
        int MaxRandomValue { get; set; }
    }

    /// <summary>
    /// Speaker assignment enumeration.
    /// </summary>
    public enum SpeakerAssignment
    {
        Default = 0,
        LeftFront,
        RightFront,
        Center,
        LeftBack,
        RightBack,
        LeftCenterBack,
        RightCenterBack,
        Subwoofer,
        BothFronts,
        BothRears,
        BothCenterRears,
        CenterAndSubwoofer
    }

    /// <summary>
    /// Speaker assignment effect.
    /// </summary>
    public interface ISpeakerAssignmentEffect : IEffect
    {
        /// <summary>
        /// The assignment.
        /// </summary>
        SpeakerAssignment Assignment { get; set; }
    }

    public interface IBalanceEffect : IIntEffect
    {
        /// <summary>
        /// Whether the balance is being panned
        /// </summary>
        bool IsPanning { get; set; }

        /// <summary>
        /// Start value for panning
        /// </summary>
        int PanningStart { get; set; }

        /// <summary>
        /// End value for panning
        /// </summary>
        int PanningEnd { get; set; }
    }

    public interface IReverbEffect : IEffect
    {
        /// <summary>
        /// Delay of the reverb
        /// </summary>
        int Delay { get; set; }

        /// <summary>
        /// Level (volume) of the reverb
        /// </summary>
        int Level { get; set; }
    }

    /// <summary>
    /// Optional effects when playing.
    /// </summary>
    public interface IEffects
    {
        /// <summary>
        /// Relative volume, in percent.
        /// </summary>
        int Volume { get; set; }

        /// <summary>
        /// Time for fade-in, in milliseconds.
        /// </summary>
        int FadeInTime { get; set; }

        /// <summary>
        /// Time for fade-out, in milliseconds.
        /// </summary>
        int FadeOutTime { get; set; }

        /// <summary>
        /// Whether the volume is determined randomly.
        /// </summary>
        bool HasRandomVolume { get; set; }

        /// <summary>
        /// Minimum random volume.
        /// </summary>
        int MinRandomVolume { get; set; }

        /// <summary>
        /// Maximum random volume.
        /// </summary>
        int MaxRandomVolume { get; set; }

        /// <summary>
        /// Pitch effect
        /// </summary>
        IIntEffect Pitch { get; }

        /// <summary>
        /// Balance effect. Mutually exclusive with Speaker Assignment.
        /// </summary>
        IBalanceEffect Balance { get; }

        /// <summary>
        /// Volume in Dezibel
        /// </summary>
        IIntEffect VolumeDB { get; }

        /// <summary>
        /// Speaker assignment. Mutally exclusive with Balance.
        /// </summary>
        ISpeakerAssignmentEffect SpeakerAssignment { get; }

        /// <summary>
        /// Reverb effect
        /// </summary>
        IReverbEffect Reverb { get; }

        /// <summary>
        /// Tempo effect
        /// </summary>
        IIntEffect Tempo { get; }
    }

    /// <summary>
    /// An element consisting of a physical file.
    /// </summary>
    public interface IFileElement : IElement
    {
        /// <summary>
        /// The name of the file.
        /// </summary>
        String FileName { get; }
        /// <summary>
        /// The path of the file, relative to the Music / Sound directory.
        /// </summary>
        String FilePath { get; set; }
        /// <summary>
        /// Type of the file
        /// </summary>
        SoundFileType SoundFileType { get; set; }
        /// <summary>
        /// Effects during playing
        /// </summary>
        IEffects Effects { get; }
    }

    /// <summary>
    /// An element which can be given a random chance to play.
    /// </summary>
    public interface IRandomizedElement : IElement
    {
        /// <summary>
        /// The chance -- between 0 and 100, in percent.
        /// </summary>
        int RandomChance { get; set; }
    }

    /// <summary>
    /// An element whose start can be delayed.
    /// </summary>
    public interface IDelayableElement : IElement
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
    public interface IRepeatableElement : IElement
    {
        /// <summary>
        /// How many times the element shall be played. -1 for infinite. 0 is not allowed.
        /// </summary>
        int RepeatCount { get; set; }
        /// <summary>
        /// Fixed delay before repeating the element.
        /// </summary>
        TimeSpan FixedIntermediateDelay { get; set; }
        /// <summary>
        /// Maximum additional time before repeating the element.
        /// </summary>
        /// <remarks>
        /// When playing, the repetition will be delayed by a random time between
        /// the fixed intermediate delay and the maximum random delay.
        /// </remarks>
        TimeSpan MaximumRandomIntermediateDelay { get; set;  }
    }

    /// <summary>
    /// Tagging interface for the 'special' interface for elements in a container.
    /// </summary>
    public interface IContainerElement : IElement
    {
        /// <summary>
        /// The inner element wrapped in the delayable element
        /// </summary>
        IElement InnerElement { get; }
    }

    /// <summary>
    /// Base interface for an element in a parallel container. 
    /// Parallel elements can be delayed and repeated.
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
    /// Base interface for a container.
    /// </summary>
    public interface IGeneralElementContainer : IElement
    {
        /// <summary>
        /// Returns a shallow copy of the elements in the container
        /// </summary>
        IList<IContainerElement> GetGeneralElements();

        /// <summary>
        /// Adds a new element to the container.
        /// </summary>
        IElement AddGeneralElement(IElement element);

        /// <summary>
        /// Adds an imported element to the container.
        /// </summary>
        IList<IElement> AddGeneralImportedElement(IXmlWritable element);

        /// <summary>
        /// Inserts an existing element into the container.
        /// </summary>
        void InsertGeneralElement(int index, IElement element);

        /// <summary>
        /// Removes an element from the container. Has no effect if the element is not in the container.
        /// </summary>
        void RemoveElement(int id);

        /// <summary>
        /// Returns all file elements in the container or in sub-containers of the container.
        /// </summary>
        IList<IFileElement> GetFileElements();
    }

    /// <summary>
    /// Interface for a container.
    /// A container for choice elements could e.g. be a 'playlist'.
    /// A container for parallel elements could e.g. be a background for fighting.
    /// A container for sequential elements could e.g. be a special effect.
    /// </summary>
    public interface IElementContainer<T> : IGeneralElementContainer where T : IContainerElement
    {
        /// <summary>
        /// Adds an element to the container.
        /// </summary>
        T AddElement(IElement element);

        /// <summary>
        /// Returns a shallow copy of the elements in the container.
        /// </summary>
        IList<T> GetElements();

        /// <summary>
        /// Returns the element with the specified Id.
        /// </summary>
        T GetElement(int id);
    }


    /// <summary>
    /// Special container which can reorder elements.
    /// </summary>
    public interface IReorderableContainer<T> : IElementContainer<T> where T : IContainerElement
    {
        void MoveElements(int startIndex, int endIndex, int offset);
    }

    /// <summary>
    /// Special interface for a sequential container
    /// </summary>
    public interface ISequentialContainer : IReorderableContainer<ISequentialElement>
    {
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
        ElementFinished,
        /// <summary>
        /// No trigger defined (yet)
        /// </summary>
        None
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
        Int32 TargetElementId { get; set; }

        /// <summary>
        /// Shall sound be stopped before triggering the target?
        /// </summary>
        bool StopSounds { get; set; }

        /// <summary>
        /// Shall music be stopped before triggering the target?
        /// </summary>
        bool StopMusic { get; set; }

        /// <summary>
        /// If there shall be cross-fading for old music.
        /// </summary>
        bool CrossFadeMusic { get; set; }

        /// <summary>
        /// If there shall be fading for old music.
        /// </summary>
        bool FadeMusic { get; set; }

        /// <summary>
        /// Cross fading time for music, in milliseconds.
        /// </summary>
        Int32 FadeMusicTime { get; set; }

        /// <summary>
        /// Serializes to an xml stream.
        /// </summary>
        /// <param name="writer">Writer for the xml</param>
        void WriteToXml(System.Xml.XmlWriter writer);
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
        Int32 ElementId { get; set; }
    }

}
