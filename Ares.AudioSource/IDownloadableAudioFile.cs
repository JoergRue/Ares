using Ares.AudioSource;
using Ares.Data;
using Ares.ModelInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.AudioSource
{
    /// <summary>
    /// A file provided by an audio source
    /// </summary>
    public interface IDeployableAudioFile
    {
        string Filename { get; }

        SoundFileType FileType { get; }

        IAudioSource AudioSource { get; }

        /// <summary>
        /// Deploy this object (including anything that is required, i.e. audio files required by an IModeElementSearchResult).
        /// All audio files will be placed at the given relative path beneath either the sounds or music directory - depending on their type.
        /// </summary>
        /// <returns></returns>
        AudioDeploymentResult Deploy(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider);
    }

    public interface IDownloadableAudioFile: IDeployableAudioFile
    {
        /// <summary>
        /// Size of this download.
        /// Any unit can be used for the values returned by this property, but the unit should
        /// be consistent within all Files/Results from a single IAudioSource
        /// </summary>
        double? DownloadSize { get; }

        /// <summary>
        /// Download this object (including anything that is required, i.e. audio files required by an IModeElementSearchResult).
        /// All audio files will be placed at the given relative path beneath either the sounds or music directory - depending on their type.
        /// </summary>
        /// <returns></returns>
        AudioDeploymentResult Download(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider);

        string SourceUrl { get; }
    }

    /// <summary>
    /// This interface is resposible for defining where on disk & within the ARES library a IDownloadableAudioFile should be 
    /// placed.
    /// 
    /// The actual implementation may define where within the libraries (if at all) each file will be placed.
    /// </summary>
    public interface ITargetDirectoryProvider
    {
        /// <summary>
        /// Return the relative path to the folder (within the appropriate ARES library) where the given audio file will be placed.
        /// </summary>
        /// <param name="audioFile"></param>
        /// <returns></returns>
        string GetFolderWithinLibrary(IDeployableAudioFile audioFile);

        /// <summary>
        /// Return the relative path (filename & folder within the appropriate ARES library) where the given audio file will be placed.
        /// </summary>
        /// <param name="audioFile"></param>
        /// <returns></returns>
        string GetPathWithinLibrary(IDeployableAudioFile audioFile);

        /// <summary>
        /// Return the absolute path (filename & full path on the filesystem) where the given audio file will be placed.
        /// </summary>
        /// <param name="audioFile"></param>
        /// <returns></returns>
        string GetFullPath(IDeployableAudioFile audioFile);
    }

    /// <summary>
    /// This class encapsulates information on the outcome of downloading an audio file.
    /// </summary>
    public class AudioDeploymentResult
    {
        private ResultState m_State;
        private string m_Message;
        private Exception m_Cause;

        enum ResultState
        {
            SUCCESS,
            ERROR
        }

        public static AudioDeploymentResult SUCCESS = new AudioDeploymentResult(ResultState.SUCCESS, null);
        public static AudioDeploymentResult ERROR(string message)
        {
            return new AudioDeploymentResult(ResultState.ERROR, message);
        }
        public static AudioDeploymentResult ERROR(string message, Exception cause)
        {
            return new AudioDeploymentResult(ResultState.ERROR, message, cause);
        }

        private AudioDeploymentResult(ResultState state, string message)
        {
            this.m_State = state;
            this.m_Message = message;
            this.m_Cause = null;
        }

        private AudioDeploymentResult(ResultState state, string message, Exception cause)
        {
            this.m_State = state;
            this.m_Message = message;
            this.m_Cause = cause;
        }

    }
}
