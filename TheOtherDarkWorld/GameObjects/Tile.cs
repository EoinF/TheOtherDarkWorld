using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Tile : GameObject
    {
        public const float WIDTH = 10;
        public const float HEIGHT = 10;

        private Block _block;
        public Block Block
        {
            get
            {
                return _block;
            }
            set
            {
                if (value == null)
                    Colour = Color.White;
                else
                {
                    Colour = value.Colour;
                }
                _block = value;
            }
        }
        public Item[] Items { get; set; }

        public Tile(int x, int y, Color Colour, Item[] Items = null, Block Block = null)
            : base(new Vector2(x * 10, y * 10), 0, Colour, Vector2.Zero, Vector2.Zero, 0, 10, 10)
        {
            this.Block = Block;
            this.Items = Items;
        }

        public Tile(int x, int y, Item[] Items = null, Block Block = null)
            : base(new Vector2(x * 10, y * 10), 0, Color.White, Vector2.Zero, Vector2.Zero, 0, 10, 10)
        {
            this.Block = Block;
            this.Items = Items;
        }
    }
}
