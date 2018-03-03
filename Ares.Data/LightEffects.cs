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

        private bool m_SetsMasterBrightness = false;
        private int m_MasterBrightness = 255;

        private bool m_SetsLeftRightMix = false;
        private int m_LeftRightMix = 127;

        private bool m_SetsLeftScene = false;
        private int m_LeftScene = 1;

        private bool m_SetsRightScene = false;
        private int m_RightScene = 1;

        private static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        private static int Clamp(int value)
        {
            return Clamp(value, 0, 255);
        }

        public bool SetsMasterBrightness
        {
            get { return m_SetsMasterBrightness; }
            set { m_SetsMasterBrightness = value; }
        }
        public int MasterBrightness
        {
            get { return m_MasterBrightness; }
            set { m_MasterBrightness = Clamp(value); }
        }

        public bool SetsLeftRightMix
        {
            get { return m_SetsLeftRightMix; }
            set { m_SetsLeftRightMix = value; }
        }
        public int LeftRightMix
        {
            get { return m_LeftRightMix; }
            set { m_LeftRightMix = Clamp(value); }
        }

        public bool SetsLeftScene
        {
            get { return m_SetsLeftScene; }
            set { m_SetsLeftScene = value;  }
        }
        public int LeftScene
        {
            get { return m_LeftScene; }
            set { m_LeftScene = Clamp(value); }
        }

        public bool SetsRightScene
        {
            get { return m_SetsRightScene; }
            set { m_SetsRightScene = value; }
        }
        public int RightScene
        {
            get { return m_RightScene; }
            set { m_RightScene = Clamp(value); }
        }
    }
}
