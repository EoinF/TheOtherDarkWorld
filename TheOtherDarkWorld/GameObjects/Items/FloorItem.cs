using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
        public class FloorItem
        {
            public Item Item { get; set; }
            public Vector2 Position { get; set; }
            public Color Colour { get; set; }
            public float Rotation { get; set; }

            public FloorItem(Item Item, Vector2 Position)
            {
                this.Item = Item;

                if (Item is Togglable)
                    (Item as Togglable).Deactivate();

                this.Position = Position;
                Colour = Color.White;
                Rotation = (float)(new Random().NextDouble() * Math.PI);
            }

            public void Draw(SpriteBatch spriteBatch)
            {
                spriteBatch.Draw(Textures.Items, Position - Level.CurrentLevel.Players[0].Offset, Textures.GetItemRectangle(Item.Type), Colour, Rotation, Vector2.One * 8, 0.7f, SpriteEffects.None, 0.49f);
            }
        }
    }