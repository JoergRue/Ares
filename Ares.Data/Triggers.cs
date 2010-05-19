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
        {
            ElementId = reader.GetIntegerAttribute("elementId");
            reader.ReadOuterXml();
        }
    }
}
