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
