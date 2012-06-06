using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.Items;

namespace TheOtherDarkWorld
{
    public class Block
    {
        public Vector2 Position { get; private set; }
        public int Health { get; set; }
        public int Resistance { get; private set; }
        public Color Colour { get; private set; }
        public List<Item> Items { get; set; }
        public List<Trigger> Triggers { get; set; }


        public Rectanglef Rect
        {
            get { return new Rectanglef((int)Position.X, (int)Position.Y, Textures.Block.Height, Textures.Block.Width); }
        }


        public Block(int x, int y, byte Type)
        {
            Block characteristics = GameData.GameBlocks[Type];
            this.Colour = characteristics.Colour;
            this.Health = characteristics.Health;
            this.Items = characteristics.Items;
            this.Resistance = characteristics.Resistance;
            this.Triggers = characteristics.Triggers;

            Position = new Vector2(x * 10, y * 10);
        }

        public Block(int Type, Color Colour, int Health, List<Item> Items, int Resistance, List<Trigger> Triggers)
        {
            this.Colour = Colour;
            this.Health = Health;
            this.Items = Items;
            this.Resistance = Resistance;
            this.Triggers = Triggers;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Block, Position - Player.PlayerList[0].Offset, null, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.7f);
        }

    }
}

