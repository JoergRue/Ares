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

        public static IProjectPlayer ProjectPlayer
        {
            get { return sPlayer; }
        }

        public static IElementPlayer ElementPlayer
        {
            get { return sPlayer; }
        }

        public static void SetCallbacks(IProjectPlayingCallbacks callbacks)
        {
            sPlayer.ProjectCallbacks = callbacks;
        }

        public static void SetCallbacks(IElementPlayingCallbacks callbacks)
        {
            sPlayer.ElementCallbacks = callbacks;
        }

        internal static Player ThePlayer
        {
            get { return sPlayer; }
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
