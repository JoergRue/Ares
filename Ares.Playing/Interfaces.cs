using System;

namespace Ares.Playing
{
    public enum SoundFileType
    {
        Music,
        SoundEffect
    }

    public interface ISoundFile
    {
        int Id { get; }
        String Path { get; }
        SoundFileType SoundFileType { get; }
        int Volume { get; }
    }

    public delegate void PlayingFinished(int Id, int handle);

    public interface IFilePlayer : IDisposable
    {
        int PlayFile(ISoundFile file, PlayingFinished callback);
        void StopFile(int handle);
    }

    public interface IPlayingCallbacks
    {
        // TODO: error handling
    }

    public interface IProjectPlayingCallbacks
    {
        void ModeChanged(Ares.Data.IMode newMode);

        void ModeElementStarted(Ares.Data.IModeElement element);
        void ModeElementFinished(Ares.Data.IModeElement element);

        void SoundStarted(Int32 elementId);
        void SoundFinished(Int32 elementId);

        void MusicStarted(Int32 elementId);
        void MusicFinished(Int32 elementId);
    }

    public interface IElementPlayingCallbacks
    {
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

    public interface IPlayer
    {
        void StopAll();

        void SetMusicPath(String path);
        void SetSoundPath(String path);

        /// <summary>
        /// Sets a volume.
        /// </summary>
        void SetVolume(VolumeTarget target, Int32 value);

    }

    public interface IProjectPlayer : IPlayer
    {
        void SetProject(Ares.Data.IProject project);

        void KeyReceived(Int32 keyCode);

        void NextMusicTitle();
        void PreviousMusicTitle();
    }

    public interface IElementPlayer : IPlayer
    {
        void PlayElement(Ares.Data.IElement element);
        void StopElement(Ares.Data.IElement element);
    }
}
