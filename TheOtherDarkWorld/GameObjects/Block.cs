using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Block
    {
        public float Health { get; set; }
        public int Resistance { get; set; }
        public Color Colour { get;  set; }
        public Item[] Drops;
        public float Opacity { get; set; }


        public Block(int x, int y, byte Type)
        {
            Block characteristics = GameData.GameBlocks[Type];
            this.Colour = characteristics.Colour;
            this.Health = characteristics.Health;
            this.Drops = characteristics.Drops;
            this.Resistance = characteristics.Resistance;
            this.Opacity = characteristics.Opacity;
        }

        public Block() { }



    }
}

