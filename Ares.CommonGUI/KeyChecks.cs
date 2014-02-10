/*
 Copyright (c) 2011 [Joerg Ruedenauer]
 
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

namespace Ares.CommonGUI
{
    public class KeyChecks : Ares.ModelInfo.ModelCheck
    {
        public KeyChecks()
            : base(Ares.ModelInfo.CheckType.Key)
        {
        }

        private static Dictionary<int, bool> s_GlobalReservedKeys;

        static KeyChecks()
        {
            s_GlobalReservedKeys = new Dictionary<int, bool>();
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Left, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Right, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Up, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Down, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.PageDown, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.PageUp, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Insert, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Delete, true);
            s_GlobalReservedKeys.Add((int)System.Windows.Forms.Keys.Escape, true);
        }
        
        public static String GetErrorForKey(IProject project, IModeElement modeElement, int key)
        {
            IMode elementMode = null;
            foreach (IMode mode in project.GetModes())
            {
                if (mode.KeyCode == key)
                {
                    return String.Format(StringResources.KeyUsedByMode, mode.Title);
                }
                if (mode.GetElements().Contains(modeElement))
                {
                    elementMode = mode;
                }
            }
            if (elementMode != null)
            {
                foreach (IModeElement element in elementMode.GetElements())
                {
                    if (element != modeElement && element.Trigger != null && element.Trigger.TriggerType == TriggerType.Key
                        && (element.Trigger as IKeyTrigger).KeyCode == key)
                    {
                        return String.Format(StringResources.KeyUsedByModeElement, element.Title);
                    }
                }
            }
            if (s_GlobalReservedKeys.ContainsKey(key))
            {
                return StringResources.KeyGloballyReserved;
            }

            return String.Empty;
        }

        public override void DoChecks(Data.IProject project, Ares.ModelInfo.IModelErrors errors)
        {
            System.Windows.Forms.KeysConverter converter = new System.Windows.Forms.KeysConverter();
            for (int i = 0; i < project.GetModes().Count; ++i)
            {
                IMode mode = project.GetModes()[i];
                // check: no empty key
                if (mode.KeyCode == 0)
                {
                    /*
                    AddError(errors, ModelError.ErrorSeverity.Warning,
                        String.Format(StringResources.ModeNoKey, mode.Title), mode);
                    */
                }
                else
                {
                    // check: no globally reserved key
                    if (s_GlobalReservedKeys.ContainsKey(mode.KeyCode))
                    {
                        AddError(errors, Ares.ModelInfo.ModelError.ErrorSeverity.Error,
                            String.Format(StringResources.ModeKeyGloballyReserved,
                            converter.ConvertToString((System.Windows.Forms.Keys)mode.KeyCode)), mode);
                    }
                    // check: key not used by another mode
                    for (int j = i + 1; j < project.GetModes().Count; ++j)
                    {
                        if (project.GetModes()[j].KeyCode == mode.KeyCode)
                        {
                            AddError(errors, Ares.ModelInfo.ModelError.ErrorSeverity.Error,
                                String.Format(StringResources.DuplicateModeKey,
                                converter.ConvertToString((System.Windows.Forms.Keys)mode.KeyCode),
                                mode.Title, project.GetModes()[j].Title), mode);
                        }
                    }
                }
                // check mode elements
                for (int j = 0; j < mode.GetElements().Count; ++j)
                {
                    IModeElement modeElement = mode.GetElements()[j];
                    // get key code, if there is one
                    int keyCode = 0;
                    if (modeElement.Trigger != null)
                    {
                        if (modeElement.Trigger.TriggerType == TriggerType.Key)
                        {
                            keyCode = ((IKeyTrigger)modeElement.Trigger).KeyCode;
                        }
                        else
                        {
                            // no key trigger, no checks
                            continue;
                        }
                    }
                    // check: no empty key
                    if (keyCode == 0)
                    {
                        /*
                        AddError(errors, ModelError.ErrorSeverity.Warning,
                            String.Format(StringResources.ModeElementNoKey, modeElement.Title), modeElement);
                         */
                    }
                    else
                    {
                        // check: no globally reserved key
                        if (s_GlobalReservedKeys.ContainsKey(keyCode))
                        {
                            AddError(errors, Ares.ModelInfo.ModelError.ErrorSeverity.Error,
                                String.Format(StringResources.ModeElementKeyGloballyReserved,
                                converter.ConvertToString((System.Windows.Forms.Keys)keyCode)), modeElement);
                        }
                        // check: key not used by a mode
                        for (int k = 0; k < project.GetModes().Count; ++k)
                        {
                            if (project.GetModes()[k].KeyCode == keyCode)
                            {
                                AddError(errors, Ares.ModelInfo.ModelError.ErrorSeverity.Error,
                                    String.Format(StringResources.ModeElementKeyUsedByMode,
                                    converter.ConvertToString((System.Windows.Forms.Keys)keyCode),
                                    modeElement.Title, project.GetModes()[k].Title), modeElement);
                            }
                        }
                        // check: key not used by another element in the same mode
                        for (int k = j + 1; k < mode.GetElements().Count; ++k)
                        {
                            IModeElement other = mode.GetElements()[k];
                            if (other.Trigger != null && other.Trigger.TriggerType == TriggerType.Key)
                            {
                                if (((IKeyTrigger)other.Trigger).KeyCode == keyCode)
                                {
                                    AddError(errors, Ares.ModelInfo.ModelError.ErrorSeverity.Error,
                                        String.Format(StringResources.DuplicateModeElementKey,
                                        converter.ConvertToString((System.Windows.Forms.Keys)keyCode),
                                        modeElement.Title, other.Title), modeElement);
                                }
                            }
                        }
                    }                    
                }
            }
        }
    }
}
