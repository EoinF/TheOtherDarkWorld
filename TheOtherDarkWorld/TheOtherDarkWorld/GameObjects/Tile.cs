using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
    public class Tile
    {
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
                    Colour = value.Colour;
                _block = value;
            }
        }
        public Vector2 Position { get; private set; }
        public Item[] Items { get; set; }
        public Trigger[] Triggers { get; set; }

        private Color LightColour {
            get
            {
                //Preserve the alpha component so that the tiles will actually be drawn
                byte alpha = Colour.A;
                Color c = Colour * Brightness;
                c.A = alpha;
                return c;
            }
        }
        private float _brightness;
        public float Brightness { get { return _brightness; } set { _brightness = value;} }

        private Color Colour
        {
            get;
            set;

            //Color c = Color.Lerp((Block != null ? Block.Colour : Color.White), LightColour, 0.5f);
            //return new Color((byte)(c.R * Brightness), (byte)(c.G * Brightness), (byte)(c.B * Brightness));

        }

        public Rectanglef Rect
        {
            get { return new Rectanglef(Position.X, Position.Y, Textures.Block.Width, Textures.Block.Height); }
        }

        public Tile(int x, int y, Color Colour, Item[] Items = null, Trigger[] Triggers = null, Block Block = null)
        {
            this.Block = Block;
            this.Items = Items;
            this.Triggers = Triggers;
            Position = new Vector2(x * 10, y * 10);
            this.Colour = Colour;
        }

        public Tile(int x, int y, Item[] Items = null, Trigger[] Triggers = null, Block Block = null)
        {
            this.Block = Block;
            this.Items = Items;
            this.Triggers = Triggers;
            Position = new Vector2(x * 10, y * 10);
            Colour = Color.White;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Block, Position - Player.PlayerList[0].Offset, null, LightColour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
        }
    }
}
