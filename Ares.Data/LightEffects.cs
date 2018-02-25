/*
 Copyright (c) 2018  [Norbert Langermann]
 Ares is (c) Joerg Ruedenauer, Light Effects extension implemented by Norbert Langermann
 
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
using System.Xml;

namespace Ares.Data
{
    class LightEffects : ElementBase, ILightEffects
    {
        internal LightEffects(int elementId, String title)
            : base(elementId)
        {
            Title = title;
        }
        public override void WriteToXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
        public override void Visit(IElementVisitor visitor)
        {
            visitor.VisitLightEffects(this);
        }
        internal LightEffects(System.Xml.XmlReader reader)
            : base(reader)
        {

        }

        private bool m_SetsMasterBrightness=false;

        public bool SetsMasterBrightness
        {
            get
            {
                return m_SetsMasterBrightness;
            }
            set
            {
                m_SetsMasterBrightness = value;
            }
        }
    }
}
