using System;
using System.Collections.Generic;

namespace Ares.Data
{
    [Serializable]
    class BackgroundSoundChoice : ElementBase, IBackgroundSoundChoice
    {

        #region IElementContainer<IChoiceElement> Members

        public IChoiceElement AddElement(IElement element)
        {
            return m_Container.AddElement(element);
        }

        public void RemoveElement(int ID)
        {
            m_Container.RemoveElement(ID);
        }

        public IList<IChoiceElement> GetElements()
        {
            return m_Container.GetElements();
        }

        #endregion

        #region IDelayableElement Members

        public TimeSpan FixedStartDelay
        {
            get
            {
                return ParallelElement.FixedStartDelay;
            }
            set
            {
                ParallelElement.FixedStartDelay = value;
            }
        }

        public TimeSpan MaximumRandomStartDelay
        {
            get
            {
                return ParallelElement.MaximumRandomStartDelay;
            }
            set
            {
                ParallelElement.MaximumRandomStartDelay = value;
            }
        }

        #endregion

        #region IRepeatableElement Members

        public bool Repeat
        {
            get
            {
                return ParallelElement.Repeat;
            }
            set
            {
                ParallelElement.Repeat = value;
            }
        }

        public TimeSpan FixedIntermediateDelay
        {
            get
            {
                return ParallelElement.FixedIntermediateDelay;
            }
            set
            {
                ParallelElement.FixedIntermediateDelay = value;
            }
        }

        public TimeSpan MaximumRandomIntermediateDelay
        {
            get
            {
                return ParallelElement.MaximumRandomIntermediateDelay;
            }
            set
            {
                ParallelElement.MaximumRandomIntermediateDelay = value;
            }
        }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitChoiceContainer(m_Container);
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("BackgroundSoundChoice");
            DoWriteToXml(writer);
            m_Container.WriteToXml(writer);
            writer.WriteEndElement();
        }

        internal BackgroundSoundChoice(Int32 id, String title)
            : base(id)
        {
            m_Container = DataModule.ElementFactory.CreateChoiceContainer(title + "_Choice");
        }

        internal BackgroundSoundChoice(System.Xml.XmlReader reader)
            : base(reader)
        {
            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.MoveToContent();
            m_Container = DataModule.TheElementFactory.CreateChoiceContainer(reader);
            reader.ReadEndElement();
        }

        internal IParallelElement ParallelElement { get; set; }

        private IElementContainer<IChoiceElement> m_Container;
    }

    class BackgroundSounds : ModeElementBase, IBackgroundSounds
    {
        public IBackgroundSoundChoice AddElement(IElement element)
        {
            BackgroundSoundChoice choice = new BackgroundSoundChoice(
                DataModule.TheElementFactory.GetNextID(), element.Title);
            IParallelElement parallelElement = m_Container.AddElement(choice);
            parallelElement.Repeat = true;
            choice.ParallelElement = parallelElement;
            m_Elements.Add(choice);
            return choice;
        }

        public void RemoveElement(Int32 id)
        {
            IBackgroundSoundChoice element = m_Elements.Find(e => e.Id == id);
            if (element != null)
            {
                m_Elements.Remove(element);
                m_Container.RemoveElement((element as BackgroundSoundChoice).ParallelElement.Id);
            }
        }

        public IList<IBackgroundSoundChoice> GetElements()
        {
            return new List<IBackgroundSoundChoice>(m_Elements);
        }

        public bool IsEndless()
        {
            foreach (IBackgroundSoundChoice choice in m_Elements)
            {
                if (choice.Repeat) return true;
            }
            return false;
        }

        #region IModeElement Members

        public ITrigger Trigger
        {
            get
            {
                return m_Trigger;
            }
            set
            {
                m_Trigger = value;
                m_Trigger.TargetElementId = m_Container.Id;
                m_Trigger.StopSounds = true;
            }
        }

        public IElement StartElement { get { return m_Container; } }

        #endregion

        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitParallelContainer(m_Container);
        }

        internal BackgroundSounds(Int32 id, String title)
            : base(id)
        {
            Title = title;
            m_Elements = new List<IBackgroundSoundChoice>();
            m_Container = DataModule.ElementFactory.CreateParallelContainer(title + "_Parallel");
        }

        public override void WriteToXml(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("BackgroundSounds");
            DoWriteToXml(writer);
            writer.WriteStartElement("SubElements");
            m_Elements.ForEach(e => e.WriteToXml(writer));
            writer.WriteEndElement();
            m_Trigger.WriteToXml(writer);
            writer.WriteEndElement();
        }

        internal BackgroundSounds(System.Xml.XmlReader reader)
            : base(reader)
        {
            m_Elements = new List<IBackgroundSoundChoice>();
            m_Container = DataModule.ElementFactory.CreateParallelContainer(Title + "_Parallel");

            if (reader.IsEmptyElement)
            {
                XmlHelpers.ThrowException(StringResources.ExpectedContent, reader);
            }
            reader.MoveToContent();
            while (reader.IsStartElement())
            {
                if (reader.IsStartElement("SubElements") && !reader.IsEmptyElement)
                {
                    reader.MoveToContent();
                    while (reader.IsStartElement())
                    {
                        if (reader.IsStartElement("BackgroundSoundChoice"))
                        {
                            BackgroundSoundChoice choice = new BackgroundSoundChoice(reader);
                            IParallelElement parallelElement = m_Container.AddElement(choice);
                            parallelElement.Repeat = true;
                            choice.ParallelElement = parallelElement;
                            m_Elements.Add(choice);
                        }
                        else
                        {
                            reader.ReadOuterXml();
                        }
                    }
                    reader.ReadEndElement();
                }
                else
                {
                    reader.ReadOuterXml();
                }
            }

            m_Trigger = DataModule.TheElementFactory.CreateTrigger(reader);
            reader.ReadEndElement();
        }        

        private ITrigger m_Trigger;

        private List<IBackgroundSoundChoice> m_Elements;
        private IElementContainer<IParallelElement> m_Container;
    }
}
