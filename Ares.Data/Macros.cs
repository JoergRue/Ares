/*
 Copyright (c) 2015  [Joerg Ruedenauer]
 
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

    #region Interfaces

    public enum MacroCommandType
    {
        WaitTime,
        WaitCondition,
        StartElement,
        StopElement,
        AddTag,
        RemoveTag,
        RemoveAllTags
    }

    public interface IMacroCommand : IElement
    {
        MacroCommandType CommandType { get; }
        IMacroCondition Condition { get; }
        void VisitMacroCommand(IMacroCommandVisitor commandVisitor);
    }

    public interface IWaitTimeCommand : IMacroCommand
    {
        Int32 TimeInMillis { get; set; }
    }

    public enum MacroConditionType
    {
        None,
        ElementRunning,
        ElementNotRunning
    }

    public interface IMacroCondition
    {
        MacroConditionType ConditionType { get; }
        IModeElement Conditional { get; }
        int ConditionalId { get; }
    }

    public interface IWaitConditionCommand : IMacroCommand
    {
        IMacroCondition AwaitedCondition { get; }
    }

    public interface IStartCommand : IMacroCommand
    {
        IModeElement StartedElement { get; }
        int StartedElementId { get; }
    }

    public interface IStopCommand : IMacroCommand
    {
        IModeElement StoppedElement { get; }
        int StoppedElementId { get; }
    }

    public interface ITagCommand : IMacroCommand
    {
        int CategoryId { get; }
        int TagId { get; }
    }

    public interface IRemoveAllTagsCommand : IMacroCommand
    {
    }

    public interface IMacroElement : IContainerElement
    {
    }

    public interface IMacroCommandVisitor
    {
        void VisitStartCommand(IStartCommand startCommand);
        void VisitStopCommand(IStopCommand stopCommand);
        void VisitWaitTimeCommand(IWaitTimeCommand waitTimeCommand);
        void VisitWaitConditionCommand(IWaitConditionCommand waitConditionCommand);
        void VisitTagCommand(ITagCommand tagCommand);
        void VisitRemoveAllTagsCommand(IRemoveAllTagsCommand removeAllTagsCommand);
    }

    public interface IMacro : IReorderableContainer<IMacroElement>
    {
        void VisitMacro(IMacroCommandVisitor commandVisitor);
    }

    public interface IMacroFactory
    {
        IMacro CreateMacro(String title);
        IWaitTimeCommand CreateWaitTime(Int32 timeInMillis, MacroConditionType conditionType, IModeElement conditionElement);
        IWaitConditionCommand CreateWaitCondition(MacroConditionType awaitedConditionType, IModeElement awaitedConditionElement, MacroConditionType conditionType, IModeElement conditionElement);
        IStartCommand CreateStartCommand(IModeElement startedElement, MacroConditionType conditionType, IModeElement conditionElement);
        IStopCommand CreateStopCommand(IModeElement startedElement, MacroConditionType conditionType, IModeElement conditionElement);
        ITagCommand CreateTagCommand(int categoryId, int tagId, bool addTag, MacroConditionType conditionType, IModeElement conditionElement);
        IRemoveAllTagsCommand CreateRemoveAllTagsCommand(MacroConditionType conditionType, IModeElement conditionElement);
    }

#endregion

#region Implementation

    [Serializable]
    class MacroCondition : IMacroCondition
    {
        public MacroConditionType ConditionType { get; set; }

        public IModeElement Conditional
        {
            get
            {
                if (m_Reference == null)
                    return null;
                IElement element = DataModule.TheElementRepository.GetElement(m_Reference.ReferencedId);
                IModeElement modeElement = element as IModeElement;
                return modeElement;
            }
        }

        public int ConditionalId
        {
            get
            {
                return m_Reference != null ? m_Reference.ReferencedId : 0;
            }
        }

        public MacroCondition(MacroConditionType conditionType, IModeElement modeElement)
        {
            ConditionType = conditionType;
            m_Reference = modeElement != null ? new ReferenceHolder(modeElement.Id) : null;
        }

        public MacroCondition(System.Xml.XmlReader reader, String name)
        {
            ConditionType = (MacroConditionType)reader.GetIntegerAttribute(name + "ConditionType");
            if (reader.GetAttribute(name + "ConditionElement") != null)
            {
                m_Reference = new ReferenceHolder(reader, name + "ConditionElement");
            }
            else
            {
                m_Reference = null;
            }
        }

        public void WriteToXml(System.Xml.XmlWriter writer, String name)
        {
            writer.WriteAttributeString(name + "ConditionType", ((int)ConditionType).ToString(System.Globalization.CultureInfo.InvariantCulture));
            if (m_Reference != null)
            {
                m_Reference.WriteToXml(writer, name + "ConditionElement");
            }
        }

        private ReferenceHolder m_Reference;
    }

    [Serializable]
    abstract class MacroCommand : ElementBase, IMacroCommand
    {
        public IMacroCondition Condition { get { return m_Condition; } }

        public abstract void VisitMacroCommand(IMacroCommandVisitor commandVisitor);

        protected void ReadToEndOfElement(System.Xml.XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                reader.Read();
            }
            else
            {
                reader.Read();
                reader.ReadInnerXml();
                reader.ReadEndElement();
            }
        }

        public abstract MacroCommandType CommandType
        {
            get;
        }

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitMacroCommand(this);
        }

        protected MacroCommand(MacroConditionType conditionType, IModeElement conditionElement)
            : base(DataModule.TheElementFactory.GetNextID())
        {
            m_Condition = new MacroCondition(conditionType, conditionElement);
        }

        protected MacroCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_Condition = new MacroCondition(reader, "active");
        }

        protected override void DoWriteToXml(System.Xml.XmlWriter writer)
        {
            base.DoWriteToXml(writer);
            m_Condition.WriteToXml(writer, "active");
        }

        private MacroCondition m_Condition;
    }

    [Serializable]
    class WaitTimeCommand : MacroCommand, IWaitTimeCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitWaitTimeCommand(this);
        }

        public override MacroCommandType CommandType { get { return MacroCommandType.WaitTime; } }

        public Int32 TimeInMillis { get; set; }

        public WaitTimeCommand(Int32 timeInMillis, MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            TimeInMillis = timeInMillis;
            Title = "WaitTime";
        }

        public WaitTimeCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            TimeInMillis = reader.GetIntegerAttribute("TimeMs");
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("WaitTimeCommand");
            DoWriteToXml(writer);
            writer.WriteAttributeString("TimeMs", TimeInMillis.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteEndElement();
        }
    }

    [Serializable]
    class WaitConditionCommand : MacroCommand, IWaitConditionCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitWaitConditionCommand(this);
        }

        public override MacroCommandType CommandType { get { return MacroCommandType.WaitCondition; } }

        public IMacroCondition AwaitedCondition { get { return m_Condition; } }

        public WaitConditionCommand(MacroConditionType awaitedConditionType, IModeElement awaitedConditionElement, MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            m_Condition = new MacroCondition(awaitedConditionType, awaitedConditionElement);
            Title = "WaitCondition";
        }

        public WaitConditionCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_Condition = new MacroCondition(reader, "awaited");
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("WaitConditionCommand");
            DoWriteToXml(writer);
            m_Condition.WriteToXml(writer, "awaited");
            writer.WriteEndElement();
        }

        private MacroCondition m_Condition;
    }

    [Serializable]
    class StartCommand : MacroCommand, IStartCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitStartCommand(this);
        }

        public override MacroCommandType CommandType { get { return MacroCommandType.StartElement; } }

        public IModeElement StartedElement
        {
            get
            {
                return DataModule.ElementRepository.GetElement(m_StartedElement.ReferencedId) as IModeElement;
            }
        }

        public int StartedElementId
        {
            get
            {
                return m_StartedElement.ReferencedId;
            }
        }

        public StartCommand(IModeElement startedElement, MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            m_StartedElement = new ReferenceHolder(startedElement.Id);
            Title = "Start";
        }

        public StartCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_StartedElement = new ReferenceHolder(reader, "startedElement");
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("StartCommand");
            DoWriteToXml(writer);
            m_StartedElement.WriteToXml(writer, "startedElement");
            writer.WriteEndElement();
        }

        private ReferenceHolder m_StartedElement;
    }

    [Serializable]
    class StopCommand : MacroCommand, IStopCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitStopCommand(this);
        }

        public override MacroCommandType CommandType { get { return MacroCommandType.StopElement; } }

        public IModeElement StoppedElement
        {
            get
            {
                return DataModule.ElementRepository.GetElement(m_StoppedElement.ReferencedId) as IModeElement;
            }
        }

        public int StoppedElementId
        {
            get
            {
                return m_StoppedElement.ReferencedId;
            }
        }

        public StopCommand(IModeElement stoppedElement, MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            m_StoppedElement = new ReferenceHolder(stoppedElement.Id);
            Title = "Stop";
        }

        public StopCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_StoppedElement = new ReferenceHolder(reader, "stoppedElement");
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("StopCommand");
            DoWriteToXml(writer);
            m_StoppedElement.WriteToXml(writer, "stoppedElement");
            writer.WriteEndElement();
        }

        private ReferenceHolder m_StoppedElement;
    }

    [Serializable]
    class TagCommand : MacroCommand, ITagCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitTagCommand(this);
        }

        public override MacroCommandType CommandType
        {
            get { return m_AddTag ? MacroCommandType.AddTag : MacroCommandType.RemoveTag; }
        }

        public int CategoryId { get { return m_CategoryId; } }
        public int TagId { get { return m_TagId; } }

        public TagCommand(int categoryId, int tagId, bool addTag, MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            Title = addTag ? "AddTag" : "RemoveTag";
            m_CategoryId = categoryId;
            m_TagId = tagId;
            m_AddTag = addTag;
        }

        public TagCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_CategoryId = reader.GetIntegerAttribute("categoryId");
            m_TagId = reader.GetIntegerAttribute("tagId");
            m_AddTag = reader.GetBooleanAttribute("addTag");
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("TagCommand");
            writer.WriteAttributeString("categoryId", m_CategoryId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("tagId", m_TagId.ToString(System.Globalization.CultureInfo.InvariantCulture));
            writer.WriteAttributeString("addTag", m_AddTag ? "true" : "false");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        private int m_CategoryId;
        private int m_TagId;
        private bool m_AddTag;
    }

    [Serializable]
    class RemoveAllTagsCommand : MacroCommand, IRemoveAllTagsCommand
    {
        public override void VisitMacroCommand(IMacroCommandVisitor commandVisitor)
        {
            commandVisitor.VisitRemoveAllTagsCommand(this);
        }

        public override MacroCommandType CommandType
        {
            get { return MacroCommandType.RemoveAllTags; }
        }

        public RemoveAllTagsCommand(MacroConditionType conditionType, IModeElement conditionElement)
            : base(conditionType, conditionElement)
        {
            Title = "RemoveAllTags";
        }

        public RemoveAllTagsCommand(System.Xml.XmlReader reader)
            : base(reader)
        {
            ReadToEndOfElement(reader);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("RemoveAllTagsCommand");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }
    }

    class MacroElement : ContainerElement, IMacroElement
    {
        public void WriteToXml(System.Xml.XmlWriter writer)
        {
            InnerElement.WriteToXml(writer);
        }

        public MacroElement()
        {
        }
    }

    class Macro : Container<IMacroElement, MacroElement>, IMacro
    {
        public Macro(int id, String title)
            : base(id, title)
        {
        }

        public Macro(System.Xml.XmlReader reader)
            : base(reader)
        {
        }

        protected override bool CanAddElement(IElement element)
        {
            return element is IMacroCommand;
        }

        protected override MacroElement ReadContainerElement(System.Xml.XmlReader reader)
        {
            IMacroCommand command = DataModule.TheMacroFactory.CreateMacroCommand(reader);
            if (command != null)
            {
                MacroElement element = new MacroElement();
                element.InnerElement = command;
                return element;
            }
            else
            {
                return null;
            }
        }

        protected override void ReadSubclassAttributes(System.Xml.XmlReader reader)
        {
        }


        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitMacro(this);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("Macro");
            DoWriteToXml(writer);
            writer.WriteEndElement();
        }

        public void MoveElements(int startIndex, int endIndex, int offset)
        {
            System.Collections.Generic.List<MacroElement> elements = m_Elements.GetRange(startIndex, endIndex - startIndex + 1);
            m_Elements.RemoveRange(startIndex, endIndex - startIndex + 1);
            m_Elements.InsertRange(startIndex + offset, elements);
        }

        public void VisitMacro(IMacroCommandVisitor commandVisitor)
        {
            foreach (IMacroElement element in m_Elements)
            {
                IMacroCommand command = element != null ? element.InnerElement as IMacroCommand : null;
                if (command != null)
                    command.VisitMacroCommand(commandVisitor);
            }
        }

        public bool ShowArtistColumn { get { return false; } set { } }

        public bool ShowAlbumColumn { get { return false; } set { } }
    }

    class MacroFactory : IMacroFactory
    {
        public IMacro CreateMacro(String title)
        {
            return new Macro(DataModule.TheElementFactory.GetNextID(), title);
        }

        public IWaitTimeCommand CreateWaitTime(int timeInMillis, MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new WaitTimeCommand(timeInMillis, conditionType, conditionElement);
        }

        public IWaitConditionCommand CreateWaitCondition(MacroConditionType awaitedConditionType, IModeElement awaitedConditionElement, MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new WaitConditionCommand(awaitedConditionType, awaitedConditionElement, conditionType, conditionElement);
        }

        public IStartCommand CreateStartCommand(IModeElement startedElement, MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new StartCommand(startedElement, conditionType, conditionElement);
        }

        public IStopCommand CreateStopCommand(IModeElement startedElement, MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new StopCommand(startedElement, conditionType, conditionElement);
        }

        public ITagCommand CreateTagCommand(int categoryId, int tagId, bool addTag, MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new TagCommand(categoryId, tagId, addTag, conditionType, conditionElement);
        }

        public IRemoveAllTagsCommand CreateRemoveAllTagsCommand(MacroConditionType conditionType, IModeElement conditionElement)
        {
            return new RemoveAllTagsCommand(conditionType, conditionElement);
        }

        public IMacroCommand CreateMacroCommand(System.Xml.XmlReader reader)
        {
            if (reader.IsStartElement("WaitTimeCommand"))
                return new WaitTimeCommand(reader);
            else if (reader.IsStartElement("WaitConditionCommand"))
                return new WaitConditionCommand(reader);
            else if (reader.IsStartElement("StartCommand"))
                return new StartCommand(reader);
            else if (reader.IsStartElement("StopCommand"))
                return new StopCommand(reader);
            else if (reader.IsStartElement("TagCommand"))
                return new TagCommand(reader);
            else if (reader.IsStartElement("RemoveAllTagsCommand"))
                return new RemoveAllTagsCommand(reader);
            else
            {
                reader.ReadOuterXml();
                return null;
            }
        }
    }

#endregion
}