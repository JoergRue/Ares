using System;
using System.Collections.Generic;
using System.Text;
using Ares.Playing;
using Un4seen.Bass;

namespace PlayerTest
{
    class SoundFile : ISoundFile
    {
        public Int32 Id { get; set; }
        public String Path { get; set; }
        public SoundFileType SoundFileType { get; set; }

        public SoundFile(Int32 id, String path, SoundFileType type)
        {
            Id = id;
            Path = path;
            SoundFileType = type;
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

            Bass.BASS_Free();
        }

        static void Main(string[] args)
        {
            (new Program()).DoTest();
        }
    }
}
