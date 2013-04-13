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

namespace Ares.Data
{
    [Serializable]
    abstract class Trigger
    {
        public Int32 TargetElementId { get; set; }

        public bool StopMusic { get; set; }

        public bool StopSounds { get; set; }

        public bool CrossFadeMusic { get; set; }

        public bool FadeMusic { get; set; }

        public Int32 FadeMusicTime { get; set; }

        public bool FadeSounds { get; set; }

        public Int32 FadeSoundTime { get; set; }

        protected void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("stopMusic", StopMusic ? "true" : "false");
            writer.WriteAttributeString("stopSounds", StopSounds ? "true" : "false");
            writer.WriteAttributeString("targetId", TargetElementId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("crossFadeMusic", CrossFadeMusic ? "true" : "false");
            writer.WriteAttributeString("fadeMusic", FadeMusic ? "true" : "false");
            writer.WriteAttributeString("crossFadeMusicTime", FadeMusicTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("fadeSounds", FadeSounds ? "true" : "false");
            writer.WriteAttributeString("fadeSoundTime", FadeSoundTime.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        protected Trigger()
        {
            StopMusic = false;
            StopSounds = false;
            CrossFadeMusic = false;
            FadeMusic = false;
            FadeMusicTime = 0;
        }

        protected Trigger(System.Xml.XmlReader reader)
        {
            StopMusic = reader.GetBooleanAttribute("stopMusic");
            StopSounds = reader.GetBooleanAttribute("stopSounds");
            TargetElementId = reader.GetIntegerAttribute("targetId");
            CrossFadeMusic = reader.GetBooleanAttributeOrDefault("crossFadeMusic", false);
            FadeMusic = reader.GetBooleanAttributeOrDefault("fadeMusic", false);
            FadeMusicTime = reader.GetIntegerAttributeOrDefault("crossFadeMusicTime", 0);
            FadeSounds = reader.GetBooleanAttributeOrDefault("fadeSounds", false);
            FadeSoundTime = reader.GetIntegerAttributeOrDefault("fadeSoundTime", 0);
        }
    }

    public enum Keys
    {
        Back = 8,
        Tab = 9,
        Return = 13,
        Escape = 27,
        Space = 32,
        PageUp = 33,
        PageDown = 34,
        End = 35,
        Home = 36,
        Left = 37,
        Up = 38,
        Right = 39,
        Down = 40,
        Insert = 45,
        Delete = 46,
        NumPad0 = 96,
        NumPad1 = 97,
        NumPad2 = 98,
        NumPad3 = 99,
        NumPad4 = 100,
        NumPad5 = 101,
        NumPad6 = 102,
        NumPad7 = 103,
        NumPad8 = 104,
        NumPad9 = 105,
        F1 = 112,
        F2 = 113,
        F3 = 114,
        F4 = 115,
        F5 = 116,
        F6 = 117,
        F7 = 118,
        F8 = 119,
        F9 = 120,
        F10 = 121,
        F11 = 122,
        F12 = 123,
        OemSemicolon = 186,
        OemComma = 188,
        OemMinus = 189,
        OemPeriod = 190,
        Oem2 = 191,
        Oem3 = 192,
        Oem4 = 219
    }

    [Serializable]
    class KeyTrigger : Trigger, IKeyTrigger
    {
        public TriggerType TriggerType { get { return TriggerType.Key; } }

        public Int32 KeyCode { get; set; }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("KeyTrigger");
            DoWriteToXml(writer);
            writer.WriteAttributeString("Key", KeyCode.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        internal KeyTrigger()
        {
            KeyCode = 0;
        }

        internal KeyTrigger(System.Xml.XmlReader reader)
            : base(reader)
        {
            KeyCode = reader.GetIntegerAttribute("Key");
            reader.ReadOuterXml();
        }
    }

    [Serializable]
    class ElementFinishTrigger : Trigger, IElementFinishTrigger
    {
        public TriggerType TriggerType { get { return TriggerType.ElementFinished; } }

        public Int32 ElementId { get; set; }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("ElementFinishTrigger");
            DoWriteToXml(writer);
            writer.WriteAttributeString("elementId", ElementId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }

        internal ElementFinishTrigger()
        {
            ElementId = 0;
        }

        internal ElementFinishTrigger(System.Xml.XmlReader reader)
            : base(reader)
        {
            ElementId = reader.GetIntegerAttribute("elementId");
            reader.ReadOuterXml();
        }
    }

    [Serializable]
    class NoTrigger : Trigger, ITrigger
    {
        public TriggerType TriggerType { get { return TriggerType.None; } }

        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("NoTrigger");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        internal NoTrigger() { }

        internal NoTrigger(System.Xml.XmlReader reader)
            : base(reader)
        {
            reader.ReadOuterXml();
        }
    }
}
