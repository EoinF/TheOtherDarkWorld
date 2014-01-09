using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheOtherDarkWorld.GameObjects
{
    public class Goggles : Togglable
    {
        public Goggles(int type, Entity owner = null)
            : base(type, owner: owner)
        {

        }

        public Goggles()
        {

        }
    }
}
