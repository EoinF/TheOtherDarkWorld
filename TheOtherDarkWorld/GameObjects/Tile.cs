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
                    Colour = value.Colour;

                _block = value;
            }
        }
        public Item[] Items { get; set; }

        public override void ResetLighting()
        {
            base.ResetLighting();
            if (Block != null)
                Block.Brightness = 0;
        }

        public Tile(Texture2D Texture, int x, int y, Color Colour, Item[] Items = null, Block Block = null)
            : base(Texture, new Vector2(x * 10, y * 10), 0, Colour, Vector2.Zero, Vector2.Zero, 0, 10, 10)
        {
            this.Block = Block;
            this.Items = Items;
            ResetLighting();
        }

        public Tile(Texture2D Texture, int x, int y, Item[] Items = null, Block Block = null)
            : base(Texture, new Vector2(x * 10, y * 10), 0, Color.White, Vector2.Zero, Vector2.Zero, 0, 10, 10)
        {
            this.Block = Block;
            this.Items = Items;
            ResetLighting();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Block != null)
            {
                if (IsVisible)
                {
                    Block.ApplyLighting();
                    spriteBatch.Draw(Texture, Position + Origin - StateManager.Offset, null, Colour, 0, Vector2.Zero, 1, SpriteEffects.None, UI.GAMEOBJECT_DEPTH_DEFAULT);
                }
            }
            else
                base.Draw(spriteBatch);
        }

    }
}
