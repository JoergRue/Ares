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
    }

    public delegate void PlayingFinished(int Id, int handle);

    public interface IFilePlayer : IDisposable
    {
        int PlayFile(ISoundFile file, PlayingFinished callback);
        void StopFile(int handle);
    }

    public interface IPlayingCallbacks
    {
        void ModeChanged(Ares.Data.IMode newMode);

        void ModeElementStarted(Ares.Data.IModeElement element);
        void ModeElementFinished(Ares.Data.IModeElement element);

        void SoundStarted(Int32 elementId);
        void SoundFinished(Int32 elementId);

        void MusicStarted(Int32 elementId);
        void MusicFinished(Int32 elementId);
    }

    public interface IPlayer
    {
        void SetProject(Ares.Data.IProject project);

        void KeyReceived(Int32 keyCode);

        void NextMusicTitle();
        void PreviousMusicTitle();

        void StopAll();
    }
}
