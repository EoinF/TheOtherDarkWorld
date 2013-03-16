using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class Event
    {
        public Block TargetBlock { get; private set; }
        public String TargetAction { get; private set; }
        public Effect Effect { get; private set; }
    }
}
