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
    public class ModelChecks
    {
        public static ModelChecks Instance
        {
            get
            {
                if (sInstance == null)
                {
                    sInstance = new ModelChecks();
                }
                return sInstance;
            }
        }

        private static ModelChecks sInstance;

        public IProject Project { get; set; }

        public String GetErrorForKey(IModeElement modeElement, int key)
        {
            IMode elementMode = null;
            foreach (IMode mode in Project.GetModes())
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
            return String.Empty;
        }
    }
}
