using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheOtherDarkWorld.GameObjects;

namespace TheOtherDarkWorld
{
    public class InventoryElement : UIElement
    {
        public Item Item { get; set; }

        public InventoryElement(Texture2D Texture = null, Vector2 Position = new Vector2(),
            Item Item = null,
            bool IsActive = true, CursorType CursorType = UI.CURSOR_DEFAULT,
            float opacity = UI_INHERIT)
            : base(Color.White, Color.Orange, Texture, Position, null, UI_INHERIT, UI_INHERIT, IsActive, true, CursorType, opacity: opacity)
        {
            this.Item = Item;
            this.Texture = Texture;
        }

        public override Rectangle? SrcRect
        {
            get { return Textures.GetItemRectangle(Item.Type); }
        }

        /// <summary>
        /// Gets the colour the item should be displayed as. It should be darker if the gun is reloading
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private Color GetReloadColour(Gun gun, Color original)
        {
            Color temp = original;

            temp *= 1 - ((float)gun.ReloadCooldown / (gun.ReloadTime * 1.3f));
            //Restore the alpha
            temp.A = original.A;
            temp.B = 255;
            return temp;
        }

        public override bool SwapWith(UIElement e)
        {
            //
            // A special case occurs when the two items being swapped have the same type. Instead of swapping them,
            // we just transfer some of the first item into the second
            //
            InventoryElement ie = (e as InventoryElement);

            if (ie.Item.Type == this.Item.Type)
            {
                int amountMissing = ie.Item.MaxAmount - ie.Item.Amount;
                if (amountMissing > 0)
                {
                    if (this.Item.Amount > amountMissing) //If we have more than enough to fill the missing amount
                    {
                        ie.Item.Amount = ie.Item.MaxAmount;
                        this.Item.Amount -= amountMissing;
                    }
                    else //We don't have enough to fill up the missing amount
                    {
                        ie.Item.Amount += this.Item.Amount;
                        this.Item.Amount = 0;
                    }
                }
                return false;
            }
            else
            {
                return base.SwapWith(e);
            }
        }

        public override void Draw(SpriteBatch spriteBatch, float extraLayerDepth = 0)
        {
            if (IsActive)
            {
                if (Item != null)
                {
                    Color colour = Colour;
                    if (Item is Gun)
                        colour = GetReloadColour((Gun)Item, Colour);

                    if (Textures.Items != null)
                        spriteBatch.Draw(Textures.Items, Position + new Vector2(2,2), SrcRect, colour, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth + 0.05f + extraLayerDepth);
                    spriteBatch.DrawString(Textures.Fonts[0], Item.Amount.ToString(), Position + new Vector2(4, 22), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, LayerDepth +  0.05f + extraLayerDepth);

                }
            }
        }
    }
}
