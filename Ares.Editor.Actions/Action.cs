using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.Editor.Actions
{
    public abstract class Action
    {
        public abstract void Do();
        public abstract void Undo();
    }
}
