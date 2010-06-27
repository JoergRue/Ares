using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ares.Data;

namespace Ares.ModelInfo
{
    public static class DataExtensions
    {
        public static bool IsEndless(this IModeElement element)
        {
            if (element.StartElement == null)
            {
                return false;
            }
            return IsEndless(element.StartElement);
        }

        public static bool AlwaysStartsMusic(this IModeElement element)
        {
            if (element.StartElement == null)
                return false;
            return AlwaysStartsMusic(element.StartElement);
        }

        private static bool AlwaysStartsMusic(IElement element)
        {
            if (element is IMusicList)
            {
                return true;
            }
            else if (element is IContainerElement)
            {
                IElement inner = (element as IContainerElement).InnerElement;
                if (inner != element && AlwaysStartsMusic(inner))
                    return true;
            }
            else if (element is IElementContainer<IParallelElement>)
            {
                IList<IParallelElement> parElements = (element as IElementContainer<IParallelElement>).GetElements();
                foreach (IParallelElement parallel in parElements)
                {
                    if (AlwaysStartsMusic(parallel))
                        return true;
                }
            }
            else if (element is IElementContainer<ISequentialElement>)
            {
                IList<ISequentialElement> seqElements = (element as IElementContainer<ISequentialElement>).GetElements();
                if (seqElements.Count > 0 && AlwaysStartsMusic(seqElements[0]))
                    return true;
            }
            return false;
        }

        private static bool IsEndless(IElement element)
        {
            if (element is IRepeatableElement)
            {
                if ((element as IRepeatableElement).RepeatCount == -1)
                    return true;
            }
            if (element is IContainerElement)
            {
                IElement inner = (element as IContainerElement).InnerElement;
                if (inner != element && IsEndless(inner))
                    return true;
            }
            else if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement subElement in ((element as IGeneralElementContainer).GetGeneralElements()))
                {
                    if (IsEndless(subElement))
                        return true;
                }
            }
            return false;
        }
    }
}
