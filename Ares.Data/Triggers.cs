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

        protected void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteAttributeString("stopMusic", StopMusic ? "true" : "false");
            writer.WriteAttributeString("stopSounds", StopSounds ? "true" : "false");
            writer.WriteAttributeString("targetId", TargetElementId.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        protected Trigger()
        {
            StopMusic = false;
            StopSounds = false;
        }

        protected Trigger(System.Xml.XmlReader reader)
        {
            StopMusic = reader.GetBooleanAttribute("stopMusic");
            StopSounds = reader.GetBooleanAttribute("stopSounds");
            TargetElementId = reader.GetIntegerAttribute("targetId");
        }
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
