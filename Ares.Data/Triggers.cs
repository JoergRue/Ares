using System;

namespace Ares.Data
{
    [Serializable]
    abstract class Trigger
    {
        public IElement TargetElement { get; set; }

        public bool StopMusic { get; set; }

        public bool StopSounds { get; set; }

        protected Trigger()
        {
            StopMusic = false;
            StopSounds = false;
        }
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

        public Int32 ElementId { get; set; }

        internal ElementFinishTrigger()
        {
            ElementId = 0;
        }
    }
}
