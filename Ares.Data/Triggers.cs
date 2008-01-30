using System;

namespace Ares.Data
{
    [Serializable]
    abstract class Trigger
    {
        public IElement TargetElement { get; set; }
    }

    [Serializable]
    class KeyTrigger : Trigger, IKeyTrigger
    {
        public TriggerType TriggerType { get { return TriggerType.Key; } }

        public Int32 KeyCode { get; set; }

        internal KeyTrigger()
        {
            KeyCode = 0;
        }
    }

    [Serializable]
    class ElementFinishTrigger : Trigger, IElementFinishTrigger
    {
        public TriggerType TriggerType { get { return TriggerType.ElementFinished; } }

        public Int32 ElementID { get; set; }

        internal ElementFinishTrigger()
        {
            ElementID = 0;
        }
    }
}
