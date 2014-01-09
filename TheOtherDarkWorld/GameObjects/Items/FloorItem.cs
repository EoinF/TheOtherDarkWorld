using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheOtherDarkWorld.GameObjects
{
        public class FloorItem : GameObject
        {
            public Item Item { get; set; }
            public float Rotation { get; set; }

            public FloorItem(Item Item, Vector2 Position)
                : base(Textures.Items, Position, 0, Color.White, Vector2.Zero, Vector2.Zero, -1, 0, 0)
            {
                this.Item = Item;

                if (Item is Togglable)
                    (Item as Togglable).Deactivate();

                this.Position = Position;
                Colour = Color.White;
                Rotation = (float)(new Random().NextDouble() * Math.PI);
            }

            public void Update(Tile[,] Tiles)
            {
                //
                //Check if the entity is lit up and/or visible
                //
                int tilex = (int)(Position.X / 10);
                int tiley = (int)(Position.Y / 10);

                if (tilex >= 0 && tilex + 1 < Tiles.GetLength(0)
                    && tiley >= 0 && tiley + 1 < Tiles.GetLength(1))
                {
                    float averageBrightness = (Tiles[tilex, tiley].Brightness
                        + Tiles[tilex + 1, tiley].Brightness
                        + Tiles[tilex, tiley + 1].Brightness
                        + Tiles[tilex + 1, tiley + 1].Brightness)
                            / 4f;

                    Brightness = averageBrightness;

                    IsVisible = Tiles[tilex, tiley].IsVisible
                        || Tiles[tilex + 1, tiley].IsVisible
                        || Tiles[tilex, tiley + 1].IsVisible
                        || Tiles[tilex + 1, tiley + 1].IsVisible;
                }
            }

            public override void Draw(SpriteBatch spriteBatch)
            {
                ApplyLighting();
                if (IsVisible)
                    spriteBatch.Draw(Texture, Position - StateManager.Offset, Textures.GetItemRectangle(Item.Type), Colour, Rotation, Vector2.One * 8, 0.7f, SpriteEffects.None, UI.FLOORITEM_DEPTH_DEFAULT);
            }
        }
    }