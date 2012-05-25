using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld
{
    public class Trigger:Action
    {
        public String Description { get; private set; }
        public Event Event { get; set; }
    }
}
