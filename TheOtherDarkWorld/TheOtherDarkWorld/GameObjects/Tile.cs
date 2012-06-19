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
        public Block Block { get; set; }
        public Vector2 Position { get; private set; }
        public Item[] Items { get; set; }
        public Trigger[] Triggers { get; set; }
        private Color LightColour { get; set; }
        private float _brightness;
        public float Brightness
        {
            get
            {
                return _brightness;
            }
            set
            {
                if (value <= 1)
                    _brightness = value;
                else
                    _brightness = 1;
            }
        }
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
            this.LightColour = LightColour;
            Brightness = 0;
        }

        public Tile(int x, int y, Item[] Items = null, Trigger[] Triggers = null, Block Block = null)
        {
            this.Block = Block;
            this.Items = Items;
            this.Triggers = Triggers;
            Position = new Vector2(x * 10, y * 10);
            this.LightColour = Color.White;
            Brightness = 0;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Textures.Block, Position - Player.PlayerList[0].Offset, null, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
        }
    }
}
