using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ares.Data;
using Ares.ModelInfo;
using System.IO;
using System.Reflection;
using System;

namespace Ares.AudioSource.Test
{

    /// <summary>
    /// This IAudioSource provides dummy search results to test all available functionality for all types of search results.
    /// 
    /// Note that actual audio is not included, the audio source will only "download" empty files (SOUND_RESOURCE_NAME / MUSIC_RESOURCE_NAME).
    /// To include actual audio please add appropriate files (sound.mp3/music.mp3) to the project and configure them to be embedded as resources.
    /// </summary>
    public class TestAudioSource : IAudioSource
    {
        public const string SOUND_RESOURCE_NAME = "sound.mp3";
        public const string MUSIC_RESOURCE_NAME = "music.mp3";

        public Bitmap Icon { get { return ImageResources.aressmall; } }

        public string Id { get { return "test"; } }

        public string Name { get { return "Test"; } }

        public Task<IEnumerable<ISearchResult>> Search(string query, int pageSize, int pageIndex, IAbsoluteProgressMonitor monitor, out int? totalNumberOfResults)
        {
            List<ISearchResult> results = new List<ISearchResult>();

            FileSearchResult soundResult = new FileSearchResult(this, AudioSearchResultType.SoundFile, SOUND_RESOURCE_NAME);
            soundResult.Title = "Sound-Datei (" + query + ")";

            FileSearchResult musicResult = new FileSearchResult(this, AudioSearchResultType.MusicFile, MUSIC_RESOURCE_NAME);
            musicResult.Title = "Musik-Datei (" + query + ")";

            ModeElementSearchResult modeResult = new ModeElementSearchResult(this, soundResult, musicResult);
            modeResult.Title = "Szenario (" + query + ")";

            results.Add(soundResult);
            results.Add(musicResult);
            results.Add(modeResult);

            totalNumberOfResults = results.Count;

            var completionSource = new TaskCompletionSource<IEnumerable<ISearchResult>>();
            completionSource.SetResult(results);
            return completionSource.Task;
        }

        public bool IsAudioTypeSupported(AudioSearchResultType type)
        {
            return true;
        }

        public void ExtractEmbeddedFile(string resName, string fileName)
        {
            if (File.Exists(fileName)) File.Delete(fileName);

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(fileName));
            Assembly assembly = Assembly.GetExecutingAssembly();

            using (var input = assembly.GetManifestResourceStream(resName))
            using (var output = File.Open(fileName, FileMode.CreateNew))
            {
                if (input == null) throw new FileNotFoundException(resName + ": Embedded resoure file not found");

                var buffer = new byte[32768];
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, read);
                }
                output.Flush();
            }
        }
    }

    public class FileSearchResult : BaseSearchResult, IFileSearchResult
    {
        private string m_ResourceName;
        private AudioSearchResultType m_Type;

        public FileSearchResult(TestAudioSource audioSource, AudioSearchResultType type, string resourceName) : base(audioSource, type)
        {
            this.m_ResourceName = resourceName;
            this.m_Type = type;
        }

        public string Filename { get; internal set; }

        public SoundFileType FileType { get; internal set; }
        
        public new TestAudioSource AudioSource { get; internal set; }

        public double? DeploymentCost { get { return null; } }

        public AudioDeploymentResult Deploy(IAbsoluteProgressMonitor monitor, ITargetDirectoryProvider targetDirectoryProvider)
        {
            AudioSource.ExtractEmbeddedFile(m_ResourceName, targetDirectoryProvider.GetFullPath(this));
            return AudioDeploymentResult.SUCCESS;
        }
    }

    public class ModeElementSearchResult : BaseSearchResult, IModeElementSearchResult
    {
        private FileSearchResult m_MusicResource;
        private FileSearchResult m_SoundResource;

        public ModeElementSearchResult(TestAudioSource audioSource, FileSearchResult soundResource, FileSearchResult musicResource) : base(audioSource, AudioSearchResultType.ModeElement)
        {
            this.m_SoundResource = soundResource;
            this.m_MusicResource = musicResource;

            this.FilesToBeDownloaded.Add(m_MusicResource);
            this.FilesToBeDownloaded.Add(m_SoundResource);
        }

        public IModeElement GetModeElementDefinition(ITargetDirectoryProvider targetDirectoryProvider)
        {
            IElementContainer<IParallelElement> container = DataModule.ElementFactory.CreateParallelContainer("Test-Szenario");
            IModeElement modeElement = DataModule.ElementFactory.CreateModeElement("Test-Szenario", container);

            IRandomBackgroundMusicList music = DataModule.ElementFactory.CreateRandomBackgroundMusicList("Musik");
            container.AddElement(music);

            IBackgroundSounds sounds = DataModule.ElementFactory.CreateBackgroundSounds("Sounds");
            container.AddElement(sounds);

            IBackgroundSoundChoice soundChoice1 = sounds.AddElement("Auswahl 1");

            music.AddElement(DataModule.ElementFactory.CreateFileElement(targetDirectoryProvider.GetPathWithinLibrary(m_MusicResource), SoundFileType.Music));
            soundChoice1.AddElement(DataModule.ElementFactory.CreateFileElement(targetDirectoryProvider.GetPathWithinLibrary(m_SoundResource), SoundFileType.SoundEffect));

            return modeElement;
        }
    }

}
