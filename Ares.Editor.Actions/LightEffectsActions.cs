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
using Ares.Data;

namespace Ares.Editor.Actions
{
    abstract public class LightEffectsValueAction<T> : Action
    {
        public LightEffectsValueAction(ILightEffects element, T val)
        {
            m_Element = element;
            m_Value = val;
            m_OldValue = GetElementValue();
        }

        public void SetData(T val)
        {
            m_Value = val;
        }

        public override void Do(IProject project)
        {
            SetElementValue(m_Value);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        public override void Undo(IProject project)
        {
            SetElementValue(m_OldValue);
            ElementChanges.Instance.ElementChanged(m_Element.Id);
        }

        abstract protected T GetElementValue();
        abstract protected void SetElementValue(T val);

        protected ILightEffects m_Element;
        protected T m_Value;
        protected T m_OldValue;
    }


    public class LightEffectsSetsMasterBrightnessAction : LightEffectsValueAction<bool>
    {
        public LightEffectsSetsMasterBrightnessAction(ILightEffects element, bool val) : base(element, val) { }
        protected override bool GetElementValue() { return m_Element.SetsMasterBrightness; }
        protected override void SetElementValue(bool val) { m_Element.SetsMasterBrightness = val; }
    }
    public class LightEffectsSetsLeftRightMixAction : LightEffectsValueAction<bool>
    {
        public LightEffectsSetsLeftRightMixAction(ILightEffects element, bool val) : base(element, val) { }
        protected override bool GetElementValue() { return m_Element.SetsLeftRightMix; }
        protected override void SetElementValue(bool val) { m_Element.SetsLeftRightMix = val; }
    }
    public class LightEffectsSetsLeftSceneAction : LightEffectsValueAction<bool>
    {
        public LightEffectsSetsLeftSceneAction(ILightEffects element, bool val) : base(element, val) { }
        protected override bool GetElementValue() { return m_Element.SetsLeftScene; }
        protected override void SetElementValue(bool val) { m_Element.SetsLeftScene = val; }
    }
    public class LightEffectsSetsRightSceneAction : LightEffectsValueAction<bool>
    {
        public LightEffectsSetsRightSceneAction(ILightEffects element, bool val) : base(element, val) { }
        protected override bool GetElementValue() { return m_Element.SetsRightScene; }
        protected override void SetElementValue(bool val) { m_Element.SetsRightScene = val; }
    }


    public class LightEffectsMasterBrightnessAction : LightEffectsValueAction<int>
    {
        public LightEffectsMasterBrightnessAction(ILightEffects element, int val) : base(element,val) {}
        protected override int GetElementValue()            { return m_Element.MasterBrightness; }
        protected override void SetElementValue(int val)    { m_Element.MasterBrightness = val; }
    }
    public class LightEffectsLeftRightMixAction : LightEffectsValueAction<int>
    {
        public LightEffectsLeftRightMixAction(ILightEffects element, int val) : base(element, val) { }
        protected override int GetElementValue() { return m_Element.LeftRightMix; }
        protected override void SetElementValue(int val) { m_Element.LeftRightMix = val; }
    }
    public class LightEffectsLeftSceneAction : LightEffectsValueAction<int>
    {
        public LightEffectsLeftSceneAction(ILightEffects element, int val) : base(element, val) { }
        protected override int GetElementValue() { return m_Element.LeftScene; }
        protected override void SetElementValue(int val) { m_Element.LeftScene = val; }
    }
    public class LightEffectsRightSceneAction : LightEffectsValueAction<int>
    {
        public LightEffectsRightSceneAction(ILightEffects element, int val) : base(element, val) { }
        protected override int GetElementValue() { return m_Element.RightScene; }
        protected override void SetElementValue(int val) { m_Element.RightScene = val; }
    }

}