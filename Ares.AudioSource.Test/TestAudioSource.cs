using System;
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

        public ICollection<ISearchResult> GetSearchResults(string query, AudioSearchResultType? type, int pageSize, int pageIndex, IProgressMonitor monitor, CancellationToken token, out int? totalNumberOfResults)
        {
            List<ISearchResult> results = new List<ISearchResult>();

            FileSearchResult soundResult = new FileSearchResult(this, AudioSearchResultType.SoundFile, SOUND_RESOURCE_NAME);
            soundResult.Title = "Sound-Datei (" + query + ")";

            FileSearchResult musicResult = new FileSearchResult(this, AudioSearchResultType.MusicFile, MUSIC_RESOURCE_NAME);
            musicResult.Title = "Musik-Datei (" + query + ")";
            
            ModeElementSearchResult modeResult = new ModeElementSearchResult(this, SOUND_RESOURCE_NAME, MUSIC_RESOURCE_NAME);
            modeResult.Title = "Szenario (" + query + ")";

            results.Add(soundResult);
            results.Add(musicResult);
            results.Add(modeResult);

            totalNumberOfResults = results.Count;
            return results;
        }

        public bool IsAudioTypeSupported(AudioSearchResultType type)
        {
            return true;
        }

        public static void ExtractEmbeddedFile(string resName, string fileName)
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

    public abstract class AbstractSimpleSearchResult : ISearchResult
    {
        private TestAudioSource m_AudioSource;
        private AudioSearchResultType m_Type;

        public AbstractSimpleSearchResult(TestAudioSource audioSource, AudioSearchResultType type)
        {
            this.m_AudioSource = audioSource;
            this.m_Type = type;
            this.Tags = new List<string>();
        }

        public IAudioSource AudioSource { get { return m_AudioSource;  } }

        public string Author { get; set; }
        public double AverageRating { get; set; }
        public string Description { get; set; }
        public double DownloadSize { get; set; }
        public TimeSpan Duration { get; set; }
        public string Id { get; set; }
        public string License { get; set; }
        public int NumberOfRatings { get; set; }
        public AudioSearchResultType ResultType { get { return m_Type;  } }
        public List<string> Tags { get; set; }
        public string Title { get; set; }

        public abstract AudioDownloadResult Download(string musicTargetDirectory, string soundsTargetDirectory, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize);
    }

    public class FileSearchResult : AbstractSimpleSearchResult, IFileSearchResult
    {
        private string m_ResourceName;

        public FileSearchResult(TestAudioSource audioSource, AudioSearchResultType type, string resourceName): base(audioSource, type)
        {
            this.m_ResourceName = resourceName;
        }

        public override AudioDownloadResult Download(string musicTargetDirectory, string soundsTargetDirectory, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize)
        {
            string targetDirectory = this.ResultType == AudioSearchResultType.MusicFile ? musicTargetDirectory : soundsTargetDirectory;
            string targetFile = Path.Combine(targetDirectory, GetDownloadFilename());

            if (!File.Exists(targetFile))
            {
                TestAudioSource.ExtractEmbeddedFile(m_ResourceName, targetFile);
            }

            return AudioDownloadResult.SUCCESS;
        }

        public string GetDownloadFilename()
        {
            return m_ResourceName;
        }
    }

    public class ModeElementSearchResult : AbstractSimpleSearchResult, IModeElementSearchResult
    {
        private string m_MusicResource;
        private string m_SoundResource;

        public ModeElementSearchResult(TestAudioSource audioSource, string soundResource, string musicResource) : base(audioSource, AudioSearchResultType.ModeElement)
        {
            this.m_SoundResource = soundResource;
            this.m_MusicResource = musicResource;
        }

        public override AudioDownloadResult Download(string musicTargetDirectory, string soundsTargetDirectory, IProgressMonitor monitor, CancellationToken cancellationToken, double totalSize)
        {
            TestAudioSource.ExtractEmbeddedFile(m_SoundResource, Path.Combine(soundsTargetDirectory, m_SoundResource));
            TestAudioSource.ExtractEmbeddedFile(m_MusicResource, Path.Combine(musicTargetDirectory, m_MusicResource));

            return AudioDownloadResult.SUCCESS;
        }

        public IModeElement GetModeElementDefinition(string relativeDownloadPath)
        {
            IElementContainer<IParallelElement> container = DataModule.ElementFactory.CreateParallelContainer("Test-Szenario");
            IModeElement modeElement = DataModule.ElementFactory.CreateModeElement("Test-Szenario", container);

            IRandomBackgroundMusicList music = DataModule.ElementFactory.CreateRandomBackgroundMusicList("Musik");
            container.AddElement(music);

            IBackgroundSounds sounds = DataModule.ElementFactory.CreateBackgroundSounds("Sounds");
            container.AddElement(sounds);

            IBackgroundSoundChoice soundChoice1 = sounds.AddElement("Auswahl 1");

            music.AddElement(DataModule.ElementFactory.CreateFileElement(Path.Combine(relativeDownloadPath,m_MusicResource),SoundFileType.Music));
            soundChoice1.AddElement(DataModule.ElementFactory.CreateFileElement(Path.Combine(relativeDownloadPath, m_SoundResource), SoundFileType.SoundEffect));

            return modeElement;
        }
    }

}
