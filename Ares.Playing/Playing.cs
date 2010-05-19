using System;
using System.Collections.Generic;
using System.Text;

namespace Ares.Playing
{
    public static class PlayingModule
    {
        public static IFilePlayer CreatePlayer()
        {
            return new FilePlayer();
        }

        public static IPlayer Player
        {
            get { return sPlayer; }
        }

        public static void SetCallbacks(IPlayingCallbacks callbacks)
        {
            sPlayer.Callbacks = callbacks;
        }

        internal static IFilePlayer FilePlayer
        {
            get { return sFilePlayer; }
        }

        internal static Random Randomizer
        {
            get { return sRandom; }
        }

        private static Player sPlayer = new Player();
        private static FilePlayer sFilePlayer = new FilePlayer();
        private static Random sRandom = new Random();
   }
}
