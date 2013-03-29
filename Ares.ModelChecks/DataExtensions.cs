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
            else if (element is IMusicByTags)
                return true;
            return false;
        }

        private static bool IsEndless(IElement element)
        {
            return IsEndless(new HashSet<IElement>(), element);
        }

        private static bool IsEndless(HashSet<IElement> checkedReferences, IElement element)
        {
            if (element is IRepeatableElement)
            {
                if ((element as IRepeatableElement).RepeatCount == -1)
                    return true;
            }
            if (element is IContainerElement)
            {
                IElement inner = (element as IContainerElement).InnerElement;
                if (inner != element && IsEndless(checkedReferences, inner))
                    return true;
            }
            else if (element is IGeneralElementContainer)
            {
                foreach (IContainerElement subElement in ((element as IGeneralElementContainer).GetGeneralElements()))
                {
                    if (IsEndless(checkedReferences, subElement))
                        return true;
                }
            }
            else if (element is Ares.Data.IReferenceElement)
            {
                if (checkedReferences.Contains(element))
                    return false;
                checkedReferences.Add(element);
                Ares.Data.IElement referencedElement = Ares.Data.DataModule.ElementRepository.GetElement((element as Ares.Data.IReferenceElement).ReferencedId);
                if (referencedElement != null && IsEndless(checkedReferences, referencedElement))
                {
                    return true;
                }
            }
            else if (element is IMusicByTags)
                return true;

            return false;
        }
    }
}
