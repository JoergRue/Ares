/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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
using System.Windows.Forms;

namespace Ares.Editor.Controls
{
    enum TimeUnit
    {
        Milliseconds = 0,
        Seconds,
        Minutes
    }

    class TimeConversion
    {
        public static int GetTimeInUnit(int milliseconds, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Milliseconds:
                    return milliseconds;
                case TimeUnit.Seconds:
                    return milliseconds / 1000;
                case TimeUnit.Minutes:
                    return milliseconds / 60000;
                default:
                    return milliseconds;
            }
        }

        public static int GetTimeInMillis(int time, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Milliseconds:
                    return time;
                case TimeUnit.Seconds:
                    return time * 1000;
                case TimeUnit.Minutes:
                    return time * 60000;
                default:
                    return time;
            }
        }

        public static int GetTimeInMillis(NumericUpDown upDown, ComboBox combo)
        {
            return GetTimeInMillis((int)upDown.Value, (TimeUnit)combo.SelectedIndex);
        }

        public static int GetTimeInUnit(int milliseconds, ComboBox combo)
        {
            return GetTimeInUnit(milliseconds, (TimeUnit)combo.SelectedIndex);
        }

        public static int GetTimeInUnit(TimeSpan time, ComboBox combo)
        {
            return GetTimeInUnit(time, (TimeUnit)combo.SelectedIndex);
        }

        public static int GetTimeInUnit(TimeSpan time, TimeUnit unit)
        {
            switch (unit)
            {
                case TimeUnit.Milliseconds:
                    return (int)time.TotalMilliseconds;
                case TimeUnit.Seconds:
                    return (int)time.TotalSeconds;
                case TimeUnit.Minutes:
                    return (int)time.TotalMinutes;
                default:
                    return (int)time.TotalMilliseconds;
            }
        }
         
    }
}
