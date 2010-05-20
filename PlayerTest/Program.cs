using System;
using System.Collections.Generic;
using System.Text;
using Ares.Playing;
using Ares.Data;
using Un4seen.Bass;

namespace PlayerTest
{
    class SoundFile : ISoundFile
    {
        public Int32 Id { get; set; }
        public String Path { get; set; }
        public Ares.Playing.SoundFileType SoundFileType { get; set; }

        public SoundFile(Int32 id, String path, Ares.Playing.SoundFileType type)
        {
            Id = id;
            Path = path;
            SoundFileType = type;
        }
    }

    class Callbacks : IPlayingCallbacks
    {

        private String GetTitle(int elementId)
        {
            return Ares.Data.DataModule.ElementRepository.GetElement(elementId).Title;
        }

        public void ModeChanged(Ares.Data.IMode newMode)
        {
            System.Console.WriteLine("Mode is now " + newMode.Title);
        }

        public void ModeElementStarted(Ares.Data.IModeElement element)
        {
            System.Console.WriteLine("ModeElement started: " + element.Title);
        }

        public void ModeElementFinished(Ares.Data.IModeElement element)
        {
            System.Console.WriteLine("ModeElement finished " + element.Title);
        }

        public void SoundStarted(int elementId)
        {
            System.Console.WriteLine("Sound started: " + GetTitle(elementId));
        }

        public void SoundFinished(int elementId)
        {
            System.Console.WriteLine("Sound finished: " + GetTitle(elementId));
        }

        public void MusicStarted(int elementId)
        {
            System.Console.WriteLine("Music started: " + GetTitle(elementId));
        }

        public void MusicFinished(int elementId)
        {
            System.Console.WriteLine("Music finished: " + GetTitle(elementId));
        }
    }

    class Program
    {
        System.Threading.AutoResetEvent finishEvent = new System.Threading.AutoResetEvent(false);

        void DoTest()
        {
            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                // TODO: better exception
                throw new Exception("Couldn't init BASS engine");
            }

            /*
            using (IFilePlayer player = PlayingModule.CreatePlayer())
            {
                System.Console.WriteLine("Playing music file");
                SoundFile file = new SoundFile(1, @"D:\Projekte\Ares\trunk\PlayerTest\Defeat.mp3", SoundFileType.Music);
                player.PlayFile(file, new PlayingFinished((id, handle) => 
                    {
                        System.Console.WriteLine("Finished playing id " + id);
                        finishEvent.Set();
                    }));
                System.Threading.Thread.Sleep(2000);
                SoundFile file2 = new SoundFile(2, @"D:\Projekte\Ares\trunk\PlayerTest\PferdeWiehern.wav", SoundFileType.SoundEffect);
                player.PlayFile(file2, (id, handle) => System.Console.WriteLine("Finished playing id " + id));
                System.Threading.Thread.Sleep(2000);
                player.PlayFile(file2, (id, handle) => System.Console.WriteLine("Finished playing id " + id));
                finishEvent.WaitOne();
            }
            */
            IElementFactory factory = DataModule.ElementFactory;
            IProject project = DataModule.ProjectManager.CreateProject("Test");
            ISequentialBackgroundMusicList musicList = factory.CreateSequentialBackgroundMusicList("My Music");
            musicList.FixedIntermediateDelay = TimeSpan.FromSeconds(5.0);
            musicList.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\Defeat.mp3", Ares.Data.SoundFileType.Music));
            musicList.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\Witchdrums.mp3", Ares.Data.SoundFileType.Music));

            IBackgroundSounds sounds = factory.CreateBackgroundSounds("My Sounds");
            IBackgroundSoundChoice choice1 = sounds.AddElement("Pferde");
            choice1.FixedStartDelay = TimeSpan.FromSeconds(2.0);
            choice1.FixedIntermediateDelay = TimeSpan.FromSeconds(2.0);
            choice1.MaximumRandomIntermediateDelay = TimeSpan.FromSeconds(10.0);
            choice1.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\PferdeWiehern.wav", Ares.Data.SoundFileType.SoundEffect));
            choice1.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\PferdeSchnauben.wav", Ares.Data.SoundFileType.SoundEffect));
            IBackgroundSoundChoice choice2 = sounds.AddElement("Vögel");
            choice2.FixedStartDelay = TimeSpan.FromSeconds(5.0);
            choice2.FixedIntermediateDelay = TimeSpan.FromSeconds(3.0);
            choice2.MaximumRandomIntermediateDelay = TimeSpan.FromSeconds(15.0);
            choice2.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\Vogel0000.wav", Ares.Data.SoundFileType.SoundEffect)).RandomChance = 60;
            choice2.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\Vogel0002.wav", Ares.Data.SoundFileType.SoundEffect)).RandomChance = 20;
            choice2.AddElement(factory.CreateFileElement(@"D:\Projekte\Ares\trunk\PlayerTest\VogelFlattern0000.wav", Ares.Data.SoundFileType.SoundEffect)).RandomChance = 20;

            IElementContainer<IParallelElement> modeContainer = factory.CreateParallelContainer("ModeContainer");
            modeContainer.AddElement(musicList);
            modeContainer.AddElement(sounds);

            IKeyTrigger trigger = factory.CreateKeyTrigger();
            trigger.KeyCode = 10;

            IModeElement modeElement = factory.CreateModeElement("myMode", modeContainer);
            modeElement.Trigger = trigger;

            IMode mode = project.AddMode("myMode");
            mode.KeyCode = 20;
            mode.AddElement(modeElement);

            Callbacks callbacks = new Callbacks();
            PlayingModule.SetCallbacks(callbacks);

            IPlayer player = PlayingModule.Player;
            player.SetProject(project);
            player.KeyReceived(20);
            player.KeyReceived(10);

            ConsoleKeyInfo info = System.Console.ReadKey();
            while (info.KeyChar != 'q')
            {
                if (info.KeyChar == 'm')
                    player.KeyReceived(10);
                info = System.Console.ReadKey();
            }
            player.StopAll();

            Bass.BASS_Free();
        }

        static void Main(string[] args)
        {
            (new Program()).DoTest();
        }
    }
}
