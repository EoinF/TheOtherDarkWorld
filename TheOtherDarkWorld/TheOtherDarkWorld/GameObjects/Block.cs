using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TheOtherDarkWorld.GameObjects
{
    public class Block
    {
        public int Health { get; set; }
        public int Resistance { get; private set; }
        public Color Colour { get;  set; }
        public Item[] Drops;


        public Block(int x, int y, byte Type)
        {
            Block characteristics = GameData.GameBlocks[Type];
            this.Colour = characteristics.Colour;
            this.Health = characteristics.Health;
            this.Drops = characteristics.Drops;
            this.Resistance = characteristics.Resistance;

        }

        public Block(int Type, Color Colour, int Health, Item[] Drops, int Resistance)
        {
            this.Colour = Colour;
            this.Health = Health;
            this.Drops = Drops;
            this.Resistance = Resistance;
        }



    }
}

