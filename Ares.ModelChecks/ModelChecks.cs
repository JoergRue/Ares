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
