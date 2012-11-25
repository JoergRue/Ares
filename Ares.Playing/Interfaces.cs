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

namespace Ares.Playing
{
    /// <summary>
    /// Type of a file: music or sound
    /// </summary>
    public enum SoundFileType
    {
        Music,
        SoundEffect
    }

    /// <summary>
    /// One sound file.
    /// </summary>
    public interface ISoundFile
    {
        int Id { get; }
        /// <summary>
        /// Full path
        /// </summary>
        String Path { get; }
        SoundFileType SoundFileType { get; }
        int Volume { get; }
        Ares.Data.IEffects Effects { get; }
    }

    /// <summary>
    /// Delegate called when a file has finished playing, either because 
    /// it was stopped or because it has ended.
    /// </summary>
    /// <remarks>
    /// All callbacks may be called on a different thread.
    /// </remarks>
    public delegate void PlayingFinished(int Id, int handle);

    /// <summary>
    /// Player for a single file. The interface is only for testing
    /// purposes in the Ares project.
    /// </summary>
    public interface IFilePlayer : IDisposable
    {
        int PlayFile(ISoundFile file, int fadeInTime, PlayingFinished callback, bool loop);
        void StopFile(int handle);
        void StopFile(int handle, bool fadeOut, int fadeOutTime);
    }

    /// <summary>
    /// Base interface for callbacks during playing.
    /// </summary>
    /// <remarks>
    /// All callbacks may be called on a different thread.
    /// </remarks>
    public interface IPlayingCallbacks
    {
        /// <summary>
        /// An error occurred while trying to play an element.
        /// </summary>
        void ErrorOccurred(int elementId, String errorMessage);
    }

    /// <summary>
    /// Callbacks for a project player.
    /// </summary>
    /// <remarks>
    /// All callbacks may be called on a different thread.
    /// </remarks>
    public interface IProjectPlayingCallbacks : IPlayingCallbacks
    {
        /// <summary>
        /// A new mode was activated.
        /// </summary>
        void ModeChanged(Ares.Data.IMode newMode);

        /// <summary>
        /// A new mode element was started.
        /// </summary>
        void ModeElementStarted(Ares.Data.IModeElement element);
        /// <summary>
        /// A mode element was stopped or has finished playing.
        /// </summary>
        void ModeElementFinished(Ares.Data.IModeElement element);

        /// <summary>
        /// A new sound effect was started.
        /// </summary>
        void SoundStarted(Int32 elementId);
        /// <summary>
        /// A sound effect was stopped or has finished playing.
        /// </summary>
        void SoundFinished(Int32 elementId);

        /// <summary>
        /// A music file was started.
        /// </summary>
        void MusicStarted(Int32 elementId);
        /// <summary>
        /// A music file was stopped or has finished playing.
        /// </summary>
        void MusicFinished(Int32 elementId);

        /// <summary>
        /// Volume changed because of an element start.
        /// </summary>
        /// <param name="target">volume which changed</param>
        /// <param name="newValue">new volume value</param>
        void VolumeChanged(VolumeTarget target, int newValue);

        /// <summary>
        /// A music playlist was started.
        /// </summary>
        void MusicPlaylistStarted(Int32 elementId);

        /// <summary>
        /// The current music playlist was stopped.
        /// </summary>
        void MusicPlaylistFinished();

        /// <summary>
        /// Repeat mode for music was changed.
        /// </summary>
        void MusicRepeatChanged(bool repeat);
    }

    /// <summary>
    /// Callbacks for an element player.
    /// </summary>
    /// <remarks>
    /// All callbacks may be called on a different thread.
    /// </remarks>
    public interface IElementPlayingCallbacks : IPlayingCallbacks
    {
        /// <summary>
        /// An element was stopped or has finished playing.
        /// </summary>
        void ElementFinished(Int32 elementId);
    }

    /// <summary>
    /// Global Volumes which can be set.
    /// </summary>
    public enum VolumeTarget
    {
        /// <summary>
        /// Sound effects.
        /// </summary>
        Sounds = 0,
        /// <summary>
        /// Music files.
        /// </summary>
        Music = 1,
        /// <summary>
        /// Overall volume.
        /// </summary>
        Both = 2
    }

    /// <summary>
    /// Basic interface for the player.
    /// </summary>
    public interface IPlayer
    {
        /// <summary>
        /// Stop all sound and music.
        /// </summary>
        void StopAll();

        /// <summary>
        /// Sets the base directory for music files.
        /// </summary>
        void SetMusicPath(String path);
        /// <summary>
        /// Sets the base directory for sound files.
        /// </summary>
        void SetSoundPath(String path);

        /// <summary>
        /// Sets a volume.
        /// </summary>
        void SetVolume(VolumeTarget target, Int32 value);
    }

    /// <summary>
    /// Interface for a player which plays whole projects.
    /// This is used by the Ares Player application.
    /// </summary>
    public interface IProjectPlayer : IPlayer
    {
        /// <summary>
        /// Sets the project.
        /// </summary>
        void SetProject(Ares.Data.IProject project);

        /// <summary>
        /// Tells the player that a key has been received.
        /// </summary>
        /// <returns>whether the key was handled</returns>
        /// <remarks>
        /// Only mode changes and mode element changes are
        /// handled here. Volume / full stop / next / previous
        /// must be handled by the client.
        /// </remarks>
        bool KeyReceived(Int32 keyCode);

        /// <summary>
        /// Tells the player to start / stop a specific element.
        /// </summary>
        /// <param name="elementID">ID of the element.</param>
        /// <returns>whether switch was successful</returns>
        /// <remarks>
        /// Only top-level elements of a mode can be switched.
        /// </remarks>
        bool SwitchElement(Int32 elementID);

        /// <summary>
        /// Tells the player to switch to a specific mode.
        /// </summary>
        /// <param name="modeTitle">title of the mode</param>
        /// <returns>whether switch was successful</returns>
        bool SetMode(String modeTitle);

        /// <summary>
        /// Plays the next music title.
        /// </summary>
        void NextMusicTitle();
        /// <summary>
        /// Plays the previous music title again.
        /// </summary>
        void PreviousMusicTitle();
        /// <summary>
        /// Plays a defined music title from the current list.
        /// No effect if the title isn't found in the list, since that 
        /// could happen by accident (request sent, but music list already finished).
        /// </summary>
        void SetMusicTitle(Int32 elementId);

        /// <summary>
        /// Whether the current music title shall be repeated endlessly.
        /// Default is false.
        /// </summary>
        bool RepeatCurrentMusic { get; set; }
    }

    /// <summary>
    /// Player which plays independent elements. 
    /// This is used by the Ares.Editor application.
    /// </summary>
    public interface IElementPlayer : IPlayer
    {
        /// <summary>
        /// Plays an element.
        /// </summary>
        void PlayElement(Ares.Data.IElement element);
        /// <summary>
        /// Stops playing an element.
        /// </summary>
        void StopElement(Ares.Data.IElement element);
    }
}
